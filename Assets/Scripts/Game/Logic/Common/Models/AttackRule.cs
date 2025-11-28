using System;
using Game.Logic.Common.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Common.Models
{
    [Serializable] [HideReferenceObjectPicker]
    public class AttackRule
    {
        #region Inspector

        [SerializeField] private AttackType type;
        [SerializeField] private int cost = 5;
        [SerializeField] private int radius = 1;

        #endregion

        public AttackType Type => type;
        public int Cost => cost;
        public int Radius => radius;
    }
}