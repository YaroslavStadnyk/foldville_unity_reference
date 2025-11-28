using System;
using System.Collections.Generic;
using System.Linq;
using Core.Configs;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.UI.Structs;
using Grid.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Game.Configs
{
    public class GUIConfig : BaseConfig<GUIConfig>
    {
#if UNITY_EDITOR
        [MenuItem("Game/Select " + nameof(GUIConfig))]
        private static void SelectUIConfig()
        {
            SelectInstanceInEditor();
        }
#endif

        #region Inspector

        [OdinSerialize] private List<Color> _playerColors = new() { Color.blue, Color.red, Color.green, Color.yellow };

        [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Type", ValueLabel = "Preset")]
        [OdinSerialize] private Dictionary<ResourceType, ResourceUIPreset> _resourceUIPresets = new();

        [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Type", ValueLabel = "Preset")]
        [OdinSerialize] private Dictionary<QuestState, QuestStateUIPreset> _questStateUIPresets = new();

        [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Type", ValueLabel = "Preset")]
        [OdinSerialize] private Dictionary<FunctionType, FunctionTypeUIPreset> _functionTypeUIPresets = new();

        [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Type", ValueLabel = "Preset")]
        [OdinSerialize] private Dictionary<AttackType, AttackTypeUIPreset> _attackTypeUIPresets = new();

        [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Type", ValueLabel = "Preset")]
        [OdinSerialize] private Dictionary<TileType, TileTypeUIPreset> _tileTypeUIPresets = new();

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>().ToArray();
            _resourceUIPresets.SetupKeys(resourceTypes);

            var questStates = Enum.GetValues(typeof(QuestState)).Cast<QuestState>().ToArray();
            _questStateUIPresets.SetupKeys(questStates);

            var functionTypes = Enum.GetValues(typeof(FunctionType)).Cast<FunctionType>().ToArray();
            _functionTypeUIPresets.SetupKeys(functionTypes);

            var attackTypes = Enum.GetValues(typeof(AttackType)).Cast<AttackType>().ToArray();
            _attackTypeUIPresets.SetupKeys(attackTypes);

            var tileTypes = Enum.GetValues(typeof(TileType)).Cast<TileType>().ToArray();
            _tileTypeUIPresets.SetupKeys(tileTypes);
        }

        #endregion

        public IReadOnlyList<Color> PlayerColors => _playerColors;
        public IReadOnlyDictionary<ResourceType, ResourceUIPreset> ResourceUIPresets => _resourceUIPresets;
        public IReadOnlyDictionary<QuestState, QuestStateUIPreset> QuestStateUIPresets => _questStateUIPresets;
        public IReadOnlyDictionary<FunctionType, FunctionTypeUIPreset> FunctionTypeUIPresets => _functionTypeUIPresets;
        public IReadOnlyDictionary<AttackType, AttackTypeUIPreset> AttackTypeUIPresets => _attackTypeUIPresets;
        public IReadOnlyDictionary<TileType, TileTypeUIPreset> TileTypeUIPresets => _tileTypeUIPresets;
    }
}