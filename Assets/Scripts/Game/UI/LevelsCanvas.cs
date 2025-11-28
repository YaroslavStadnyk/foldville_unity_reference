using Core.Ordinaries;
using Game.UI.Components.Pages;
using UnityEngine;

namespace Game.UI
{
    public class LevelsCanvas : SingletonBehaviour<LevelsCanvas>
    {
        [SerializeField] private LevelsPage levelsPage;
        public LevelsPage Page => levelsPage;
    }
}
