::切换到当前bat目录
set name=%1
cd /d %~dp0

::源目录
set sPath=%cd%\..\Assets\StreamingAssets\

::排除文件
set xfTag=StreamingAssets* *.meta *.manifest shader*

::获取目的路径
set xmlPath=%cd%\config.xml

for /f "tokens=*" %%i in ('findstr "<resPath>.*</resPath>" %xmlPath%')do set "s=%%i"
set "s=%s:"=“”%"
for /f "delims=<" %%j in ("%s:*<resPath>=%")do set "cid=%%j"
set "cid=%cid:“”="%"
echo 所取值为: %cid%
set tPath=%cid%

robocopy %sPath% %tPath% /mt:120 /e *$%name%*.u outpic$*.u /s /nfl /ndl /tee
echo 源路径%sPath%
echo 目标路径%tPath%
pause
