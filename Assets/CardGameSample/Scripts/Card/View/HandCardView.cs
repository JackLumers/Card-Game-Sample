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
        
        private RectTransform _rectTransform;
        
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

        public async UniTask Move(Vector3 localPosition)
        {
            try
            {
                _movingTweener?.Kill();
                _movingTweener = _rectTransform.DOLocalMove(
                    new Vector3(localPosition.x, localPosition.y, localPosition.z), 
                    moveSpeed).SetSpeedBased(true);
                await _movingTweener;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        
        public UniTask Move(Vector2 screenPointerPosition)
        {
            return Move(new Vector3(screenPointerPosition.x, screenPointerPosition.y, _rectTransform.localPosition.z));
        }

        public UniTask Show(bool show)
        {
            throw new System.NotImplementedException();
        }

        private void Awake()
        {
            _rectTransform = (RectTransform) transform;
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

        private async UniTask LoadCardSprite(string key)
        {
            try
            {
                if (_cardSpriteHandle.IsValid())
                {
                    Addressables.Release(_cardSpriteHandle);
                }
            
                _cardSpriteHandle = Addressables.LoadAssetAsync<Sprite>(key);
                Sprite sprite = await _cardSpriteHandle;
                cardImage.sprite = sprite;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
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