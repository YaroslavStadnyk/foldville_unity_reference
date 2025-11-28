using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Environment.Common
{
    public class LineCap : MonoBehaviour
    {
        #region Inspector

        [Required] [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int index = -1;

        [Space] [SerializeField] [ChildGameObjectsOnly] private ObjectFacing facing;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (lineRenderer != null)
            {
                UpdatePosition();
            }

            if (facing != null)
            {
                UpdateRotation();
                facing.UpdateRotation();
            }
        }
#endif

        #endregion

        private void OnEnable()
        {
            DoFacing();
        }

        private void OnDisable()
        {
            _lineCapTween.Pause();
        }

        private Tweener _lineCapTween;

        private void DoFacing()
        {
            if (_lineCapTween.IsActive())
            {
                _lineCapTween.Play();
            }
            else
            {
                _lineCapTween = DOTween.To(LineCapSetter, 0, 1, 1)
                    .SetLoops(-1)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }
        }

        private void LineCapSetter(float value)
        {
            if (lineRenderer != null)
            {
                UpdatePosition();
            }

            if (facing != null)
            {
                UpdateRotation();
            }
        }

        private void UpdatePosition()
        {
            var positionCount = lineRenderer.positionCount;
            if (positionCount <= index || positionCount < -index)
            {
                return;
            }

            var position = GetLinePosition(index);
            transform.position = lineRenderer.useWorldSpace ? position : lineRenderer.transform.position + position;
        }

        private void UpdateRotation()
        {
            var startIndex = index;
            var endIndex = index >= 0 ? index + 1 : index - 1;

            var positionCount = lineRenderer.positionCount;
            if (positionCount <= endIndex || positionCount < -endIndex)
            {
                return;
            }

            var startPosition = GetLinePosition(startIndex);
            var endPosition = GetLinePosition(endIndex);
            var lineDirection = (endPosition - startPosition).normalized;
            facing.upwards = lineDirection;
        }

        private Vector3 GetLinePosition(int positionIndex)
        {
            return positionIndex >= 0
                ? lineRenderer.GetPosition(positionIndex)
                : lineRenderer.GetPosition(lineRenderer.positionCount + positionIndex);
        }
    }
}