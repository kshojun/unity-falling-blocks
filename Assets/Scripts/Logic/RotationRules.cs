namespace FallingBlocks.Logic
{
    /// <summary>回転のルール。壁や床にぶつかるときは、位置をずらして回せないか試す(壁蹴り)。</summary>
    public static class RotationRules
    {
        // ずらし方の候補。左右を先に、次に上へ。床に接したミノが回れるよう、上方向も含める。
        // 正式な SRS(スーパーローテーションシステム)のキックテーブルではなく、簡易版。
        private static readonly Cell[] Kicks =
        {
            new Cell(0, 0),
            new Cell(-1, 0),
            new Cell(1, 0),
            new Cell(-2, 0),
            new Cell(2, 0),
            new Cell(0, 1),
            new Cell(-1, 1),
            new Cell(1, 1),
            new Cell(0, 2),
        };

        public static bool TryRotate(Board board, Piece piece, int direction, out Piece result)
        {
            var rotated = piece.Rotated(direction);
            foreach (var kick in Kicks)
            {
                var candidate = rotated.Moved(kick.X, kick.Y);
                if (board.CanPlace(candidate))
                {
                    result = candidate;
                    return true;
                }
            }

            result = piece;
            return false;
        }
    }
}
