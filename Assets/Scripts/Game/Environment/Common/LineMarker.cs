using Core.Extensions;
using Core.Models;
using Core.Ordinaries;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Environment.Common
{
    public class LineMarker : PoolBehaviour
    {
        #region Inspector

        [Required] [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] public AnimationCurve deltaCurve = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        [SerializeField] public Vector3 deltaVector = Vector3.up;
        [SerializeField] [Min(2)] public int stepsCount = 30;

        [Space] [SerializeField] public PaintableGroup paintGroup = new();

#if UNITY_EDITOR
        [OnInspectorInit] [OnInspectorDispose]
        public void OnValidate()
        {
            if (lineRenderer != null && lineRenderer.positionCount >= 2)
            {
                var position = transform.position;
                var startPosition = position + lineRenderer.GetPosition(0);
                var endPosition = position + lineRenderer.GetPosition(lineRenderer.positionCount - 1);
                SetPositions(startPosition, endPosition);

                if (!paintGroup.targetRenderers.Contains(lineRenderer))
                {
                    paintGroup.targetRenderers.Insert(0, lineRenderer);
                }
            }
        }
#endif

        #endregion

        public void SetPositions(Vector3 startPosition, Vector3 endPosition, Space space = Space.World)
        {
            if (stepsCount < 2)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            var startLocalPosition = Vector3.zero;
            var endLocalPosition = endPosition - startPosition;
            transform.SetPosition(startPosition, space);

            var positions = new Vector3[stepsCount];
            for (var i = 0; i < stepsCount; i++)
            {
                var linearTime = i / (stepsCount - 1.0f);
                var linearPosition = Vector3.LerpUnclamped(startLocalPosition, endLocalPosition, linearTime);

                var curveTime = deltaCurve.Evaluate(linearTime);
                var curvePosition = deltaVector * curveTime;

                var position = linearPosition + curvePosition;
                positions[i] = position;
            }

            lineRenderer.positionCount = stepsCount;
            lineRenderer.SetPositions(positions);
        }
    }
}