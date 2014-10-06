#pragma once
#include "Color.h"

namespace Anarian {
	struct ConstantBufferChangesEveryPrim
	{
		DirectX::XMFLOAT4X4 worldMatrix;
		Color meshColor;
		Color diffuseColor;
		Color specularColor;
		float specularPower;
	};
}