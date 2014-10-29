#pragma once
#ifdef Anarian_DirectX_Mode
#include "IMeshObject.h"

namespace Anarian
{
	class DirectXMesh :
		public IMeshObject
	{
		friend class MeshFactory;
		friend class Model;
	private:
		std::vector<Microsoft::WRL::ComPtr<ID3D11Buffer>>  m_vertexBuffer;
		std::vector<Microsoft::WRL::ComPtr<ID3D11Buffer>>  m_indexBuffer;

		UINT m_vertexCPUAccess; 
		UINT m_indexCPUAccess;

	public:
		DirectXMesh();
		~DirectXMesh();

		void CreateBuffers(ID3D11Device *device, UINT vertexCPUAccess = 0, UINT indexCPUAccess = 0);
		void Render(ID3D11DeviceContext *context, int bufferIndex = 0);

		bool IsVertexBufferCPUAccessable() {
			if (m_vertexCPUAccess == D3D11_CPU_ACCESS_FLAG::D3D11_CPU_ACCESS_WRITE)
				return true;
			return false;
		}
		bool IsIndexBufferCPUAccessable() {
			if (m_indexCPUAccess == D3D11_CPU_ACCESS_FLAG::D3D11_CPU_ACCESS_WRITE)
				return true;
			return false;
		}

		ID3D11Buffer* VertexBuffer(int index) {
			return m_vertexBuffer[index].Get();
		};
		ID3D11Buffer* IndexBuffer(int index) {
			return m_indexBuffer[index].Get();
		};
	};
}
#endif