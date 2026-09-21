using FallingBlocks.Logic;
using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>盤面の全マスを SpriteRenderer で描く。マスの GameObject は最初に一度だけ作って使い回す。</summary>
    public sealed class BoardView : MonoBehaviour
    {
        private const float CellScale = 0.94f;

        private SpriteRenderer[,] cellRenderers;
        private SpriteRenderer[] pieceRenderers;
        private SpriteRenderer[] ghostRenderers;

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

            // 着地位置の予告(ゴースト)と落下中のミノ用の 4 マスずつ。盤面のマスより手前に描く
            ghostRenderers = CreatePieceRenderers("Ghost", sprite, 1);
            pieceRenderers = CreatePieceRenderers("Piece", sprite, 2);
        }

        /// <summary>積まれたブロック(盤面)と、落下中のミノ、着地位置のゴーストを描く。</summary>
        public void Render(Board board, Piece piece, Piece ghost)
        {
            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    cellRenderers[x, y].color = BlockColors.Of(board.Get(x, y));
                }
            }

            var color = BlockColors.Of((int)piece.Type + 1);
            var ghostColor = new Color(color.r, color.g, color.b, 0.3f);
            RenderPiece(ghostRenderers, ghost, ghostColor);
            RenderPiece(pieceRenderers, piece, color);
        }

        private static void RenderPiece(SpriteRenderer[] renderers, Piece piece, Color color)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                var cell = piece.GetCell(i);
                var block = renderers[i];

                // 盤面より上にはみ出したマスは表示しない
                block.enabled = cell.Y < Board.Height;
                block.transform.localPosition = new Vector3(cell.X, cell.Y, 0f);
                block.color = color;
            }
        }

        private SpriteRenderer[] CreatePieceRenderers(string namePrefix, Sprite sprite, int sortingOrder)
        {
            var renderers = new SpriteRenderer[TetrominoShapes.CellCount];
            for (int i = 0; i < renderers.Length; i++)
            {
                var block = CreateSquare($"{namePrefix}{i}", sprite, transform);
                block.transform.localScale = new Vector3(CellScale, CellScale, 1f);
                block.sortingOrder = sortingOrder;
                renderers[i] = block;
            }

            return renderers;
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
