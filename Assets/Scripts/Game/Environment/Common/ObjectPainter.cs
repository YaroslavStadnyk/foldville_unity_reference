using System.Linq;
using Core.Interfaces;
using Core.Models;
using Core.Ordinaries;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Environment.Common
{
    public class ObjectPainter : PoolBehaviour, IPaintable
    {
        #region Inspector

        [HideLabel] [SerializeField] protected PaintableGroup paintGroup = new();

        [Button]
        private void CastChildRenderers()
        {
            paintGroup.targetRenderers = GetComponentsInChildren<Renderer>().ToList();
        }

        [Button]
        private void CastChildGraphics()
        {
            paintGroup.targetGraphics = GetComponentsInChildren<MaskableGraphic>().ToList();
        }

        #endregion

        public PaintableGroup GetPaintableGroup()
        {
            return paintGroup;
        }

        public void SetColor(Color color, float flow = 1.0f)
        {
            paintGroup.SetColor(color, flow);
        }

        public void ResetColor()
        {
            paintGroup.ResetColor();
        }
    }
}