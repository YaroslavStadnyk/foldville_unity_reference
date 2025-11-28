using System;
using System.Linq;
using Core.Interfaces;
using Core.Serialization;
using Game.Environment.Common;
using Grid.Common;
using Grid.Hexagonal;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.Environment.Tiles
{
    public class HexTileDynamic : MonoBehaviour, IPaintable
    {
        #region Inspector

        [SerializeField] private bool castChildRenderers = true;

        [DictionaryDrawerSettings(KeyLabel = "Type", ValueLabel = "Instance")]
        [Space] [SerializeField] private SerializedDictionary<TileType, HexTile> tileInstances = new();

        [Button]
        private void ReplaceTileInstances()
        {
#if UNITY_EDITOR
            var types = Enum.GetValues(typeof(TileType)).Cast<TileType>().ToArray();
            foreach (var type in types)
            {
                var tilePrefab = HexTileConfig.Instance.GetTilePrefab(type);
                if (tilePrefab == null)
                {
                    continue;
                }

                if (tileInstances.TryGetValue(type, out var oldTileInstance) && oldTileInstance != null)
                {
                    DestroyImmediate(oldTileInstance.gameObject);
                }

                var newTileInstance = PrefabUtility.InstantiatePrefab(tilePrefab, transform) as HexTile;
                if (newTileInstance == null)
                {
                    return;
                }

                tileInstances[type] = newTileInstance;

                if (castChildRenderers && newTileInstance.TryGetComponent<ObjectPainter>(out var objectPainter))
                {
                    objectPainter.GetPaintableGroup().targetRenderers = objectPainter.GetComponentsInChildren<Renderer>().ToList();
                }
            }
#endif
        }

        #endregion

        private HexTile _activeTile;
        private ObjectPainter _activePainter;

        private Color _lastColor = Color.clear;
        private float _lastFlow = 0.0f;

        private void Awake()
        {
            Reset();

            foreach (var tileInstance in tileInstances.Values)
            {
                tileInstance.gameObject.SetActive(false);
            }
        }

        private HexTile GetTileInstance(TileType type)
        {
            if (tileInstances.TryGetValue(type, out var tileInstance) && tileInstance != null)
            {
                return tileInstance;
            }

            var tilePrefab = HexTileConfig.Instance.GetTilePrefab(type);
            if (tilePrefab == null)
            {
                return null;
            }

            var newTileInstance = Instantiate(tilePrefab, transform);
            if (newTileInstance == null)
            {
                return null;
            }

            tileInstances[type] = newTileInstance;

            if (castChildRenderers && newTileInstance.TryGetComponent<ObjectPainter>(out var objectPainter))
            {
                objectPainter.GetPaintableGroup().targetRenderers = objectPainter.GetComponentsInChildren<Renderer>(true).ToList();
            }

            return newTileInstance;
        }

        public void SetType(TileType type)
        {
            if (_activeTile != null)
            {
                _activeTile.gameObject.SetActive(false);
            }

            _activeTile = GetTileInstance(type);
            if (_activeTile == null)
            {
                return;
            }

            _activeTile.gameObject.SetActive(true);

            if (_activeTile.TryGetComponent(out _activePainter))
            {
                _activePainter.SetColor(_lastColor, _lastFlow);
            }
        }

        /// <param name="color"> target color. </param>
        /// <param name="flow"> use 0% of initial color and 100% of target color if equals 1F. </param>
        public void SetColor(Color color, float flow = 1.0f)
        {
            _lastColor = color;
            _lastFlow = flow;

            if (_activePainter != null)
            {
                _activePainter.SetColor(color, flow);
            }
        }

        public void ResetColor()
        {
            SetColor(Color.clear, 0.0f);
        }

        public void Reset()
        {
            SetColor(Color.clear, 0.0f);

            if (_activeTile != null)
            {
                _activeTile.gameObject.SetActive(false);
            }

            _activeTile = null;
        }
    }
}