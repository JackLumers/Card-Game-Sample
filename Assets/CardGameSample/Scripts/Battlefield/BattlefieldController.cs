using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace CardGameSample.Scripts.Battlefield
{
    public sealed class BattlefieldController : MonoBehaviour
    {
        [SerializeField] private List<BattlefieldCell> enemyCells;
        [SerializeField] private List<BattlefieldCell> playerCells;

        public ReadOnlyCollection<BattlefieldCell> EnemyCells => enemyCells.AsReadOnly();
        public ReadOnlyCollection<BattlefieldCell> PlayerCells => playerCells.AsReadOnly();

        public bool IsPlayerCellsFull
        {
            get
            {
                int fullCellsCount = playerCells.Count(battlefieldCell => !ReferenceEquals(battlefieldCell.Card, null));
                return fullCellsCount == playerCells.Count;
            }
        }

        /// <summary>
        /// Resets battlefield to the initial state
        /// </summary>
        public void ResetState()
        {
            foreach (var enemyCell in EnemyCells)
            {
                enemyCell.RemoveCard();
            }

            foreach (var playerCell in playerCells)
            {
                playerCell.RemoveCard();
            }
        }
    }
}