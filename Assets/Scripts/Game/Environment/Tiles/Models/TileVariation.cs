using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.Structs;
using Grid.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Game.Environment.Tiles.Models
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class TileVariation
    {
        #region Inspector

        [ListDrawerSettings(CustomAddFunction = nameof(AddTileType))] 
        [HorizontalGroup] [OdinSerialize] public HashSet<TileType> IncludedTileTypes = new();
        [HorizontalGroup] [OdinSerialize] public List<GameObjectRange> GameObjectRanges = new();

        private void AddTileType()
        {
            var tileTypes = Enum.GetValues(typeof(TileType)).Cast<TileType>();
            foreach (var tileType in tileTypes)
            {
                if (IncludedTileTypes.Add(tileType))
                {
                    return;
                }
            }
        }

        #endregion

        public void UpdateGameObjects(IEnumerable<TileType> tileTypes)
        {
            var includedTileTypesCount = CountIncludedTileTypes(tileTypes);
            foreach (var gameObjectRange in GameObjectRanges)
            {
                gameObjectRange.SetActive(includedTileTypesCount);
            }
        }

        public int CountIncludedTileTypes(IEnumerable<TileType> tileTypes)
        {
            var includedTileTypesCount = 0;

            var tileTypesCount = tileTypes.CountDuplicates();
            foreach (var (tileType, tileTypeCount) in tileTypesCount)
            {
                if (IncludedTileTypes.Contains(tileType))
                {
                    includedTileTypesCount += tileTypeCount;
                }
            }

            return includedTileTypesCount;
        }
    }
}