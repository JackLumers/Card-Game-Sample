using System.Collections.Generic;
using System.Collections.ObjectModel;
using CardGameSample.Scripts.Card;
using UnityEngine;

namespace CardGameSample.Scripts
{
    [CreateAssetMenu(fileName = "TestCardDeck", menuName = "Create Test Card Deck")]
    public sealed class TestCardDeck : ScriptableObject
    {
        [SerializeField] private List<CardModelRef> cardModelsRefs = new List<CardModelRef>();

        public ReadOnlyCollection<CardModelRef> Cards => cardModelsRefs.AsReadOnly();
    }
}