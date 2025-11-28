using UnityEngine;

namespace Game.Players.Player
{
    public abstract class PlayerExtension : MonoBehaviour
    {
        private PlayerBehaviour _contextBehaviour;

        public virtual void Initialize(PlayerBehaviour contextBehaviour)
        {
            _contextBehaviour = contextBehaviour;
        }

        public PlayerBehaviour ContextBehaviour
        {
            get
            {
                if (_contextBehaviour == null)
                {
                    Debug.LogError($"{typeof(PlayerBehaviour)}: {nameof(_contextBehaviour)} is null.");
                }

                return _contextBehaviour;
            }
        }

        public virtual bool IsEnabled
        {
            get
            {
                if (_contextBehaviour == null)
                {
                    enabled = false;
                }

                return enabled;
            }
            set
            {
                if (enabled == value)
                {
                    return;
                }

                if (value && _contextBehaviour == null)
                {
                    Debug.LogError($"{typeof(PlayerBehaviour)}: {nameof(_contextBehaviour)} is null.");
                    return;
                }

                enabled = value;
            }
        }
    }
}