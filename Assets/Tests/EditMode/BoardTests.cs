using FallingBlocks.Logic;
using NUnit.Framework;

namespace FallingBlocks.Tests
{
    public class BoardTests
    {
        private static void FillRow(Board board, int y, int exceptX = -1)
        {
            for (int x = 0; x < Board.Width; x++)
            {
                if (x != exceptX)
                {
                    board.Set(x, y, 1);
                }
            }
        }

        [Test]
        public void IsEmpty_WallsAndFloorAreBlocked()
        {
            var board = new Board();

            Assert.IsFalse(board.IsEmpty(-1, 5));
            Assert.IsFalse(board.IsEmpty(Board.Width, 5));
            Assert.IsFalse(board.IsEmpty(3, -1));
        }

        [Test]
        public void IsEmpty_AboveTheBoardIsOpen()
        {
            var board = new Board();

            Assert.IsTrue(board.IsEmpty(3, Board.Height));
            Assert.IsTrue(board.IsEmpty(3, Board.Height + 5));
        }

        [Test]
        public void CanPlace_IsFalseWhenOverlappingLockedBlocks()
        {
            var board = new Board();
            var piece = Piece.Spawn(TetrominoType.T).Moved(0, -10);
            Assert.IsTrue(board.CanPlace(piece));

            var cell = piece.GetCell(0);
            board.Set(cell.X, cell.Y, 1);

            Assert.IsFalse(board.CanPlace(piece));
        }

        [Test]
        public void Place_ReturnsFalseWhenACellIsAboveTheBoard()
        {
            var board = new Board();

            Assert.IsTrue(board.Place(Piece.Spawn(TetrominoType.T)));
            Assert.IsFalse(board.Place(Piece.Spawn(TetrominoType.T).Moved(0, 2)));
        }

        [Test]
        public void ClearFullLines_RemovesFullRowsAndDropsTheRowsAbove()
        {
            var board = new Board();
            FillRow(board, 0);
            board.Set(2, 1, 5);
            FillRow(board, 2, exceptX: 4);

            int cleared = board.ClearFullLines();

            Assert.AreEqual(1, cleared);
            Assert.AreEqual(5, board.Get(2, 0));   // 1 段目の 5 が最下段に落ちた
            Assert.AreEqual(0, board.Get(4, 1));   // 穴のあった行は 1 段下がっても穴のまま
            Assert.AreEqual(1, board.Get(3, 1));
            Assert.AreEqual(0, board.Get(2, 2));
        }

        [Test]
        public void ClearFullLines_ClearsFourLinesEvenWithARowBetween()
        {
            var board = new Board();
            FillRow(board, 0);
            FillRow(board, 1);
            FillRow(board, 2, exceptX: 0);
            FillRow(board, 3);
            FillRow(board, 4);

            int cleared = board.ClearFullLines();

            Assert.AreEqual(4, cleared);
            Assert.AreEqual(0, board.Get(0, 0));   // 揃っていなかった行だけが残り、最下段へ
            Assert.AreEqual(1, board.Get(1, 0));
            Assert.AreEqual(0, board.Get(1, 1));
        }

        [Test]
        public void ClearFullLines_ReturnsZeroAndKeepsTheBoardWhenNothingIsFull()
        {
            var board = new Board();
            FillRow(board, 3, exceptX: 7);

            Assert.AreEqual(0, board.ClearFullLines());
            Assert.AreEqual(1, board.Get(6, 3));
            Assert.AreEqual(0, board.Get(7, 3));
        }
    }
}
