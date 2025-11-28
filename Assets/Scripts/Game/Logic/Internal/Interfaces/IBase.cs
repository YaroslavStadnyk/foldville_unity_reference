using System;

namespace Game.Logic.Internal.Interfaces
{
    public interface IBase
    {
        public void Initialize(Action<IBase> callback);
        public void Destroy();
    }
}