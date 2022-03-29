using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CardGameSample.Scripts.Card.View
{
    public interface ICardView
    {
        public int AttackPoints { set; }

        public int HealthPoints { set; }
        
        /// <summary>
        /// Sets the key for card sprite and updates the sprite
        /// </summary>
        public string CardSprite { set; }

        /// <summary>
        /// Moves card's view changing it's local position in the scene
        /// </summary>
        public UniTask Move(Vector3 localPosition);

        /// <summary>
        /// Shows card's view with animation and enabling/disabling it's game object
        /// </summary>
        public UniTask Show(bool show);
    }
}