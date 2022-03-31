using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CardGameSample.Scripts.Timer
{
    public sealed class TextTimer : ATimer
    {
        [SerializeField] private TextMeshProUGUI timerText = null;
        
        public string timeFormat = @"mm\:ss";

        private void Awake()
        {
            timerText.text = new TimeSpan(0).ToString(timeFormat);
        }

        protected override async UniTask UiHandleTimer(float durationInMillis, CancellationToken cancellationToken)
        {
            timerText.gameObject.SetActive(true);

            timerText.text = TimeSpan.FromMilliseconds(durationInMillis).ToString(timeFormat);
            await TimerProcess(durationInMillis - durationInMillis % 1000, cancellationToken)
                .SuppressCancellationThrow();
        }

        protected override void OnStopAndSetDuration(float durationInMillis)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = TimeSpan.FromMilliseconds(durationInMillis).ToString(timeFormat);
        }

        private async UniTask TimerProcess(float timerStartMillis, CancellationToken cancellationToken)
        {
            while (timerStartMillis > 0)
            {
                await UniTask.Delay(1000, cancellationToken: cancellationToken);
                
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Timer process canceled.");
                    return;
                }
                
                timerStartMillis -= 1000f;
                if (timerStartMillis < 0) timerStartMillis = 0;

                timerText.text = TimeSpan.FromMilliseconds(timerStartMillis).ToString(timeFormat);
            }
        }
    }
}