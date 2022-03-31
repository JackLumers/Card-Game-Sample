using CardGameSample.Scripts.Card.View;
using UnityEngine;

namespace CardGameSample.Scripts.Card
{
    [RequireComponent(typeof(ICardView))]
    public class CardPresenter : MonoBehaviour
    {
        public ICardView CardView { get; private set; }
        public CardModelRef CardModelRef { get; private set; }
        
        private CardModel _cardModel;

        private void Awake()
        {
            CardView = GetComponent<ICardView>();
        }

        public int AttackPoints
        {
            get => _cardModel.AttackPoints;
            set
            {
                _cardModel.AttackPoints = value;
                CardView.AttackPoints = value;
            }
        }

        public int HealthPoints
        {
            get => _cardModel.HealthPoints;
            set
            {
                _cardModel.HealthPoints = value;
                CardView.HealthPoints = value;
            }
        }

        public string SpriteId
        {
            get => _cardModel.SpriteId;
            set
            {
                _cardModel.SpriteId = value;
                CardView.SpriteId = value;
            }
        }

        public void InitModel(CardModelRef modelRef)
        {
            CardModelRef = modelRef;
            _cardModel = CardModelRef.CreateModel();

            SubscribeModelActions(_cardModel);
            
            CardView.AttackPoints = _cardModel.AttackPoints;
            CardView.HealthPoints = _cardModel.HealthPoints;
            CardView.SpriteId = _cardModel.SpriteId;
        }

        private void SubscribeModelActions(CardModel model)
        {
            model.AttackPointsChanged += newValue =>
            {
                CardView.AttackPoints = newValue;
            };
            
            model.HealthPointsChanged += (newValue) =>
            {
                Debug.Log("Health points changed event!");
                CardView.HealthPoints = newValue;
            };
            
            model.SpriteKeyChanged += (newValue) =>
            {
                CardView.SpriteId = newValue;
            };
        }
    }
}
