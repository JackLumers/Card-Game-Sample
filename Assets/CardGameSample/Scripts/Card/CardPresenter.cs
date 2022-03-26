using UnityEngine;

namespace CardGameSample.Scripts.Card
{
    public class CardPresenter : MonoBehaviour
    {
        private ICardView _cardView;
        
        public void Init(ICardView view)
        {
            _cardView = view;
        }
    }
}
