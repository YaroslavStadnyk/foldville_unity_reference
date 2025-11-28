using Core.UI;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.Holders
{
    public class PlayerHolder : AnimatedPage
    {
        [Space] [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text idText;
        [SerializeField] private TMP_Text performanceText;

        [Space] [SerializeField] private Image performanceImage;
        [SerializeField] private Sprite winnerIcon;
        [SerializeField] private Sprite loserIcon;

        public void Initialize(string playerID, PartyPlayerStats playerStats)
        {
            idText.text = playerID;
            SetState(playerStats.state);
            SetPerformance(playerStats.performance);
        }

        public void SetState(PartyPlayerState playerState)
        {
            performanceImage.sprite = playerState is PartyPlayerState.Won or PartyPlayerState.Default ? winnerIcon : loserIcon;
        }

        public void SetPerformance(PartyPlayerPerformance playerPerformance)
        {
            performanceText.text = GetPerformanceText(playerPerformance);
        }

        private string GetPerformanceText(PartyPlayerPerformance playerPerformance)
        {
            return $"{playerPerformance.capturesCount} captures\n" +
                   $"{playerPerformance.attacksCount} attacks\n" +
                   $"{playerPerformance.buildingsCount} buildings\n" +
                   $"{playerPerformance.spendsCount} {playerPerformance.SpendsType.ToString().ToLower()} spends";
        }
    }
}