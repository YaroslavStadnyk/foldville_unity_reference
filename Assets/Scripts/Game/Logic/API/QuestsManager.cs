using System.Collections.Generic;
using System.Linq;
using Game.Logic.Common.Blocks.Quests;
using Game.Logic.Common.Enums;
using Game.Logic.Configs;
using Game.Players.Player;
using UnityEngine;

namespace Game.Logic.API
{
    public class QuestsManager : MonoBehaviour, IGameManager
    {
        // private IQuestsManager _impl;

        public readonly List<LevelQuest> LevelQuests = new();

        public void Initialize(GameMode gameMode)
        {
            TerminateQuests();
            LevelQuests.Clear();

            if (gameMode is GameMode.Campaign)
            {
                LevelQuests.AddRange(GameConfig.Instance.CampaignModePreset.SelectedLevel.Quests);
            }

            InitializeQuests();
        }

        public void ResetImplementation()
        {
            TerminateQuests();
        }

        private void InitializeQuests()
        {
            foreach (var quest in LevelQuests)
            {
                if (quest == null)
                {
                    continue;
                }

                quest.Initialize();
                quest.OnCompleted += OnQuestCompleted;
            }
        }

        private void TerminateQuests()
        {
            foreach (var quest in LevelQuests)
            {
                quest?.Terminate();
            }
        }

        private void OnQuestCompleted(bool isSuccess)
        {
            if (PlayerProfile.LocalLatest == null)
            {
                return;
            }

            if (LevelQuests.Any(quest => quest?.State is QuestState.Executing))
            {
                return;
            }

            var questsCount = LevelQuests.Count;
            var failsCount = LevelQuests.Count(quest => quest?.State is QuestState.Failed);

            // TODO levels saving
            var levelIndex = GameConfig.Instance.CampaignModePreset.SelectedLevelIndex;
            var levelInfos = GameManager.Instance.LevelInfos;
            if (levelInfos.Count > levelIndex)
            {
                var levelInfo = levelInfos[levelIndex];
                levelInfo.StarsCount = questsCount - failsCount;
                levelInfos[levelIndex] = levelInfo;
            }

            var playerState = failsCount == questsCount ? PartyPlayerState.Lost : PartyPlayerState.Won;
            GameManager.Instance.Party.SetPlayerState(PlayerProfile.LocalLatest.ID, playerState);
        }
    }
}