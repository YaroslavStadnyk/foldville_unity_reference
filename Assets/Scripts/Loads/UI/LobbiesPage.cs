#if !UNITY_WEBGL
using System.Collections.Generic;
using Epic.OnlineServices.Lobby;
using EpicTransport;
using Game;
using Game.Logic.Common.Enums;
using Game.Logic.Configs;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
#endif

namespace Loads.UI
{
#if !UNITY_WEBGL
	public class LobbiesPage : EOSLobby
	{
		[SerializeField] private Button closeButton;
		[SerializeField] private Button createLobbyButton;
		[SerializeField] private Button refreshLobbiesButton;

		[Space] [SerializeField] private LobbyListItem listItemPrefab;
		[SerializeField] private Transform listHolder;

		/*
		public const string LOBBY_NAME_KEY = "LOBBY_NAME";

		private readonly List<LobbyListItem> _lobbies = new List<LobbyListItem>();

		private LoadsController _loadsController;

		public void Initialize(LoadsController loadsController)
		{
			_loadsController = loadsController;
		}

		public void Show()
		{
			gameObject.SetActive(true);
			ClearList();
			FindLobbies();
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public override void Start()
		{
			base.Start();

			closeButton.onClick.AddListener(OnCloseButtonClicked);
			createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
			refreshLobbiesButton.onClick.AddListener(OnRefreshButtonClicked);
		}

		private void OnEnable()
		{
			CreateLobbySucceeded += OnCreateLobbySuccess;
			JoinLobbySucceeded += OnJoinLobbySuccess;
			FindLobbiesSucceeded += OnFindLobbiesSuccess;
		}

		private void OnDisable()
		{
			CreateLobbySucceeded -= OnCreateLobbySuccess;
			JoinLobbySucceeded -= OnJoinLobbySuccess;
			FindLobbiesSucceeded -= OnFindLobbiesSuccess;
		}

		private void OnCreateLobbySuccess(List<Attribute> attributes)
		{
			GameManager.Instance.CacheLobby(currentLobbyId, ConnectedLobbyDetails);
			GameManager.Instance.InitializeManagers(GameMode.OnlineMultiplayer);

			NetworkManager.singleton.StartHost();
		}

		private void OnJoinLobbySuccess(List<Attribute> attributes)
		{
			GameManager.Instance.CacheLobby(currentLobbyId, ConnectedLobbyDetails);
			GameManager.Instance.InitializeManagers(GameMode.OnlineMultiplayer);

			NetworkManager.singleton.networkAddress = attributes.Find(x => x.Data != null && x.Data.Value.Key == hostAddressKey).Data?.Value.AsUtf8;
			NetworkManager.singleton.StartClient();
		}

		private void OnFindLobbiesSuccess(List<LobbyDetails> lobbiesFound)
		{
			ClearList();

			foreach (var lobby in lobbiesFound)
			{
				var listItem = Instantiate(listItemPrefab, listHolder);
				listItem.Initialize(lobby, OnLobbyClicked);

				_lobbies.Add(listItem);
			}
		}

		private void ClearList()
		{
			foreach (var listItem in _lobbies)
			{
				Destroy(listItem.gameObject);
			}

			_lobbies.Clear();
		}

		private void OnCloseButtonClicked()
		{
			_loadsController.ShowMenuPage();
		}

		private void OnCreateLobbyButtonClicked()
		{
			CreateLobby((uint) GameConfig.Instance.MultiplayerPreset.PlayersRange.y, LobbyPermissionLevel.Publicadvertised,
				false, new[]
				{
					new AttributeData {Key = LOBBY_NAME_KEY, Value = EOSSDKComponent.LocalUserAccountIdString} //TODO: use player name?
				});
		}

		private void OnRefreshButtonClicked()
		{
			FindLobbies();
		}

		private void OnLobbyClicked(LobbyDetails lobbyDetails)
		{
			JoinLobby(lobbyDetails);
		}
		*/
	}
#endif
}