#include "pch.h"
#ifdef Anarian_DirectX_Mode
#include "PNTVertex.h"
#include "MeshFactory.h"
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

	//IMeshObject::~IMeshObject();
}

void DirectXMesh::CreateBuffers(ID3D11Device *device)
{
	if (m_vertices.size() == 0)
		return;
	if (m_indices.size() == 0)
		return;

	// Create the Vertex Buffer
	D3D11_BUFFER_DESC bd = { 0 };
	bd.ByteWidth = sizeof(Anarian::Verticies::PNTVertex) * m_vertices.size();
	bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;

	D3D11_SUBRESOURCE_DATA srd = { &m_vertices[0], 0, 0 };
	device->CreateBuffer(&bd, &srd, &m_vertexBuffer);

	// Create the Index Buffer
	D3D11_BUFFER_DESC ibd = { 0 };
	ibd.ByteWidth = sizeof(short) * m_indices.size();
	ibd.BindFlags = D3D11_BIND_INDEX_BUFFER;

	D3D11_SUBRESOURCE_DATA isrd = { &m_indices[0], 0, 0 };
	device->CreateBuffer(&ibd, &isrd, &m_indexBuffer);
}

void DirectXMesh::Render(ID3D11DeviceContext *context, int bufferIndex)
{
	uint32 stride = sizeof(Anarian::Verticies::PNTVertex);
	uint32 offset = 0;

	context->IASetVertexBuffers(bufferIndex, 1, m_vertexBuffer.GetAddressOf(), &stride, &offset);
	context->IASetIndexBuffer(m_indexBuffer.Get(), DXGI_FORMAT_R16_UINT, 0);

	// set the primitive topology
	context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

	// draw
	context->DrawIndexed(m_indexCount, 0, 0);
}
#endif