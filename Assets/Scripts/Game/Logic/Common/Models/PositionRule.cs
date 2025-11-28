using System;
using System.Collections.Generic;
using Core.Serialization;
using Grid.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Common.Models
{
    [Serializable] [HideReferenceObjectPicker]
    public class PositionRule
    {
        #region Inspector

        [LabelText("Must Be Placed On")] [SerializeField] private List<TileType> requiredUnderlay = new() { TileType.Ground };

        [DictionaryDrawerSettings(KeyLabel = "Type", ValueLabel = "Radius")]
        [Space] [SerializeField] private SerializedDictionary<TileType, int> requiredNeighbors = new();

        #endregion

        public IReadOnlyList<TileType> RequiredUnderlay => requiredUnderlay;
        public IReadOnlyDictionary<TileType, int> RequiredNeighbors => requiredNeighbors;
    }
}