using System;
using System.Collections.Generic;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Grid.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Models
{
    [CreateAssetMenu(fileName = "New " + nameof(FactionsDefinition), menuName = nameof(FactionsDefinition), order = 1)]
    public class FactionsDefinition : SerializedScriptableObject
    {
        #region Inspector

        [Space] [LabelText("Max Length")] [OdinSerialize] [Min(1)] private int _maxOriginLength = 3;
        [ListDrawerSettings(CustomAddFunction = nameof(AddOriginType))] [OnValueChanged(nameof(OnOriginChanged), true)]
        [LabelText("Default Node")] [OdinSerialize] private HashSet<TileType> _defaultOriginTypes = new() { TileType.House };

        [DictionaryDrawerSettings(KeyLabel = "Origin", ValueLabel = "Target")] [OnValueChanged(nameof(OnNodesChanged), true)]
        [Space] [OdinSerialize] private Dictionary<TileType, HashSet<TileType>> _nodes = new();

        [NonSerialized] [ShowInInspector] [PropertyOrder(-2)] private bool _advancedSettings = false;

        [ShowIf(nameof(_advancedSettings))] [ListDrawerSettings(CustomAddFunction = nameof(AddIgnoredType))]
        [Space] [OdinSerialize] [PropertyOrder(-1)] private List<TileType> _ignoredTypes = new();

        private void AddOriginType()
        {
            _defaultOriginTypes.AddUniqueValue();
        }

        private void AddIgnoredType()
        {
            _ignoredTypes.AddUniqueValue();
        }

        private void OnNodesChanged()
        {
            // var types = Enum.GetValues(typeof(TileType)).Cast<TileType>().ToArray();
            // var filteredTypes = types.Where(IsNotIgnored).ToArray();

            foreach (var ignoredType in _ignoredTypes)
            {
                if (_nodes.Remove(ignoredType))
                {
                    Debug.LogWarning($"The {ignoredType} type is ignored.");
                }
            }

            foreach (var node in _nodes.Values)
            {
                foreach (var ignoredType in _ignoredTypes)
                {
                    if (node.Remove(ignoredType))
                    {
                        Debug.LogWarning($"The {ignoredType} type is ignored.");
                    }
                }
            }
        }

        private void OnOriginChanged()
        {
            foreach (var ignoredType in _ignoredTypes)
            {
                if (_defaultOriginTypes.Remove(ignoredType))
                {
                    Debug.LogWarning($"The {ignoredType} type is ignored.");
                }
            }
        }

        #endregion

        public const int Cost = 1;
        public const ResourceType CostType = ResourceType.FactionPoints;
        public int MaxOriginLenght => _maxOriginLength;
        public IReadOnlyCollection<TileType> DefaultOrigin => _defaultOriginTypes;
        public IReadOnlyDictionary<TileType, HashSet<TileType>> Nodes => _nodes;
    }
}