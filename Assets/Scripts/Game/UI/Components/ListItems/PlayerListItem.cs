using System.Collections.Generic;
using Core.Extensions;
using Core.Models;
using Core.Ordinaries;
using Game.Configs;
using Game.Players.Player;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.ListItems
{
    public class PlayerListItem : PoolBehaviour
    {
        [SerializeField] private TMP_Text playerIDText;
        [SerializeField] private TMP_Text progressText;

        [BoxGroup("Selection")] [SerializeField] private Image selectionImage;
        [BoxGroup("Selection")] [SerializeField] private float selectionSizeScale = 1.625f;
        private float _initialDeltaSizeY;

        [BoxGroup("Default State")] [SerializeField] private float playerColorsFlow = 0.5f;
        [BoxGroup("Default State")] [ShowInInspector] private IReadOnlyList<Color> PlayerColors => GUIConfig.Instance.PlayerColors;
        [BoxGroup("Default State")] [HideLabel] [SerializeField] [PropertyOrder(1)] private PaintableGroup paintableGroup = new();

        [BoxGroup("Lost State")] [SerializeField] private Image lostImage;
        [BoxGroup("Lost State")] [SerializeField] private float lostColorFlow = 0.5f;
        [BoxGroup("Lost State")] [SerializeField] private Color lostColor = Color.grey;
        [BoxGroup("Lost State")] [HideLabel] [SerializeField] [PropertyOrder(1)] private PaintableGroup lostPaintableGroup = new();

        private void Awake()
        {
            _initialDeltaSizeY = (transform as RectTransform)?.sizeDelta.y ?? 0f;

            SetValue(0);
            Deselect();
        }

        public void Initialize(string playerID)
        {
            playerIDText.text = PlayerProfile.GetNickname(playerID);
            var playerColor = PlayerProfile.GetColor(playerID);
            paintableGroup.SetColor(playerColor, playerColorsFlow);
        }

        public void SetValue(int value)
        {
            progressText.text = $"{value.ToShort()}%";
        }

        public void Select()
        {
            if (transform is RectTransform rectTransform)
            {
                rectTransform.sizeDelta = rectTransform.sizeDelta.WithY(_initialDeltaSizeY * selectionSizeScale);
            }

            selectionImage.gameObject.SetActive(true);
        }

        public void Deselect()
        {
            if (transform is RectTransform rectTransform)
            {
                rectTransform.sizeDelta = rectTransform.sizeDelta.WithY(_initialDeltaSizeY);
            }

            selectionImage.gameObject.SetActive(false);
        }

        public void SetLostState()
        {
            lostImage.gameObject.SetActive(true);
            lostPaintableGroup.SetColor(lostColor, lostColorFlow);
        }
    }
}