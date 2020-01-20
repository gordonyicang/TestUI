::切换到当前bat目录
cd /d %~dp0

::获取目的路径
set xmlPath=%cd%\config.xml

for /f "tokens=*" %%i in ('findstr "<resPath>.*</resPath>" %xmlPath%')do set "s=%%i"
set "s=%s:"=“”%"
for /f "delims=<" %%j in ("%s:*<resPath>=%")do set "cid=%%j"
set "cid=%cid:“”="%"
echo 所取值为: %cid%
set tPath=%cid%

cd %tPath%\..\..\tools
call clearResStreamingAsset.bat