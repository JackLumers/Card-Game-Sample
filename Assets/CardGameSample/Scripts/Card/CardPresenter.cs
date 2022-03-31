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

            SubscribeModelActions(_cardModel);
        }

        public void InitModel(CardModelRef modelRef)
        {
            CardModelRef = modelRef;
            _cardModel = CardModelRef.CreateModel();

            SubscribeModelActions(_cardModel);
            
            CardView.AttackPoints = _cardModel.AttackPoints;
            CardView.HealthPoints = _cardModel.HealthPoints;
            CardView.CardSprite = _cardModel.SpriteId;
        }

        private void SubscribeModelActions(CardModel model)
        {
            model.AttackPointsChanged += (newValue) =>
            {
                CardView.AttackPoints = newValue;
            };
            
            model.HealthPointsChanged += (newValue) =>
            {
                CardView.HealthPoints = newValue;
            };
            
            model.SpriteKeyChanged += (newValue) =>
            {
                CardView.CardSprite = newValue;
            };
        }
    }
}
