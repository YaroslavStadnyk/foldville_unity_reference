using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.UI.Structs
{
    [Serializable] [HideReferenceObjectPicker]
    public struct ResourceUIPreset
    {
        [OdinSerialize] [PreviewField(Height = 32)] public Sprite originalIcon;
        [OdinSerialize] [PreviewField(Height = 32)] public Sprite secondaryIcon;
    }
}