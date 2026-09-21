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

        /// <summary>左右に 1 マス動かす。動かせたら true。</summary>
        public bool TryMove(int dx)
        {
            if (IsGameOver)
            {
                return false;
            }

            var moved = Current.Moved(dx, 0);
            if (!Board.CanPlace(moved))
            {
                return false;
            }

            Current = moved;
            return true;
        }

        /// <summary>direction が +1 なら時計回り、-1 なら反時計回りに回す。回せたら true。</summary>
        public bool TryRotate(int direction)
        {
            if (IsGameOver || !RotationRules.TryRotate(Board, Current, direction, out var rotated))
            {
                return false;
            }

            Current = rotated;
            return true;
        }

        /// <summary>自分の操作で 1 マス落とす(ソフトドロップ)。落とせたら true。</summary>
        public bool SoftDrop()
        {
            if (IsGameOver)
            {
                return false;
            }

            var moved = Current.Moved(0, -1);
            if (!Board.CanPlace(moved))
            {
                return false;
            }

            Current = moved;
            fallTimer = 0f;
            return true;
        }

        /// <summary>いちばん下まで一気に落として、その場に固定する(ハードドロップ)。</summary>
        public void HardDrop()
        {
            if (IsGameOver)
            {
                return;
            }

            Current = GetGhost();
            LockCurrent();
        }

        /// <summary>いま落とした場合に着地する位置(ゴースト表示用)。</summary>
        public Piece GetGhost()
        {
            var landing = Current;
            while (Board.CanPlace(landing.Moved(0, -1)))
            {
                landing = landing.Moved(0, -1);
            }

            return landing;
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
