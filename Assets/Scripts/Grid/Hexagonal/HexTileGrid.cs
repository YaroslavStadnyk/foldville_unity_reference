using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Grid.Common;
using MathModule.Models;
using MathModule.Structs;
using UnityEngine;

namespace Grid.Hexagonal
{
    public class HexTileGrid : TileGrid
    {
        protected readonly HexMap<HexTile> HexTileMap = new();

        [SerializeField] private bool destroyTileEnabled = true;

        public event Action<HexTile> OnTileCreated = null;
        public event Action<HexTile> OnTileRemoved = null;

        protected override void Awake()
        {
            base.Awake();

            if (destroyTileEnabled)
            {
                OnTileRemoved += DestroyTile;
            }
        }

        public override void Recalculate()
        {
            HexTileMap.Clear();

            foreach (var child in transform.GetChilds())
            {
                if (child.TryGetComponent(out HexTile hexTile))
                {
                    var indexPosition = hexTile.IndexPosition;
                    hexTile.IndexPosition = indexPosition;

                    HexTileMap.Set(indexPosition, hexTile);
                }
            }
        }

        public override ITile GetTile(Int2 indexPosition)
        {
            return HexTileMap.Get(indexPosition);
        }

        public override List<ITile> GetTiles()
        {
            return HexTileMap.Cells.Values.Cast<ITile>().ToList();
        }

        public override bool CreateTile(Int2 indexPosition, TileType type)
        {
            var existedTile = GetTile(indexPosition);
            if (existedTile != null)
            {
                Debug.LogWarning("You're trying to create a tile on a busy index position!");
                return false;
            }

            var hexTilePrefab = HexTileConfig.Instance.GetTilePrefab(type);
            var hexTile = Instantiate(hexTilePrefab, transform);
            hexTile.IndexPosition = indexPosition;

            HexTileMap.Set(indexPosition, hexTile);

            OnTileCreated?.Invoke(hexTile);
            return true;
        }

        public override bool RemoveTile(Int2 indexPosition)
        {
            if (HexTileMap.Remove(indexPosition, out var removedHexTile))
            {
                OnTileRemoved?.Invoke(removedHexTile);
                return true;
            }

            return false;
        }

        private static void DestroyTile(HexTile hexTile)
        {
            if (hexTile != null)
            {
                Destroy(hexTile.gameObject);
            }
        }

        public override Int2 ConvertToIndexPosition(Vector3 position)
        {
            return HexTile.ConvertToIndexPosition(position);
        }

        public override Vector2 ConvertToPosition(Int2 indexPosition)
        {
            return HexTile.ConvertToPosition(indexPosition);
        }
    }
}