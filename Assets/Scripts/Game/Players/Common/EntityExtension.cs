using UnityEngine;

namespace Game.Players.Common
{
    public abstract class EntityExtension
    {
        private EntityBehaviour _contextBehaviour;

        public virtual void Initialize(EntityBehaviour contextBehaviour)
        {
            _contextBehaviour = contextBehaviour;
        }

        public EntityBehaviour ContextBehaviour
        {
            get
            {
                if (_contextBehaviour == null)
                {
                    Debug.LogError($"{typeof(EntityBehaviour)}: {nameof(_contextBehaviour)} is null.");
                }

                return _contextBehaviour;
            }
        }
    }
}