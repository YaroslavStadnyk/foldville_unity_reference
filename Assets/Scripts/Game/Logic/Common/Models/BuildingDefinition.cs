using System;
using Game.Logic.Common.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Common.Models
{
    [Serializable] [HideReferenceObjectPicker]
    public class BuildingDefinition
    {
        #region Inspector

        [SerializeField] private FunctionType functionType;

        [Space] [LabelText("Building Cost")] [SerializeField] private int cost = 5;
        [LabelText("Radius / Weight")] [SerializeField] [Min(0)] private int radius = 5;

        [Space] [SerializeField] private BonusRule bonus = new();

        [Space] [SerializeField] private PositionRule position = new();

        [ShowIf(nameof(FunctionType), FunctionType.Attacking)]
        [Space] [SerializeField] private AttackRule attack = new();

        #endregion

        public FunctionType FunctionType => functionType;
        public int Cost => cost;
        public const ResourceType CostType = ResourceType.Energy;
        public int Radius => radius;

        public BonusRule BonusRule => bonus;
        public PositionRule PositionRule => position;
        public AttackRule AttackRule => attack;
    }
}