using Game.Logic.Common.Enums;
using Game.UI;

namespace Game.Environment.Common
{
    public class ObjectBonusLabel : ObjectLabel
    {
        private string _suffix;

        public void SetType(ResourceType type)
        {
            _suffix = type is ResourceType.Energy ? Emojis.EnergyCoin : "$";
        }

        public void SetValue(int value)
        {
            Text = $"{value.ToString("+#;-#;0")} {_suffix}";
        }
    }
}