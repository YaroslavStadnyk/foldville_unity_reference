using Core.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.UI.Components
{
    public class TimerUI : AnimatedPage
    {
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text timerText;

        [Space] [SerializeField] [Min(0.0001f)] private float alarmValue;
        [SerializeField] private Gradient alarmGradient;
        [SerializeField] private AnimationCurve pulsationScale = AnimationCurve.Linear(0, 1, 1, 1.2f);
        [SerializeField] private float pulsationFrequency = 1.0f;

        private Tweener _timerTween;
        private Color _initialTimerColor;
        private Vector3 _initialScale;

        protected override void Awake()
        {
            base.Awake();

            _initialTimerColor = timerText.color;
            _initialScale = timerText.transform.localScale;
        }

        public void StartTimer(string label, float durationInSeconds)
        {
            labelText.text = label;

            if (durationInSeconds <= 0)
            {
                StopTimer();
            }
            else
            {
                DoTimer(durationInSeconds);
            }

            if (Mathf.RoundToInt(durationInSeconds) == -1)
            {
                timerText.text = Emojis.Infinity;
                Show();
            }
        }

        public void StopTimer()
        {
            if (_timerTween.IsActive())
            {
                _timerTween.Complete(true);
            }
        }

        private Tweener DoTimer(float durationInSeconds)
        {
            if (_timerTween.IsActive())
            {
                _timerTween.ChangeValues(durationInSeconds, 0.0f, durationInSeconds).Restart();
            }
            else
            {
                _timerTween = DOTween.To(TimerSetter, durationInSeconds, 0.0f, durationInSeconds)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                    .OnPlay(Show)
                    .OnComplete(Hide)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }

            return _timerTween;
        }

        private void TimerSetter(float value)
        {
            var seconds = Mathf.CeilToInt(value);
            timerText.text = seconds.ToString();

            var alarmTime = 1f - value / alarmValue;
            timerText.color = value > alarmValue ? _initialTimerColor : alarmGradient.Evaluate(alarmTime);

            var pulsationTime = Mathf.Abs(Mathf.Sin(value * pulsationFrequency * Mathf.PI));
            timerText.transform.localScale = Vector3.LerpUnclamped(_initialScale, _initialScale * pulsationScale.Evaluate(alarmTime), pulsationTime);
        }
    }
}