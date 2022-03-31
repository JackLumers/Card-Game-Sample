using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CardGameSample.Scripts.Timer
{
    public abstract class ATimer : MonoBehaviour
    {
        protected System.Timers.Timer Timer;

        private bool _destroyed = false;
        private CancellationTokenSource _cancellationTokenSource;

        private void OnDestroy()
        {
            StopTimer();
            _destroyed = true;
        }

        private void OnEnable()
        {
            if (Timer is {Enabled: true})
            {
                UiHandleTimer((float) Timer.Interval, _cancellationTokenSource.Token)
                    .SuppressCancellationThrow().Forget();
            }
        }

        private void OnDisable()
        {
            StopTimer();
        }

        public async UniTask StartTimer(float durationInMillis, [CanBeNull] Action onDone = null)
        {
            try
            {
                gameObject.SetActive(true);
            
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
            
                if (durationInMillis < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(durationInMillis), "Timer duration argument out of range! Must be >= 0.");
                }

                Timer?.Dispose();
                Timer = new System.Timers.Timer(durationInMillis);
                Timer.Start();
                Timer.AutoReset = false;
                Timer.Elapsed += (sender, args) => HandleTimer(onDone);

                await UiHandleTimer(durationInMillis, _cancellationTokenSource.Token)
                    .SuppressCancellationThrow();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        
        public void StopTimer()
        {
            _cancellationTokenSource?.Cancel();
            Timer?.Dispose();
        }

        public void StopAndSetDuration(float durationInMillis)
        {
            StopTimer();
            OnStopAndSetDuration(durationInMillis);
        }
        
        protected abstract UniTask UiHandleTimer(float durationInMillis, CancellationToken cancellationToken);

        protected abstract void OnStopAndSetDuration(float durationInMillis);

        private void HandleTimer([CanBeNull] Action onDone)
        {
            if (_destroyed) return;
            Debug.Log("Timer done!");
            onDone?.Invoke();
        }
        
        [ContextMenu("[RuntimeTest] -> Test timer")]
        public void TestTimer()
        {
            if (Application.isPlaying)
            {
                StartTimer(5000).Forget();
            }
        }
    }
}