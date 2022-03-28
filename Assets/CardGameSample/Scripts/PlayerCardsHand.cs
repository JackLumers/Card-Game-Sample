using System;
using System.Collections.Generic;
using CardGameSample.Scripts.Card;
using CardGameSample.Scripts.ScriptableValues;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        [SerializeField] private Vector3 openedPosition;
        [SerializeField] private Vector3 closedPosition;
        [SerializeField] private Vector3 spawnCardPosition;
        [Space]
        [SerializeField] private Vector3 openedScale = Vector3.one;
        [SerializeField] private Vector3 closedScale = new Vector3(0.5f, 0.5f, 1f);
        [Space]
        [SerializeField] private float cardGivingTime = 0.5f;
        [SerializeField] private float spaceBetweenCards = 0.5f;
        
        private readonly HashSet<CardPresenter> _cards = new HashSet<CardPresenter>();
        private float _cardsStartPosition;
        
        private void Awake()
        {
            cardPrefab.Populate(cardsPoolPopulation.Value);
            _cardsStartPosition = -cardsContainer.rect.width / 2;
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

            var cardTransform = cardObject.transform;
            cardTransform.SetParent(cardsContainer);
            cardTransform.localRotation = Quaternion.identity;
            cardTransform.localScale = Vector3.one;

            cardObject.gameObject.SetActive(false);

            cardObject.InitModel(modelRef);

            _cards.Add(cardObject);
            
            await GivingCardAnimationProcess(cardObject, _cards.Count, cardGivingTime);
        }

        private async UniTask GivingCardAnimationProcess(CardPresenter cardObject, int currentCardsCount, float time)
        {
            try
            {
                cardObject.transform.localPosition = spawnCardPosition;
                cardObject.gameObject.SetActive(true);
                
                var position = CalculateNextCardPosition(spaceBetweenCards, currentCardsCount);
                await cardObject.transform.DOLocalMove(position, time).ToUniTask();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private Vector3 CalculateNextCardPosition(float space, int currentCardsCount)
        {
            if (currentCardsCount <= 1)
            {
                return new Vector3(_cardsStartPosition, 0, 0);
            }
            
            var positionX = _cardsStartPosition + (space * currentCardsCount - space);
            return new Vector3(positionX, 0, 0);
        }
    }
}
