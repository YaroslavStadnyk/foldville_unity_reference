using Core.Extensions;
using Core.Pooling;
using Core.UI;
using Game.Logic.Common.Enums;
using Game.UI.Components.ListItems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Components
{
    public class QuestsUI : AnimatedPage
    {
        [HideLabel] [BoxGroup("List Items Pool")]
        [SerializeField] private PoolDictionary<QuestListItem, int> listItemsPool = new();

        private int _nextQuestID;

        public int ShowQuest(string description)
        {
            if (description.IsNullOrEmpty())
            {
                return -1;
            }

            _nextQuestID += 1;

            var listItem = listItemsPool.Spawn(_nextQuestID);
            listItem.Initialize(description);

            return _nextQuestID;
        }

        public bool HideQuest(int questID)
        {
            return listItemsPool.Release(questID);
        }

        public void HideAllQuests()
        {
            listItemsPool.ReleaseAll();
        }

        public void SetQuestState(int questID, QuestState state)
        {
            if (listItemsPool.SpawnedBehaviours.TryGetValue(questID, out var listItem) && listItem != null)
            {
                listItem.SetState(state);
            }
        }
    }
}