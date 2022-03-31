using System;
using System.Threading;
using CardGameSample.Scripts.Battlefield;
using CardGameSample.Scripts.Card;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CardGameSample.Scripts.BattleController.States
{
    [CreateAssetMenu(fileName = "PlayerTurnState", menuName = "Battle State/PlayerTurnState")]
    public class PlayerTurnState : ABattleState
    {
        [SerializeField] private ABattleState nextState;
        [SerializeField] private int turnDurationMillis = 15000;

        private CancellationTokenSource _stateCancellationTokenSource;

        public override void ResetState()
        {
            _stateCancellationTokenSource?.Cancel();
            
            BattleController.CardPlaced -= OnCardPlaced;
        }

        public override void Enter()
        {
            _stateCancellationTokenSource?.Cancel();
            _stateCancellationTokenSource = new CancellationTokenSource();

            BattleController.SetTurnCount(BattleController.CurrentTurnCount + 1);
            BattleController.CardPlaced += OnCardPlaced;
            
            TurnProcess(_stateCancellationTokenSource.Token).Forget();
        }
        
        public override void Exit()
        {
            _stateCancellationTokenSource?.Cancel();
            
            BattleController.CardPlaced -= OnCardPlaced;
            BattleController.TurnTimer.StopTimer();
        }
        
        private void OnCardPlaced(CardModelRef cardModelRef, BattlefieldCell cell)
        {
            if (!BattleController.BattlefieldController.IsPlayerCellsFull) return;
            
            _stateCancellationTokenSource?.Cancel();
                
            StateMachine.ChangeState(nextState);
        }
        
        private async UniTask TurnProcess(CancellationToken cancellationToken)
        {
            try
            {
                BattleController.BlockPlayerHandInput(true);
                
                await BattleController.FillPlayerHand().AttachExternalCancellation(cancellationToken);
                
                BattleController.BlockPlayerHandInput(false);
                
                await BattleController.TurnTimer.StartTimer(turnDurationMillis).AttachExternalCancellation(cancellationToken);
                
                OnTimerEnd();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private void OnTimerEnd()
        {
            _stateCancellationTokenSource?.Cancel();
            StateMachine.ChangeState(nextState);
        }
    }
}