using FallingBlocks.Logic;
using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>スコア・レベル・ライン数の表示と、ゲームオーバー画面。</summary>
    public sealed class HudView : MonoBehaviour
    {
        private static readonly Color LabelColor = new Color(0.85f, 0.85f, 0.92f);
        private static readonly Color ValueColor = Color.white;

        private const float PanelX = -4f;
        private const float BoardCenterX = (Board.Width - 1) / 2f;
        private const float BoardCenterY = (Board.Height - 1) / 2f;

        // 表示中の値を覚えておき、変わったときだけ文字を作り直す(毎フレーム文字列を作らないため)
        private int shownScore = -1;
        private int shownLevel = -1;
        private int shownLines = -1;

        private TextMesh scoreText;
        private TextMesh levelText;
        private TextMesh linesText;
        private GameObject gameOverGroup;

        public void Initialize()
        {
            scoreText = CreateStat("SCORE", 11.5f);
            levelText = CreateStat("LEVEL", 8.5f);
            linesText = CreateStat("LINES", 5.5f);
            gameOverGroup = CreateGameOverScreen();
        }

        public void Show(GameState state)
        {
            if (state.Score != shownScore)
            {
                shownScore = state.Score;
                scoreText.text = shownScore.ToString();
            }

            if (state.Level != shownLevel)
            {
                shownLevel = state.Level;
                levelText.text = shownLevel.ToString();
            }

            if (state.Lines != shownLines)
            {
                shownLines = state.Lines;
                linesText.text = shownLines.ToString();
            }

            gameOverGroup.SetActive(state.IsGameOver);
        }

        /// <summary>見出しとその下の数値を作り、数値の TextMesh を返す。</summary>
        private TextMesh CreateStat(string label, float y)
        {
            WorldText.Create($"{label}Label", transform, label, new Vector3(PanelX, y, 0f),
                TextAnchor.MiddleCenter, LabelColor);
            return WorldText.Create($"{label}Value", transform, "0", new Vector3(PanelX, y - 1.1f, 0f),
                TextAnchor.MiddleCenter, ValueColor, 1.4f);
        }

        private GameObject CreateGameOverScreen()
        {
            var group = new GameObject("GameOver");
            group.transform.SetParent(transform, false);

            // 盤面全体を暗くする
            var dim = BlockSprite.CreateRenderer("Dim", group.transform);
            dim.transform.localPosition = new Vector3(BoardCenterX, BoardCenterY, 0f);
            dim.transform.localScale = new Vector3(Board.Width + 0.4f, Board.Height + 0.4f, 1f);
            dim.color = new Color(0f, 0f, 0f, 0.75f);
            dim.sortingOrder = 10;

            var title = WorldText.Create("Title", group.transform, "GAME OVER",
                new Vector3(BoardCenterX, BoardCenterY + 1.5f, 0f), TextAnchor.MiddleCenter, Color.white, 1.6f);
            title.GetComponent<MeshRenderer>().sortingOrder = 11;

            var hint = WorldText.Create("Hint", group.transform, "Press R to restart",
                new Vector3(BoardCenterX, BoardCenterY - 1f, 0f), TextAnchor.MiddleCenter, LabelColor, 0.9f);
            hint.GetComponent<MeshRenderer>().sortingOrder = 11;

            group.SetActive(false);
            return group;
        }
    }
}
