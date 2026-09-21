using System;
using System.Linq;
using FallingBlocks.Logic;
using NUnit.Framework;

namespace FallingBlocks.Tests
{
    public class TetrominoShapesTests
    {
        private static readonly TetrominoType[] AllTypes = (TetrominoType[])Enum.GetValues(typeof(TetrominoType));

        [Test]
        public void EveryShapeHasFourDistinctCellsInsideItsBox([ValueSource(nameof(AllTypes))] TetrominoType type)
        {
            int box = TetrominoShapes.BoxSize(type);

            for (int rotation = 0; rotation < 4; rotation++)
            {
                var cells = TetrominoShapes.Get(type, rotation);

                Assert.AreEqual(4, cells.Select(c => (c.X, c.Y)).Distinct().Count(), $"{type} rotation {rotation}");
                Assert.IsTrue(cells.All(c => c.X >= 0 && c.X < box && c.Y >= 0 && c.Y < box), $"{type} rotation {rotation}");
            }
        }

        [Test]
        public void FourRotationsReturnToTheOriginalShape([ValueSource(nameof(AllTypes))] TetrominoType type)
        {
            var original = TetrominoShapes.Get(type, 0).Select(c => (c.X, c.Y)).OrderBy(c => c).ToArray();
            var again = TetrominoShapes.Get(type, 4).Select(c => (c.X, c.Y)).OrderBy(c => c).ToArray();

            Assert.AreEqual(original, again);
        }

        [Test]
        public void VerticalIIsInTheThirdColumnOfItsBox()
        {
            var cells = TetrominoShapes.Get(TetrominoType.I, 1);

            Assert.IsTrue(cells.All(c => c.X == 2));
            Assert.AreEqual(new[] { 0, 1, 2, 3 }, cells.Select(c => c.Y).OrderBy(y => y).ToArray());
        }

        [Test]
        public void RotatingTClockwisePointsItsNubToTheRight()
        {
            // 出現時の T は、出っぱりが上を向いている:  .X. / XXX
            // 時計回りに 90 度回すと、出っぱりが右を向く:  .X. / .XX / .X.
            var cells = TetrominoShapes.Get(TetrominoType.T, 1).Select(c => (c.X, c.Y)).OrderBy(c => c).ToArray();

            var expected = new[] { (1, 0), (1, 1), (1, 2), (2, 1) };
            Assert.AreEqual(expected, cells);
        }

        [Test]
        public void OLooksTheSameInEveryRotation()
        {
            var original = TetrominoShapes.Get(TetrominoType.O, 0).Select(c => (c.X, c.Y)).OrderBy(c => c).ToArray();

            for (int rotation = 1; rotation < 4; rotation++)
            {
                var rotated = TetrominoShapes.Get(TetrominoType.O, rotation).Select(c => (c.X, c.Y)).OrderBy(c => c).ToArray();
                Assert.AreEqual(original, rotated);
            }
        }

        [Test]
        public void Spawn_FitsOnAnEmptyBoardAndTouchesTheTopRow([ValueSource(nameof(AllTypes))] TetrominoType type)
        {
            var piece = Piece.Spawn(type);

            Assert.IsTrue(new Board().CanPlace(piece));
            Assert.AreEqual(Board.Height - 1, Enumerable.Range(0, 4).Max(i => piece.GetCell(i).Y));
        }
    }
}
