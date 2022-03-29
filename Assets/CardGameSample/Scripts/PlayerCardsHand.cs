using System;
using System.Collections.Generic;
using CardGameSample.Scripts.Card;
using CardGameSample.Scripts.ScriptableValues;
using Cysharp.Threading.Tasks;
using ToolBox.Pools;
using UnityEngine;

namespace CardGameSample.Scripts
{
    public class PlayerCardsHand : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private ScriptableInt cardsPoolPopulation;
        
        [Space] [SerializeField] private RectTransform cardsContainer;
        [Space]
        [SerializeField] private Vector3 spawnCardPosition;
        [Space]
        [Space]
        [SerializeField] private float cardGivingTime = 0.5f;
        [SerializeField] private float spaceBetweenCards = 0.5f;
        
        private readonly List<CardPresenter> _cards = new List<CardPresenter>();
        
        private void Awake()
        {
            cardPrefab.Populate(cardsPoolPopulation.Value);
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
            
            var cardObject = cardPrefab.Reuse().GetComponent<CardPresenter>();
            cardObject.InitModel(modelRef);
            cardObject.gameObject.SetActive(true);
            
            var cardTransform = cardObject.transform;
            cardTransform.SetParent(cardsContainer);
            cardTransform.localPosition = spawnCardPosition;
            cardTransform.localRotation = Quaternion.identity;
            cardTransform.localScale = Vector3.one;

            _cards.Add(cardObject);
            
            await RepositionCards();
        }

        public async UniTask RepositionCards()
        {
            try
            {
                if(_cards.Count == 0) return;
            
                float cardWidth = ((RectTransform)cardPrefab.transform).rect.width;
                var positions = CalculateCardPositions(cardWidth, spaceBetweenCards, _cards.Count);

                List<UniTask> movingTasks = new List<UniTask>();
                for (int i = 0; i < positions.Length; i++)
                {
                    movingTasks.Add(_cards[i].Move(positions[i]));
                }
            
                await UniTask.WhenAll(movingTasks);
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
