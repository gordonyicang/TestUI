::�л�����ǰbatĿ¼
cd /d %~dp0

::ԴĿ¼
set sPath=%cd%\..\Assets\StreamingAssets_ios\

::�ų��ļ�
set xfTag=StreamingAssets* *.meta *.manifest shader*

::��ȡĿ��·��
set xmlPath=%cd%\config.xml

for /f "tokens=*" %%i in ('findstr "<resIosPath>.*</resIosPath>" %xmlPath%')do set "s=%%i"
set "s=%s:"=����%"
for /f "delims=<" %%j in ("%s:*<resIosPath>=%")do set "cid=%%j"
set "cid=%cid:����="%"
echo ��ȡֵΪ: %cid%
set tPath=%cid%

::����svn
svn up %tPath%
svn revert %tPath%

robocopy %sPath% %tPath% /mt:120 /e /xf %xfTag% /nfl /ndl /tee
echo Դ·��%sPath%
echo Ŀ��·��%tPath%

::svn ci %tPath% -m "���ű����Զ��ύ��ԴĿ¼"
pause
