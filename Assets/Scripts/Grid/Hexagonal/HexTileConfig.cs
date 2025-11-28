using System;
using System.Linq;
using Core.Configs;
using Core.Extensions;
using Core.Serialization;
using Grid.Common;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Grid.Hexagonal
{
    public class HexTileConfig : BaseConfig<HexTileConfig>
    {
        #region Inspector

#if UNITY_EDITOR
        [MenuItem("Grid/Select " + nameof(HexTileConfig))]
        private static void SelectConfig()
        {
            SelectInstanceInEditor();
        }

        [LabelText("Gizmos Mesh (Editor Only)")] [SerializeField] internal Mesh gizmosMesh;
#endif

        [OnValueChanged(nameof(OnTypePrefabChanged))]
        [LabelText("Prefabs")] [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "TileType", ValueLabel = "HexTile Prefab")]
        [Space] [SerializeField] private SerializedDictionary<TileType, HexTile> typePrefabDictionary = new();

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            var types = Enum.GetValues(typeof(TileType)).Cast<TileType>().ToArray();
            typePrefabDictionary.SetupKeys(types);

            UpdateTypePrefabs();
        }

        private void OnTypePrefabChanged()
        {
            UpdateTypePrefabs();
        }

        internal void UpdateTypePrefabs()
        {
            foreach (var (type, hexTilePrefab) in typePrefabDictionary)
            {
                if (hexTilePrefab == null)
                {
                    Debug.LogError($"{nameof(HexTile)} of type " + type + $" not found! Check out the {nameof(HexTileConfig)}.", this);
                    continue;
                }

                if (hexTilePrefab.Type != type)
                {
                    hexTilePrefab.Type = type;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(hexTilePrefab);
#endif
                }
            }
        }

        #endregion

        public HexTile GetTilePrefab(TileType type)
        {
            if (typePrefabDictionary.TryGetValue(type, out var hexTilePrefab) && hexTilePrefab != null)
            {
                if (hexTilePrefab.Type != type)
                {
                    hexTilePrefab.Type = type;
                }

                return hexTilePrefab;
            }

            Debug.LogError($"{nameof(HexTile)} of type " + type + $" not found! Check out the {nameof(HexTileConfig)}.", this);
            return null;
        }

        public bool HasTilePrefab(TileType type)
        {
            return typePrefabDictionary.TryGetValue(type, out var hexTilePrefab) && hexTilePrefab != null;
        }
    }
}