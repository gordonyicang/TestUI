local pool = {};
local table_insert = table.insert;
local table_remove = table.remove;

function table.deepcopy(origin, target)
	if origin == nil or target == nil then
		return;
	end

	for key, var in pairs(origin) do
		if type(var) == 'table' then
			target[key] = {};
			table.deepcopy(var, target[key]);
		else
			target[key] = var;
		end
	end
end

function table.indexOf(target,value)
	for key, var in pairs(target) do 
		if value == var then
			return key;
		end
	end
	return -1;
end

function table.clear(target)
	for key, var in pairs(target) do
		target[key] = nil;
	end
end

function table.clearlist(target)
	while #target > 0 do
		table_remove(target, #target);
	end
end

--从池中取出一个table
function table.getfrompool()
	if #pool > 0 then
		return table_remove(pool, #pool);
	else
		return {};
	end
end

--把table返回池中
--data      table
--includeMe 是否把data也放回池中[默认:是]
function table.put2pool(data, includeMe)
	if data~=nil and type(data)=='table' then
		--清空表所有值
		for k,v in pairs(data) do
			if v~=nil and type(v)=='table' then
				table.put2pool(v, true);
			end
			data[k] = nil;
		end

		if includeMe==nil or includeMe==true then
			table_insert(pool, data);
		end
	end
end

--返回池大小
function table.getpoolsize()
	return #pool;
end
