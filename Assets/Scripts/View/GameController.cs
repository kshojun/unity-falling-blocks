using FallingBlocks.Logic;
using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>ゲーム全体を動かす MonoBehaviour。シーンの空の GameObject にアタッチして使う。</summary>
    public sealed class GameController : MonoBehaviour
    {
        private GameState state;
        private BoardView boardView;
        private readonly InputHandler input = new InputHandler();

        private void Start()
        {
            var boardObject = new GameObject("BoardView");
            boardView = boardObject.AddComponent<BoardView>();
            boardView.Initialize();

            state = new GameState(new System.Random());
            SetupCamera();
        }

        private void Update()
        {
            input.Update(state, Time.deltaTime);
            if (input.RestartRequested)
            {
                state = new GameState(new System.Random());
            }

            state.Tick(Time.deltaTime);
            boardView.Render(state.Board, state.Current, state.GetGhost());
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
