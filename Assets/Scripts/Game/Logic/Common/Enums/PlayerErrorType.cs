using System;

namespace Game.Logic.Common.Enums
{
    [Serializable]
    public enum PlayerErrorType
    {
        None,
        NullReference,
        PlayerNotFound,
        ItemNotFound,
        NotEnoughResources,
        UnavailableAction
    }
}