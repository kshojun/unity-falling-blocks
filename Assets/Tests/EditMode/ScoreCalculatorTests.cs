using FallingBlocks.Logic;
using NUnit.Framework;

namespace FallingBlocks.Tests
{
    public class ScoreCalculatorTests
    {
        [TestCase(0, 1, 0)]
        [TestCase(1, 1, 100)]
        [TestCase(2, 1, 300)]
        [TestCase(3, 1, 500)]
        [TestCase(4, 1, 800)]
        [TestCase(4, 3, 2400)]
        [TestCase(1, 5, 500)]
        public void LineClear_PointsGrowWithLinesAndLevel(int lines, int level, int expected)
        {
            Assert.AreEqual(expected, ScoreCalculator.LineClear(lines, level));
        }
    }
}
