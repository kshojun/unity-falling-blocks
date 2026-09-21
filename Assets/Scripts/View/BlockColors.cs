using UnityEngine;

namespace FallingBlocks.View
{
    public static class BlockColors
    {
        public static readonly Color Empty = new Color(0.13f, 0.13f, 0.18f);

        // 盤面の値 1〜7 に対応する色(0 は空きマスなので使わない)
        private static readonly Color[] Palette =
        {
            Empty,
            new Color(0.25f, 0.85f, 0.95f), // 1: 水色
            new Color(0.98f, 0.85f, 0.25f), // 2: 黄色
            new Color(0.70f, 0.40f, 0.90f), // 3: 紫
            new Color(0.40f, 0.85f, 0.40f), // 4: 緑
            new Color(0.95f, 0.35f, 0.35f), // 5: 赤
            new Color(0.30f, 0.45f, 0.95f), // 6: 青
            new Color(0.98f, 0.60f, 0.25f), // 7: オレンジ
        };

        public static Color Of(int value)
        {
            return value >= 0 && value < Palette.Length ? Palette[value] : Empty;
        }
    }
}
