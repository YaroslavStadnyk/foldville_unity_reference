using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;

namespace Game.Players.Common.Extensions
{
    public class Resources : EntityExtension
    {
        public int this[ResourceType type]
        {
            get => GameManager.Instance.Resources.Data.FirstOrDefault(new ResourceKey(ContextBehaviour.LatestID, type));
            set => GameManager.Instance.Resources.Data[new ResourceKey(ContextBehaviour.LatestID, type)] = value;
        }
    }
}