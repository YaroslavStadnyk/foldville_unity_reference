using System;
using System.Collections.Generic;
using Core.Attributes;
using Game.Logic.Common.Blocks.Quests;
using Game.Logic.Common.Models;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct CampaignLevel
    {
        [ScenePath] public string gameSceneName;

        [Space] public LevelLogic levelLogic;
        public FactionsDefinition factionsDefinition;
        public int secondsPerTurn;

        [HideInInspector] [NonSerialized, OdinSerialize] public List<LevelQuest> Quests;

#if UNITY_EDITOR

        [PropertySpace]
        [ShowInInspector] private LevelQuest Quest1 { get => GetQuest(0); set => SetQuest(0, value); }
        [ShowInInspector] private LevelQuest Quest2 { get => GetQuest(1); set => SetQuest(1, value); }
        [ShowInInspector] private LevelQuest Quest3 { get => GetQuest(2); set => SetQuest(2, value); }

        private LevelQuest GetQuest(int index)
        {
            return Quests == null || Quests.Count <= index ? null : Quests[index];
        }

        private void SetQuest(int index, LevelQuest value)
        {
            Quests ??= new List<LevelQuest>(3);
            while (Quests.Count <= index)
            {
                Quests.Add(null);
            }

            Quests[index] = value;
        }

#endif
    }
}