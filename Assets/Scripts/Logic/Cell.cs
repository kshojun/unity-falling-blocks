namespace FallingBlocks.Logic
{
    /// <summary>盤面上の 1 マスの座標。</summary>
    public readonly struct Cell
    {
        public int X { get; }
        public int Y { get; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
