#pragma once
#ifdef Anarian_DirectX_Mode
#include "IMeshObject.h"

namespace Anarian
{
	class DirectXMesh :
		public IMeshObject
	{
		friend class MeshFactory;
	private:
		std::vector<Microsoft::WRL::ComPtr<ID3D11Buffer>>  m_vertexBuffer;
		std::vector<Microsoft::WRL::ComPtr<ID3D11Buffer>>  m_indexBuffer;

	public:
		DirectXMesh();
		~DirectXMesh();

		void CreateBuffers(ID3D11Device *device);
		void Render(ID3D11DeviceContext *context, int bufferIndex = 0);


		Microsoft::WRL::ComPtr<ID3D11Buffer> VertexBuffer(int index) {
			return m_vertexBuffer[index];
		};
		Microsoft::WRL::ComPtr<ID3D11Buffer> IndexBuffer(int index) {
			return m_indexBuffer[index];
		};
	};
}
#endif