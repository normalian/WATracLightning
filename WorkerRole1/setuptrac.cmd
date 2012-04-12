@echo off

@rem Trac Lightning がインストールされていない場合はインストールし、再起動される
if exist Z:\TracLight\start.bat goto runtrac
"%~dp0Assets\download.vbs" https://normalianfeasibility.blob.core.windows.net/trac/TracLightning.exe
"%~dp0TracLightning.exe" /VERYSILENT /LOG="log.txt" /DIR="Z:\TracLight"
goto end

@rem Trac Lightning がインストール済みの場合、Trac Lightningを実行する
:runtrac

@rem start Z:\TracLight\start.bat
call Z:\TracLight\start.bat

:end