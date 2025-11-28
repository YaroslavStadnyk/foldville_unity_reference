using Game;
using Game.Players.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.UI
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button addPlayerButton;
        [SerializeField] private Button startGameButton;

        private void OnEnable()
        {
            GameEvents.Instance.OnPartyPlayerJoined += OnPartyPlayersChanged;
            GameEvents.Instance.OnPartyPlayerLeaved += OnPartyPlayersChanged;

            leaveButton.onClick.AddListener(OnLeaveButtonClicked);
            addPlayerButton.onClick.AddListener(OnAddPlayerButtonClicked);
            startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnPartyPlayerJoined -= OnPartyPlayersChanged;
            GameEvents.Instance.OnPartyPlayerLeaved -= OnPartyPlayersChanged;

            leaveButton.onClick.RemoveListener(OnLeaveButtonClicked);
            addPlayerButton.onClick.RemoveListener(OnAddPlayerButtonClicked);
            startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        }

        private void OnPartyPlayersChanged(string playerID)
        {
            var party = GameManager.Instance.Party;
            if (party == null)
            {
                return;
            }

            addPlayerButton.interactable = !party.IsFull();
        }

        public void OnLeaveButtonClicked()
        {
            GameManager.Instance.LeaveLobby();
        }

        public void OnAddPlayerButtonClicked()
        {
            PlayerProfile.LocalLatest.AddOwnedPlayer();
        }

        public void OnStartGameButtonClicked()
        {
            var party = GameManager.Instance.Party;
            if (party == null)
            {
                return;
            }

            party.StartGameSession();
        }
    }
}