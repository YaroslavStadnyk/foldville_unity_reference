using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Environment.Tiles.Models
{
    public interface ITilePath
    {
        public void Initialize();
        public bool Switch(IEnumerable<int> pathIndex);
        public void Reset();
    }

    [Serializable] [HideReferenceObjectPicker]
    public class QuickHexTilePath : ITilePath
    {
        [Space] [OdinSerialize] private GameObject _originObject;
        [LabelText("Direction Object (→)")] [OdinSerialize] private GameObject _directionObject;

        [NonSerialized] private GameObject[] _directionObjects;

        public void Initialize()
        {
            _directionObjects = new GameObject[6];
            _directionObjects[0] = _directionObject;

            Reset();
        }

        public bool Switch(IEnumerable<int> pathIndex)
        {
            Reset();

            if (_originObject != null)
            {
                _originObject.SetActive(true);
            }

            foreach (var directionIndex in pathIndex)
            {
                if (directionIndex < 0 || directionIndex >= _directionObjects.Length)
                {
                    continue;
                }

                var directionObject = _directionObjects[directionIndex];
                if (directionObject == null)
                {
                    directionObject = SpawnDirectionObject(directionIndex);
                    _directionObjects[directionIndex] = directionObject;
                }

                directionObject.SetActive(true);
            }

            return true;
        }

        private GameObject SpawnDirectionObject(int directionIndex)
        {
            var directionObjectsParent = _directionObject.transform.parent;
            var directionAngle = directionIndex * HexConstants.AngleDegree;

            var directionObject = Object.Instantiate(_directionObject, directionObjectsParent);
            directionObject.transform.RotateAround(directionObjectsParent.position, Vector3.up, directionAngle);

            return directionObject;
        }

        public void Reset()
        {
            if (_originObject != null)
            {
                _originObject.SetActive(false);
            }

            foreach (var directionObject in _directionObjects)
            {
                if (directionObject != null)
                {
                    directionObject.SetActive(false);
                }
            }
        }
    }

    [Serializable] [HideReferenceObjectPicker]
    public class ComplexHexTilePath : ITilePath
    {
        [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Type", ValueLabel = "GameObject")] 
        [LabelText("Path Objects (Custom)")] [OdinSerialize] private Dictionary<string, GameObject> _pathTypeObjects = new();

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            _pathTypeObjects.SetupKeys(PathTypes);
        }

        public void Initialize()
        {
            Reset();
        }

        public bool Switch(IEnumerable<int> pathIndex)
        {
            Reset();

            var pathType = GetPathType(pathIndex.ToArray(), out var angle);
            if (!_pathTypeObjects.TryGetValue(pathType, out var pathObject) || pathObject == null)
            {
                return false;
            }

            pathObject.transform.rotation = Quaternion.Euler(pathObject.transform.rotation.eulerAngles.WithY(-angle));
            pathObject.SetActive(true);

            return true;
        }

        private static string GetPathType(int[] pathIndex, out int angle)
        {
            angle = 0;

            var pathIndexLength = pathIndex.Length;
            switch (pathIndexLength)
            {
                case 0:
                    return PathTypes[0];
                case HexConstants.AnglesCount:
                    return PathTypes[^1];
            }

            foreach (var pathType in PathTypes)
            {
                if (pathType.Length != pathIndexLength)
                {
                    continue;
                }

                for (angle = HexConstants.AngleDegree; angle <= HexConstants.TotalAnglesDegree; angle += HexConstants.AngleDegree)
                {
                    for (var i = 0; i < pathIndexLength; i++)
                    {
                        pathIndex[i] += 1;
                        pathIndex[i] %= HexConstants.AnglesCount;
                    }

                    if (pathType == ConvertToPathType(pathIndex))
                    {
                        return pathType;
                    }
                }
            }

            return "";
        }

        private static string ConvertToPathType(int[] pathIndex)
        {
            Array.Sort(pathIndex);

            var pathType = "";
            foreach (var directionIndex in pathIndex)
            {
                pathType += DirectionTypes[directionIndex];
            }

            return pathType;
        }

        public void Reset()
        {
            foreach (var pathObject in _pathTypeObjects.Values)
            {
                if (pathObject != null)
                {
                    pathObject.SetActive(false);
                }
            }
        }

        private static readonly string[] PathTypes =
        {
            "",
            "→",
            "→↘",
            "→↙",
            "→←",
            "→↘↙",
            "→↙←",
            "→↙↖",
            "→←↖",
            "→↘↙←",
            "→↘←↖",
            "→↙←↖",
            "→↘↙←↖",
            "→↘↙←↖↗"
        };

        private static readonly string[] DirectionTypes =
        {
            "→",
            "↘",
            "↙",
            "←",
            "↖",
            "↗"
        };
    }    
}