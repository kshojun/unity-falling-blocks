namespace FallingBlocks.Logic
{
    /// <summary>得点の計算。ライン消去は、消した行数が多いほど、またレベルが高いほど高得点。</summary>
    public static class ScoreCalculator
    {
        // 1 行〜4 行を同時に消したときの基本点(0 行は 0 点)
        private static readonly int[] LineClearPoints = { 0, 100, 300, 500, 800 };

        public const int SoftDropPointsPerCell = 1;
        public const int HardDropPointsPerCell = 2;

        public static int LineClear(int lines, int level)
        {
            if (lines <= 0)
            {
                return 0;
            }

            int index = lines < LineClearPoints.Length ? lines : LineClearPoints.Length - 1;
            return LineClearPoints[index] * level;
        }
    }
}
