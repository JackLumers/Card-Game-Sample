using System;
using CardGameSample.Scripts.Card.View;
using CardGameSample.Scripts.Input;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CardGameSample.Scripts
{
    public sealed class PlayerCardsHandInputHandler : MonoBehaviour
    {
        [SerializeField] private HoldableObjectInputHandler holdableObjectInputHandler;
        [SerializeField] private PlayerCardsHand playerCardsHand;
        [SerializeField] private LayerMask placeObjectLayerMask;

        private void Awake()
        {
            holdableObjectInputHandler.Moved += OnCardMoved;
            holdableObjectInputHandler.Holden += OnCardHold;
        }

        /// <summary>
        /// Called when player canceled hold action and card is above object where it can be placed.
        /// This object is defined by <see cref="placeObjectLayerMask"/>
        /// </summary>
        public event Action<HandCardView, GameObject> CallPlaceCard;

        public void BlockInput(bool block)
        {
            holdableObjectInputHandler.gameObject.SetActive(!block);
            playerCardsHand.BlockInput(block);
        }
        
        private void OnCardMoved(GameObject cardObject, Vector2 screenPoint)
        {
            if (cardObject.TryGetComponent(out HandCardView cardView))
            {
                var ray = holdableObjectInputHandler.Camera.ScreenPointToRay(screenPoint);
                
                // If card is above battlefield cell
                if (Physics.Raycast(ray, holdableObjectInputHandler.Camera.farClipPlane, placeObjectLayerMask))
                {
                    cardView.TintBackground(true);
                }
                else
                {
                    cardView.TintBackground(false);
                }
            }
        }

        private void OnCardHold(GameObject cardObject, Vector2 screenPoint, bool holden)
        {
            if (!holden && cardObject.TryGetComponent(out HandCardView cardView))
            {
                cardView.TintBackground(false);
                
                var ray = holdableObjectInputHandler.Camera.ScreenPointToRay(screenPoint);
                
                // If card is above something with needed layerMask
                if (Physics.Raycast(ray, out RaycastHit hit, holdableObjectInputHandler.Camera.farClipPlane, placeObjectLayerMask))
                {
                    CallPlaceCard?.Invoke(cardView, hit.collider.gameObject);
                }
                else
                {
                    playerCardsHand.RepositionCards().Forget();
                }
            }
        }
    }
}