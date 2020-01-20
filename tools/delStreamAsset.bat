::切换到当前bat目录
cd /d %~dp0

::目的目录
set tPath=%cd%\..\Assets\StreamingAssets\

::清空streamingAsset资源
del /f/s/q %tPath%
rmdir /s/q %tPath%
md %tPath%

echo !!!!!!!!清空资源目录完成!!!!!!!!!!!!!!!!!

