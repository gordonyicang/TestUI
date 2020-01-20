-- Queue Table
-- Uses a table as queue, use <table>:enqueue(value) and <table>:dequeue()
-- Lua 5.1 compatible

-- GLOBAL
Queue = { }

-- Create a Table with queue functions
function Queue:Create()

    -- queue table
    local t = { }
    -- entry table
    t._et = { }

    -- enqueue one or more element to queue
    function t:enqueue(...)
        if ... then
            local targs = { ...}
            -- add values
            for _, v in ipairs(targs) do
                table.insert(self._et, v)
            end
        end
    end

    -- pop a value from the stack
    function t:dequeue(num)

        -- get num values from stack
        local num = num or 1

        -- return table
        local entries = { }

        -- get values into entries
        for i = 1, num do
            -- get last entry
            if #self._et > 0 then
                table.insert(entries, self._et[1])
                -- remove last value
                table.remove(self._et, 1)
            else
                break
            end
        end
        -- return unpacked entries
        return unpack(entries)
    end

    -- get head
    function t:gethead()
        if #self._et > 0 then
            return self._et[1]
        end
        return nil
    end

    -- get entries
    function t:getn()
        return #self._et
    end

    -- list values
    function t:list()
        for i, v in pairs(self._et) do
            print(i, v)
        end
    end

    return t
end

-- CHILLCODE™
