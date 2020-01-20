MatrixUtils = {}

function MatrixUtils.forward(matrix)
	local vz = matrix:GetColumn(2);
	return Vector3(vz.x, vz.y, vz.z);
end

function MatrixUtils.upwards(matrix)
	local vy = matrix:GetColumn(1);
    return Vector3(vy.x, vy.y, vy.z);
end

function MatrixUtils.right(matrix)
	local vx = matrix:GetColumn(0);
    return Vector3(vx.x, vx.y, vx.z);
end

function MatrixUtils.rotation(matrix)
	return Quaternion:LookRotation(this:forward(matrix), this:upwards(matrix));
end

function MatrixUtils.position(matrix)
	return Vector3(matrix.m03, matrix.m13, matrix.m23);
end