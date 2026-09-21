using FallingBlocks.Logic;
using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>ゲーム全体を動かす MonoBehaviour。シーンの空の GameObject にアタッチして使う。</summary>
    public sealed class GameController : MonoBehaviour
    {
        private Board board;
        private BoardView boardView;

        private void Start()
        {
            board = new Board();

            // 動作確認用: 最下段と、その上にいくつかブロックを置いてみる
            for (int x = 0; x < Board.Width; x++)
            {
                board.Set(x, 0, x % 7 + 1);
            }
            board.Set(3, 1, 3);
            board.Set(4, 1, 3);
            board.Set(4, 2, 5);

            var boardObject = new GameObject("BoardView");
            boardView = boardObject.AddComponent<BoardView>();
            boardView.Initialize();

            SetupCamera();
        }

        private void Update()
        {
            boardView.Render(board);
        }

        private static void SetupCamera()
        {
            var cam = Camera.main;
            cam.orthographic = true;
            cam.orthographicSize = Board.Height / 2f + 1.5f;
            cam.transform.position = new Vector3((Board.Width - 1) / 2f, (Board.Height - 1) / 2f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.07f, 0.07f, 0.10f);
        }
    }
}
