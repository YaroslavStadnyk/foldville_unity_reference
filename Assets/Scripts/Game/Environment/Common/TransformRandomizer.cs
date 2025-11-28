using System;
using System.Collections.Generic;
using Core.Extensions;
using Core.Structs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Environment.Common
{
    public class TransformRandomizer : SerializedMonoBehaviour
    {
        #region Inspector

        [OdinSerialize] [PropertyOrder(-2)] private List<Transform> _targetTransforms = new();
        [OdinSerialize] private Vector3Range _positionOffset;
        [OdinSerialize] private Vector3Range _rotationOffset;
        [OdinSerialize] private Vector3Range _scaleOffset;

        [Button] [PropertyOrder(-1)]
        private void CastChildTransforms()
        {
            _targetTransforms = transform.GetChilds();
        }

        #endregion

        private readonly List<Vector3> _initialLocalPositions = new();
        private readonly List<Vector3> _initialLocalRotations = new();
        private readonly List<Vector3> _initialLocalScales = new();

        private void Awake()
        {
            foreach (var targetTransform in _targetTransforms)
            {
                _initialLocalPositions.Add(targetTransform.localPosition);
                _initialLocalRotations.Add(targetTransform.localRotation.eulerAngles);
                _initialLocalScales.Add(targetTransform.localScale);
            }
        }

        private void OnEnable()
        {
            Randomize();
        }

        public void Randomize()
        {
            for (var i = 0; i < _targetTransforms.Count; i++)
            {
                var targetTransform = _targetTransforms[i];

                targetTransform.localPosition = _initialLocalPositions[i] + _positionOffset.random;
                targetTransform.localRotation = Quaternion.Euler(_initialLocalRotations[i] + _rotationOffset.random);
                targetTransform.localScale = _initialLocalScales[i] + _scaleOffset.random;
            }
        }
    }
}