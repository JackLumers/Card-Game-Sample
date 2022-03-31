using System;
using System.Threading;
using CardGameSample.Scripts.Battlefield;
using CardGameSample.Scripts.Card;
using CardGameSample.Scripts.Card.View;
using Cysharp.Threading.Tasks;
using ToolBox.Pools;
using UnityEngine;

namespace CardGameSample.Scripts.BattleController.States
{
    [CreateAssetMenu(fileName = "PlayerTurnState", menuName = "Battle State/PlayerTurnState")]
    public class PlayerTurnState : ABattleState
    {
        [SerializeField] private ABattleState nextState;
        [SerializeField] private int turnDurationMillis = 15000;
        [SerializeField] private int maxCardsToPlay = 2;

        private int _currentsCardsPlayedCount = 0;
        private CancellationTokenSource _stateCancellationTokenSource;

        public override void ResetState()
        {
            _stateCancellationTokenSource?.Cancel();
            _currentsCardsPlayedCount = 0;
            
            BattleController.CardPlaced -= OnCardPlaced;
        }

        public override void Enter()
        {
            _stateCancellationTokenSource?.Cancel();
            _stateCancellationTokenSource = new CancellationTokenSource();
            _currentsCardsPlayedCount = 0;
            
            BattleController.SetTurnCount(BattleController.CurrentTurnCount + 1);

            BattleController.CardPlaced += OnCardPlaced;
            
            BattleController.TurnTimer.StopAndSetDuration(0);
            TurnProcess(_stateCancellationTokenSource.Token).Forget();
        }
        
        public override void Exit()
        {
            _stateCancellationTokenSource?.Cancel();
            
            BattleController.CardPlaced -= OnCardPlaced;
            BattleController.TurnTimer.StopTimer();
        }

        private async UniTask TurnProcess(CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested) return;
            
            BattleController.BlockPlayerHandInput(true);
            
            try
            {
                await BattleController.FillPlayerHand()
                    .AttachExternalCancellation(cancellationToken)
                    .SuppressCancellationThrow();
                
                if(cancellationToken.IsCancellationRequested) return;
                
                BattleController.BlockPlayerHandInput(false);
                
                await BattleController.TurnTimer.StartTimer(turnDurationMillis)
                    .AttachExternalCancellation(cancellationToken)
                    .SuppressCancellationThrow();
                
                if(cancellationToken.IsCancellationRequested) return;

                StateMachine.ChangeState(nextState);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        
        private void OnCardPlaced(HandCardView handCardView, BattlefieldCell cell)
        {
            _currentsCardsPlayedCount++;
            
            if (_currentsCardsPlayedCount == maxCardsToPlay)
            {
                BattleController.BlockPlayerHandInput(true);
            }
            
            WaitForCardAttackAnimationAndProceed(handCardView, cell).Forget();
        }

        private async UniTask WaitForCardAttackAnimationAndProceed(HandCardView handCardView, BattlefieldCell cell)
        {
            BattleController.BlockPlayerHandInput(true);
            
            var modelRef = handCardView.Presenter.CardModelRef;

            try
            {
                await handCardView.AnimatePlaceCardOnCell(handCardView, cell);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            handCardView.gameObject.Release();
            
            if (_stateCancellationTokenSource.IsCancellationRequested) return;
            
            await cell.PlaceCard(modelRef)
                .AttachExternalCancellation(_stateCancellationTokenSource.Token)
                .SuppressCancellationThrow();
            
            if (_stateCancellationTokenSource.IsCancellationRequested) return;
            
            await BattleController.BattlefieldController.PlayerCellAttack(cell)
                .AttachExternalCancellation(_stateCancellationTokenSource.Token)
                .SuppressCancellationThrow();

            if (_stateCancellationTokenSource.IsCancellationRequested) return;
            
            if (_currentsCardsPlayedCount == maxCardsToPlay)
            {
                BattleController.TurnTimer.StopAndSetDuration(0);

                _stateCancellationTokenSource?.Cancel();
                StateMachine.ChangeState(nextState);
            }
            else
            {
                BattleController.BlockPlayerHandInput(false);
            }
        }
    }
}