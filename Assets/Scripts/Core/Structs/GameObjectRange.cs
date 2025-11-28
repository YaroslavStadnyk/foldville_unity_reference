using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Core.Structs
{
    [Serializable]
    public struct GameObjectRange
    {
        [HorizontalGroup] [HideLabel] [OdinSerialize] [MinMaxSlider(0, 6)] public Vector2Int range;
        [HorizontalGroup] [HideLabel] [OdinSerialize] public GameObject gameObject;

        public void SetActive(int value)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(value >= range.x && value <= range.y);
            }
        }
    }
}