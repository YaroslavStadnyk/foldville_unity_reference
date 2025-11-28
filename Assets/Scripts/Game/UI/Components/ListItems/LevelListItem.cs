using System;
using System.Collections.Generic;
using Core.UI;
using Game.Logic.Common.Structs;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.ListItems
{
    public class LevelListItem : AnimatedPage
    {
        #region Inspector

        [BoxGroup("Stars")] [SerializeField] private GameObject starImagesRoot;
        [BoxGroup("Stars")] [SerializeField] private List<Sprite> starSprites = new() { null };
        [BoxGroup("Stars")] [SerializeField] private List<Color> starColors = new() { Color.white };

        [SerializeField] private TMP_Text label;
        [SerializeField] private Button launchButton;
        [SerializeField] [ReadOnly] private Canvas canvas;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            SetupComponents();
        }

        #endregion

        private CampaignLevelInfo _levelInfo;

        protected override void Awake()
        {
            base.Awake();
            SetupComponents();
        }

        private void SetupComponents()
        {
            if (canvas == null && !TryGetComponent(out canvas))
            {
                // Debug.LogError($"{name} {nameof(canvas)} is missing.");
            }
        }

        public void Initialize(CampaignLevelInfo levelInfo, Action<CampaignLevelInfo> onLaunch)
        {
            _levelInfo = levelInfo;
            label.text = $"Level {_levelInfo.Index + 1}";
            UpdateStarsCount(levelInfo.StarsCount);

            OnLaunch = onLaunch;
        }

        public void UpdateStarsCount(int starsCount)
        {
            var starImages = starImagesRoot.GetComponentsInChildren<Image>();
            for (var i = 0; i < starImages.Length; i++)
            {
                var starIndex = starsCount > i ? i + 1 : 0;
                var starSprite = starSprites[Mathf.Min(starIndex, starSprites.Count - 1)];
                var starColor = starColors[Mathf.Min(starIndex, starColors.Count - 1)];

                var starImage = starImages[i];
                starImage.sprite = starSprite;
                starImage.color = starColor;
            }
        }

        private void OnEnable()
        {
            if (canvas != null)
            {
                canvas.worldCamera = GameManager.Instance.Camera;
            }

            launchButton.onClick.AddListener(OnLaunchButtonClicked);
        }

        private void OnDisable()
        {
            launchButton.onClick.RemoveListener(OnLaunchButtonClicked);
        }

        private event Action<CampaignLevelInfo> OnLaunch;

        private void OnLaunchButtonClicked()
        {
            OnLaunch?.Invoke(_levelInfo);
        }
    }
}