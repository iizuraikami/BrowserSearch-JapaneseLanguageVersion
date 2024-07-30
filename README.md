# BrowserSearch
これはPowerToys Run用のプラグインです。 デフォルトブラウザの履歴を読み込んで、その項目を検索したり、そのURLを開いたりすることができます。

<p align="center">
    <img src="./Screenshots/1.png" width="500"/>
</p>

## 対応ブラウザ
* Arc
* Brave
* Firefox
* Google Chrome
* Microsoft Edge (Chromium バージョン)
* Thorium
* Vivaldi Browser
* Wavebox

ChromiumやFirefoxをベースとした他のブラウザのサポートも簡単に追加できます。あなたの既定ブラウザが対応していない場合は、フォーク元のリポジトリのissueを開いてください。

## インストール方法
* PowerToysを終了させる
* 最新版のダウンロード [最新版](https://github.com/iizuraikami/BrowserSearch-JapaneseLanguageVersion/releases)
* zipファイルを展開する
* 解凍したフォルダを移動する `BrowserSearch` を `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\`
* PowerToysを起動する

## ビルド方法 
* このリポジトリをクローンする
* `BrowserSearch`フォルダーの中に、`libs`というフォルダを作成する
* 以下のファイルを `%ProgramFiles%PowerToys` から `libs` にコピーする。
    * Wox.Plugin.dll
    * Wox.Infrastructure.dll
    * Microsoft.Data.Sqlite.dll
    * PowerToys.Settings.UI.Lib.dll
* Visual Studioでプロジェクトを開き、リリースモードでビルドする。
* 出力されたフォルダー`net8.0-windows`を`%LOCALAPPDATA%%Microsoft¥PowerToys¥PowerToys RunPlugins`にコピーする 
* (オプション)コピーしたフォルダーをBrowserSearchにリネームする
