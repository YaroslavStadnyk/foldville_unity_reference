using System.Collections.Generic;
using System.Linq;
using MathModule.Structs;
using UnityEngine;

namespace Grid.Common
{
    public abstract class TileGrid : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Recalculate();
        }

        public abstract void Recalculate();

        #region Get

        public abstract ITile GetTile(Int2 indexPosition);

        public abstract List<ITile> GetTiles();

        public List<ITile> GetTiles(Int2 indexPosition, IEnumerable<Int2> form)
        {
            return form.Select(localIndexPosition => GetTile(indexPosition + localIndexPosition)).ToList();
        }

        #endregion

        #region Create

        public abstract bool CreateTile(Int2 indexPosition, TileType type);

        public void CreateTiles(Int2 indexPosition, TileType type, IEnumerable<Int2> form)
        {
            foreach (var localIndexPosition in form)
            {
                CreateTile(indexPosition + localIndexPosition, type);
            }
        }

        #endregion

        #region Remove

        public abstract bool RemoveTile(Int2 indexPosition);

        public void RemoveTiles(Int2 indexPosition, IEnumerable<Int2> form)
        {
            foreach (var localIndexPosition in form)
            {
                RemoveTile(indexPosition + localIndexPosition);
            }
        }

        #endregion

        public abstract Int2 ConvertToIndexPosition(Vector3 position);
        public abstract Vector2 ConvertToPosition(Int2 indexPosition);
    }
}