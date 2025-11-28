using System;
using System.Collections.Generic;
using Board.Interfaces;
using Board.Services;
using Core.Attributes;
using Core.Extensions;
using Core.Managers;
using Core.Ordinaries;
#if !UNITY_WEBGL
using Epic.OnlineServices.Lobby;
#endif
using Game.Environment;
using Game.Logic.API;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Game.Logic.Internal.Network;
using Game.UI;
using JetBrains.Annotations;
using Mirror;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [LabelText("Preview Scene")] [SerializeField] [ScenePath] private string previewScenePath;
        private static GameObject _previewSceneRootGameObject;

        [Space] [SerializeField] private PartyManager partyManager;
        [SerializeField] private GridDataManager gridDataManager;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ResourcesManager resourcesManager;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private FactionsManager factionsManager;
        [SerializeField] private QuestsManager questsManager;

        [CanBeNull] private string _lobbyId;
#if !UNITY_WEBGL
        [CanBeNull] private LobbyDetails _lobbyDetails;
#endif

        // TODO levels saving
        private static readonly List<CampaignLevelInfo> _levelInfos = new();
        public List<CampaignLevelInfo> LevelInfos
        {
            get
            {
                if (!_levelInfos.IsNullOrEmpty())
                {
                    return _levelInfos;
                }

                var levels = GameConfig.Instance.CampaignModePreset.Levels;
                for (var i = 0; i < levels.Count; i++)
                {
                    var levelInfo = new CampaignLevelInfo { Index = i, Name = levels[i].gameSceneName, StarsCount = 0};
                    _levelInfos.Add(levelInfo);
                }

                return _levelInfos;
            }
        }

        public override void Awake()
        {
            base.Awake();
            RegisterServices();

            Application.targetFrameRate = 60;
            Time.timeScale = 1f;

            SceneManager.sceneLoaded += OnSceneLoaded;
            LoadPreviewScene();
        }

        private void LoadPreviewScene()
        {
            if (_previewSceneRootGameObject != null)
            {
                return;
            }

            _previewSceneRootGameObject = new GameObject("PreviewSceneRoot");
            DontDestroyOnLoad(_previewSceneRootGameObject);

            var previewSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.None);
            var previewSceneOperation = SceneManager.LoadSceneAsync(previewScenePath, previewSceneParameters);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.path == previewScenePath)
            {
                OnPreviewSceneLoaded(scene, loadSceneMode);
            }

            UpdatePreviewSceneEnabled(scene);
        }

        private void OnPreviewSceneLoaded(Scene previewScene, LoadSceneMode loadSceneMode)
        {
            foreach (var rootGameObject in previewScene.GetRootGameObjects())
            {
                SceneManager.MoveGameObjectToScene(rootGameObject, _previewSceneRootGameObject.scene);
                rootGameObject.transform.parent = _previewSceneRootGameObject.transform;
            }

            SceneManager.UnloadSceneAsync(previewScenePath);
        }

        private void UpdatePreviewSceneEnabled(Scene loadedScene)
        {
            if (loadedScene.path == previewScenePath || loadedScene == _previewSceneRootGameObject.scene)
            {
                return;
            }

            var isPreviewSceneEnabled = loadedScene.path == SceneManager.GetSceneByBuildIndex(0).path ||
                                        loadedScene.path == GameConfig.Instance.MultiplayerPreset.LobbySceneName ||
                                        loadedScene.path == GameConfig.Instance.CampaignModePreset.LobbySceneName;

            _previewSceneRootGameObject.SetActive(isPreviewSceneEnabled);
        }

        public GameMode Mode { get; private set; }

        public void InitializeManagers(GameMode gameMode)
        {
            ResetState();
            Mode = gameMode;

            partyManager.Initialize(gameMode);
            ((PartyManagerNetwork) NetworkManager.singleton).SetTransport(gameMode is not GameMode.OnlineMultiplayer);

            gridDataManager.Initialize(gameMode);
            boardManager.Initialize(gameMode);
            resourcesManager.Initialize(gameMode);
            turnManager.Initialize(gameMode);
            factionsManager.Initialize(gameMode);
            questsManager.Initialize(gameMode);
        }

        //TODO: do this when the game data has to be cleared
        public void ResetState()
        {
            ClearReferences();
            ResetManagers();
            //TODO: load menu scene?
        }

#if !UNITY_WEBGL
        public void CacheLobby(string lobbyId, LobbyDetails lobbyDetails)
        {
            _lobbyId = lobbyId;
            _lobbyDetails = lobbyDetails;
        }
