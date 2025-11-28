using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Game.Configs;
using Game.Logic.Common.Structs;
using Game.Players.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Environment.Common
{
    public class PlayerObjectPainter : ObjectPainter
    {
        [SerializeField] private float playerColorsFlow = 0.5f;
        [ShowInInspector] private IReadOnlyList<Color> PlayerColors => GUIConfig.Instance.PlayerColors;

        public void SetColor(string playerID)
        {
            SetColor(PlayerProfile.GetColor(playerID), playerColorsFlow);
        }

        private void Start()
        {
            var localPlayerProfile = PlayerProfile.LocalLatest;
            if (localPlayerProfile == null)
            {
                return;
            }

            var localPlayerIDs = localPlayerProfile.OwnedIDs;
            if (localPlayerIDs == null || localPlayerIDs.Count < 2)
            {
                return;
            }

            var localPlayerBehaviour = PlayerBehaviour.LocalLatest;
            if (localPlayerBehaviour == null)
            {
                return;
            }

            SetColor(localPlayerBehaviour.LatestID);
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            if (newTurn.playerID.IsNullOrEmpty())
            {
                return;
            }

            var localPlayerProfile = PlayerProfile.LocalLatest;
            if (localPlayerProfile == null)
            {
                return;
            }

            var localPlayerIDs = localPlayerProfile.OwnedIDs;
            if (localPlayerIDs == null || localPlayerIDs.Count < 2)
            {
                return;
            }

            if (localPlayerIDs.Contains(newTurn.playerID))
            {
                SetColor(newTurn.playerID);
            }
        }
    }
}