using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>画像ファイルを用意せず、コードで作る 1×1 ユニットの白い正方形スプライト。</summary>
    public static class BlockSprite
    {
        private static Sprite cached;

        public static Sprite Get()
        {
            if (cached != null)
            {
                return cached;
            }

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            // pixelsPerUnit を 1 にすると、1 ピクセルの画像がちょうど 1 ユニットの大きさになる
            cached = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            return cached;
        }

        /// <summary>白い正方形を表示する SpriteRenderer 付きの GameObject を、parent の子として作る。</summary>
        public static SpriteRenderer CreateRenderer(string objectName, Transform parent)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(parent, false);
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Get();
            return spriteRenderer;
        }
    }
}
