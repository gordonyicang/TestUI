--性能采样脚本

ProfilerSimple ={};

--开始采样
function ProfilerSimple.BeginSample(name)
	if SystemConfig~=nil and SystemConfig.beginSample==true then
        Profiler.BeginSample(name);
    end
end

--结束采样
function ProfilerSimple.EndSample()
	--AniSystemDriver.isEditor==false and 
	if SystemConfig~=nil and SystemConfig.beginSample==true then
        Profiler.EndSample();
    end
end