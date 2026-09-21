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

        /// <summary>
        /// ミノの 4 マスを盤面に書き込む(固定する)。
        /// 盤面より上にはみ出したマスがあれば、その分は書き込まず false を返す。
        /// </summary>
        public bool Place(Piece piece)
        {
            bool allInside = true;
            for (int i = 0; i < TetrominoShapes.CellCount; i++)
            {
                var cell = piece.GetCell(i);
                if (cell.Y >= Height)
                {
                    allInside = false;
                    continue;
                }

                cells[cell.X, cell.Y] = (int)piece.Type + 1;
            }

            return allInside;
        }

        /// <summary>揃った行を消して、上の行を詰める。消した行数を返す。</summary>
        public int ClearFullLines()
        {
            int cleared = 0;
            int writeY = 0;

            // 下の行から順に見て、揃っていない行だけを下へ詰めてコピーしていく
            for (int y = 0; y < Height; y++)
            {
                if (IsRowFull(y))
                {
                    cleared++;
                    continue;
                }

                if (writeY != y)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        cells[x, writeY] = cells[x, y];
                    }
                }

                writeY++;
            }

            // 詰めた分だけ空いた最上段側を空にする
            for (int y = writeY; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    cells[x, y] = 0;
                }
            }

            return cleared;
        }

        private bool IsRowFull(int y)
        {
            for (int x = 0; x < Width; x++)
            {
                if (cells[x, y] == 0)
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
