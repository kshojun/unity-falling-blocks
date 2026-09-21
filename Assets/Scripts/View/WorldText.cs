using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>
    /// ワールド座標に置く文字(TextMesh)を作る。フォントアセットの取り込みが不要で、
    /// 盤面やパネルと同じ座標系で配置できる。
    /// </summary>
    public static class WorldText
    {
        private static Font font;

        public static TextMesh Create(string objectName, Transform parent, string text, Vector3 localPosition,
            TextAnchor anchor, Color color)
        {
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            var go = new GameObject(objectName);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;

            var mesh = go.AddComponent<TextMesh>();
            mesh.font = font;
            mesh.text = text;
            mesh.anchor = anchor;
            mesh.color = color;

            // 文字を大きめに作って縮小すると、拡大表示してもぼやけない
            mesh.fontSize = 64;
            mesh.characterSize = 0.1f;

            go.GetComponent<MeshRenderer>().sharedMaterial = font.material;
            return mesh;
        }
    }
}
