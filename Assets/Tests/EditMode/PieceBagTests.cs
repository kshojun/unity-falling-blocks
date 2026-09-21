using System;
using System.Linq;
using FallingBlocks.Logic;
using NUnit.Framework;

namespace FallingBlocks.Tests
{
    public class PieceBagTests
    {
        [Test]
        public void EverySevenPiecesContainEachTypeExactlyOnce()
        {
            for (int seed = 0; seed < 20; seed++)
            {
                var bag = new PieceBag(new Random(seed));

                for (int round = 0; round < 5; round++)
                {
                    var seven = Enumerable.Range(0, 7).Select(_ => bag.Next()).ToArray();
                    Assert.AreEqual(7, seven.Distinct().Count(), $"seed {seed}, round {round}");
                }
            }
        }

        [Test]
        public void SameSeedGivesTheSameSequence()
        {
            var a = new PieceBag(new Random(9));
            var b = new PieceBag(new Random(9));

            for (int i = 0; i < 30; i++)
            {
                Assert.AreEqual(a.Next(), b.Next());
            }
        }

        [Test]
        public void PeekDoesNotConsumeAndMatchesWhatComesNext()
        {
            var bag = new PieceBag(new Random(4));

            var peeked = Enumerable.Range(0, 10).Select(i => bag.Peek(i)).ToArray();
            var peekedAgain = Enumerable.Range(0, 10).Select(i => bag.Peek(i)).ToArray();
            var drawn = Enumerable.Range(0, 10).Select(_ => bag.Next()).ToArray();

            Assert.AreEqual(peeked, peekedAgain);
            Assert.AreEqual(peeked, drawn);
        }

        [Test]
        public void TheSameTypeNeverComesMoreThanTwiceInARow()
        {
            var bag = new PieceBag(new Random(2));
            var sequence = Enumerable.Range(0, 700).Select(_ => bag.Next()).ToArray();

            int run = 1;
            int longestRun = 1;
            for (int i = 1; i < sequence.Length; i++)
            {
                run = sequence[i] == sequence[i - 1] ? run + 1 : 1;
                longestRun = Math.Max(longestRun, run);
            }

            Assert.LessOrEqual(longestRun, 2);
        }
    }
}
