
exception = {};

--return false:异常,true:正常
function exception:pcall(func, ...)
    if Application.platform == RuntimePlatform.OSXEditor then  --Application.platform == RuntimePlatform.IPhonePlayer or 
        func(...);
        return true;
    else
        local ok, err = pcall(func, ...);
        if ok == false then
            Error(err);
        end
        return ok;
    end
end

function exception:xpcall(func, ...)
    if Application.platform == RuntimePlatform.OSXEditor then
        local data = func(...);
        return true, data;
    else
        local ok, data = xpcall(func, debug.traceback, ...);
        if ok == false then
            Error(data);
        end
        return ok, data;
    end
	
end