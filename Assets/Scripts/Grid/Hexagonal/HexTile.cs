using Core;
using Grid.Common;
using MathModule.Structs;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Grid.Hexagonal
{
    public class HexTile : MonoBehaviour, ITile
    {
        #region Inspector

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!gameObject.IsPrefab())
            {
                return;
            }

            var mesh = HexTileConfig.Instance.gizmosMesh;
            if (mesh == null)
            {
                return;
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireMesh(mesh, transform.position, Quaternion.Euler(0, 30, 0), new Vector3(1, 0, 1));
        }
#endif

        [InfoBox("The tile type depends on the " + nameof(HexTileConfig) + ".")]
        [SerializeField] [ReadOnly] private TileType type;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            if (!gameObject.IsPrefab())
            {
                return;
            }

            HexTileConfig.Instance.UpdateTypePrefabs();
        }

        #endregion

        public TileType Type { get => type; internal set => type = value; }

        #region Convertation

        private const float HexSize = 0.5f;
        private const float HalfSqrt3 = 0.866025403784438646f;

        public static Int2 ConvertToIndexPosition(Vector3 position)
        {
            const float delta = HexSize * HalfSqrt3;
            position /= delta;

            var indexPosition = new Int2();
            indexPosition.y = Mathf.RoundToInt(position.z / HalfSqrt3);
            indexPosition.x = Mathf.RoundToInt(position.x - indexPosition.y * 0.5f);

            return indexPosition;
        }

        public static Vector2 ConvertToPosition(Int2 indexPosition)
        {
            const float delta = HexSize * HalfSqrt3;

            var position = new Vector2();
            position.x = indexPosition.x + indexPosition.y * 0.5f;
            position.y = indexPosition.y * HalfSqrt3;

            return position * delta;
        }

        #endregion

        #region Transform

        public Int2 IndexPosition
        {
            get => GetIndexPosition(transform, Space.Self);
            set => SetIndexPosition(transform, value, Space.Self);
        }

        public static Int2 GetIndexPosition(Transform transform, Space space = Space.Self)
        {
            var position = GetPosition(transform, space);
            var indexPosition = ConvertToIndexPosition(position);

            return indexPosition;
        }

        public static void SetIndexPosition(Transform transform, Int2 indexPosition, Space space = Space.Self)
        {
            var position = GetPosition(transform, space);
            var convertedPosition = ConvertToPosition(indexPosition);

            position.x = convertedPosition.x;
            position.z = convertedPosition.y;

            SetPosition(transform, position, space);
        }

        private static Vector3 GetPosition(Transform transform, Space space = Space.Self)
        {
            return space == Space.Self ? transform.localPosition : transform.position;
        }

        private static void SetPosition(Transform transform, Vector3 position, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                transform.localPosition = position;
            }
            else
            {
                transform.position = position;
            }
        }

        #endregion
    }
}