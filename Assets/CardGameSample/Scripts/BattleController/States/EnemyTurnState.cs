using System;
using System.Threading;
using CardGameSample.Scripts.BattleController.States;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CardGameSample.Scripts.BattleController
{
    [CreateAssetMenu(fileName = "EnemyTurnState", menuName = "Battle State/EnemyTurnState")]
    public class EnemyTurnState : ABattleState
    {
        [SerializeField] private ABattleState nextState;
        private CancellationTokenSource _stateCancellationTokenSource;

        public override void ResetState()
        {
            _stateCancellationTokenSource?.Cancel();
        }

        public override void Enter()
        {
            _stateCancellationTokenSource?.Cancel();
            _stateCancellationTokenSource = new CancellationTokenSource();
            
            TurnProcess(_stateCancellationTokenSource.Token).Forget();
        }

        private async UniTask TurnProcess(CancellationToken cancellationToken)
        {
            try
            {
                BattleController.BlockPlayerHandInput(true);
                await BattleController.FillEnemyCells().AttachExternalCancellation(cancellationToken);
                Exit();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public override void Exit()
        {
            _stateCancellationTokenSource?.Cancel();
        }
    }
}