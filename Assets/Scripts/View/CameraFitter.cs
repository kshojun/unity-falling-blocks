using UnityEngine;

namespace FallingBlocks.View
{
    /// <summary>盤面と左右のパネルがすべて画面に収まるよう、カメラの位置と大きさを合わせる。</summary>
    public static class CameraFitter
    {
        // ワールド座標での表示範囲(盤面は x = -0.5〜9.5、y = -0.5〜19.5)
        private const float MinX = -7.5f;
        private const float MaxX = 17.5f;
        private const float MinY = -1f;
        private const float MaxY = 21f;

        public static void Fit(Camera cam)
        {
            float width = MaxX - MinX;
            float height = MaxY - MinY;

            // 縦に収める大きさと、横に収める大きさのうち、大きい方を選ぶ
            float sizeForHeight = height / 2f;
            float sizeForWidth = width / 2f / cam.aspect;

            cam.orthographic = true;
            cam.orthographicSize = Mathf.Max(sizeForHeight, sizeForWidth);
            cam.transform.position = new Vector3((MinX + MaxX) / 2f, (MinY + MaxY) / 2f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.07f, 0.07f, 0.10f);
        }
    }
}
