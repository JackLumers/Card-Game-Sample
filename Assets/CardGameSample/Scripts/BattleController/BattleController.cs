using System;
using System.Collections.Generic;
using System.Threading;
using CardGameSample.Scripts.BattleController.States;
using CardGameSample.Scripts.Battlefield;
using CardGameSample.Scripts.Card;
using CardGameSample.Scripts.Card.View;
using CardGameSample.Scripts.ScriptableValues;
using CardGameSample.Scripts.Timer;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CardGameSample.Scripts.BattleController
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private PlayerCardsHandInputHandler cardsHandInputHandler;
        [SerializeField] private PlayerCardsHand cardsHand;
        [SerializeField] private BattlefieldController battlefieldController;
        [SerializeField] private TestCardDeck testCardDeck;
        [SerializeField] private ScriptableInt maxCardsInHand;

        [Header("UI")] [SerializeField] private ATimer turnTimer;
        [SerializeField] private Button resetButton;
        [SerializeField] private TextMeshProUGUI turnCounterText;

        [Space][SerializeField] private ABattleState initialState;
        [SerializeField] private List<ABattleState> battleStates;
        
        public ATimer TurnTimer => turnTimer;
        public BattlefieldController BattlefieldController => battlefieldController;
        
        private BattleStateMachine _stateMachine;
        
        [CanBeNull] private CancellationTokenSource _fillingPlayerHandCts;
        [CanBeNull] private CancellationTokenSource _fillingEnemyHandCts;
        [CanBeNull] private CancellationTokenSource _placingCardOnCellCts;

        /// <summary>
        /// Called when player placed a card.
        /// </summary>
        public event Action<HandCardView, BattlefieldCell> CardPlaced;
        
        public int CurrentTurnCount { get; private set; }
        
        private void Awake()
        {
            _stateMachine = new BattleStateMachine();
            _placingCardOnCellCts = new CancellationTokenSource();
            
            foreach (var battleState in battleStates)
            {
                battleState.Initialize(this, _stateMachine);
            }
            
            cardsHandInputHandler.CallPlaceCard += OnCallPlaceCard;
            
            resetButton.onClick.AddListener(ResetState);
            
            StartBattle();
        }

        private void OnDestroy()
        {
            // States are ScriptableObjects so we need to reset
            // them because they are living outside playmode too
            _stateMachine.CurrentState.ResetState();
        }

        public void StartBattle()
        {
            _stateMachine.Initialize(initialState);
        }

        /// <summary>
        /// Resets battle to the initial state
        /// </summary>
        public void ResetState()
        {
            _fillingPlayerHandCts?.Cancel();
            _fillingEnemyHandCts?.Cancel();
            _placingCardOnCellCts?.Cancel();
            
            _placingCardOnCellCts = new CancellationTokenSource();
            
            turnTimer.StopTimer();
            SetTurnCount(0);
            
            _stateMachine.CurrentState.ResetState();
            battlefieldController.ResetState();
            cardsHand.ResetState();
            
            StartBattle();
        }
        
        public void BlockPlayerHandInput(bool block)
        {
            cardsHandInputHandler.BlockInput(block);
        }

        public void SetTurnCount(int turn)
        {
            CurrentTurnCount = turn;
            turnCounterText.text = CurrentTurnCount.ToString();
        }
        
        public async UniTask FillPlayerHand()
        {
            _fillingPlayerHandCts?.Cancel();
            _fillingPlayerHandCts = new CancellationTokenSource();

            try
            {
                while (cardsHand.Cards.Count < maxCardsInHand.Value)
                {
                    int randomCardIndex = Random.Range(0, testCardDeck.Cards.Count);
                    
                    await cardsHand.AddCard(testCardDeck.Cards[randomCardIndex])
                        .AttachExternalCancellation(_fillingPlayerHandCts.Token)
                        .SuppressCancellationThrow();
                    
                    if(_fillingPlayerHandCts.IsCancellationRequested) return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public async UniTask FillEnemyCells()
        {
            _fillingEnemyHandCts?.Cancel();
            _fillingEnemyHandCts = new CancellationTokenSource();
            
            try
            {
                foreach (var cell in battlefieldController.EnemyCells)
                {
                    int randomCardIndex = Random.Range(0, testCardDeck.Cards.Count);
                    
                    await cell.PlaceCard(testCardDeck.Cards[randomCardIndex])
                        .AttachExternalCancellation(_fillingEnemyHandCts.Token)
                        .SuppressCancellationThrow();
                    
                    if(_fillingEnemyHandCts.IsCancellationRequested) return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private void OnCallPlaceCard(HandCardView handCardView, GameObject objectForCardPlacing)
        {
            if (!objectForCardPlacing.TryGetComponent(out BattlefieldCell cell)) return;
            cardsHand.RemoveCard(handCardView);
            cardsHand.RepositionCards().Forget();
            CardPlaced?.Invoke(handCardView, cell);
        }
    }
}