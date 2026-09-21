using System;
using FallingBlocks.Logic;
using NUnit.Framework;

namespace FallingBlocks.Tests
{
    public class RotationRulesTests
    {
        private static readonly TetrominoType[] AllTypes = (TetrominoType[])Enum.GetValues(typeof(TetrominoType));

        [Test]
        public void CanRotateOnTheFloor([ValueSource(nameof(AllTypes))] TetrominoType type)
        {
            var board = new Board();
            var piece = Piece.Spawn(type);
            while (board.CanPlace(piece.Moved(0, -1)))
            {
                piece = piece.Moved(0, -1);
            }

            Assert.IsTrue(RotationRules.TryRotate(board, piece, 1, out var clockwise));
            Assert.IsTrue(board.CanPlace(clockwise));
            Assert.IsTrue(RotationRules.TryRotate(board, piece, -1, out _));
        }

        [Test]
        public void CanRotateAgainstBothWalls([ValueSource(nameof(AllTypes))] TetrominoType type)
        {
            var board = new Board();

            foreach (int direction in new[] { -1, 1 })
            {
                var piece = Piece.Spawn(type).Moved(0, -8);
                while (board.CanPlace(piece.Moved(direction, 0)))
                {
                    piece = piece.Moved(direction, 0);
                }

                Assert.IsTrue(RotationRules.TryRotate(board, piece, 1, out _), $"{type} 壁 {direction} 時計回り");
                Assert.IsTrue(RotationRules.TryRotate(board, piece, -1, out _), $"{type} 壁 {direction} 反時計回り");
            }
        }

        [Test]
        public void RotationFailsAndKeepsThePieceWhenBoxedIn()
        {
            var board = new Board();
            var piece = Piece.Spawn(TetrominoType.T).Moved(0, -10);

            // 盤面をすべて埋めてから、ミノ自身の 4 マスだけを空ける
            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    board.Set(x, y, 1);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                var cell = piece.GetCell(i);
                board.Set(cell.X, cell.Y, 0);
            }

            Assert.IsFalse(RotationRules.TryRotate(board, piece, 1, out var result));
            Assert.AreEqual(piece.Rotation, result.Rotation);
        }
    }
}
