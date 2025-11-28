using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Serialization
{
    [Serializable]
    public class SerializedHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private List<T> valueData = new();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();

            foreach (var value in valueData)
            {
                this.Add(value);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            valueData.Clear();

            foreach (var value in this)
            {
                valueData.Add(value);
            }
        }
    }
}