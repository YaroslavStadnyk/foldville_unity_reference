using System.Collections.Generic;
using Grid.Common;
using MathModule.Structs;

namespace Game.Logic.Internal.Interfaces
{
    public interface IGridDataManager : IBase
    {
        public IDictionary<Int2, TileType> InitialTypes { get; set; }
        public IDictionary<Int2, TileType> Types { get; }
        public IDictionary<Int2, string> Captures { get; }
    }
}