# Falling Blocks

Unity で作る落ち物パズル(テトリス風)のサンプルプロジェクトです。
Zenn Book「Unityで作る落ち物パズル(テトリス風) 入門」の教材として、章ごとにタグを打っています。

- Unity: 6000.4.6f1(Universal 2D テンプレート)
- 入力: Input System パッケージ(キーボード)
- 公開: WebGL

## 章とタグ

各章の完成状態は Git タグ `chapter-N` で確認できます。

```sh
git checkout chapter-5
```

| タグ | 内容 |
|---|---|
| chapter-1 | プロジェクト作成と `.gitignore` |
| chapter-2 | 盤面(`Board`)とグリッド描画 |
| chapter-3 | テトリミノの形と回転 |
| chapter-4 | ブロックの描画 |
| chapter-5 | 落下・ロック・ライン消去 |
| chapter-6 | キー入力(移動・回転・ドロップ) |
| chapter-7 | NEXT・HOLD・7-bag |
| chapter-8 | スコア・レベル・ゲームオーバー |
| chapter-9 | EditMode テスト |
| chapter-10 | WebGL ビルドと公開 |

## 操作

| キー | 動作 |
|---|---|
| ← → | 左右移動 |
| ↓ | ソフトドロップ |
| ↑ / X | 右回転 |
| Z | 左回転 |
| Space | ハードドロップ |
| C / Shift | HOLD |
| R | リスタート(ゲームオーバー時) |

## 注意

「テトリス」は商標です。本プロジェクトは学習用の落ち物パズルであり、公式のゲームとは関係ありません。
