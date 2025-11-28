using MathModule.Structs;

namespace Grid.Common
{
    public interface ITile
    {
        public TileType Type { get; }
        public Int2 IndexPosition { get; set; }
    }
}