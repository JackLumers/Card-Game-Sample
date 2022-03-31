using System;
using System.Threading;
using CardGameSample.Scripts.Battlefield;
using CardGameSample.Scripts.Input;
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
    public class HandCardView : MonoBehaviour, ICardView, IHoldableObject
    {
        #region SerializableFields

        [SerializeField] private CardPresenter presenter;

        [Space] [SerializeField] private TextMeshProUGUI attackPointsText;
        [SerializeField] private TextMeshProUGUI healthPointsText;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image backgroundImage;

        [Space] [SerializeField] private Color backgroundTintColor;

        [Space] [SerializeField] private string attackPrefix = "ATK:";
        [SerializeField] private string healthPrefix = "HP:";

        [Space] [SerializeField] private float scaleSpeed = 1f;
        [SerializeField] private float scaleWhileHoldValue = 1.2f;

        #endregion

        #region Fields

        private AsyncOperationHandle<Sprite> _cardSpriteHandle;
        [CanBeNull] private CancellationTokenSource _cardSpriteLoadingCts;
        [CanBeNull] private TweenerCore<Vector3, Vector3, VectorOptions> _movingTweener;
        [CanBeNull] private TweenerCore<Vector3, Vector3, VectorOptions> _scalingTweener;
        [CanBeNull] private TweenerCore<Quaternion, Quaternion, NoOptions> _rotationTweener;


        private Color _baseBackgroundColor;
        private RectTransform _rectTransform;

        #endregion

        #region Properties

        public CardPresenter Presenter => presenter;

        public GameObject GameObject => gameObject;

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

        #endregion

        #region Methods

        public async UniTask AnimatePlaceCardOnCell(HandCardView handCardView, BattlefieldCell battlefieldCell)
        {
            await UniTask.WhenAll
            (
                handCardView.Move(battlefieldCell.CellCanvas.transform.position, 0.5f, false),
                handCardView.Rotate(battlefieldCell.CellCanvas.transform.rotation, 0.5f, false)
            );
        }

        public async UniTask MoveLocal(Vector3 localPosition, float speedOrDuration, bool speedBased)
        {
            try
            {
                gameObject.SetActive(true);
                _movingTweener?.Kill();
                _movingTweener = _rectTransform.DOLocalMove(
                    new Vector3(localPosition.x, localPosition.y, localPosition.z),
                    speedOrDuration).SetSpeedBased(speedBased);
                await _movingTweener;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public async UniTask Move(Vector3 worldPosition, float speedOrDuration, bool speedBased)
        {
            try
            {
                gameObject.SetActive(true);
                _movingTweener?.Kill();
                _movingTweener = _rectTransform.DOMove(
                    new Vector3(worldPosition.x, worldPosition.y, worldPosition.z),
                    speedOrDuration).SetSpeedBased(speedBased);
                await _movingTweener;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public async UniTask Rotate(Quaternion rotation, float speedOrDuration, bool speedBased)
        {
            try
            {
                gameObject.SetActive(true);
                _rotationTweener?.Kill();
                _rotationTweener = _rectTransform.DORotateQuaternion(rotation,
                    speedOrDuration).SetSpeedBased(speedBased);
                await _rotationTweener;
                _rotationTweener = null;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public UniTask MoveByHold(Vector2 localPosition, float speed)
        {
            return MoveLocal(
                new Vector3(localPosition.x, localPosition.y, _rectTransform.localPosition.z),
                speed, true);
        }

        public void Hold(Vector2 screenPointerPosition, bool hold)
        {
            if (hold)
            {
                _scalingTweener?.Kill();
                _scalingTweener = transform.DOScale(
                        new Vector3(scaleWhileHoldValue, scaleWhileHoldValue, 1f), scaleSpeed)
                    .SetSpeedBased(true);
            }
            else
            {
                _scalingTweener?.Kill();
                _scalingTweener = transform.DOScale(
                        new Vector3(1, 1, 1), scaleSpeed)
                    .SetSpeedBased(true);
            }
        }

        public void TintBackground(bool tint)
        {
            if (tint)
            {
                backgroundImage.color = backgroundTintColor;
            }
            else
            {
                backgroundImage.color = _baseBackgroundColor;
            }
        }

        private async UniTask LoadCardSprite(string key)
        {
            _cardSpriteLoadingCts?.Cancel();
            _cardSpriteLoadingCts = new CancellationTokenSource();
            
            try
            {
                if (_cardSpriteHandle.IsValid())
                {
                    Addressables.Release(_cardSpriteHandle);
                }

                _cardSpriteHandle = Addressables.LoadAssetAsync<Sprite>(key);
                
                var gettingSpriteTask = await _cardSpriteHandle
                    .WithCancellation(_cardSpriteLoadingCts.Token)
                    .SuppressCancellationThrow();
                
                if (!gettingSpriteTask.IsCanceled)
                {
                    cardImage.sprite = gettingSpriteTask.Result;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        #endregion
        
        #region MonoBehaviourCallbacks

        private void Awake()
        {
            _rectTransform = (RectTransform) transform;
            _baseBackgroundColor = backgroundImage.color;
        }

        private void OnDestroy()
        {
            if (_cardSpriteHandle.IsValid())
            {
                Addressables.Release(_cardSpriteHandle);
            }
        }

        #endregion
    }
}