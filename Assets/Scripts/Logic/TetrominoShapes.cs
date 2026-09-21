namespace FallingBlocks.Logic
{
    /// <summary>
    /// 各ミノの形。出現時の向きだけを定義し、残りの回転は「箱の中で 90 度回す」計算で作る。
    /// 座標は箱の左下が (0, 0) で、y は上に増える。
    /// </summary>
    public static class TetrominoShapes
    {
        public const int CellCount = 4;

        // 回転の基準になる正方形の箱の大きさ(I は 4×4、O は 2×2、ほかは 3×3)
        private static readonly int[] BoxSizes = { 4, 2, 3, 3, 3, 3, 3 };

        // 出現時の向き。順番は TetrominoType の I, O, T, S, Z, J, L と同じ。
        private static readonly Cell[][] SpawnCells =
        {
            // I:  ....   O:  XX   T:  .X.   S:  .XX   Z:  XX.   J:  X..   L:  ..X
            //     ....       XX       XXX       XX.       .XX       XXX       XXX
            //     XXXX
            new[] { new Cell(0, 2), new Cell(1, 2), new Cell(2, 2), new Cell(3, 2) },
            new[] { new Cell(0, 0), new Cell(1, 0), new Cell(0, 1), new Cell(1, 1) },
            new[] { new Cell(1, 2), new Cell(0, 1), new Cell(1, 1), new Cell(2, 1) },
            new[] { new Cell(1, 2), new Cell(2, 2), new Cell(0, 1), new Cell(1, 1) },
            new[] { new Cell(0, 2), new Cell(1, 2), new Cell(1, 1), new Cell(2, 1) },
            new[] { new Cell(0, 2), new Cell(0, 1), new Cell(1, 1), new Cell(2, 1) },
            new[] { new Cell(2, 2), new Cell(0, 1), new Cell(1, 1), new Cell(2, 1) },
        };

        // [ミノの種類][回転 0〜3][4 マス]
        private static readonly Cell[][][] Rotations = BuildRotations();

        /// <summary>指定した種類・向きの 4 マス(箱の中の座標)を返す。rotation は何度でも増減してよい。</summary>
        public static Cell[] Get(TetrominoType type, int rotation)
        {
            int index = ((rotation % 4) + 4) % 4;
            return Rotations[(int)type][index];
        }

        public static int BoxSize(TetrominoType type)
        {
            return BoxSizes[(int)type];
        }

        /// <summary>出現時の向きで、いちばん上にあるマスの y 座標。</summary>
        public static int TopY(TetrominoType type)
        {
            int top = 0;
            foreach (var cell in SpawnCells[(int)type])
            {
                if (cell.Y > top)
                {
                    top = cell.Y;
                }
            }

            return top;
        }

        private static Cell[][][] BuildRotations()
        {
            var result = new Cell[SpawnCells.Length][][];
            for (int t = 0; t < SpawnCells.Length; t++)
            {
                int n = BoxSizes[t];
                result[t] = new Cell[4][];
                result[t][0] = SpawnCells[t];

                for (int r = 1; r < 4; r++)
                {
                    var previous = result[t][r - 1];
                    var rotated = new Cell[CellCount];
                    for (int i = 0; i < CellCount; i++)
                    {
                        // 時計回りに 90 度: (x, y) → (y, n - 1 - x)
                        rotated[i] = new Cell(previous[i].Y, n - 1 - previous[i].X);
                    }

                    result[t][r] = rotated;
                }
            }

            return result;
        }
    }
}
