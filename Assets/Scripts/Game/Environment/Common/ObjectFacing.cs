using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Environment.Common
{
    public class ObjectFacing : MonoBehaviour
    {
        [Serializable]
        public enum AlignmentMode
        {
            Screen,
            Camera
        }

        [SerializeField] private AlignmentMode alignmentMode;
        [SerializeField] private Vector3 rotationOffset = Vector3.zero;
        [SerializeField] [ReadOnly] public Vector3 upwards = Vector3.up;

        private void OnEnable()
        {
            DoFacing();
        }

        private void OnDisable()
        {
            _facingTween.Pause();
        }

        private Tweener _facingTween;

        private void DoFacing()
        {
            if (_facingTween.IsActive())
            {
                _facingTween.Play();
            }
            else
            {
                _facingTween = DOTween.To(FacingSetter, 0, 1, 1)
                    .SetLoops(-1)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }
        }

        private void FacingSetter(float value)
        {
            UpdateRotation();
        }

        public void UpdateRotation()
        {
            var targetCamera = GameManager.Instance.Camera;
            if (targetCamera == null)
            {
                return;
            }

            if (alignmentMode is AlignmentMode.Screen)
            {
                UpdateScreenAlignment(targetCamera);
            }
            else
            {
                UpdateCameraAlignment(targetCamera);
            }
        }

        private void UpdateScreenAlignment(Camera targetCamera)
        {
            var screenRotation = targetCamera.transform.rotation;
            transform.rotation = screenRotation * Quaternion.Euler(rotationOffset);
        }

        private void UpdateCameraAlignment(Camera targetCamera)
        {
            var lookDirection = (transform.position - targetCamera.transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(lookDirection, upwards);
            transform.rotation = lookRotation * Quaternion.Euler(rotationOffset);
        }
    }
}