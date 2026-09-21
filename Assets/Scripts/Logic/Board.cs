namespace FallingBlocks.Logic
{
    /// <summary>
    /// 10×20 の盤面。0 は空きマス、1 以上は置かれたブロックの種類を表す。
    /// 座標は左下が (0, 0) で、x は右、y は上に増える。
    /// </summary>
    public sealed class Board
    {
        public const int Width = 10;
        public const int Height = 20;

        private readonly int[,] cells = new int[Width, Height];

        public int Get(int x, int y)
        {
            return cells[x, y];
        }

        public void Set(int x, int y, int value)
        {
            cells[x, y] = value;
        }

        /// <summary>
        /// そのマスにブロックを置けるか。左右の壁と床は「埋まっている」扱い、
        /// 盤面より上(ミノが出現する場所)は「空き」扱いにする。
        /// </summary>
        public bool IsEmpty(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0)
            {
                return false;
            }

            if (y >= Height)
            {
                return true;
            }

            return cells[x, y] == 0;
        }

        /// <summary>ミノの 4 マスがすべて空きマスの上にあるか。</summary>
        public bool CanPlace(Piece piece)
        {
            for (int i = 0; i < TetrominoShapes.CellCount; i++)
            {
                var cell = piece.GetCell(i);
                if (!IsEmpty(cell.X, cell.Y))
                {
                    return false;
                }
            }

            return true;
        }

        public void Clear()
        {
            System.Array.Clear(cells, 0, cells.Length);
        }
    }
}
