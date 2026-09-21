using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>WebGL ビルドを docs/ フォルダに出力する。GitHub Pages が docs/ をそのまま公開できる。</summary>
public static class BuildScript
{
    private const string OutputPath = "docs";

    // メニューの Tools > Build WebGL からも、コマンドラインの -executeMethod BuildScript.BuildWebGL からも呼べる
    [MenuItem("Tools/Build WebGL")]
    public static void BuildWebGL()
    {
        ConfigureForWebGL();

        var options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
            locationPathName = OutputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };

        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;
        Debug.Log($"WebGL build: {summary.result} ({summary.totalSize / 1024 / 1024} MB, {summary.totalTime})");

        if (summary.result != BuildResult.Succeeded && Application.isBatchMode)
        {
            EditorApplication.Exit(1);
        }
    }

    private static void ConfigureForWebGL()
    {
        PlayerSettings.productName = "Falling Blocks";

        // Brotli で圧縮して転送量を減らす。ただし GitHub Pages のように、サーバー側で
        // Content-Encoding ヘッダーを設定できない場所では、そのままだと読み込みに失敗する。
        // そこで Decompression Fallback を有効にして、ブラウザ側(JavaScript)で展開させる
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.decompressionFallback = true;

        // 既定の「ビルド時間優先」だと wasm が約 49 MB になるので、ファイルサイズ優先(LTO)にする
        UnityEditor.WebGL.UserBuildSettings.codeOptimization = UnityEditor.WebGL.WasmCodeOptimization.DiskSizeLTO;

        // 表示領域の大きさ(4:3)
        PlayerSettings.defaultWebScreenWidth = 960;
        PlayerSettings.defaultWebScreenHeight = 720;
    }
}
