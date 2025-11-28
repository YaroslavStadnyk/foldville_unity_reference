using Core.Extensions;
using Game.Logic.Common.Enums;

namespace Game.UI.Components.Widgets
{
    public class QuestsWidget : QuestsUI
    {
        private void Start()
        {
            var quests = GameManager.Instance.Quests.LevelQuests;
            if (quests.IsNullOrEmpty())
            {
                Hide();
            }

            foreach (var quest in quests)
            {
                if (quest == null)
                {
                    continue;
                }

                var questID = ShowQuest(quest.Description);
                SetQuestState(questID, quest.State);
                quest.OnCompleted += isSuccess => SetQuestState(questID, isSuccess ? QuestState.Completed : QuestState.Failed);
            }
        }
    }
}