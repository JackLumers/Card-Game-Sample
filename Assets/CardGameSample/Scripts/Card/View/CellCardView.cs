using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using TMPro;
using ToolBox.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace CardGameSample.Scripts.Card.View
{
    public class CellCardView : MonoBehaviour, ICardView
    {
        [SerializeField] private CardPresenter presenter;
        [Space]
        [SerializeField] private TextMeshProUGUI attackPointsText;
        [SerializeField] private TextMeshProUGUI healthPointsText;
        [SerializeField] private Image cardImage;

        [SerializeField] private string attackPrefix = "ATK:";
        [SerializeField] private string healthPrefix = "HP:";

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float showDuration = 0.5f;
        
        private AsyncOperationHandle<Sprite> _cardSpriteHandle;
        private TweenerCore<float, float, FloatOptions> _fadeTweener;
        
        public CardPresenter Presenter => presenter;

        public int AttackPoints
        {
            set => attackPointsText.text = $"{attackPrefix} {value}";
        }

        public int HealthPoints
        {
            set => healthPointsText.text = $"{healthPrefix} {value}";
        }

        public string SpriteId
        {
            set => LoadCardSprite(value).Forget();
        }

        public async UniTask ShowAnimation(bool show)
        {
            try
            {
                if (show)
                {
                    gameObject.SetActive(true);
                    canvasGroup.alpha = 0;
                    
                    _fadeTweener?.Kill();
                    _fadeTweener = canvasGroup.DOFade(1, showDuration);
                    await _fadeTweener;
                    _fadeTweener = null;
                }
                else
                {
                    canvasGroup.alpha = 1;
                    
                    _fadeTweener?.Kill();
                    _fadeTweener = canvasGroup.DOFade(0, showDuration);
                    await _fadeTweener;
                    _fadeTweener = null;
                    gameObject.SetActive(false);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
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
