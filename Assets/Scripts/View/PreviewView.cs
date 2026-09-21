using FallingBlocks.Logic;
using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>NEXT や HOLD の欄に、ミノを 1 つ小さく表示する。</summary>
    public sealed class PreviewView : MonoBehaviour
    {
        private const float Scale = 0.7f;
        private const float CellScale = 0.94f;

        private SpriteRenderer[] blocks;

        public void Initialize()
        {
            transform.localScale = new Vector3(Scale, Scale, 1f);

            blocks = new SpriteRenderer[TetrominoShapes.CellCount];
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = BlockSprite.CreateRenderer($"Block{i}", transform);
                blocks[i].transform.localScale = new Vector3(CellScale, CellScale, 1f);
                blocks[i].sortingOrder = 1;
            }
        }

        /// <summary>type のミノを欄の中央に表示する。null なら何も表示しない。brightness で暗くできる。</summary>
        public void Show(TetrominoType? type, float brightness = 1f)
        {
            if (!type.HasValue)
            {
                foreach (var block in blocks)
                {
                    block.enabled = false;
                }

                return;
            }

            var cells = TetrominoShapes.Get(type.Value, 0);

            // 4 マスを囲む四角形の中心が、欄の中心に来るようにずらす
            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
            foreach (var cell in cells)
            {
                minX = Mathf.Min(minX, cell.X);
                maxX = Mathf.Max(maxX, cell.X);
                minY = Mathf.Min(minY, cell.Y);
                maxY = Mathf.Max(maxY, cell.Y);
            }

            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;

            var baseColor = BlockColors.Of((int)type.Value + 1);
            var color = new Color(baseColor.r * brightness, baseColor.g * brightness, baseColor.b * brightness, 1f);

            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].enabled = true;
                blocks[i].transform.localPosition = new Vector3(cells[i].X - centerX, cells[i].Y - centerY, 0f);
                blocks[i].color = color;
            }
        }
    }
}
