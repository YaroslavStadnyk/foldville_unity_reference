using System;
using Core;
using Core.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Common.Blocks.Player
{
    [Serializable]
    [InlineProperty]
    public abstract class PlayerInteger
    {
        public abstract int GetValue(string playerID);
    }

    [Serializable]
    public class Default : PlayerInteger
    {
        [SerializeField] private int value;

        public Default()
        {
        }

        public Default(int value)
        {
            this.value = value;
        }

        public override int GetValue(string playerID)
        {
            return value;
        }
    }

    [Serializable]
    public class BuildingsBonus : PlayerInteger
    {
        public override int GetValue(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                DebugUtility.LogWarning(this, "target player ID is null or empty.");
                return 0;
            }

            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                DebugUtility.LogWarning(this, "hex grid not found.");
                return 0;
            }

            return hexGrid.BonusRuleCounter.Total(playerID);
        }
    }
}