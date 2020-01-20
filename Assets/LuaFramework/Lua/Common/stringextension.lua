-- 参数:待分割的字符串,分割字符
-- 返回:子串表.(含有空串)
function string.split(str, split_char)
    local sub_str_tab = { };
    while (true) do
        local pos = string.find(str, split_char);
        if (not pos) then
            sub_str_tab[#sub_str_tab + 1] = str;
            break;
        end
        local sub_str = string.sub(str, 1, pos - 1);
        sub_str_tab[#sub_str_tab + 1] = sub_str;
        str = string.sub(str, pos + 1, #str);
    end
    return sub_str_tab;
end

function string.split2(str, split)
    local hash = { }
    if string.len(split) == 0 then
        return nil;
    end
    for i = 1, string.len(split) do
        hash[string.sub(split, i, i)] = true;
    end
    local result = { };
    local startIndex = 1
    local endIndex = 1
    local strLen = string.len(str)
    while endIndex <= strLen do
        if hash[string.sub(str, endIndex, endIndex)] == true then
            if endIndex > startIndex + 1 then
                table.insert(result, string.sub(str, startIndex, endIndex - 1))
            end
            startIndex = endIndex + 1
        end
        endIndex = endIndex + 1;
    end
    if endIndex - 1 > startIndex then
        table.insert(result, string.sub(str, startIndex, endIndex - 1));
    end
    return result;
end

function string.split2number(str, split_char)
    local sub_str_tab = { };
    while (true) do
        local pos = string.find(str, split_char);
        if (not pos) then
            sub_str_tab[#sub_str_tab + 1] = tonumber(str);
            break;
        end
        local sub_str = string.sub(str, 1, pos - 1);
        sub_str_tab[#sub_str_tab + 1] = tonumber(sub_str);
        str = string.sub(str, pos + 1, #str);
    end
    return sub_str_tab;
end

-- 字符串替换
function string.replace(str, old_char, new_char)
    return string.gsub(str, old_char, new_char);
end

-- 判断字符串是否为nil或''
-- return true:空串,false:非空串
function string.isnullorempty(str)
    if str ~= nil and string.len(str) > 0 then
        return false;
    else
        return true;
    end
end

--去掉str左右空格符
function string.trim(str)
    if str == nil then return '' end
    return (string.gsub(str, "^%s*(.-)%s*$", "%1"));
end

-- 格式化时间差
-- beginTime开始时间戳
function string.formattime(beginTime)
    return string.format('%.3f',(os.clock() - beginTime));
end

-- 返回保留2位小数点数字的字符串
function string.formatnumber(number)
    return string.format('%.2f', number);
end