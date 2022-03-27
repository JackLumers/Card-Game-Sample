using System;

namespace CardGameSample.Scripts.Card
{
    public struct CardModel
    {
        private string _spriteId;
        private int _attackPoints;
        private int _healthPoints;

        public CardModel(string cardRefId, string spriteId, int attackPoints, int healthPoints)
        {
            CardRefId = cardRefId;
            _spriteId = spriteId;
            _attackPoints = attackPoints;
            _healthPoints = healthPoints;
            
            SpriteKeyChanged = null;
            AttackPointsChanged = null;
            HealthPointsChanged = null;
        }

        public string CardRefId { get; }

        public string SpriteId
        {
            get => _spriteId;
            set
            {
                _spriteId = value;
                SpriteKeyChanged?.Invoke(value);
            }
        }

        public int AttackPoints
        {
            get => _attackPoints;
            set
            {
                _attackPoints = value;
                AttackPointsChanged?.Invoke(value);
            }
        }

        public int HealthPoints
        {
            get => _healthPoints;
            set
            {
                _healthPoints = value;
                HealthPointsChanged?.Invoke(value);
            }
        }
        
        public event Action<string> SpriteKeyChanged;
        public event Action<int> AttackPointsChanged;
        public event Action<int> HealthPointsChanged;
    }
}
