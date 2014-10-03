#pragma once
#ifdef Anarian_DirectX_Mode

namespace Anarian {
	struct ConstantBufferDefault
	{
		DirectX::XMFLOAT4 color;
	};

	struct ConstantBufferChangeOnResize
	{
		DirectX::XMFLOAT4X4 projection;
	};
}
#endif