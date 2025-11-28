using System.Collections.Generic;
using Board.Structs;
using Game.Logic.Common.Enums;
using Game.Logic.Configs;
using Game.Logic.Internal.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.API
{
    /// <summary>
    /// It was created to sync the board events and their data.
    /// </summary>
    public class BoardManager : MonoBehaviour, IGameManager
    {
        private IBoardManager _impl;

        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IDictionary<string, CardInfo> Cards => _impl?.Cards;
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IDictionary<string, DeskInfo> Desks => _impl?.Desks;
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IDictionary<string, HandInfo> Hands => _impl?.Hands;

        public void Initialize(GameMode gameMode)
        {
            var implPrefab = DeveloperConfig.Instance.GetBoardManagerPrefab(gameMode);
            implPrefab.Initialize(OnInitialize);
        }

        public void ResetImplementation()
        {
            if (_impl == null)
                return;
            
            _impl.Destroy();
            _impl = null;
        }

        private void OnInitialize(IBase impl)
        {
            _impl = impl as IBoardManager;
            if (_impl == null)
            {
                Debug.LogError($"{typeof(IBoardManager)} not casted.");
                return;
            }

            if (impl is Component implComponent)
            {
                implComponent.transform.parent = transform;
            }
        }
    }
}