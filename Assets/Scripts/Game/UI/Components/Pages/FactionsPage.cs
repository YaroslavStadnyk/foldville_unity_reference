using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Players.Player;
using Grid.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.Pages
{
    public class FactionsPage : FactionsUI
    {
        // [SerializeField] private Button openButton;
        // [SerializeField] private Button closeButton;

        public override void OnBuy(TileType tileType)
        {
            base.OnBuy(tileType);

            if (PlayerBehaviour.LocalLatest == null)
            {
                return;
            }

            PlayerBehaviour.LocalLatest.Selection.FactionType = tileType;
            PlayerBehaviour.LocalLatest.ExploreSelectedFaction();
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnFactionsOriginsChanged += OnFactionsOriginsChanged;
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;

            // openButton.onClick.AddListener(Show);
            // closeButton.onClick.AddListener(Hide);
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnFactionsOriginsChanged -= OnFactionsOriginsChanged;
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;

            // openButton.onClick.RemoveListener(Show);
            // closeButton.onClick.RemoveListener(Hide);
        }

        private void OnFactionsOriginsChanged(OperationType operationType, string playerID, List<TileType> types)
        {
            if (PlayerProfile.LocalLatest == null || !PlayerProfile.LocalLatest.OwnedIDs.Contains(playerID))
            {
                return;
            }

            RestartUpdateFactionsCoroutine(playerID);
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            if (PlayerProfile.LocalLatest == null || !PlayerProfile.LocalLatest.OwnedIDs.Contains(newTurn.playerID))
            {
                return;
            }

            RestartUpdateFactionsCoroutine(newTurn.playerID);
        }

        #region Coroutines

        private Coroutine _updateFactionsCoroutine;

        private IEnumerator UpdateFactions(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                Hide();
                yield break;
            }

            while (PlayerBehaviour.LocalLatest == null || PlayerBehaviour.LocalLatest.LatestID != playerID)
            {
                yield return new WaitForEndOfFrame();
            }

            var factions = GameManager.Instance.Factions;
            if (factions == null)
            {
                yield break;
            }

            if (factions.IsAvailable(playerID))
            {
                Show();
            }
            else
            {
                Hide();
                yield break;
            }

            var targetTypes = factions.GetOrigin(playerID);
            if (targetTypes.IsNullOrEmpty())
            {
                HideAllFactions();
                yield break;
            }

            foreach (var type in listItemsPool.SpawnedBehaviours.Keys.ToArray())
            {
                if (!targetTypes.Contains(type))
                {
                    HideFaction(type);
                }
            }

            foreach (var targetType in targetTypes)
            {
                ShowFaction(targetType);
            }
        }

        private void RestartUpdateFactionsCoroutine(string playerID)
        {
            if (_updateFactionsCoroutine != null)
            {
                StopCoroutine(_updateFactionsCoroutine);
            }

            _updateFactionsCoroutine = StartCoroutine(UpdateFactions(playerID));
        }

        #endregion
    }
}