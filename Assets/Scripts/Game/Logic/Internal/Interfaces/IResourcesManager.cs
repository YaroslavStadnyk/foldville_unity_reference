using System.Collections.Generic;
using Game.Logic.Common.Structs;

namespace Game.Logic.Internal.Interfaces
{
    public interface IResourcesManager : IBase
    {
        public IDictionary<ResourceKey, int> Resources { get; }
    }
}