
local EventDispatcher = {};
local events = {};
local eventState= {};
local addEvents = {};
local delEvents = {};
local cachePool = {};
local table_insert = table.insert;
local table_remove = table.remove;

local function GetFromPool()
	if #cachePool >0 then
		return table_remove(cachePool, #cachePool);
	else
		return {};
	end
end

local function Put2Pool(data)
	if data ~= nil then
		while #data > 0 do
			table_remove(data, #data);
		end
		table_insert(cachePool, data);
	end
end

function EventDispatcher:AddEventListener(event, handler, handler_self)
	if not event or type(event) ~= "string" then
		error(string.format("event parameter in addlistener function has to be string, %s not right.", type(event)));
	end
	if not handler or type(handler) ~= "function" then
		error(string.format("handler parameter in addlistener function has to be function, %s not right", type(handler)));
	end

	if not events[event] then
		events[event] = GetFromPool();
	end

	local tab = GetFromPool();
	table_insert(tab, handler);
	table_insert(tab, handler_self);
	table_insert(tab, event);

	if eventState[event] == nil then
		table_insert(events[event], tab);
	else
		table_insert(addEvents, tab);
	end
end

--广播事件
--event 事件名称
-- ...  携带不定参数
function EventDispatcher:TriggerEvent(event, ...)
	local eventList = events[event];
	if eventList ~=nil then
		eventState[event] = true; --执行中
		ProfilerSimple.BeginSample(string.format('TriggerEvent event=%s,num=%s', event, #eventList));
		for k,v in pairs(eventList) do
			exception:xpcall(v[1], v[2], ...);
		end
		ProfilerSimple.EndSample();
		eventState[event] = nil;  --执行结束
	end
	self:DelWaitEvents();
	self:AddWaitEvents();
end

function EventDispatcher:AddWaitEvents()
	while #addEvents > 0 do
		local event = addEvents[1];
		table_remove(addEvents, 1);
		table_insert(events[event[3]], event);
	end
end

function EventDispatcher:DelWaitEvents()
	while #delEvents > 0 do
		local event = table_remove(delEvents, 1);
		self:ImmediatelyRemoveEventListener(event[3], event[1], event[2]);
		Put2Pool(event);
	end
end

--判断是否已注册的事件
--return [true:已注册,false:未注册]
function EventDispatcher:HasRegisterEvent(event)
	return event ~= nil and events[event] ~= nil;
end

--移除事件监听
--event        事件名称
--handler      事件处理函数
--handler_self 事件处理函数所在对象实例
function EventDispatcher:RemoveEventListener(event, handler, handler_self)
	if events[event] ~= nil and handler~=nil and handler_self~=nil then
		if eventState[event] == nil then 
			self:ImmediatelyRemoveEventListener(event, handler, handler_self);
		else
			local tab = GetFromPool();
			table_insert(tab, handler);
			table_insert(tab, handler_self);
			table_insert(tab, event);
			table_insert(delEvents, tab);
		end
	end
end

function EventDispatcher:ImmediatelyRemoveEventListener(event, handler, handler_self)
	for i=#events[event], 1, -1 do
		local v = events[event][i];
		if v[1] == handler and v[2] == handler_self then
			table_remove(events[event], i);
			Put2Pool(v);
			break;
		end
	end
end

return EventDispatcher;