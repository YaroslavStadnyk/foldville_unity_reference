using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Game.Configs;
using Game.Logic.Configs;
using Game.Players.Common;
using UnityEngine;

namespace Game.Players.Player
{
    public class PlayerProfile : EntityProfile
    {
        public static PlayerProfile LocalLatest { get; private set; }

        public static bool IsLocalID(string playerID)
        {
            return playerID != null && LocalLatest != null && LocalLatest.OwnedIDs.Contains(playerID);
        }

        public override void OnStartLocalPlayer()
        {
            LocalLatest = this;

            for (var i = 1; i < GameConfig.Instance.MultiplayerPreset.PlayersRange.x; i++)
            {
                LocalLatest.AddOwnedPlayer();
            }
        }

        #region Nicknames

        private static readonly Dictionary<string, string> NicknamesDictionary = new();

        public static string GetNickname(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                return playerID;
            }

            if (NicknamesDictionary.TryGetValue(playerID, out var nickname))
            {
                return nickname;
            }

            NicknamesDictionary[playerID] = playerID;
            return playerID;
        }

        public static void SetNickname(string playerID, string nickname)
        {
            if (playerID.IsNullOrEmpty() || nickname.IsNullOrEmpty())
            {
                return;
            }

            NicknamesDictionary[playerID] = nickname;
        }

        #endregion

        #region Colors

        private static readonly Dictionary<string, Color> ColorsDictionary = new();

        public static Color GetColor(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                return Color.clear;
            }

            if (ColorsDictionary.TryGetValue(playerID, out var captureColor))
            {
                return captureColor;
            }

            var playerColors = GUIConfig.Instance.PlayerColors;
            var nextColorIndex = ColorsDictionary.Count % playerColors.Count;
            var nextColor = playerColors[nextColorIndex];
            ColorsDictionary[playerID] = nextColor;

            return nextColor;
        }

        #endregion
    }
}