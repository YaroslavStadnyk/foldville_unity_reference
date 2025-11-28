using System;
using Core.Extensions;
using DG.Tweening;
using UnityEngine;

namespace Game.Environment
{
    public class CameraAnimator : MonoBehaviour
    {
        [Serializable]
        public enum AnimationMode
        {
            Static,
            Dynamic
        }

        [Serializable]
        public class StaticAnimation
        {
            [SerializeField] private float timeScale = 1f;
            [SerializeField] private Transform subjectPoint;
            [SerializeField] private Transform rotateAroundPoint;
            [SerializeField] private Transform lookAtPoint;

            private float _time;

            public float Time
            {
                get => timeScale == 0f ? 0f : _time / timeScale;
                set => _time = value * timeScale;
            }

            public (Vector3, Quaternion) GetPositionAndRotation()
            {
                var position = subjectPoint.position.RotateAround(rotateAroundPoint.position, new Vector3(0, _time, 0));
                var rotation = Quaternion.LookRotation((lookAtPoint.position - position).normalized);
                return (position, rotation);
            }
        }

        [Serializable]
        public class DynamicAnimation
        {
            [SerializeField] private float timeScale = 1f;
            [SerializeField] private Transform pointA;
            [SerializeField] private Transform pointB;

            private float _time;

            public float Time
            {
                get => timeScale == 0f ? 0f : _time * Distance / timeScale;
                set => _time = Mathf.Clamp01(value * timeScale / Distance);
            }

            private float Distance => Vector3.Distance(pointA.position, pointB.position);

            public (Vector3, Quaternion) GetPositionAndRotation()
            {
                var position = Vector3.LerpUnclamped(pointA.position, pointB.position, _time);
                var rotation = Quaternion.LerpUnclamped(pointA.rotation, pointB.rotation, _time);
                return (position, rotation);
            }
        }

        #region Inspector

        [SerializeField] public Camera targetCamera;

        [Space] [SerializeField] private AnimationMode animationMode;
        [SerializeField] public StaticAnimation staticAnimation = new();
        [SerializeField] public DynamicAnimation dynamicAnimation = new();

        [Space] [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private Ease transitionEase = Ease.InOutSine;

        #endregion

        private float _transitionTime;

        private void Start()
        {
            _transitionTime = animationMode is AnimationMode.Static ? 0f : 1f;
        }

        public AnimationMode Mode
        {
            get => animationMode;
            set
            {
                if (animationMode == value)
                {
                    return;
                }

                animationMode = value;
                DoTransition(animationMode is AnimationMode.Static ? 0f : 1f, transitionDuration, transitionEase);
            }
        }

        private void Update()
        {
            if (_transitionTime % 1f == 0f)
            {
                UpdateAnimations(animationMode);
            }
            else
            {
                // UpdateAnimations(_transitionTime);
            }
        }

        private void UpdateAnimations(AnimationMode mode)
        {
            if (mode is AnimationMode.Static)
            {
                UpdateStaticAnimation();
            }
            else
            {
                UpdateDynamicAnimation();
            }
        }

        private void UpdateStaticAnimation()
        {
            staticAnimation.Time = Time.unscaledTime;
            var (position, rotation) = staticAnimation.GetPositionAndRotation();
            targetCamera.transform.SetPositionAndRotation(position, rotation);
        }

        private void UpdateDynamicAnimation()
        {
            dynamicAnimation.Time += Input.mouseScrollDelta.y;
            var (position, rotation) = dynamicAnimation.GetPositionAndRotation();
            targetCamera.transform.SetPositionAndRotation(position, rotation);
        }

        private void UpdateAnimations(float transitionTime)
        {
            staticAnimation.Time = Time.unscaledTime;
            var (staticPosition, staticRotation) = staticAnimation.GetPositionAndRotation();

            dynamicAnimation.Time += Input.mouseScrollDelta.y;
            var (dynamicPosition, dynamicRotation) = dynamicAnimation.GetPositionAndRotation();

            var position = Vector3.LerpUnclamped(staticPosition, dynamicPosition, transitionTime);
            var rotation = Quaternion.LerpUnclamped(staticRotation, dynamicRotation, transitionTime);
            targetCamera.transform.SetPositionAndRotation(position, rotation);
        }

        #region Tweeners

        private Tweener _transitionTweener;

        private Tweener DoTransition(float endValue, float duration, Ease ease = Ease.InOutSine)
        {
            if (_transitionTweener.IsActive())
            {
                _transitionTweener.ChangeEndValue(endValue, duration, true)
                    .SetEase(ease)
                    .Restart();
            }
            else
            {
                _transitionTweener = DOTween.To(TransitionSetter, _transitionTime, endValue, duration)
                    .SetEase(ease)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }

            return _transitionTweener;
        }

        private void TransitionSetter(float value)
        {
            _transitionTime = value;
            UpdateAnimations(_transitionTime);
        }

        #endregion
    }
}
