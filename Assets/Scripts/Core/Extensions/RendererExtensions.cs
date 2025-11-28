using UnityEngine;

namespace Core.Extensions
{
    public static class RendererExtensions
    {
        /// <param name="meshRenderer"></param>
        /// <param name="color"> target color. </param>
        /// <param name="flow"> use 0% of initial color and 100% of target color if equals 1F. </param>
        /// <param name="initialColors"></param>
        public static void SetColor(this MeshRenderer meshRenderer, Color color, float flow = 1.0f, params Color[] initialColors)
        {
            var initialColorsLength = initialColors?.Length ?? 0;
            for (var i = 0; i < meshRenderer.sharedMaterials.Length; i++)
            {
                var initialColor = initialColorsLength > i ? initialColors[i] : meshRenderer.sharedMaterials[i].color;
                meshRenderer.materials[i].color = Color.LerpUnclamped(initialColor, color, flow);
            }
        }

        /// <param name="spriteRenderer"></param>
        /// <param name="color"> target color. </param>
        /// <param name="flow"> use 0% of initial color and 100% of target color if equals 1F. </param>
        /// <param name="initialColor"></param>
        public static void SetColor(this SpriteRenderer spriteRenderer, Color color, float flow = 1.0f, Color? initialColor = null)
        {
            var initialTrueColor = initialColor ?? spriteRenderer.color;
            color.a = initialTrueColor.a;

            spriteRenderer.color = Color.LerpUnclamped(initialTrueColor, color, flow);
        }

        /// <param name="lineRenderer"></param>
        /// <param name="color"> target color. </param>
        /// <param name="flow"> use 0% of initial color and 100% of target color if equals 1F. </param>
        /// <param name="initialColors"></param>
        public static void SetColor(this LineRenderer lineRenderer, Color color, float flow = 1.0f, params Color[] initialColors)
        {
            var colorGradient = lineRenderer.colorGradient;
            var colorKeys = colorGradient.colorKeys;

            var initialColorsLength = initialColors?.Length ?? 0;
            for (var i = 0; i < colorKeys.Length; i++)
            {
                var initialColor = initialColorsLength > i ? initialColors[i] : colorKeys[i].color;
                colorKeys[i].color = Color.LerpUnclamped(initialColor, color, flow);
            }

            colorGradient.colorKeys = colorKeys;
            lineRenderer.colorGradient = colorGradient;
        }
    }
}