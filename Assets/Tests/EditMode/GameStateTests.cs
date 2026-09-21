using System.Linq;
using FallingBlocks.Logic;
using NUnit.Framework;

namespace FallingBlocks.Tests
{
    public class GameStateTests
    {
        private static GameState NewGame(params TetrominoType[] sequence)
        {
            return new GameState(new FakePieceSource(sequence));
        }

        private static int FilledCellCount(Board board)
        {
            int count = 0;
            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    if (board.Get(x, y) != 0)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>下 4 段を「いちばん左の列だけ空き」にして、縦にした I で 4 行同時に消す。I が出ている状態で呼ぶ。</summary>
        private static void ClearFourLinesWithI(GameState state)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 1; x < Board.Width; x++)
                {
                    state.Board.Set(x, y, 1);
                }
            }

            state.TryRotate(1);
            while (state.TryMove(-1))
            {
            }

            state.HardDrop();
        }

        [Test]
        public void Tick_DropsThePieceOneCellPerInterval()
        {
            var state = NewGame(TetrominoType.T);
            int startY = state.Current.Y;

            state.Tick(0.5f);
            Assert.AreEqual(startY, state.Current.Y);

            state.Tick(0.5f);
            Assert.AreEqual(startY - 1, state.Current.Y);

            state.Tick(2f);
            Assert.AreEqual(startY - 3, state.Current.Y);
        }

        [Test]
        public void Tick_LocksThePieceOnTheFloorAndSpawnsTheNextOne()
        {
            var state = NewGame(TetrominoType.O, TetrominoType.T);

            for (int i = 0; i < 30; i++)
            {
                state.Tick(1f);
            }

            Assert.AreEqual(4, FilledCellCount(state.Board));
            Assert.AreEqual((int)TetrominoType.O + 1, state.Board.Get(4, 0));
            Assert.AreEqual(TetrominoType.T, state.Current.Type);
        }

        [Test]
        public void TryMove_StopsAtTheWalls()
        {
            var state = NewGame(TetrominoType.T);

            while (state.TryMove(-1))
            {
            }

            Assert.AreEqual(0, Enumerable.Range(0, 4).Min(i => state.Current.GetCell(i).X));
            Assert.IsFalse(state.TryMove(-1));

            while (state.TryMove(1))
            {
            }

            Assert.AreEqual(Board.Width - 1, Enumerable.Range(0, 4).Max(i => state.Current.GetCell(i).X));
        }

        [Test]
        public void SoftDrop_ScoresOnePointPerCellButNotAtTheFloor()
        {
            var state = NewGame(TetrominoType.T);

            state.SoftDrop();
            state.SoftDrop();
            state.SoftDrop();
            Assert.AreEqual(3, state.Score);

            while (state.SoftDrop())
            {
            }

            int scoreAtFloor = state.Score;
            Assert.IsFalse(state.SoftDrop());
            Assert.AreEqual(scoreAtFloor, state.Score);
        }

        [Test]
        public void HardDrop_LocksAtTheGhostPositionAndScoresTwoPointsPerCell()
        {
            var state = NewGame(TetrominoType.T, TetrominoType.O);
            var ghost = state.GetGhost();
            int distance = state.Current.Y - ghost.Y;

            state.HardDrop();

            for (int i = 0; i < 4; i++)
            {
                var cell = ghost.GetCell(i);
                Assert.AreEqual((int)TetrominoType.T + 1, state.Board.Get(cell.X, cell.Y));
            }

            Assert.AreEqual(distance * 2, state.Score);
            Assert.AreEqual(TetrominoType.O, state.Current.Type);
        }

        [Test]
        public void Hold_SavesThePieceAndBringsTheNextOne()
        {
            var state = NewGame(TetrominoType.T, TetrominoType.O, TetrominoType.I);

            Assert.IsNull(state.Held);
            Assert.IsTrue(state.TryHold());

            Assert.AreEqual(TetrominoType.T, state.Held);
            Assert.AreEqual(TetrominoType.O, state.Current.Type);
        }

        [Test]
        public void Hold_CanBeUsedOnlyOncePerPieceAndComesBackAfterLocking()
        {
            var state = NewGame(TetrominoType.T, TetrominoType.O, TetrominoType.I);

            state.TryHold();
            Assert.IsFalse(state.CanHold);
            Assert.IsFalse(state.TryHold());
            Assert.AreEqual(TetrominoType.O, state.Current.Type);

            state.HardDrop();
            Assert.IsTrue(state.CanHold);
        }

        [Test]
        public void Hold_SwapsWithTheHeldPiece()
        {
            var state = NewGame(TetrominoType.T, TetrominoType.O, TetrominoType.I);
            state.TryHold();      // HOLD = T、いまは O
            state.HardDrop();     // O を固定して、次の I が出る

            Assert.IsTrue(state.TryHold());

            Assert.AreEqual(TetrominoType.T, state.Current.Type);
            Assert.AreEqual(TetrominoType.I, state.Held);
        }

        [Test]
        public void ClearingFourLinesAddsLinesAndScore()
        {
            var state = NewGame(TetrominoType.I);

            for (int y = 0; y < 4; y++)
            {
                for (int x = 1; x < Board.Width; x++)
                {
                    state.Board.Set(x, y, 1);
                }
            }

            state.TryRotate(1);
            while (state.TryMove(-1))
            {
            }

            int distance = state.Current.Y - state.GetGhost().Y;
            state.HardDrop();

            Assert.AreEqual(4, state.Lines);
            Assert.AreEqual(800 + distance * 2, state.Score);
            Assert.AreEqual(0, FilledCellCount(state.Board));
        }

        [Test]
        public void LevelRisesEveryTenLinesAndTheFallGetsFaster()
        {
            var state = NewGame(TetrominoType.I);
            Assert.AreEqual(1, state.Level);
            float firstInterval = state.FallInterval;

            ClearFourLinesWithI(state);
            ClearFourLinesWithI(state);
            Assert.AreEqual(8, state.Lines);
            Assert.AreEqual(1, state.Level);

            ClearFourLinesWithI(state);
            Assert.AreEqual(12, state.Lines);
            Assert.AreEqual(2, state.Level);
            Assert.Less(state.FallInterval, firstInterval);
        }

        [Test]
        public void FallInterval_StartsAtOneSecondAndNeverGetsLongerOrShorterThanTheLimit()
        {
            Assert.AreEqual(1f, GameState.GetFallInterval(1), 1e-6f);

            for (int level = 1; level < 60; level++)
            {
                float next = GameState.GetFallInterval(level + 1);
                Assert.LessOrEqual(next, GameState.GetFallInterval(level) + 1e-6f, $"レベル {level}");
                Assert.GreaterOrEqual(next, 0.05f);
            }
        }

        [Test]
        public void StackingToTheTopEndsTheGameAndFreezesEverything()
        {
            var state = NewGame(TetrominoType.O);

            for (int i = 0; i < 100 && !state.IsGameOver; i++)
            {
                state.HardDrop();
            }

            Assert.IsTrue(state.IsGameOver);

            int score = state.Score;
            var current = state.Current;
            state.HardDrop();
            state.Tick(100f);

            Assert.IsFalse(state.TryMove(1));
            Assert.IsFalse(state.TryRotate(1));
            Assert.IsFalse(state.SoftDrop());
            Assert.IsFalse(state.TryHold());
            Assert.AreEqual(score, state.Score);
            Assert.AreEqual(current.Y, state.Current.Y);
        }
    }
}
