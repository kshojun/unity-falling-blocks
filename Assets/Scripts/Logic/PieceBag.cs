using System;
using System.Collections.Generic;

namespace FallingBlocks.Logic
{
    /// <summary>
    /// 7-bag ランダマイザ。7 種類のミノを 1 袋にしてシャッフルし、順番に配る。
    /// 袋が空になったらまた新しい袋を作るので、同じミノが極端に続かない。
    /// </summary>
    public sealed class PieceBag
    {
        private const int TypeCount = 7;

        private readonly Random random;
        private readonly List<TetrominoType> queue = new List<TetrominoType>();

        public PieceBag(Random random)
        {
            this.random = random;
        }

        /// <summary>次のミノを 1 つ取り出す。</summary>
        public TetrominoType Next()
        {
            Fill(1);
            var type = queue[0];
            queue.RemoveAt(0);
            return type;
        }

        /// <summary>取り出さずに、これから出るミノを先読みする。index 0 が次に出るミノ。</summary>
        public TetrominoType Peek(int index)
        {
            Fill(index + 1);
            return queue[index];
        }

        private void Fill(int minCount)
        {
            while (queue.Count < minCount)
            {
                AddShuffledBag();
            }
        }

        private void AddShuffledBag()
        {
            var bag = new TetrominoType[TypeCount];
            for (int i = 0; i < TypeCount; i++)
            {
                bag[i] = (TetrominoType)i;
            }

            // フィッシャー–イェーツのシャッフル
            for (int i = TypeCount - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (bag[i], bag[j]) = (bag[j], bag[i]);
            }

            queue.AddRange(bag);
        }
    }
}
