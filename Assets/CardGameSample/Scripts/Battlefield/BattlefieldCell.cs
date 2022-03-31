using System;
using CardGameSample.Scripts.Card;
using CardGameSample.Scripts.Card.View;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using ToolBox.Pools;
using UnityEngine;

namespace CardGameSample.Scripts.Battlefield
{
    public sealed class BattlefieldCell : MonoBehaviour
    {
        [SerializeField] private CellCardView cellCardPrefab;
        [SerializeField] private Canvas cellCanvas;

        public Canvas CellCanvas => cellCanvas;
        
        public int Index { get; private set; }
        
        [CanBeNull]
        public CellCardView Card { get; private set; }
        
        public void Initialize(int index)
        {
            Index = index;
        }
        
        /// <summary>
        /// Places card from in cell with animation.
        /// Card reference sets on call, but animation can be waited.
        /// </summary>
        public async UniTask PlaceCard(CardModelRef cardModelRef)
        {
            try
            {
                var cardView = cellCardPrefab.gameObject.Reuse().GetComponent<CellCardView>();
                Card = cardView;
            
                cardView.Presenter.InitModel(cardModelRef);

                var cardTransform = cardView.transform;
                cardTransform.SetParent(cellCanvas.transform);
                cardTransform.localPosition = Vector3.zero;
                cardTransform.localScale = Vector3.one;
                cardTransform.localRotation = Quaternion.identity;

                await cardView.ShowAnimation(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        /// <summary>
        /// Removes card from cell.
        /// </summary>
        public async UniTask RemoveCard()
        {
            if (!ReferenceEquals(Card, null))
            {
                var removedCard = Card;
                Card = null;

                try
                {
                    await removedCard.ShowAnimation(false);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                
                removedCard.gameObject.Release();
            }
        }
    }
}