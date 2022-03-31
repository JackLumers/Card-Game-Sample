using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CardGameSample.Scripts.Card;
using CardGameSample.Scripts.Card.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        /// <summary>
        /// Sets initial empty state of the hand.
        /// </summary>
        public void ResetState()
        {
            _repositioningCardsCts?.Cancel();
            
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
            _repositioningCardsCts?.Cancel();
            _repositioningCardsCts = new CancellationTokenSource();
            
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
            
            await RepositionCards().AttachExternalCancellation(_repositioningCardsCts.Token);
        }

        /// <summary>
        /// Removes card from hand, deactivating card view and returning it to the pool.
        /// </summary>
        public void RemoveCard(HandCardView cardView)
        {
            cardView.gameObject.SetActive(false);
            cardView.gameObject.Release();
            _cardsSet.Remove(cardView);
        }

        public void BlockInput(bool block)
        {
            itemsCanvasRaycaster.enabled = !block;
        }

        public async UniTask RepositionCards()
        {
            try
            {
                if (_cardsSet.Count == 0) return;

                float cardWidth = ((RectTransform) cardPrefab.transform).rect.width;
                var positions = CalculateCardPositions(cardWidth, spaceBetweenCards, _cardsSet.Count);

                List<UniTask> animationTasks = new List<UniTask>();

                int i = 0;
                foreach (var handCardView in _cardsSet.ToList())
                {
                    animationTasks.Add(handCardView.MoveLocal(positions[i], repositioningTime, false));
                    animationTasks.Add(handCardView.Rotate(cardsContainer.rotation, repositioningTime, false));
                    i++;
                }

                await UniTask.WhenAll(animationTasks);
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning(e);
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
