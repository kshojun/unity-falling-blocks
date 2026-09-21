namespace FallingBlocks.Logic
{
    /// <summary>
    /// 次に出すミノの供給元。本番は乱数で決める PieceBag を使い、
    /// テストでは決まった順番で返す偽物を差し込めるように、インターフェースにしておく。
    /// </summary>
    public interface IPieceSource
    {
        /// <summary>次のミノを 1 つ取り出す。</summary>
        TetrominoType Next();

        /// <summary>取り出さずに、これから出るミノを先読みする。index 0 が次に出るミノ。</summary>
        TetrominoType Peek(int index);
    }
}
