::�л�����ǰbatĿ¼
set name=%1
cd /d %~dp0

::ԴĿ¼
set sPath=%cd%\..\Assets\StreamingAssets\

::�ų��ļ�
set xfTag=StreamingAssets* *.meta *.manifest shader*

::��ȡĿ��·��
set xmlPath=%cd%\config.xml

for /f "tokens=*" %%i in ('findstr "<resPath>.*</resPath>" %xmlPath%')do set "s=%%i"
set "s=%s:"=����%"
for /f "delims=<" %%j in ("%s:*<resPath>=%")do set "cid=%%j"
set "cid=%cid:����="%"
echo ��ȡֵΪ: %cid%
set tPath=%cid%

robocopy %sPath% %tPath% /mt:120 /e *$%name%*.u outpic$*.u /s /nfl /ndl /tee
echo Դ·��%sPath%
echo Ŀ��·��%tPath%
pause
