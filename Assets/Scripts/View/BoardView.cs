using FallingBlocks.Logic;
using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>盤面の全マスを SpriteRenderer で描く。マスの GameObject は最初に一度だけ作って使い回す。</summary>
    public sealed class BoardView : MonoBehaviour
    {
        private const float CellScale = 0.94f;

        private SpriteRenderer[,] cellRenderers;

        public void Initialize()
        {
            var sprite = BlockSprite.Get();

            // 盤面の背景(少し大きい暗い四角形)
            var background = CreateSquare("Background", sprite, transform);
            background.transform.localPosition = new Vector3((Board.Width - 1) / 2f, (Board.Height - 1) / 2f, 0f);
            background.transform.localScale = new Vector3(Board.Width + 0.4f, Board.Height + 0.4f, 1f);
            background.color = new Color(0.03f, 0.03f, 0.05f);
            background.sortingOrder = -1;

            cellRenderers = new SpriteRenderer[Board.Width, Board.Height];
            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    var cell = CreateSquare($"Cell({x},{y})", sprite, transform);
                    cell.transform.localPosition = new Vector3(x, y, 0f);
                    cell.transform.localScale = new Vector3(CellScale, CellScale, 1f);
                    cellRenderers[x, y] = cell;
                }
            }
        }

        public void Render(Board board)
        {
            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    cellRenderers[x, y].color = BlockColors.Of(board.Get(x, y));
                }
            }
        }

        private static SpriteRenderer CreateSquare(string objectName, Sprite sprite, Transform parent)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(parent, false);
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            return spriteRenderer;
        }
    }
}
