# Trac Lightning on Windows Azure のプロジェクト
Windows Azure 上で Trac Lightning を動作させるためのプロジェクトです。現在、以下の機能が利用可能です。

* Trac のチケット管理システム
* SVN のバージョン管理システム（ http プロトコルのみ ）

Jenkins は現在利用できません。今後対応する予定です。


## 利用手順
本プロジェクトを pull 等で取得した後、実際の利用には以下の対応を実施する必要があります。

### vhd ファイルを ストレージサービス上に配置する
http://www.atmarkit.co.jp/fwin2k/win2ktips/1267vdisk/vdisk.html の記事を参照して *.vhd ファイルを作成し、 https://[YOUR_STORAGE_ACCOUNT_NAME].blob.core.windows.net/trac/disk.vhd に配置します

### TracLightning.exze をストレージサービス上に配置する
http://sourceforge.jp/projects/traclight/wiki/FrontPage から Trac Lightning の *.exe バイナリを取得し、ファイル名を変更して
https://[YOUR_STORAGE_ACCOUNT_NAME].blob.core.windows.net/trac/TracLightning.exe
に Trac Lightning のバイナリを配置します。

###  *.cmd ファイルを修正する
Wroker ロールプロジェクト内の setuptrac.cmd スクリプトを修正（ download.vbs スクリプトの引数URLに Trac Lightning を配置 ）
```
@echo off
@rem Trac Lightning がインストールされていない場合は同ソフトウェアをインストールし、OSを再起動する
if exist Z:\TracLight\start.bat goto runtrac
"%~dp0Assets\download.vbs" https://[YOUR_STORAGE_ACCOUNT_NAME].blob.core.windows.net/trac/TracLightning.exe
"%~dp0TracLightning.exe" /VERYSILENT /LOG="log.txt" /DIR="Z:\TracLight"
goto end
```

### *.cscfg ファイルを修正する
自身のストレージサービスの情報を追記する必要があります。Windows Azure ドライブの保存先なので、設定は必須です。

## 注意点
* Windows Azure ドライブを利用しているため、一インスタンスにのみ対応しています
* ブロブ内に TRAC_LIGHT_HOME が存在するため、ポータビリティが容易です
* バックアップの取得は、ブロブストレージのスナップショット機能を利用可能です