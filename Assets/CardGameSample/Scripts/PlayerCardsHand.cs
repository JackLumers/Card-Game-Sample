using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CardGameSample.Scripts.Card;
using CardGameSample.Scripts.Card.View;
using Cysharp.Threading.Tasks;
using ToolBox.Pools;
using UnityEngine;
using UnityEngine.UI;

namespace CardGameSample.Scripts
{
    public sealed class PlayerCardsHand : MonoBehaviour
    {
        [SerializeField] private HandCardView cardPrefab;
        
        [Space] [SerializeField] private RectTransform cardsContainer;
        [SerializeField] private GraphicRaycaster itemsCanvasRaycaster;
        
        [Space] [SerializeField] private Vector3 spawnCardPosition;
        [SerializeField] private float repositioningTime = 0.5f;
        [SerializeField] private float spaceBetweenCards = 0.5f;
        
        private readonly HashSet<HandCardView> _cardsSet = new HashSet<HandCardView>();

        public ReadOnlyCollection<HandCardView> Cards => _cardsSet.ToList().AsReadOnly();

        private CancellationTokenSource _repositioningCardsCts;
        private CancellationTokenSource _resetCts;

        private void Awake()
        {
            _resetCts = new CancellationTokenSource();
        }

        /// <summary>
        /// Sets initial empty state of the hand.
        /// </summary>
        public void ResetState()
        {
            _repositioningCardsCts?.Cancel();
            _resetCts?.Cancel();
            _resetCts = new CancellationTokenSource();
            
            foreach (var handCardView in _cardsSet.ToList())
            {
                handCardView.gameObject.Release();
                _cardsSet.Clear();
            }
        }

        /// <summary>
        /// Adds card with animation.
        /// </summary>
        public async UniTask AddCard(CardModelRef modelRef)
        {
            // TODO: Can be optimized using different pooling approach.
            // Used pooling system don't have a solution for this because we are bound to the GameObject
            // and there is no way to override GetComponent call for caching the result.
            // It's not a big overhead in this case but I just want to write it down.
            
            var cardObject = cardPrefab.gameObject.Reuse().GetComponent<HandCardView>();
            cardObject.Presenter.InitModel(modelRef);
            cardObject.gameObject.SetActive(true);
            
            var cardTransform = cardObject.transform;
            cardTransform.SetParent(cardsContainer);
            cardTransform.localPosition = spawnCardPosition;
            cardTransform.localRotation = Quaternion.identity;
            cardTransform.localScale = Vector3.one;

            _cardsSet.Add(cardObject);

            try
            {
                await RepositionCards().SuppressCancellationThrow();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        /// <summary>
        /// Removes card from hand's card set.
        /// </summary>
        public void RemoveCard(HandCardView cardView)
        {
            _cardsSet.Remove(cardView);
        }

        public void BlockInput(bool block)
        {
            itemsCanvasRaycaster.enabled = !block;
        }

        public async UniTask RepositionCards()
        {
            _repositioningCardsCts?.Cancel();
            _repositioningCardsCts = new CancellationTokenSource();

            try
            {
                if (_cardsSet.Count == 0) return;

                float cardWidth = ((RectTransform) cardPrefab.transform).rect.width;
                var positions = CalculateCardPositions(cardWidth, spaceBetweenCards, _cardsSet.Count);

                List<UniTask> animationTasks = new List<UniTask>();

                int i = 0;
                foreach (var handCardView in _cardsSet.ToList())
                {
                    if(_repositioningCardsCts.IsCancellationRequested) return;
                    
                    animationTasks.Add(handCardView.MoveLocal(positions[i], repositioningTime, false)
                        .AttachExternalCancellation(_repositioningCardsCts.Token)
                        .AttachExternalCancellation(_resetCts.Token)
                        .SuppressCancellationThrow());

                    animationTasks.Add(handCardView.Rotate(cardsContainer.rotation, repositioningTime, false)
                        .AttachExternalCancellation(_repositioningCardsCts.Token)
                        .AttachExternalCancellation(_resetCts.Token)
                        .SuppressCancellationThrow());
                    i++;
                }
                
                if(_repositioningCardsCts.IsCancellationRequested) return;
                
                await UniTask.WhenAll(animationTasks)
                    .AttachExternalCancellation(_repositioningCardsCts.Token)
                    .AttachExternalCancellation(_resetCts.Token)
                    .SuppressCancellationThrow();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private Vector3[] CalculateCardPositions(float cardWidth, float space, int currentCardsCount)
        {
            Vector3[] positions = new Vector3[currentCardsCount];
            float step = cardWidth / 2 + space / 2;
            float centerPositionX = step * (currentCardsCount - 1) / 2;
            
            for (int i = 0; i < currentCardsCount; i++)
            {
                float positionX = step * i - centerPositionX;

                positions[i] = new Vector3(positionX, 0, 0);
            }

            return positions;
        }
    }
}
