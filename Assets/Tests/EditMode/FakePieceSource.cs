using FallingBlocks.Logic;

namespace FallingBlocks.Tests
{
    /// <summary>決まった順番でミノを返す偽の供給元。並べ終わったら最初に戻る。</summary>
    public sealed class FakePieceSource : IPieceSource
    {
        private readonly TetrominoType[] sequence;
        private int position;

        public FakePieceSource(params TetrominoType[] sequence)
        {
            this.sequence = sequence;
        }

        public TetrominoType Next()
        {
            var type = Peek(0);
            position++;
            return type;
        }

        public TetrominoType Peek(int index)
        {
            return sequence[(position + index) % sequence.Length];
        }
    }
}
