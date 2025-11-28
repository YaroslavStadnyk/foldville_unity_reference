using System.Collections.Generic;
using Core.Extensions;
using Core.Interfaces;
using Game.Logic.Common.Structs;
using Game.Logic.Internal.Interfaces;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
using ShowInInspector = Sirenix.OdinInspector.ShowInInspectorAttribute;

namespace Game.Players.Common
{
    public abstract class EntityProfile : NetworkRoomPlayer, IPartyProfile, IIdentity
    {
        [SerializeField] private bool readyOnAwake = true;

        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public string ID => netIdentity == null ? "" : $"Player{netId}";
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public PartyPlayerStats Stats
        {
            get
            {
                if (!Application.isPlaying || ID.IsNullOrEmpty())
                {
                    return default;
                }

                var party = GameManager.Instance.Party;
                if (party == null)
                {
                    return default;
                }

                return party.JoinedPlayers?.FirstOrDefault(ID) ?? default;
            }
        }
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IReadOnlyCollection<string> OwnedIDs => _ownedIDs;

        private readonly HashSet<string> _ownedIDs = new();

        [Server]
        public void AddOwnedPlayer()
        {
            for (var i = 0; i <= _ownedIDs.Count; i++)
            {
                var ownedID = $"{ID}_{i}";
                if (_ownedIDs.Add(ownedID))
                {
                    GameManager.Instance.Party.Join(ownedID, readyOnAwake);
                    return;
                }
            }
        }

        [Server]
        public void RemoveOwnedPlayer()
        {
            for (var i = _ownedIDs.Count; i >= 0; i--)
            {
                var ownedID = $"{ID}_{i}";
                if (_ownedIDs.Remove(ownedID))
                {
                    GameManager.Instance.Party.Leave(ownedID);
                    return;
                }
            }
        }

        [Server]
        public void RemoveOwnedPlayer(string ownedID)
        {
            if (_ownedIDs.Remove(ownedID))
            {
                GameManager.Instance.Party.Leave(ownedID);
            }
        }

        public override void OnStartClient()
        {
            if (!NetworkServer.active)
            {
                _ownedIDs.Add(ID);
            }
        }

        public override void OnStopClient()
        {
            if (!NetworkServer.active)
            {
                _ownedIDs.Clear();
            }
        }

        public override void OnStartServer()
        {
            if (_ownedIDs.Add(ID))
            {
                GameManager.Instance.Party.Join(ID, readyOnAwake);
            }
        }

        public override void ServerReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            foreach (var ownedID in _ownedIDs)
            {
                GameManager.Instance.Party.SetPlayerReady(ownedID, newReadyState);
            }
        }

        public override void OnStopServer()
        {
            foreach (var ownedID in _ownedIDs)
            {
                GameManager.Instance.Party.Leave(ownedID);
            }

            _ownedIDs.Clear();
        }

        public void OnRoomServerLoadedPlayer(GameObject gamePlayer)
        {
            if (gamePlayer == null)
            {
                Debug.LogWarning($"{nameof(EntityProfile)} {ID} - {nameof(gamePlayer)} is null.");
                return;
            }

            if (!gamePlayer.TryGetComponent(out EntityBehaviour contextBehaviour))
            {
                Debug.LogWarning($"{nameof(EntityProfile)} {ID} - {nameof(EntityBehaviour)} not found.");
            }

            contextBehaviour.OnInitialize(this);
        }
    }
}