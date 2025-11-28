using UnityEngine;

namespace Core.Interfaces
{
    public interface IPaintable
    {
        /// <param name="color"> target color. </param>
        /// <param name="flow"> use 0% of initial color and 100% of target color if equals 1F. </param>
        public void SetColor(Color color, float flow = 1.0f);
        public void ResetColor();
    }

}