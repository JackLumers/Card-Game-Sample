using System;
using UnityEngine;

namespace CardGameSample.Scripts.Card
{
    [CreateAssetMenu(fileName = "NewCardModel", menuName = "CardModel")]
    public class CardModel : ScriptableObject
    {
        [SerializeField] private string _cardId;
        [SerializeField] private string _spriteId;
        [SerializeField] private int _attackPoints;
        [SerializeField] private int _healthPoints;

        public string CardId
        {
            get => _cardId;
            set
            {
                _cardId = value;
                cardIdChanged?.Invoke();
            }
        }

        public string SpriteId
        {
            get => _spriteId;
            set
            {
                _spriteId = value;
                spriteKeyChanged?.Invoke();
            }
        }

        public int AttackPoints
        {
            get => _attackPoints;
            set
            {
                _attackPoints = value;
                attackPointsChanged?.Invoke();
            }
        }

        public int HealthPoints
        {
            get => _healthPoints;
            set
            {
                _healthPoints = value;
                healthPointsChanged?.Invoke();
            }
        }

        public event Action cardIdChanged;
        public event Action spriteKeyChanged;
        public event Action attackPointsChanged;
        public event Action healthPointsChanged;
    }
}
