using Core.Pooling;
using Game;
using UnityEngine;

namespace Lobby.UI
{
    public class LobbyPage: MonoBehaviour
    {
        [SerializeField] private Transform listHolder;
        [SerializeField] private LobbyPlayerListItem listItemPrefab;

        private readonly PoolDictionary<LobbyPlayerListItem, string> _pool = new();

        private void Awake()
        {
            _pool.prefab = listItemPrefab;
            _pool.parent = listHolder;
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnPartyPlayerJoined += OnPartyPlayerJoined;
            GameEvents.Instance.OnPartyPlayerReadyChanged += OnPartyPlayerReadyChanged;
            GameEvents.Instance.OnPartyPlayerLeaved += OnPartyPlayerLeaved;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnPartyPlayerJoined -= OnPartyPlayerJoined;
            GameEvents.Instance.OnPartyPlayerReadyChanged -= OnPartyPlayerReadyChanged;
            GameEvents.Instance.OnPartyPlayerLeaved -= OnPartyPlayerLeaved;
        }

        private void OnPartyPlayerJoined(string playerId)
        {
            var listItem = _pool.Spawn(playerId);
            listItem.Initialize(playerId);
        }

        private void OnPartyPlayerLeaved(string playerId)
        {
            _pool.Release(playerId);
        }

        private void OnPartyPlayerReadyChanged(string playerId, bool isReady)
        {
            if (_pool.SpawnedBehaviours.ContainsKey(playerId))
            {
                _pool.SpawnedBehaviours[playerId].SetReadyState(isReady);
            }
        }
    }
}