using System;
using System.Collections.Generic;
using System.Linq;
using Core.Configs;
using Core.Extensions;
using Core.Serialization;
using Game.Logic.Common.Models;
using Grid.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Game.Logic.Configs
{
    public class GameConfig : BaseConfig<GameConfig>
    {
#if UNITY_EDITOR
        [MenuItem("Game/Select " + nameof(GameConfig))]
        private static void SelectConfig()
        {
            SelectInstanceInEditor();
        }
#endif

        #region Inspector

        [TabGroup("Campaign")]
        [HideLabel] [OdinSerialize] [InlineProperty] private CampaignModePreset _campaignModePreset = new();

        [TabGroup("Multiplayer")]
        [HideLabel] [OdinSerialize] [InlineProperty] private MultiplayerModePreset _multiplayerModePreset = new();

        [TabGroup("Buildings")]
        [NonSerialized] [ShowInInspector] private bool _advancedSettings = false;

#if UNITY_EDITOR

        [TabGroup("Buildings")] [ShowIf(nameof(_advancedSettings))] [OnValueChanged(nameof(OnIgnoredTypesChanged), true)]
        [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Type", ValueLabel = "IsIgnored")]
        [Space] [SerializeField] private SerializedDictionary<TileType, bool> ignoredTypes = new();

#endif

        [TabGroup("Buildings")] [ShowIf(nameof(_advancedSettings))]
        [DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Type", ValueLabel = "Definition")]
        [Space] [HideLabel] [SerializeField] private SerializedDictionary<TileType, BuildingDefinition> buildingDefinitions = new();

#if UNITY_EDITOR

        #region Selected

        [TabGroup("Buildings")] [HideIf(nameof(_advancedSettings))]
        [Space] [HideLabel] [NonSerialized] [ShowInInspector] private TileType _selectedType;

        [TabGroup("Buildings")] [HideIf(nameof(IsSelectedDefinitionHidden))]
        [HideLabel] [InlineProperty] [ShowInInspector] private BuildingDefinition SelectedDefinition
        {
            get
            {
                if (buildingDefinitions.TryGetValue(_selectedType, out var buildingDefinition))
                {
                    return buildingDefinition;
                }


                Debug.LogWarning($"{nameof(BuildingDefinition)} of {nameof(TileType)} '{_selectedType}' is ignored.");
                return null;
            }
            set => buildingDefinitions[_selectedType] = value;
        }

        private bool IsSelectedDefinitionHidden => _advancedSettings || !buildingDefinitions.ContainsKey(_selectedType);

        #endregion

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            var types = Enum.GetValues(typeof(TileType)).Cast<TileType>().ToArray();
            ignoredTypes.SetupKeys(types);

            var filteredTypes = types.Where(IsNotIgnored).ToArray();
            buildingDefinitions.SetupKeys(filteredTypes);

            if (!buildingDefinitions.ContainsKey(_selectedType))
            {
                _selectedType = buildingDefinitions.Keys.FirstOrDefault();
            }
        }

        private void OnIgnoredTypesChanged()
        {
            OnInspectorInit();
        }

        private bool IsNotIgnored(TileType type)
        {
            return !ignoredTypes.FirstOrDefault(type);
        }

#endif

        #endregion

        public CampaignModePreset CampaignModePreset => _campaignModePreset;
        public MultiplayerModePreset MultiplayerPreset => _multiplayerModePreset;
        public IReadOnlyDictionary<TileType, BuildingDefinition> BuildingDefinitions => buildingDefinitions;
    }
}