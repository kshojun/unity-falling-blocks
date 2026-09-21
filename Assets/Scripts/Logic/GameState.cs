using System;

namespace FallingBlocks.Logic
{
    /// <summary>
    /// ゲームの進行を管理する。Unity には依存せず、時間は Tick(deltaTime) で受け取る。
    /// </summary>
    public sealed class GameState
    {
        private const float FallInterval = 1f;

        private readonly Random random;
        private float fallTimer;

        public Board Board { get; } = new Board();
        public Piece Current { get; private set; }
        public bool IsGameOver { get; private set; }

        public GameState(Random random)
        {
            this.random = random;
            SpawnNext();
        }

        /// <summary>時間を進める。一定時間ごとに、落下中のミノを 1 マス落とす。</summary>
        public void Tick(float deltaTime)
        {
            if (IsGameOver)
            {
                return;
            }

            fallTimer += deltaTime;
            while (fallTimer >= FallInterval && !IsGameOver)
            {
                fallTimer -= FallInterval;
                StepDown();
            }
        }

        /// <summary>1 マス落とす。落とせなければその場に固定して、次のミノを出す。</summary>
        private void StepDown()
        {
            var moved = Current.Moved(0, -1);
            if (Board.CanPlace(moved))
            {
                Current = moved;
                return;
            }

            LockCurrent();
        }

        private void LockCurrent()
        {
            bool allInside = Board.Place(Current);
            Board.ClearFullLines();

            if (!allInside)
            {
                // 盤面の外にはみ出したまま固定された
                IsGameOver = true;
                return;
            }

            SpawnNext();
        }

        private void SpawnNext()
        {
            Current = Piece.Spawn((TetrominoType)random.Next(7));
            fallTimer = 0f;

            if (!Board.CanPlace(Current))
            {
                // 出現位置がすでに埋まっている
                IsGameOver = true;
            }
        }
    }
}
