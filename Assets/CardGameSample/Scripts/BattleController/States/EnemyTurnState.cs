using System;
using System.Threading;
using CardGameSample.Scripts.BattleController.States;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CardGameSample.Scripts.BattleController
{
    [CreateAssetMenu(fileName = "EnemyTurnState", menuName = "Battle State/EnemyTurnState")]
    public class EnemyTurnState : ABattleState
    {
        [SerializeField] private ABattleState nextState;
        [CanBeNull] private CancellationTokenSource _stateCancellationTokenSource;

        public override void ResetState()
        {
            _stateCancellationTokenSource?.Cancel();
        }

        public override void Enter()
        {
            BattleController.BattlefieldController.ClearEnemyCells();
            BattleController.BattlefieldController.ClearPlayerCells();
            
            _stateCancellationTokenSource?.Cancel();
            _stateCancellationTokenSource = new CancellationTokenSource();
            
            TurnProcess(_stateCancellationTokenSource.Token).Forget();
        }

        public override void Exit()
        {
            _stateCancellationTokenSource?.Cancel();
        }
        
        private async UniTask TurnProcess(CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested) return;
            
            BattleController.BlockPlayerHandInput(true);
            try
            {
                await BattleController.FillEnemyCells()
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
    }
}