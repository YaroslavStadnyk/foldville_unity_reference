using System;

namespace Game.Logic.Common.Enums
{
    [Serializable]
    public enum PartyState
    {
        Waiting,
        Starting,
        Loading,
        Playing,
        Completing
    }
}