#endif

        public void LeaveGame()
        {
            NetworkManager.singleton.StopClient();
            NetworkServer.Shutdown();

            var operation = SceneManager.LoadSceneAsync(0);
            LoadingCanvas.Instance.AddOperation(operation);

            ResetState();
        }

        public void LeaveLobby()
        {
            LeaveGame();
            /*
            if (_lobbyDetails == null)
            {
                return;
            }

            var lobbyOwnerOptions = new LobbyDetailsGetLobbyOwnerOptions();
            if (EOSSDKComponent.LocalUserProductId == _lobbyDetails.GetLobbyOwner(ref lobbyOwnerOptions))
            {
                var options = new DestroyLobbyOptions
                {
                    LobbyId = _lobbyId, 
                    LocalUserId = EOSSDKComponent.LocalUserProductId
                };
                
                EOSSDKComponent.GetLobbyInterface().DestroyLobby(ref options, null, (ref DestroyLobbyCallbackInfo callback) => {
                    NetworkManager.singleton.StopHost();
                    ResetState();
                });
            }
            else
            {
                var options = new LeaveLobbyOptions
                {
                    LobbyId = _lobbyId, 
                    LocalUserId = EOSSDKComponent.LocalUserProductId
                };
                
                EOSSDKComponent.GetLobbyInterface().LeaveLobby(ref options, null, (ref LeaveLobbyCallbackInfo callbackInfo) => {
                    NetworkManager.singleton.StopClient();
                    ResetState();
                });
            }
            */
        }

        public void CloseLobbyAndStartGame()
        {
            throw new NotImplementedException();
            /*
            if (_lobbyDetails == null)
                return;
            
            var lobbyOwnerOptions = new LobbyDetailsGetLobbyOwnerOptions();
            if (EOSSDKComponent.LocalUserProductId !=
                _lobbyDetails.GetLobbyOwner(ref lobbyOwnerOptions))
                return;
            
            var options = new DestroyLobbyOptions
            {
                LobbyId = _lobbyId, 
                LocalUserId = EOSSDKComponent.LocalUserProductId
            };
                
            EOSSDKComponent.GetLobbyInterface().DestroyLobby(ref options, null, (ref DestroyLobbyCallbackInfo callback) => {
                // Start Game?
            });*/
        }

#if !UNITY_WEBGL
        [CanBeNull] public LobbyDetails GetCachedLobbyDetails() => _lobbyDetails;
#endif

        private void ClearReferences()
        {
#if !UNITY_WEBGL
            _lobbyDetails = null;
#endif
            _hexGrid = null;
            _camera = null;
        }

        private void ResetManagers()
        {
            partyManager.ResetImplementation();
            gridDataManager.ResetImplementation();
            boardManager.ResetImplementation();
            resourcesManager.ResetImplementation();
            turnManager.ResetImplementation();
            factionsManager.ResetImplementation();
            questsManager.ResetImplementation();
        }

        private static void RegisterServices()
        {
            var cardService = new CardService();
            var deskService = new DeskService();
            var playerService = new HandService();

            ServiceManager.Instance.RegisterService<ICardService>(cardService);
            ServiceManager.Instance.RegisterService<IDeskService>(deskService);
            ServiceManager.Instance.RegisterService<IHandService>(playerService);
        }

        private static T FindInstance<T>() where T : Object
        {
            var instance = FindObjectOfType<T>();
            if (instance == null)
            {
                Debug.LogWarning("Has no " + typeof(T) + " instance on scene, but you try to get it!");
            }

            return instance;
        }

        public PartyManager Party => partyManager;
        public GridDataManager GridData => gridDataManager;
        public BoardManager Board => boardManager;
        public ResourcesManager Resources => resourcesManager;
        public TurnManager Turn => turnManager;
        public FactionsManager Factions => factionsManager;
        public QuestsManager Quests => questsManager;

        private HexGrid _hexGrid;
        public HexGrid HexGrid
        {
            get
            {
                if (_hexGrid == null)
                {
                    _hexGrid = FindInstance<HexGrid>();
                }

                return _hexGrid;
            }
        }

#if UNITY_EDITOR
        private string _lastFocusedWindowTitle = "Game";
#endif

        private Camera _camera;
        public Camera Camera
        {
            get
            {
                if (_camera == null || !_camera.gameObject.activeInHierarchy)
                {
                    _camera = Camera.main;
                    if (_camera == null)
                    {
                        _camera = Camera.current;
                    }
                }

#if UNITY_EDITOR
                var currentFocusedWindowTitle = EditorWindow.focusedWindow == null ? null : EditorWindow.focusedWindow.titleContent.text;
                if (currentFocusedWindowTitle is "Scene" or "Game")
                {
                    _lastFocusedWindowTitle = currentFocusedWindowTitle;
                }

                if (_lastFocusedWindowTitle is "Game")
                {
                    return _camera;
                }

                var sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null && sceneView.camera != null)
                {
                    return sceneView.camera;
                }
#endif

                return _camera;
            }
            set => _camera = value;
        }
    }
}