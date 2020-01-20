function clone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs(object) do
            new_table[_copy(key)] = _copy(value)
        end
        return setmetatable(new_table, getmetatable(object))
    end
    return _copy(object)
end

--Create an class.
function class(classname, super)
    local superType = type(super)
    local cls

    if superType ~= "function" and superType ~= "table" then
        superType = nil
        super = nil 
    end

    cls = {}

    -- cls.__create = super

    cls.ctor    = function() end
    cls.__cname = classname
    cls.__ctype = 1

    local superClassName = ""

    function cls.new(...)
        -- local instance = cls.__create(...)
        local instance = super(...)
        if(instance.__cname ~= nil)then
            superClassName = instance.__cname
        end
        -- copy fields from class to native object
        for k,v in pairs(cls) do
            if(type(v) == "function" and instance[k] ~= nil)then
                instance[superClassName.."_"..k] = instance[k]
                instance[k] = v
            else
                instance[k] = v 
            end
        end
        instance.class = cls
        instance:ctor(...)
        return instance
    end

    return cls
end

