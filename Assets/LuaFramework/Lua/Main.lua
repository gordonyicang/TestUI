--主入口函数。从这里开始lua逻辑
require "AniSystem/AniSystemDriver"

function Main(go)					
	print("logic start")

	AniSystemDriver:Setup(go);
	AniSystemDriver.SetupEditor(go)
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
end