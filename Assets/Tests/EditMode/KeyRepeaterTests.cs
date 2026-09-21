using FallingBlocks.Logic;
using NUnit.Framework;

namespace FallingBlocks.Tests
{
    public class KeyRepeaterTests
    {
        [Test]
        public void FiresOnPressThenWaitsForTheDelayThenRepeats()
        {
            var repeater = new KeyRepeater(delay: 0.2f, interval: 0.05f);

            Assert.AreEqual(1, repeater.Poll(true, 0.016f));   // 押した瞬間
            Assert.AreEqual(0, repeater.Poll(true, 0.10f));    // 0.10 秒: まだ待ち
            Assert.AreEqual(0, repeater.Poll(true, 0.09f));    // 0.19 秒: まだ待ち
            Assert.AreEqual(1, repeater.Poll(true, 0.02f));    // 0.21 秒: 0.2 を超えた
            Assert.AreEqual(0, repeater.Poll(true, 0.02f));    // 0.23 秒
            Assert.AreEqual(1, repeater.Poll(true, 0.03f));    // 0.26 秒: 0.25 を超えた
        }

        [Test]
        public void ReleasingTheKeyResetsTheRepeat()
        {
            var repeater = new KeyRepeater(delay: 0.2f, interval: 0.05f);

            Assert.AreEqual(1, repeater.Poll(true, 0.016f));
            Assert.AreEqual(0, repeater.Poll(false, 0.016f));
            Assert.AreEqual(1, repeater.Poll(true, 0.016f));   // 押し直したので、また最初の 1 回
        }

        [Test]
        public void ALongFrameFiresSeveralTimesAtOnce()
        {
            var repeater = new KeyRepeater(delay: 0.2f, interval: 0.05f);
            repeater.Poll(true, 0.016f);

            // 0.2 / 0.25 / 0.30 / 0.35 の 4 回分(境界ぴったりは誤差で揺れるので避ける)
            Assert.AreEqual(4, repeater.Poll(true, 0.36f));
        }
    }
}
