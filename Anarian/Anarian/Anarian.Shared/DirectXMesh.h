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
		Microsoft::WRL::ComPtr<ID3D11Buffer>  m_vertexBuffer;
		Microsoft::WRL::ComPtr<ID3D11Buffer>  m_indexBuffer;

	public:
		DirectXMesh();
		~DirectXMesh();

		void CreateBuffers(ID3D11Device *device);
		void Render(ID3D11DeviceContext *context);


		Microsoft::WRL::ComPtr<ID3D11Buffer> VertexBuffer() {
			return m_vertexBuffer;
		};
		Microsoft::WRL::ComPtr<ID3D11Buffer> IndexBuffer() {
			return m_indexBuffer;
		};
	};
}
#endif