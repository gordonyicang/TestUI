@echo off

set py=Psd2UI
set srcPath=PSDTmpOutput/
set tarPath=%cd%\PSDTmpOutput\

echo python %py% %%s -o %srcPath%
echo python %py% PSDfiles -o %srcPath% -s -v

cd /d %~dp0

%py% PSDTmp -o %srcPath% -s -v
echo ---------------------------------------------


::java -jar Json2Bytes.jar -source=%srcPath% -target=%tarPath% -type=0 -c=false

::copy %tarPath%uipsd.data %cd%\..\..\res\GameUI\
pause