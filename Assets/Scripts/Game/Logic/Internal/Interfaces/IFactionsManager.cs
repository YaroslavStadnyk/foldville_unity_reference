using System.Collections.Generic;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Grid.Common;

namespace Game.Logic.Internal.Interfaces
{
    public interface IFactionsManager : IBase
    {
        public FactionsDefinition Definition { get; set; }

        public IReadOnlyDictionary<string, List<TileType>> Origins { get; }
        public IReadOnlyDictionary<ExplorationKey, string> Explorations { get; }

        public void Register(string playerID);
        public void Unregister(string playerID);

        public bool IsAvailable(string playerID);
        public bool IsAvailable(string playerID, TileType type);
        public List<TileType> GetOrigin(string playerID);

        public void Explore(string playerID, TileType type);
    }
}