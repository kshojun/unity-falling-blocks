using FallingBlocks.Logic;
using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>ゲーム全体を動かす MonoBehaviour。シーンの空の GameObject にアタッチして使う。</summary>
    public sealed class GameController : MonoBehaviour
    {
        private Board board;
        private BoardView boardView;

        // 動作確認用: 1 秒ごとにミノを回し、一周したら次の種類に変える
        private Piece demoPiece;
        private float demoTimer;

        private void Start()
        {
            board = new Board();

            var boardObject = new GameObject("BoardView");
            boardView = boardObject.AddComponent<BoardView>();
            boardView.Initialize();

            demoPiece = Piece.Spawn(TetrominoType.I).Moved(0, -8);
            SetupCamera();
        }

        private void Update()
        {
            demoTimer += Time.deltaTime;
            if (demoTimer >= 1f)
            {
                demoTimer = 0f;
                UpdateDemo();
            }

            // デモ用: 盤面を空にして、ミノの 4 マスだけを書き込む
            board.Clear();
            for (int i = 0; i < TetrominoShapes.CellCount; i++)
            {
                var cell = demoPiece.GetCell(i);
                board.Set(cell.X, cell.Y, (int)demoPiece.Type + 1);
            }

            boardView.Render(board);
        }

        private void UpdateDemo()
        {
            // 前フレームで書き込んだミノ自身に当たらないよう、判定の前に盤面を空にする
            board.Clear();
            if (!RotationRules.TryRotate(board, demoPiece, 1, out var rotated))
            {
                return;
            }

            demoPiece = rotated;
            if (demoPiece.Rotation == 0)
            {
                var next = (TetrominoType)(((int)demoPiece.Type + 1) % 7);
                demoPiece = Piece.Spawn(next).Moved(0, -8);
            }
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
