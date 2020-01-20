::切换到当前bat目录
cd /d %~dp0

::源目录
set sPath=%cd%\..\Assets\StreamingAssets_ios\

::排除文件
set xfTag=StreamingAssets* *.meta *.manifest shader*

::获取目的路径
set xmlPath=%cd%\config.xml

for /f "tokens=*" %%i in ('findstr "<resIosPath>.*</resIosPath>" %xmlPath%')do set "s=%%i"
set "s=%s:"=“”%"
for /f "delims=<" %%j in ("%s:*<resIosPath>=%")do set "cid=%%j"
set "cid=%cid:“”="%"
echo 所取值为: %cid%
set tPath=%cid%

::更新svn
svn up %tPath%
svn revert %tPath%

robocopy %sPath% %tPath% /mt:120 /e /xf %xfTag% /nfl /ndl /tee
echo 源路径%sPath%
echo 目标路径%tPath%

::svn ci %tPath% -m "【脚本】自动提交资源目录"
pause
