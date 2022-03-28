using System.Collections.Generic;
using CardGameSample.Scripts.Card;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CardGameSample.Scripts
{
    public class TestCardDeck : MonoBehaviour
    {
        [SerializeField] private PlayerCardsHand playerCardsHand;
        [SerializeField] private List<CardModelRef> cardModelsRefs = new List<CardModelRef>();
        
        private async void OnEnable()
        {
            await UniTask.Delay(1000);
            foreach (var cardModelRef in cardModelsRefs)
            {
                await playerCardsHand.AddCard(cardModelRef);
            }
        }
    }
}