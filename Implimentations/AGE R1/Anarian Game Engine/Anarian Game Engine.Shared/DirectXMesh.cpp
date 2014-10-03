#include "pch.h"

#ifdef Anarian_DirectX_Mode
#include "DirectXMesh.h"
using namespace Anarian;
using namespace DirectX;

DirectXMesh::DirectXMesh()
	:IMeshObject()
{
}


DirectXMesh::~DirectXMesh()
{
	if (m_vertexBuffer != nullptr)
		m_vertexBuffer.ReleaseAndGetAddressOf();
	if (m_indexBuffer != nullptr)
		m_indexBuffer.ReleaseAndGetAddressOf();

	IMeshObject::~IMeshObject();
}
#endif