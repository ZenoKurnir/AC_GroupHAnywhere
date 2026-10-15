# AC_GroupHAnywhere

[English](README.md)

**アイコミ** のすべてのマップで 3P と 5P ができるようにする BepInEx 6 (IL2CPP) プラグインです。

## 問題

本編では、カフェや図書室のような狭いマップでも 3P の誘いが出て二人目の子が参加しますが、H シーンに 3P の体位が表示されません。5P も同様です。

原因は、ゲームがその体位に合った種類の H ポイントがマップにある場合にだけ体位を表示することです。3P の体位は 3P 専用のポイント、5P の体位は 5P 専用のポイントしか使えず、カフェと図書室にはどちらもありません。

## このプラグインの動作

3P または 5P の体位を受け付けるポイントがマップにない場合、その体位はマップの別のポイントを次の順で借ります。

1. 5P の場合は 3P のポイント
2. 床のポイント
3. 立ちのポイント
4. マップにあるその他のポイント

3P・5P 専用のポイントがあるマップでは何も変わりません。H シーンが終わるとすべて元に戻ります。

## 必要なもの

- アイコミ
- [BepInEx 6 (IL2CPP, bleeding edge)](https://builds.bepinex.dev/projects/bepinex_be)（一般的な Mod パックに同梱）

ほかのプラグインは不要です。単体で動作し、ほかの H 系プラグインとも併用できます。

## インストール

1. [Releases](../../releases) ページから `AC_GroupHAnywhere-v1.0.0.zip` をダウンロードします。
2. ゲームフォルダ（`Aicomi.exe` があるフォルダ）に展開し、`BepInEx/plugins/AC_GroupHAnywhere.dll` に配置されるようにします。
3. ゲームを起動します。

## 設定

`BepInEx/config/AC_GroupHAnywhere.cfg` を編集するか、ConfigurationManager プラグイン（F1）を使います。

| 設定 | 既定値 | 内容 |
| --- | --- | --- |
| Enabled | オン | 3P・5P のポイントがないマップで、3P・5P の体位がほかのポイントを使えるようにします |
| Detailed Log | オフ | マップの H ポイントと、ポイントを借りた体位を BepInEx のログに書き出します（不具合報告用） |

## 注意

- 借りたポイントは 2〜3 人用の大きさなので、狭い場所では 5P が近くの家具と重なることがあります。
- 不具合を報告するときは **Detailed Log** をオンにして再現し、`BepInEx/LogOutput.log` を添付してください。

## ビルド

.NET 6 SDK 以降と、BepInEx を一度起動したゲームフォルダが必要です。

```
cd src
dotnet build -c Release -p:GameDir="C:\path\to\Aicomi\BepInEx"
```

生成されたアセンブリが `BepInEx\interop` ではなく `BepInEx\interop-Aicomi\interop` にある場合は、`-p:InteropDir="C:\path\to\Aicomi\BepInEx\interop-Aicomi\interop"` を追加してください。

DLL は `src/bin/Release/net6.0/AC_GroupHAnywhere.dll` に出力されます。

## ライセンス

MIT。[LICENSE](LICENSE) を参照してください。
