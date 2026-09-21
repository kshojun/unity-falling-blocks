using FallingBlocks.Logic;
using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>ゲーム全体を動かす MonoBehaviour。シーンの空の GameObject にアタッチして使う。</summary>
    public sealed class GameController : MonoBehaviour
    {
        private static readonly Color LabelColor = new Color(0.85f, 0.85f, 0.92f);

        private GameState state;
        private BoardView boardView;
        private HudView hudView;
        private PreviewView holdView;
        private PreviewView[] nextViews;
        private readonly InputHandler input = new InputHandler();

        private void Start()
        {
            var boardObject = new GameObject("BoardView");
            boardView = boardObject.AddComponent<BoardView>();
            boardView.Initialize();

            var hudObject = new GameObject("HudView");
            hudView = hudObject.AddComponent<HudView>();
            hudView.Initialize();

            // 左に HOLD、右に NEXT を並べる
            WorldText.Create("HoldLabel", transform, "HOLD", new Vector3(-4f, 18.6f, 0f), TextAnchor.MiddleCenter, LabelColor);
            holdView = CreatePreview("HoldPreview", new Vector3(-4f, 16f, 0f));

            WorldText.Create("NextLabel", transform, "NEXT", new Vector3(14f, 18.6f, 0f), TextAnchor.MiddleCenter, LabelColor);
            nextViews = new PreviewView[GameState.NextCount];
            for (int i = 0; i < nextViews.Length; i++)
            {
                nextViews[i] = CreatePreview($"NextPreview{i}", new Vector3(14f, 16f - i * 3f, 0f));
            }

            state = new GameState(new System.Random());
        }

        private void Update()
        {
            input.Update(state, Time.deltaTime);
            if (input.RestartRequested)
            {
                state = new GameState(new System.Random());
            }

            state.Tick(Time.deltaTime);

            CameraFitter.Fit(Camera.main);
            boardView.Render(state.Board, state.Current, state.GetGhost());
            hudView.Show(state);

            // HOLD できない間は暗く表示する
            holdView.Show(state.Held, state.CanHold ? 1f : 0.35f);
            for (int i = 0; i < nextViews.Length; i++)
            {
                nextViews[i].Show(state.PeekNext(i));
            }
        }

        private PreviewView CreatePreview(string objectName, Vector3 localPosition)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localPosition;

            var preview = go.AddComponent<PreviewView>();
            preview.Initialize();
            return preview;
        }
    }
}
