# Trac Lightning on Windows Azure のプロジェクト
Windows Azure 上で Trac Lightning を動作させるためのプロジェクトです。現在、以下の機能が利用可能です。

* Trac のチケット管理システム
* SVN のバージョン管理システム（ http プロトコルのみ ）

Jenkins は現在利用できません。今後対応する予定です。


## 利用手順
以下を対応する必要があります。
* vhd ファイルを ストレージサービス上に配置
http://www.atmarkit.co.jp/fwin2k/win2ktips/1267vdisk/vdisk.html の記事を参考に *.vhd ファイルを作成し、 https://[YOUR_STORAGE_ACCOUNT_NAME].blob.core.windows.net/trac/disk.vhd に配置する

* TracLightning.exze をストレージサービス上に配置
https://[YOUR_STORAGE_ACCOUNT_NAME].blob.core.windows.net/trac/TracLightning.exe に Trac Lightning のバイナリを配置

* *.cmd ファイルの修正
```setuptrac.cmd の修正（ download.vbs スクリプトの引数URLに Trac Lightning を配置 ）
@echo off

@rem Trac Lightning がインストールされていない場合はインストールし、再起動される
if exist Z:\TracLight\start.bat goto runtrac
"%~dp0Assets\download.vbs" https://[YOUR_STORAGE_ACCOUNT_NAME].blob.core.windows.net/trac/TracLightning.exe
"%~dp0TracLightning.exe" /VERYSILENT /LOG="log.txt" /DIR="Z:\TracLight"
goto end
```

* *.cscfg ファイルの修正
自身のストレージサービスの情報を追記


## 注意点
* Windows Azure ドライブを利用しているため、一インスタンスにのみ対応しています
* ブロブ内に TRAC_LIGHT_HOME が存在するため、ポータビリティが容易です
* バックアップの取得は、ブロブストレージのスナップショット機能を利用可能です
