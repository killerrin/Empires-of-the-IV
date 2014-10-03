#pragma once
#ifdef Anarian_DirectX_Mode
#include "IMeshObject.h"

namespace Anarian
{
	class DirectXMesh :
		public IMeshObject
	{
	private:
		Microsoft::WRL::ComPtr<ID3D11Buffer>  m_vertexBuffer;
		Microsoft::WRL::ComPtr<ID3D11Buffer>  m_indexBuffer;

	public:
		DirectXMesh();
		~DirectXMesh();
	};
}
#endif