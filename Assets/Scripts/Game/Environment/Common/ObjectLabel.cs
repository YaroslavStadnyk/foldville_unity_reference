using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.Ordinaries;
using TMPro;
using UnityEngine;

namespace Game.Environment.Common
{
    public class ObjectLabel : PoolBehaviour, IPaintable
    {
        #region Inspector

        [SerializeField] private TMP_Text labelText;

        #endregion

        private Color _initialLabelColor;
        private Vector3 _initialLabelScale;

        protected virtual void Awake()
        {
            _initialLabelColor = labelText.color;
            _initialLabelScale = labelText.transform.localScale;
        }

        public virtual void SetColor(Color color, float flow = 1.0f)
        {
            labelText.color =  Color.LerpUnclamped(_initialLabelColor, color, flow);
        }

        public void ResetColor()
        {
            SetColor(Color.clear, 0.0f);
        }

        public string Text
        {
            get => labelText.text;
            set => labelText.text = value;
        }

        public Vector3 Size
        {
            get => transform.localScale.Divide(_initialLabelScale);
            set => transform.localScale = _initialLabelScale.Multiply(value);
        }
    }
}