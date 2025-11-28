using Game.Environment.Common;
using Grid.Hexagonal;
using UnityEngine;

namespace Game.Players.Player.Previews
{
    public class HexTilePreview : PreviewExtension
    {
        #region Inspector

        [SerializeField] private ObjectPainter hexTileOutlinePrefab;

        [Space] [SerializeField] private Color color = Color.white;
        [SerializeField] private float flow = 1.0f;

        #endregion

        private ObjectPainter _hexTileOutlineInstance;

        private HexTile _lastHexTile;

        private void Awake()
        {
            _hexTileOutlineInstance = Instantiate(hexTileOutlinePrefab, PreviewRoot);
            _hexTileOutlineInstance.SetColor(color, flow);
            _hexTileOutlineInstance.gameObject.SetActive(false);
        }

        public void Setup(HexTile newHexTile)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (newHexTile == _lastHexTile)
            {
                return;
            }

            if (newHexTile == null)
            {
                _hexTileOutlineInstance.gameObject.SetActive(false);
                return;
            }

            _hexTileOutlineInstance.gameObject.SetActive(true);
            HexTile.SetIndexPosition(PreviewRoot, newHexTile.IndexPosition);
        }
    }
}