using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Models
{
    [Serializable]
    public class PaintableGroup : IPaintable
    {
        [SerializeField] public List<Renderer> targetRenderers = new();
        [SerializeField] public List<MaskableGraphic> targetGraphics = new();

        private Dictionary<object, object> _initialColorObjects = new();

        public void SetColor(Color color, float flow = 1.0f)
        {
            SetRenderersColor(color, flow);   
            SetGraphicsColor(color, flow);   
        }

        public void ResetColor()
        {
            SetColor(Color.clear, 0.0f);
        }

        private void SetRenderersColor(Color color, float flow)
        {
            if (targetRenderers.IsNullOrEmpty())
            {
                return;
            }

            foreach (var rend in targetRenderers)
            {
                if (rend == null)
                {
                    continue;
                }

                var initialColorObject = _initialColorObjects.FirstOrDefault(rend);
                switch (rend)
                {
                    case SpriteRenderer spriteRenderer:
                    {
                        if (initialColorObject is not Color initialColor)
                        {
                            initialColor = spriteRenderer.color;
                            _initialColorObjects[rend] = initialColor;
                        }

                        spriteRenderer.SetColor(color, flow, initialColor);
                        break;
                    }
                    case LineRenderer lineRenderer:
                    {
                        if (initialColorObject is not Color[] initialColors)
                        {
                            initialColors = lineRenderer.colorGradient.colorKeys.Select(colorKey => colorKey.color).ToArray();
                            _initialColorObjects[rend] = initialColors;
                        }

                        lineRenderer.SetColor(color, flow, initialColors);
                        break;
                    }
                    case MeshRenderer meshRenderer:
                    {
                        if (initialColorObject is not Color[] initialColors)
                        {
                            initialColors = meshRenderer.sharedMaterials.Select(sharedMaterial => sharedMaterial.color).ToArray();
                            _initialColorObjects[rend] = initialColors;
                        }

                        meshRenderer.SetColor(color, flow, initialColors);
                        break;
                    }
                    default:
                    {
                        Debug.LogError($"The color function of {rend.GetType()} is not implemented.");
                        break;
                    }
                }
            }
        }

        private void SetGraphicsColor(Color color, float flow)
        {
            if (targetGraphics.IsNullOrEmpty())
            {
                return;
            }

            foreach (var graphic in targetGraphics)
            {
                if (graphic == null)
                {
                    continue;
                }

                var initialColorObject = _initialColorObjects.FirstOrDefault(graphic);
                if (initialColorObject is not Color initialColor)
                {
                    initialColor = graphic.color;
                    _initialColorObjects[graphic] = initialColor;
                }

                graphic.color = Color.LerpUnclamped(initialColor, color, flow);
            }
        }
    }
}