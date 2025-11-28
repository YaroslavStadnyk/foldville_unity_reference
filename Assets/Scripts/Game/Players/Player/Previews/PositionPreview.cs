using Core.Pooling;
using Game.Environment.Common;
using Grid.Hexagonal;
using MathModule.Models;
using MathModule.Structs;
using UnityEngine;

namespace Game.Players.Player.Previews
{
    public class PositionPreview : PreviewExtension
    {
        #region Inspector

        [SerializeField] private ObjectPainter hexTileOutlinePrefab;

        [Space] [SerializeField] private Gradient gradient = new();
        [SerializeField] private float flow = 1.0f;
        [SerializeField] [Min(0)] private int radius = 3;

        #endregion

        private readonly PoolDictionary<ObjectPainter, Int2> _hexTileOutlinePool = new();

        private void Awake()
        {
            _hexTileOutlinePool.prefab = hexTileOutlinePrefab;
            _hexTileOutlinePool.parent = PreviewRoot;

            SetupHexTileOutlines();
        }

        private void SetupHexTileOutlines()
        {
            for (var n = 0; n <= radius; n++)
            {
                var form = HexForm.Create(n, n - 1);
                foreach (var localIndexPosition in form)
                {
                    var hexTileColor = GetColorByDistance(n);

                    var hexTileOutline = _hexTileOutlinePool.Spawn(localIndexPosition);
                    hexTileOutline.SetColor(hexTileColor, flow);

                    HexTile.SetIndexPosition(hexTileOutline.transform, localIndexPosition);
                }
            }
        }

        private Color GetColorByDistance(float distance)
        {
            var time = radius <= 0 ? 0 : Mathf.Clamp01(distance / radius);
            var color = gradient.Evaluate(time);

            return color;
        }

        private Int2 _lastIndexPosition;

        public void Setup(Int2 newIndexPosition)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (newIndexPosition == _lastIndexPosition)
            {
                return;
            }

            _lastIndexPosition = newIndexPosition;

            HexTile.SetIndexPosition(PreviewRoot, newIndexPosition);
        }
    }
}