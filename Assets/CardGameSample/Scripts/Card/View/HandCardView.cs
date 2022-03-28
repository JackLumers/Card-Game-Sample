using System;
using CardGameSample.Scripts.Input;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace CardGameSample.Scripts.Card.View
{
    public class HandCardView : MonoBehaviour, ICardView, IHoldableItem
    {
        [SerializeField] private TextMeshProUGUI attackPointsText;
        [SerializeField] private TextMeshProUGUI healthPointsText;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image backgroundImage;

        [SerializeField] private string attackPrefix = "ATK:";
        [SerializeField] private string healthPrefix = "HP:";

        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float scaleSpeed = 1f;
        [SerializeField] private float scaleWhileHoldValue = 1.2f;
        
        private RectTransform rectTransform;
        
        private AsyncOperationHandle<Sprite> _cardSpriteHandle;
        private TweenerCore<Vector3, Vector3, VectorOptions> _movingTweener;
        private TweenerCore<Vector3, Vector3, VectorOptions> _scalingTweener;
        
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

        private void Awake()
        {
            rectTransform = (RectTransform) transform;
        }

        public void Hold(bool hold)
        {
            if (hold)
            {
                _scalingTweener?.Kill();
                _scalingTweener = transform.DOScale(
                    new Vector3(scaleWhileHoldValue, scaleWhileHoldValue, 1f), 
                    scaleSpeed).SetSpeedBased(true);
            }
            else
            {
                _scalingTweener?.Kill();
                _scalingTweener = transform.DOScale(
                    new Vector3(1, 1, 1), scaleSpeed).SetSpeedBased(true);
            }
        }
        
        public void Move(Vector2 localPosition)
        {
            _movingTweener?.Kill();
            _movingTweener = rectTransform.DOLocalMove(
                new Vector3(localPosition.x, localPosition.y, transform.localPosition.z), 
                moveSpeed).SetSpeedBased(true);
        }
        
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
    }
}