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
	m_vertexBuffer		= std::vector<Microsoft::WRL::ComPtr<ID3D11Buffer>> ();
	m_indexBuffer		= std::vector<Microsoft::WRL::ComPtr<ID3D11Buffer>> ();
}


DirectXMesh::~DirectXMesh()
{
	for (int i = 0; i < m_vertexBuffer.size(); i++) {
		m_vertexBuffer[i].ReleaseAndGetAddressOf();
	}

	for (int i = 0; i < m_indexBuffer.size(); i++) {
		m_indexBuffer[i].ReleaseAndGetAddressOf();
	}
	//IMeshObject::~IMeshObject();
}

void DirectXMesh::CreateBuffers(ID3D11Device *device)
{
#ifdef EnableDebug
	for (int j = 0; j < m_vertices.size(); j++) {
		for (int i = 0; i < m_vertices[j].size(); i++) {
			std::string str =
				"(" + std::to_string(m_vertices[j][i].position.x) + ", " + std::to_string(m_vertices[j][i].position.y) + ", " + std::to_string(m_vertices[j][i].position.z) + ") | " +
				"(" + std::to_string(m_vertices[j][i].normal.x) + ", " + std::to_string(m_vertices[j][i].normal.y) + ", " + std::to_string(m_vertices[j][i].normal.z) + ") | " +
				"(" + std::to_string(m_vertices[j][i].textureCoordinate.x) + ", " + std::to_string(m_vertices[j][i].textureCoordinate.y) + ") \n";
			std::wstring wstr(str.begin(), str.end());
			OutputDebugString(wstr.c_str());
		}
	}
#endif

	if (m_vertices.size() == 0)
		return;
	if (m_indices.size() == 0)
		return;

	// This will throw Null Refrence later on due to ineven numbers
	// So its better to just not allow it
	if (m_vertices.size() != m_indices.size())
		return;

	// Calculate the Tangent, Binormal and Normal values
	CalculateModelVectors();

	// Create the Vertex Buffer
	for (int i = 0; i < m_vertices.size(); i++) {
		D3D11_BUFFER_DESC bd = { 0 };
		bd.ByteWidth = sizeof(Anarian::Verticies::PNTVertex) * m_vertices[i].size();
		bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;

		Microsoft::WRL::ComPtr<ID3D11Buffer> vBuff;

		D3D11_SUBRESOURCE_DATA srd = { &m_vertices[i][0], 0, 0 };
		device->CreateBuffer(&bd, &srd, &vBuff);

		m_vertexBuffer.push_back(vBuff);
	}


	// Create the Index Buffer
	for (int i = 0; i < m_indices.size(); i++) {
		D3D11_BUFFER_DESC ibd = { 0 };
		ibd.ByteWidth = sizeof(unsigned short) * m_indices[i].size();
		ibd.BindFlags = D3D11_BIND_INDEX_BUFFER;

		Microsoft::WRL::ComPtr<ID3D11Buffer> iBuff;

		D3D11_SUBRESOURCE_DATA isrd = { &m_indices[i][0], 0, 0 };
		device->CreateBuffer(&ibd, &isrd, &iBuff);

		m_indexBuffer.push_back(iBuff);
	}
}

void DirectXMesh::Render(ID3D11DeviceContext *context, int bufferIndex)
{
	uint32 stride = sizeof(Anarian::Verticies::PNTVertex);
	uint32 offset = 0;

	// Set the primitive topology
	context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

	for (int i = 0; i < m_vertexBuffer.size(); i++) {
		// Set the buffers
		context->IASetVertexBuffers(bufferIndex, 1, m_vertexBuffer[i].GetAddressOf(), &stride, &offset);
		context->IASetIndexBuffer(m_indexBuffer[i].Get(), DXGI_FORMAT_R16_UINT, 0);

		// Draw
		context->DrawIndexed(m_indices[i].size(), 0, 0);
	}
}
#endif