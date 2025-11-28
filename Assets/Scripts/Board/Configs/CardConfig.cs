using System.Collections;
using System.Collections.Generic;
using Board.Models;
using Core.Configs;
using Core.Models;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;

namespace Board.Configs
{
    public class CardConfig : BaseConfig<CardConfig>
    {
#if UNITY_EDITOR
        [MenuItem("Board/Select " + nameof(CardConfig))]
        private static void SelectCardConfig()
        {
            SelectInstanceInEditor();
        }
#endif

        [DictionaryDrawerSettings(KeyLabel = "Tag", ValueLabel = "Card Templates")]
        [HideLabel] [OdinSerialize] private Dictionary<string, RandomPack<Card>> _cardTemplates = new();

        public IReadOnlyDictionary<string, RandomPack<Card>> CardTemplates => _cardTemplates;

        /// <summary>
        /// It's used to make dropdown values in <b>Editor</b>:
        /// [ValueDropdown("@CardConfig.Dropdown")]
        /// </summary>
        public static IEnumerable Dropdown => Instance._cardTemplates.Keys;
    }
}