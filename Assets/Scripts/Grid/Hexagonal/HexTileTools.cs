using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Grid.Hexagonal
{
    public class HexTileTools
    {
#if UNITY_EDITOR
        [MenuItem("Grid/Tools/Replace Missing Tile Prefabs")]
        private static void ReplaceMissingTilePrefabs()
        {
            foreach (var hexTile in Object.FindObjectsOfType<HexTile>())
            {
                if (PrefabUtility.IsPrefabAssetMissing(hexTile.transform))
                {
                    ReplaceHexTilePrefab(hexTile);
                }
            }

            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
        }

        [MenuItem("Grid/Tools/Replace Tile Prefabs")]
        private static void ReplaceTilePrefabs()
        {
            foreach (var hexTile in Object.FindObjectsOfType<HexTile>())
            {
                ReplaceHexTilePrefab(hexTile);
            }

            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static void ReplaceHexTilePrefab(HexTile hexTile)
        {
            var tilePrefab = HexTileConfig.Instance.GetTilePrefab(hexTile.Type);
            if (tilePrefab == null)
            {
                return;
            }

            var tileInstance = PrefabUtility.InstantiatePrefab(tilePrefab, hexTile.transform.parent) as HexTile;
            if (tileInstance == null)
            {
                Debug.LogError($"{typeof(HexTile)} instance is null.");
                return;
            }

            tileInstance.transform.position = hexTile.transform.position;
            tileInstance.transform.rotation = hexTile.transform.rotation;
            tileInstance.transform.localScale = hexTile.transform.localScale;

            Object.DestroyImmediate(hexTile.gameObject);
        }
#endif
    }
}