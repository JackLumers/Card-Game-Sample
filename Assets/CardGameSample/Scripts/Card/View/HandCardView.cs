using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace CardGameSample.Scripts.Card.View
{
    public class HandCardView : MonoBehaviour, ICardView
    {
        [SerializeField] private TextMeshProUGUI attackPointsText;
        [SerializeField] private TextMeshProUGUI healthPointsText;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image backgroundImage;

        [SerializeField] private string attackPrefix = "ATK:";
        [SerializeField] private string healthPrefix = "HP:";

        private AsyncOperationHandle<Sprite> _cardSpriteHandle;

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

        public event Action Press;

        private async UniTask LoadCardSprite(string key)
        {
            if (_cardSpriteHandle.IsValid())
            {
                Addressables.Release(_cardSpriteHandle);
            }
            
            _cardSpriteHandle = Addressables.LoadAssetAsync<Sprite>(key);
            Sprite sprite = await _cardSpriteHandle;
            cardImage.sprite = sprite;
        }

        private void OnDestroy()
        {
            if (_cardSpriteHandle.IsValid())
            {
                Addressables.Release(_cardSpriteHandle);
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Debug.Log(context.ReadValue<float>());
        }
    }
}