@echo off

@rem Trac Lightning がインストールされていない場合は同ソフトウェアをインストールし、OSを再起動する
if exist Z:\TracLight\start.bat goto runtrac
"%~dp0Assets\download.vbs" https://[YOUR_STORAGE_ACCOUNT_NAME].blob.core.windows.net/trac/TracLightning.exe
"%~dp0TracLightning.exe" /VERYSILENT /LOG="log.txt" /DIR="Z:\TracLight"
goto end

:runtrac
@rem Trac Lightning がインストール済みの場合、Trac Lightningを実行する
call Z:\TracLight\start.bat

:end