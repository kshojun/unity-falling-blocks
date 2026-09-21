namespace FallingBlocks.Logic
{
    /// <summary>
    /// 落下中のミノ。位置と向きを持つ不変(イミュータブル)な値で、
    /// 動かす・回すときは新しい Piece を返す。「試してみて、置けたら採用する」と書きやすい。
    /// </summary>
    public readonly struct Piece
    {
        public TetrominoType Type { get; }
        public int Rotation { get; }

        /// <summary>箱の左下の盤面座標。</summary>
        public int X { get; }
        public int Y { get; }

        public Piece(TetrominoType type, int rotation, int x, int y)
        {
            Type = type;
            Rotation = ((rotation % 4) + 4) % 4;
            X = x;
            Y = y;
        }

        /// <summary>盤面の上端に、左右中央寄りで出現させる。</summary>
        public static Piece Spawn(TetrominoType type)
        {
            int x = (Board.Width - TetrominoShapes.BoxSize(type)) / 2;
            int y = Board.Height - 1 - TetrominoShapes.TopY(type);
            return new Piece(type, 0, x, y);
        }

        public Piece Moved(int dx, int dy)
        {
            return new Piece(Type, Rotation, X + dx, Y + dy);
        }

        /// <summary>direction が +1 なら時計回り、-1 なら反時計回り。</summary>
        public Piece Rotated(int direction)
        {
            return new Piece(Type, Rotation + direction, X, Y);
        }

        /// <summary>index 番目(0〜3)のマスの盤面座標。</summary>
        public Cell GetCell(int index)
        {
            var local = TetrominoShapes.Get(Type, Rotation)[index];
            return new Cell(X + local.X, Y + local.Y);
        }
    }
}
