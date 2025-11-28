using System;
using System.Collections.Generic;
using Core.Serialization;
using Game.Logic.Common.Enums;
using Grid.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Common.Models
{
    [Serializable] [HideReferenceObjectPicker]
    public class BonusRule
    {
        #region Inspector

        // [MinValue(nameof(ValueMin))] [MaxValue(nameof(ValueMax))]
        [SerializeField] private int value = 1;

        // [LabelText("Value (Min - Max)")] [MinMaxSlider(-10, 10)]
        // [SerializeField] private Vector2Int valueRange = new (0, 5);

        [Space] [SerializeField] private InteractionType interactionType;
        [SerializeField] private InteractionTarget interactionTarget;

        [DictionaryDrawerSettings(KeyLabel = "Type", ValueLabel = "Value")]
        [Space] [SerializeField] private SerializedDictionary<TileType, int> interactionPairs = new();

        #endregion

        public int Value => value;
        public int ValueMin => int.MinValue; // valueRange.x;
        public int ValueMax => int.MaxValue; // valueRange.y;

        public InteractionType InteractionType => interactionType;
        public InteractionTarget InteractionTarget => interactionTarget;
        public IReadOnlyDictionary<TileType, int> InteractionPairs => interactionPairs;
    }
}