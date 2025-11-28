using System;
using Core.Pooling;
using Core.UI;
using Game.Environment;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Game.UI.Components.ListItems;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Components.Pages
{
    public class LevelsPage : AnimatedPage
    {
        #region Inspector

        [Title("Animation")] [BoxGroup(nameof(Page))] [SerializeField] private float levelItemsAnimationDelay = 1f;

        [SerializeField] private CameraAnimator cameraAnimator;

        [Space] [SerializeField] private Transform listItemsRoot;
        [SerializeField] private PoolList<LevelListItem> listItemsPool = new();

        #endregion

        protected override void Awake()
        {
            base.Awake();
            InitializeListItems();
        }

        private void InitializeListItems()
        {
            var levelInfos = GameManager.Instance.LevelInfos;
            var levelsCount = Mathf.Min(listItemsRoot.childCount, levelInfos.Count);
            for (var i = 0; i < levelsCount; i++)
            {
                var listItem = listItemsPool.Spawn();
                listItem.Initialize(levelInfos[i], LaunchLevel);

                var listItemRoot = listItemsRoot.GetChild(i);
                listItem.transform.SetParent(listItemRoot);

                listItem.animationDelay = levelItemsAnimationDelay * ((float)i / levelsCount);
            }
        }

        private void UpdateListItemsStarsCount()
        {
            var listItems = listItemsPool.SpawnedBehaviours;
            var levelInfos = GameManager.Instance.LevelInfos;
            var levelsCount = Mathf.Min(listItems.Count, levelInfos.Count);
            for (var i = 0; i < levelsCount; i++)
            {
                listItems[i].UpdateStarsCount(levelInfos[i].StarsCount);
            }
        }

        public override void Show()
        {
            UpdateListItemsStarsCount();

            base.Show();
            ShowListItems();

            cameraAnimator.dynamicAnimation.Time = 0f;
            cameraAnimator.Mode = CameraAnimator.AnimationMode.Dynamic;
        }

        private void ShowListItems()
        {
            foreach (var listItem in listItemsPool.SpawnedBehaviours)
            {
                if (listItem != null)
                {
                    listItem.Show();
                }
            }
        }

        public override void Hide()
        {
            base.Hide();
            HideListItems();

            cameraAnimator.Mode = CameraAnimator.AnimationMode.Static;
        }

        private void HideListItems()
        {
            foreach (var listItem in listItemsPool.SpawnedBehaviours)
            {
                if (listItem != null)
                {
                    listItem.Hide();
                }
            }
        }

        public Action OnBackToMenu;

        public void BackToMenu()
        {
            OnBackToMenu?.Invoke();
        }

        private void LaunchLevel(CampaignLevelInfo levelInfo)
        {
            GameConfig.Instance.CampaignModePreset.SelectedLevelIndex = levelInfo.Index;
            GameManager.Instance.InitializeManagers(GameMode.Campaign);
            NetworkManager.singleton.StartHost();
        }
    }
}