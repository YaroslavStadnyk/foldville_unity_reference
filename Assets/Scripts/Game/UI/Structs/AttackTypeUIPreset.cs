using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.UI.Structs
{
    [Serializable] [HideReferenceObjectPicker]
    public struct AttackTypeUIPreset
    {
        [OdinSerialize] [PreviewField(Height = 32)] public Sprite originalIcon;
        [OdinSerialize] [TextArea] public string functionDescription;
        [OdinSerialize] [TextArea] public string controlDescription;
    }
}