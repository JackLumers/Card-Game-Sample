using UnityEngine;

namespace CardGameSample.Scripts.Card
{
    [CreateAssetMenu(fileName = "NewCardModelRef", menuName = "Create Card Model Reference")]
    public class CardModelRef : ScriptableObject
    {
        [SerializeField] private string cardRefId;
        [SerializeField] private string spriteId;
        [SerializeField] private int attackPoints;
        [SerializeField] private int healthPoints;

        public CardModel CreateModel()
        {
            return new CardModel(cardRefId, spriteId, attackPoints, healthPoints);
        }
    }
}