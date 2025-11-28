using System;
using Core.UI;
#if !UNITY_WEBGL
using Epic.OnlineServices.Auth;
using EpicTransport;
#endif
using Game;
using Game.Logic.Common.Enums;
using Game.Logic.Configs;
using Game.Players.Player;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Loads.UI
{
	public class MenuPage : AnimatedPage
	{
		[SerializeField] private Button campaignButton;
		[SerializeField] private Button localGameButton;
		[SerializeField] private Button multiplayerButton;
		[SerializeField] private Button logoutButton;

		[Space] [SerializeField] private TMP_Text userIdText;

		private LoadsController _loadsController;

		public void Initialize(LoadsController loadsController)
		{
			_loadsController = loadsController;
		}

		private void Start()
		{
			campaignButton.onClick.AddListener(OnCampaignButtonClicked);
			localGameButton.onClick.AddListener(OnLocalGameButtonClicked);
			multiplayerButton.onClick.AddListener(OnMultiplayerButtonClicked);
			logoutButton.onClick.AddListener(OnLogoutButtonClicked);
		}

		public override void Show()
		{
			base.Show();
			userIdText.text = ""; //EOSSDKComponent.LocalUserProductIdString;
		}

		private void OnCampaignButtonClicked()
		{
			// GameManager.Instance.InitializeManagers(GameMode.Campaign);
			// NetworkManager.singleton.StartHost();
			_loadsController.ShowLevelsPage();
		}

		private void OnLocalGameButtonClicked()
		{
			GameManager.Instance.InitializeManagers(GameMode.OfflineMultiplayer);
			NetworkManager.singleton.StartHost();
		}

		private void OnMultiplayerButtonClicked()
		{
			throw new NotImplementedException();
			// _loadsController.SignInEpicGamesUser();
			// _loadsController.ShowLobbiesPage();
		}

		private void OnLogoutButtonClicked()
		{
			throw new NotImplementedException();
			/*
			var logoutOptions = new LogoutOptions {LocalUserId = EOSSDKComponent.LocalUserAccountId};
			EOSSDKComponent.GetAuthInterface().Logout(ref logoutOptions, null, (ref LogoutCallbackInfo data) =>
				{
					LoadsPlayerPrefsManager.ClearUserData();
					_loadsController.SignInLocalUser();
				});*/
		}
	}
}