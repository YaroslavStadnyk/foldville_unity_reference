using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.UI.Structs
{
    [Serializable] [HideReferenceObjectPicker]
    public struct FunctionTypeUIPreset
    {
        [OdinSerialize] [PreviewField(Height = 32)] public Sprite originalIcon;
    }
}