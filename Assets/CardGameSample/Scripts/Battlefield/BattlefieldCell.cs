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
        
        [CanBeNull]
        public CellCardView Card { get; private set; }

        public async UniTask PlaceCard(CardModelRef cardModelRef)
        {
            try
            {
                RemoveCard();

                var cardView = cellCardPrefab.gameObject.Reuse().GetComponent<CellCardView>();
                Card = cardView;
            
                cardView.Presenter.InitModel(cardModelRef);

                var cardTransform = cardView.transform;
                cardTransform.SetParent(cellCanvas.transform);
                cardTransform.localPosition = Vector3.zero;
                cardTransform.localScale = Vector3.one;
                cardTransform.localRotation = Quaternion.identity;

                await cardView.Show(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public void RemoveCard()
        {
            if (!ReferenceEquals(Card, null))
            {
                Card.Show(false).Forget();
                Card = null;
            }
        }
    }
}