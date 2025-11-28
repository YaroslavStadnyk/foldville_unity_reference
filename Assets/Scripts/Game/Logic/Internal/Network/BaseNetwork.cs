using System;
using Game.Logic.Internal.Interfaces;
using Mirror;

namespace Game.Logic.Internal.Network
{
    public abstract class BaseNetwork : NetworkBehaviour, IBase
    {
        #region Prefab

        private event Action<IBase> InitializeCallback;

        public void Initialize(Action<IBase> callback)
        {
            InitializeCallback = callback;

            if (NetworkManager.singleton is PartyManagerNetwork partyManagerNetwork)
            {
                partyManagerNetwork.OnServerStarted += OnServerStarted;
            }
        }

        private void OnServerStarted()
        {
            if (NetworkManager.singleton is PartyManagerNetwork partyManagerNetwork)
            {
                partyManagerNetwork.OnServerStarted -= OnServerStarted;
            }

            var instance = Instantiate(this);
            NetworkServer.Spawn(instance.gameObject);

            InitializeCallback?.Invoke(instance);
        }

        #endregion

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}