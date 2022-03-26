using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace CardGameSample.Scripts.Card
{
    [RequireComponent(typeof(CardPresenter))]
    public class BattleCardView : MonoBehaviour, ICardView
    {
        [SerializeField] private TextMeshProUGUI attackPointsText;
        [SerializeField] private TextMeshProUGUI healthPointsText;
        [SerializeField] private Image cardImage;
        
        [SerializeField] private string attackPrefix = "ATK:";
        [SerializeField] private string healthPrefix = "HP:";

        private CardPresenter _cardPresenter;
        private AsyncOperationHandle<Sprite> _cardSpriteHandle;

        private void Awake()
        {
            _cardPresenter = GetComponent<CardPresenter>();
            _cardPresenter.Init(this);
        }

        public int AttackPoints
        {
            set => attackPointsText.text = $"{attackPrefix} {value}";
        }

        public int HealthPoints
        {
            set => healthPointsText.text = $"{healthPrefix} {value}";
        }

        public string CardSprite
        {
            set => LoadCardSprite(value).Forget();
        }

        private async UniTask LoadCardSprite(string key)
        {
            Addressables.Release(_cardSpriteHandle);
            _cardSpriteHandle = Addressables.LoadAssetAsync<Sprite>(key);
            Sprite sprite = await _cardSpriteHandle;
            cardImage.sprite = sprite;
        }

        private void OnDestroy()
        {
            Addressables.Release(_cardSpriteHandle);
        }
    }
}
