using Game.Logic.Configs;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
	[InitializeOnLoad]
	public static class DefaultSceneLoader
	{
		static DefaultSceneLoader()
		{
			if (!DeveloperConfig.Instance.IsDefaultSceneLoaderEnabled)
			{
				EditorSceneManager.playModeStartScene = null;
				return;
			}

			var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
			var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
			EditorSceneManager.playModeStartScene = sceneAsset;
		}
	}
}