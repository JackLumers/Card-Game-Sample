using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CardGameSample.Scripts.Battlefield
{
    public sealed class BattlefieldController : MonoBehaviour
    {
        [SerializeField] private List<BattlefieldCell> enemyCells;
        [SerializeField] private List<BattlefieldCell> playerCells;

        public ReadOnlyCollection<BattlefieldCell> EnemyCells => enemyCells.AsReadOnly();
        public ReadOnlyCollection<BattlefieldCell> PlayerCells => playerCells.AsReadOnly();

        [CanBeNull] private CancellationTokenSource _resetCts;
        
        private void Awake()
        {
            _resetCts = new CancellationTokenSource();
            
            for (int i = 0; i < EnemyCells.Count; i++)
            {
                EnemyCells[i].Initialize(i);
            }

            for (int i = 0; i < PlayerCells.Count; i++)
            {
                PlayerCells[i].Initialize(i);
            }
        }

        /// <summary>
        /// Call to fight player card and enemy card against each other.
        /// If there is no enemy card in the cell above player's cell, nothing will happened.
        /// </summary>
        public async UniTask PlayerCellAttack(BattlefieldCell playerCell)
        {
            if (ReferenceEquals(playerCell.Card, null))
            {
                Debug.LogWarning("Trying to attack by empty player cell!");
                return;
            }

            // If enemy's cell that above the player's cell has a card
            var enemyCell = enemyCells[playerCell.Index];
            if (!ReferenceEquals(enemyCell.Card, null))
            {
                playerCell.Card.Presenter.HealthPoints -= enemyCell.Card.Presenter.AttackPoints;
                enemyCell.Card.Presenter.HealthPoints -= playerCell.Card.Presenter.AttackPoints;

                // Could be attack animation
                await UniTask.Delay(1000, cancellationToken: _resetCts.Token)
                    .AttachExternalCancellation(_resetCts.Token)
                    .SuppressCancellationThrow();

                if (playerCell.Card.Presenter.HealthPoints <= 0)
                {
                    playerCell.RemoveCard().Forget();
                }

                if (enemyCell.Card.Presenter.HealthPoints <= 0)
                {
                    enemyCell.RemoveCard().Forget();
                }
            }
        }

        public void ClearPlayerCells()
        {
            foreach (var playerCell in PlayerCells)
            {
                playerCell.RemoveCard().Forget();
            }
        }
        
        public void ClearEnemyCells()
        {
            foreach (var enemyCell in EnemyCells)
            {
                enemyCell.RemoveCard().Forget();
            }
        }

        /// <summary>
        /// Resets battlefield to the initial state
        /// </summary>
        public void ResetState()
        {
            _resetCts?.Cancel();
            _resetCts = new CancellationTokenSource();
            
            ClearPlayerCells();
            ClearEnemyCells();
        }
    }
}