@echo ImageChannelSpliter @kuyou@163.com 
@echo %1 
@echo %2 
@echo %3 
@set pngPath=%1
@set alphaPath=%2
@set etcToolPath=%3

cd /d %etcToolPath%
convert %pngPath% -channel Alpha -separate %alphaPath%
convert %pngPath% -channel Alpha -threshold 100%% +channel %pngPath%
@echo DOEN
