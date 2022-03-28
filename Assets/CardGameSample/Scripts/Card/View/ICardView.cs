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
    }
}