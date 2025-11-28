using System.Collections;
using System.Collections.Generic;
using Board.Models;
using Core.Configs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;

namespace Board.Configs
{
    public class DeskConfig : BaseConfig<DeskConfig>
    {
#if UNITY_EDITOR
        [MenuItem("Board/Select " + nameof(DeskConfig))]
        private static void SelectDeskConfig()
        {
            SelectInstanceInEditor();
        }
#endif

        [DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Desk Template")]
        [HideLabel] [OdinSerialize] private Dictionary<string, Desk> _deskTemplates = new();

        public IReadOnlyDictionary<string, Desk> DeskTemplates => _deskTemplates;

        /// <summary>
        /// It's used to make dropdown values in <b>Editor</b>:
        /// [ValueDropdown("@DeskConfig.Dropdown")]
        /// </summary>
        public static IEnumerable Dropdown => Instance._deskTemplates.Keys;
    }
}