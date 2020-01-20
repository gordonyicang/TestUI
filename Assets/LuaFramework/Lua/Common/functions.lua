isLogOpen = true;

function Debug(str)
	if isLogOpen == true then
		Util.LogDebug(str);
	end
end

--输出日志--
function Info(str)
	if isLogOpen == true then
	    Util.LogInfo(str);
	end
end

--错误日志--
function Error(str, includeStack)
	if isLogOpen == true then
		if includeStack == nil or includeStack == true then 
			Util.LogError(str.."\n"..',reason='..debug.traceback());
		else
			Util.LogError(str.."\n");
		end
	end
end

--警告日志--
function Warin(str) 
	if isLogOpen == true then
		Util.LogWarin(str);
	end
end


--查找对象--
function find(str)
	return GameObject.Find(str);
end

function destroy(obj)
	if obj ~= nil then
		GameObject.Destroy(obj);
	end
end

function destroyImmediate(obj)
	if obj ~= nil then
		GameObject.DestroyImmediate(obj);
	end
end

function newObject(prefab)
	return GameObject.Instantiate(prefab);
end

function child(str)
	return transform:FindChild(str);
end

function subGet(childNode, typeName)		
	return child(childNode):GetComponent(typeName);
end

function findPanel(str) 
	local obj = find(str);
	if obj == nil then
		error(str.." is null");
		return nil;
	end
	return obj:GetComponent("BaseLua");
end