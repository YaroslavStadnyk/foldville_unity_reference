using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.Pages
{
    public class LoadingPage : AnimatedPage
    {
        [SerializeField] private TMP_Text progressionText;
        [SerializeField] private Image progressionFill;

        public string ProgressionText
        {
            get => progressionText.text;
            set => progressionText.text = value;
        }

        public float ProgressionFill
        {
            get => progressionFill.fillAmount;
            set => progressionFill.fillAmount = value;
        }
    }
}
