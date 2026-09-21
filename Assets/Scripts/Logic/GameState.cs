using System;

namespace FallingBlocks.Logic
{
    /// <summary>
    /// ゲームの進行を管理する。Unity には依存せず、時間は Tick(deltaTime) で受け取る。
    /// </summary>
    public sealed class GameState
    {
        /// <summary>NEXT に表示するミノの数。</summary>
        public const int NextCount = 3;

        private const int LinesPerLevel = 10;
        private const float MinFallInterval = 0.05f;

        private readonly PieceBag bag;
        private float fallTimer;
        private bool canHold = true;

        public Board Board { get; } = new Board();
        public Piece Current { get; private set; }
        public bool IsGameOver { get; private set; }

        public int Score { get; private set; }
        public int Lines { get; private set; }

        /// <summary>レベルは 1 から始まり、10 ライン消すごとに 1 つ上がる。</summary>
        public int Level => Lines / LinesPerLevel + 1;

        /// <summary>1 マス落ちるまでの秒数。レベルが上がるほど短くなる。</summary>
        public float FallInterval => GetFallInterval(Level);

        /// <summary>HOLD に入っているミノ。空なら null。</summary>
        public TetrominoType? Held { get; private set; }

        /// <summary>いま HOLD できるか。1 つのミノにつき 1 回だけ。</summary>
        public bool CanHold => canHold;

        public GameState(Random random)
        {
            bag = new PieceBag(random);
            SpawnNext();
        }

        /// <summary>これから出るミノの先読み。index 0 が次に出るミノ。</summary>
        public TetrominoType PeekNext(int index)
        {
            return bag.Peek(index);
        }

        /// <summary>落下中のミノを HOLD と入れ替える。HOLD が空なら、次のミノが出てくる。</summary>
        public bool TryHold()
        {
            if (IsGameOver || !canHold)
            {
                return false;
            }

            var incoming = Held ?? bag.Next();
            Held = Current.Type;
            Spawn(incoming);
            canHold = false;
            return true;
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
            Score += ScoreCalculator.SoftDropPointsPerCell;
            return true;
        }

        /// <summary>いちばん下まで一気に落として、その場に固定する(ハードドロップ)。</summary>
        public void HardDrop()
        {
            if (IsGameOver)
            {
                return;
            }

            var landing = GetGhost();
            Score += (Current.Y - landing.Y) * ScoreCalculator.HardDropPointsPerCell;
            Current = landing;
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

            // 得点は、行を消す前のレベルで計算する
            int cleared = Board.ClearFullLines();
            Score += ScoreCalculator.LineClear(cleared, Level);
            Lines += cleared;

            if (!allInside)
            {
                // 盤面の外にはみ出したまま固定された
                IsGameOver = true;
                return;
            }

            SpawnNext();
        }

        /// <summary>
        /// レベルごとの落下間隔。1 秒から始まり、レベルが上がるほど指数的に短くなる。
        /// </summary>
        public static float GetFallInterval(int level)
        {
            double seconds = Math.Pow(0.8 - (level - 1) * 0.007, level - 1);
            return Math.Max(MinFallInterval, (float)seconds);
        }

        private void SpawnNext()
        {
            Spawn(bag.Next());
            canHold = true;
        }

        private void Spawn(TetrominoType type)
        {
            Current = Piece.Spawn(type);
            fallTimer = 0f;

            if (!Board.CanPlace(Current))
            {
                // 出現位置がすでに埋まっている
                IsGameOver = true;
            }
        }
    }
}
