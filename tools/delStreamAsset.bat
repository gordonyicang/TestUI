::�л�����ǰbatĿ¼
cd /d %~dp0

::Ŀ��Ŀ¼
set tPath=%cd%\..\Assets\StreamingAssets\

::���streamingAsset��Դ
del /f/s/q %tPath%
rmdir /s/q %tPath%
md %tPath%

echo !!!!!!!!�����ԴĿ¼���!!!!!!!!!!!!!!!!!

