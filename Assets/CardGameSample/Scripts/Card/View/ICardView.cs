using System;
using CardGameSample.Generated.Input;

namespace CardGameSample.Scripts.Card.View
{
    public interface ICardView : CardInputActions.ICardActions
    {
        public int AttackPoints { set; }

        public int HealthPoints { set; }
        
        /// <summary>
        /// Sets the key for card sprite and updates the sprite
        /// </summary>
        public string CardSprite { set; }

        public event Action Press;
    }
}