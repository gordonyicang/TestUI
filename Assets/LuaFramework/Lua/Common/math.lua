local precision = 0.05;  --精准度5cm

function math.randomInt(min, max)
    return math.random(min, max);
end

function math.randomFloat(min, max)
    return min +(max - min) * math.random();
end

-- 弧度转角度
function math.radian2Angle(radian)
    return math.deg(radian);
end

-- 角度转弧度
function math.angle2Radian(angle)
    return math.rad(angle);
end

-- 生成非重复随机序列
-- min int
-- max int
-- @return table
function math.randomSequence(min, max)
    local x = 0;
    local temp = 0;
    if min > max then
        temp = min;
        min = max;
        max = temp;
    end
    local arr = { };
    for i = 1, max - min + 1 do
        table.insert(arr, 0);
    end
    for i = min, max do
        arr[i - min] = i;
    end
    for i = #arr, 1, -1 do
        local x = this:RandomInt(0, i);
        temp = arr[i];
        arr[i] = arr[x];
        arr[x] = temp;
    end
    return arr;
end

-- 判断两个向量值是否相等
function math.equalsVector3(src, tar)
    return math.equals6arg(src.x, src.y, src.z, tar.x, tar.y, tar.z);
end

function math.equals6arg(x1, y1, z1, x2, y2, z2)
    return math.abs(x1 - x2) < precision and math.abs(y1 - y2) < precision and math.abs(z1 - z2) < precision;
end

function math.equals(x1, z1, x2, z2)
    return math.abs(x1-x2) < precision and math.abs(z1-z2) < precision;
end

function math.equalsFloat(float_a, float_b)
    return math.abs(float_a - float_b) < precision;
end

function math.round(num, numDecimalPlaces)
    local mult = 10 ^(numDecimalPlaces or 0)
    return math.floor(num * mult + 0.5) / mult
end

function math.digit(num)
    local result = 1;
    if num < 10 then
        return result;
    else
        return result + math.digit(math.floor(num / 10));
    end
end

function math.sign(num)
    return num>=0 and 1 or -1;
end