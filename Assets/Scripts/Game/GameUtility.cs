using System.Collections.Generic;
using System.Linq;
using Board.Structs;
using Core.Extensions;
using Game.Logic.Common.Models;
using Game.Logic.Configs;
using Grid.Common;
using MathModule.Structs;

namespace Game
{
    public static class GameUtility
    {
        #region Linq

        public static bool Contains(this IEnumerable<ITile> tiles, TileType requiredType)
        {
            foreach (var tile in tiles)
            {
                if (tile == null)
                {
                    continue;
                }

                if (tile.Type == requiredType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this IEnumerable<TileType> types, TileType requiredType)
        {
            foreach (var type in types)
            {
                if (type == requiredType)
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<TileType> GetTypes(this IEnumerable<ITile> tiles)
        {
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    yield return tile.Type;
                }
            }
        }

        #endregion

        public static BuildingDefinition GetBuildingDefinition(this ITile tile)
        {
            return tile == null ? null : GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(tile.Type);
        }

        public static BuildingDefinition GetBuildingDefinition(this CardInfo cardInfo)
        {
            return GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(cardInfo.Type);
        }
    }
}