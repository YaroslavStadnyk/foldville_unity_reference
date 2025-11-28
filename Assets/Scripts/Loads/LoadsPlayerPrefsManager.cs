using Core.Ordinaries;
using UnityEngine;

namespace Loads
{
	public class LoadsPlayerPrefsManager : Singleton<LoadsPlayerPrefsManager>
	{
		private const string CredentialTypeKey = "CREDENTIAL_TYPE";

		public enum AuthType
		{
			None = -1,
			LocalUser = 0,
			EpicGames = 1
		}

		public static void SaveAuthType(AuthType type)
		{
			PlayerPrefs.SetInt(CredentialTypeKey, (int) type);
		}

		public static AuthType GetSavedAuthType()
		{
			return (AuthType) PlayerPrefs.GetInt(CredentialTypeKey, -1);
		}

		public static void ClearUserData()
		{
			PlayerPrefs.DeleteKey(CredentialTypeKey);
		}
	}
}