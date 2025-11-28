using System;
using Core.UI;
#if !UNITY_WEBGL
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using EpicTransport;
#endif
using Game.Environment;
using Game.UI;
using Game.UI.Components.Pages;
using Sirenix.OdinInspector;
using UnityEngine;
#if !UNITY_WEBGL
using Credentials = Epic.OnlineServices.Auth.Credentials;
using LoginCallbackInfo = Epic.OnlineServices.Auth.LoginCallbackInfo;
using LoginOptions = Epic.OnlineServices.Auth.LoginOptions;
#endif

namespace Loads.UI
{
	public class LoadsController : MonoBehaviour
	{
		[SerializeField] private MenuPage menuPage;
		[SerializeField] [ReadOnly] private LevelsPage levelsPage;

		private void Start()
		{
			InitializePages();
			//ShowLoadsPage("Loading...");
			ShowMenuPage();
			/*
			EOSSDKComponent.Initialize();

			var savedAuthType = LoadsPlayerPrefsManager.GetSavedAuthType();
			if (savedAuthType is LoadsPlayerPrefsManager.AuthType.None)
			{
				SignInLocalUser();
				return;
			}

			LoginCachedUser();
			*/
		}

		/*
		public void SignInLocalUser()
		{
			ShowMenuPage();
			//LoadsPlayerPrefsManager.SaveAuthType(LoadsPlayerPrefsManager.AuthType.LocalUser);
			//CreateDeviceIdUser();
		}
		*/
/*
		public void SignInEpicGamesUser()
		{
			throw new NotImplementedException();
			// LoadsPlayerPrefsManager.SaveAuthType(LoadsPlayerPrefsManager.AuthType.EpicGames);
			// LoginEpicGamesAccount();
		}

		private void CreateDeviceIdUser()
		{
			// var options = new CreateDeviceIdOptions {DeviceModel = SystemInfo.deviceModel};
			// EOSSDKComponent.GetConnectInterface().CreateDeviceId(ref options, null, OnCreateLocalUserCompleted);
		}

		private void OnCreateLocalUserCompleted(ref CreateDeviceIdCallbackInfo data)
		{
			if (data.ResultCode is Result.Success or Result.DuplicateNotAllowed)
			{
				LoginToConnectInterface(null);
				return;
			}

			Debug.LogError("Create Local User failed: " + data.ResultCode);
		}

		private void LoginCachedUser()
		{
			var savedAuthType = LoadsPlayerPrefsManager.GetSavedAuthType();
			if (savedAuthType == LoadsPlayerPrefsManager.AuthType.LocalUser)
			{
				CreateDeviceIdUser();
				return;
			}

			var credentials = new Credentials
			{
				Type = LoginCredentialType.PersistentAuth
			};

			var options = new LoginOptions
			{
				Credentials = credentials,
				ScopeFlags = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList | AuthScopeFlags.Presence
			};

			EOSSDKComponent.GetAuthInterface().Login(ref options, null, OnLoginCompleted);
		}

		private void LoginEpicGamesAccount()
		{
			var credentials = new Credentials
			{
				Type = LoginCredentialType.AccountPortal
			};

			var options = new LoginOptions
			{
				Credentials = credentials,
				ScopeFlags = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList | AuthScopeFlags.Presence
			};

			EOSSDKComponent.GetAuthInterface().Login(ref options, null, OnLoginCompleted);
		}

		private void OnLoginCompleted(ref LoginCallbackInfo data)
		{
			if (data.ResultCode is not Result.Success)
			{
				Debug.LogError("Login returned code:" + data.ResultCode);
				LoadsPlayerPrefsManager.ClearUserData();
				SignInLocalUser();
				return;
			}

			EOSSDKComponent.LocalUserAccountId = data.LocalUserId;
			data.LocalUserId.ToString(out var userId);
			EOSSDKComponent.LocalUserAccountIdString = userId;

			LoginToConnectInterface(data);
		}

		private void LoginToConnectInterface(LoginCallbackInfo? data)
		{
			var loginOptions = new Epic.OnlineServices.Connect.LoginOptions();

			var savedAuthType = LoadsPlayerPrefsManager.GetSavedAuthType();
			var credentials = new Epic.OnlineServices.Connect.Credentials
			{
				Type = savedAuthType is LoadsPlayerPrefsManager.AuthType.EpicGames
					? ExternalCredentialType.Epic
					: ExternalCredentialType.DeviceidAccessToken
			};

			if (savedAuthType is LoadsPlayerPrefsManager.AuthType.EpicGames && data != null)
			{
				var authInterface = EOSSDKComponent.GetAuthInterface();
				var options = new CopyUserAuthTokenOptions();
				if (authInterface.CopyUserAuthToken(ref options, data.Value.LocalUserId, out var token) is not Result.Success)
				{
					Debug.LogError("Could not copy user token:" + data.Value.ResultCode);
					LoadsPlayerPrefsManager.ClearUserData();
					SignInLocalUser();
					return;
				}

				credentials.Token = token?.AccessToken;
			}
			else if (savedAuthType is LoadsPlayerPrefsManager.AuthType.LocalUser)
			{
				loginOptions.UserLoginInfo = new UserLoginInfo {DisplayName = "Local User"};
			}

			loginOptions.Credentials = credentials;

			EOSSDKComponent.GetConnectInterface().Login(ref loginOptions, null, OnConnectInterfaceLoginCompleted);
		}

		private void OnConnectInterfaceLoginCompleted(ref Epic.OnlineServices.Connect.LoginCallbackInfo data)
		{
			if (data.ResultCode is Result.Success)
			{
				EOSSDKComponent.LocalUserProductId = data.LocalUserId;
				data.LocalUserId.ToString(out var userId);
				EOSSDKComponent.LocalUserProductIdString = userId;

				Debug.Log("Connect interface login success.");
				ShowMenuPage();
			}
			else if (Epic.OnlineServices.Common.IsOperationComplete(data.ResultCode))
			{
				Debug.Log("Login returned " + data.ResultCode + "\nRetrying...");
				var createUserOptions = new CreateUserOptions {ContinuanceToken = data.ContinuanceToken};
				EOSSDKComponent.GetConnectInterface().CreateUser(ref createUserOptions, null, OnCreateUserCompleted);
			}
			else
			{
				Debug.LogError("Login returned code:" + data.ResultCode);
				LoadsPlayerPrefsManager.ClearUserData();
				SignInLocalUser();
			}
		}

		private void OnCreateUserCompleted(ref CreateUserCallbackInfo createUserCallback)
		{
			if (createUserCallback.ResultCode is not Result.Success)
			{
				Debug.LogError("Create User error: " + createUserCallback.ResultCode);
				LoadsPlayerPrefsManager.ClearUserData();
				SignInLocalUser();
				return;
			}

			EOSSDKComponent.LocalUserProductId = createUserCallback.LocalUserId;
			createUserCallback.LocalUserId.ToString(out var userId);
			EOSSDKComponent.LocalUserProductIdString = userId;

			LoginToConnectInterface(null);
		}*/

		private void InitializePages()
		{
			menuPage.Initialize(this);
			levelsPage = LevelsCanvas.Instance.Page;
			if (levelsPage != null)
			{
				levelsPage.OnBackToMenu = ShowMenuPage;
			}
			//lobbiesPage.Initialize(this);
			//levelsPage.Initialize(this);
		}

		private void HidePages()
		{
			menuPage.Hide();
			//lobbiesPage.Hide();
			if (levelsPage != null)
			{
				levelsPage.Hide();
			}
		}

		public void ShowMenuPage()
		{
			HidePages();
			menuPage.Show();
		}

		public void ShowLobbiesPage()
		{
			//lobbiesPage.Show();
		}

		public void ShowLevelsPage()
		{
			HidePages();
			if (levelsPage != null)
			{
				levelsPage.Show();
			}
		}
	}
}