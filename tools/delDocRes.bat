::�л�����ǰbatĿ¼
cd /d %~dp0

::��ȡĿ��·��
set xmlPath=%cd%\config.xml

for /f "tokens=*" %%i in ('findstr "<resPath>.*</resPath>" %xmlPath%')do set "s=%%i"
set "s=%s:"=����%"
for /f "delims=<" %%j in ("%s:*<resPath>=%")do set "cid=%%j"
set "cid=%cid:����="%"
echo ��ȡֵΪ: %cid%
set tPath=%cid%

cd %tPath%\..\..\tools
call clearResStreamingAsset.bat