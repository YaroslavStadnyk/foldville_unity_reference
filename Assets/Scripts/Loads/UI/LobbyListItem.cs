#if !UNITY_WEBGL
using System;
using Epic.OnlineServices.Lobby;
using Game.Logic.Configs;
using TMPro;
using UnityEngine.UI;
#endif
using UnityEngine;

namespace Loads.UI
{
	public class LobbyListItem : MonoBehaviour
	{
#if !UNITY_WEBGL
		[SerializeField] private TMP_Text nameText;
		[SerializeField] private TMP_Text playerCountText;
		[SerializeField] private Button joinButton;

		private LobbyDetails _lobbyDetails;
		private Action<LobbyDetails> _onClicked;

		public void Initialize(LobbyDetails lobbyDetails, Action<LobbyDetails> onClicked)
		{
			throw new NotImplementedException();
			/*
			_lobbyDetails = lobbyDetails;
			_onClicked = onClicked;

			var copyNameOptions = new LobbyDetailsCopyAttributeByKeyOptions {AttrKey = LobbiesPage.LOBBY_NAME_KEY};
			_lobbyDetails.CopyAttributeByKey(ref copyNameOptions, out var lobbyNameAttribute);

			if (lobbyNameAttribute != null)
			{
				nameText.text = lobbyNameAttribute.Value.Data?.Value.AsUtf8;
			}

			var playerCountOptions = new LobbyDetailsGetMemberCountOptions();
			var playerCount = _lobbyDetails.GetMemberCount(ref playerCountOptions);
			playerCountText.text = $"{playerCount}/{GameConfig.Instance.MultiplayerPreset.PlayersRange.y}";
			*/
		}

		private void Start()
		{
			joinButton.onClick.AddListener(OnJoinButtonClicked);
		}

		private void OnJoinButtonClicked()
		{
			_onClicked(_lobbyDetails);
		}
#endif
	}
}