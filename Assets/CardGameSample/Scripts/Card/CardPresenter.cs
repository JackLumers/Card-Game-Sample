using CardGameSample.Scripts.Card.View;
using UnityEngine;

namespace CardGameSample.Scripts.Card
{
    [RequireComponent(typeof(ICardView))]
    public class CardPresenter : MonoBehaviour
    {
        private ICardView _cardView;
        private CardModelRef _cardModelRef;
        
        private CardModel _cardModel;

        private void Awake()
        {
            _cardView = GetComponent<ICardView>();

            SubscribeModelActions(_cardModel);
        }
        
        public void InitModel(CardModelRef modelRef)
        {
            _cardModelRef = modelRef;
            _cardModel = _cardModelRef.CreateModel();

            SubscribeModelActions(_cardModel);
            
            _cardView.AttackPoints = _cardModel.AttackPoints;
            _cardView.HealthPoints = _cardModel.HealthPoints;
            _cardView.CardSprite = _cardModel.SpriteId;
        }
        
        private void SubscribeModelActions(CardModel model)
        {
            model.AttackPointsChanged += (newValue) =>
            {
                _cardView.AttackPoints = newValue;
            };
            
            model.HealthPointsChanged += (newValue) =>
            {
                _cardView.HealthPoints = newValue;
            };
            
            model.SpriteKeyChanged += (newValue) =>
            {
                _cardView.CardSprite = newValue;
            };
        }
    }
}
