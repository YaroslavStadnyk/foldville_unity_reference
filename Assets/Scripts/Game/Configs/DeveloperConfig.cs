using Core.Configs;
using Game.Logic.Common.Enums;
using Game.Logic.Internal.Interfaces;
using Game.Logic.Internal.Network;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.Logic.Configs
{
    public class DeveloperConfig : BaseConfig<DeveloperConfig>
    {
#if UNITY_EDITOR
        [MenuItem("Game/Select " + nameof(DeveloperConfig))]
        private static void SelectConfig()
        {
            SelectInstanceInEditor();
        }
#endif

        #region Inspector

        [Tooltip("If enabled, the first scene is loaded by default in the Editor.")]
        [SerializeField] private bool isDefaultSceneLoaderEnabled;

        [Title("Managers (Network)")] [SerializeField] private GridDataManagerNetwork gridDataManagerNetworkPrefab;
        [SerializeField] private PartyManagerNetwork partyManagerNetworkPrefab;
        [SerializeField] private BoardManagerNetwork boardManagerNetworkPrefab;
        [SerializeField] private ResourcesManagerNetwork resourcesManagerNetworkPrefab;
        [SerializeField] private TurnManagerNetwork turnManagerNetworkPrefab;
        [SerializeField] private FactionsManagerNetwork factionsManagerNetworkPrefab;

        #endregion

        public bool IsDefaultSceneLoaderEnabled => isDefaultSceneLoaderEnabled;

        public IGridDataManager GetGridDataManagerPrefab(GameMode gameMode)
        {
            return gridDataManagerNetworkPrefab;
        }

        public IPartyManager GetPartyManagerPrefab(GameMode gameMode)
        {
            return partyManagerNetworkPrefab;
        }

        public IBoardManager GetBoardManagerPrefab(GameMode gameMode)
        {
            return boardManagerNetworkPrefab;
        }

        public IResourcesManager GetResourcesManagerPrefab(GameMode gameMode)
        {
            return resourcesManagerNetworkPrefab;
        }

        public ITurnManager GetTurnManagerPrefab(GameMode gameMode)
        {
            return turnManagerNetworkPrefab;
        }

        public IFactionsManager GetFactionsManagerPrefab(GameMode gameMode)
        {
            return factionsManagerNetworkPrefab;
        }
    }
}