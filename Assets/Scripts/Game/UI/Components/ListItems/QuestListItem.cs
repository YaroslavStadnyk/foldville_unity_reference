using Core.Extensions;
using Core.Ordinaries;
using Game.Configs;
using Game.Logic.Common.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.ListItems
{
    public class QuestListItem : PoolBehaviour
    {
        [SerializeField] private Image stateImage;
        [SerializeField] private TMP_Text descriptionText;

        public void Initialize(string description)
        {
            stateImage.sprite = GUIConfig.Instance.QuestStateUIPresets.FirstOrDefault(QuestState.Executing).originalIcon;
            descriptionText.text = description;
        }

        public void SetState(QuestState state)
        {
            stateImage.sprite = GUIConfig.Instance.QuestStateUIPresets.FirstOrDefault(state).originalIcon;
        }
    }
}