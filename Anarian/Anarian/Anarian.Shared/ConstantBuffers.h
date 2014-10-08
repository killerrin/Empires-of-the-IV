#pragma once
#include "Color.h"

namespace Anarian {

	struct ConstantBufferChangesOnResize
	{
		DirectX::XMFLOAT4X4 cameraProjection;
	};

	struct ConstantBufferChangesEveryFrame
	{
		DirectX::XMFLOAT4X4 cameraView;
	};

	struct ConstantBufferChangesEveryPrim
	{
		DirectX::XMFLOAT4X4 modelWorldMatrix;
		Color meshColor;
		Color diffuseColor;
		Color specularColor;
		float specularPower;
	};
}