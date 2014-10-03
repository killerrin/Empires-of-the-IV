#include "pch.h"
#include "CubeMesh.h"
#include "DirectXSample.h"
#include "ConstantBuffers.h"

using namespace Microsoft::WRL;
using namespace DirectX;

CubeMesh::CubeMesh(_In_ ID3D11Device *device)
{
	// create vertices to represent the corners of the Hypercraft
	PNTVertex cubeVertices[] =
	{
		{ XMFLOAT3(-0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(0.0f, 0.0f) },    // side 1
		{ XMFLOAT3(0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(0.0f, 1.0f) },
		{ XMFLOAT3(-0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(1.0f, 0.0f) },
		{ XMFLOAT3(0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(1.0f, 1.0f) },

		{ XMFLOAT3(-0.5f, -0.5f, -0.5f),	XMFLOAT3(0.0f, 0.0f, -1.0f),	XMFLOAT2(0.0f, 0.0f) },    // side 2
		{ XMFLOAT3(-0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 0.0f, -1.0f),	XMFLOAT2(0.0f, 1.0f) },
		{ XMFLOAT3(0.5f, -0.5f, -0.5f),		XMFLOAT3(0.0f, 0.0f, -1.0f),	XMFLOAT2(1.0f, 0.0f) },
		{ XMFLOAT3(0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 0.0f, -1.0f),	XMFLOAT2(1.0f, 1.0f) },

		{ XMFLOAT3(-0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(0.0f, 0.0f) },    // side 3
		{ XMFLOAT3(-0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(0.0f, 1.0f) },
		{ XMFLOAT3(0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(1.0f, 0.0f) },
		{ XMFLOAT3(0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(1.0f, 1.0f) },

		{ XMFLOAT3(-0.5f, -0.5f, -0.5f),	XMFLOAT3(0.0f, -1.0f, 0.0f),	XMFLOAT2(0.0f, 0.0f) },    // side 4
		{ XMFLOAT3(0.5f, -0.5f, -0.5f),		XMFLOAT3(0.0f, -1.0f, 0.0f),	XMFLOAT2(0.0f, 1.0f) },
		{ XMFLOAT3(-0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, -1.0f, 0.0f),	XMFLOAT2(1.0f, 0.0f) },
		{ XMFLOAT3(0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, -1.0f, 0.0f),	XMFLOAT2(1.0f, 1.0f) },

		{ XMFLOAT3(0.5f, -0.5f, -0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(0.0f, 0.0f) },    // side 5
		{ XMFLOAT3(0.5f, 0.5f, -0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(0.0f, 1.0f) },
		{ XMFLOAT3(0.5f, -0.5f, 0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(1.0f, 0.0f) },
		{ XMFLOAT3(0.5f, 0.5f, 0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(1.0f, 1.0f) },

		{ XMFLOAT3(-0.5f, -0.5f, -0.5f),	XMFLOAT3(-1.0f, 0.0f, 0.0f),	XMFLOAT2(0.0f, 0.0f) },    // side 6
		{ XMFLOAT3(-0.5f, -0.5f, 0.5f),		XMFLOAT3(-1.0f, 0.0f, 0.0f),	XMFLOAT2(0.0f, 1.0f) },
		{ XMFLOAT3(-0.5f, 0.5f, -0.5f),		XMFLOAT3(-1.0f, 0.0f, 0.0f),	XMFLOAT2(1.0f, 0.0f) },
		{ XMFLOAT3(-0.5f, 0.5f, 0.5f),		XMFLOAT3(-1.0f, 0.0f, 0.0f),	XMFLOAT2(1.0f, 1.0f) },
	};

	// create the vertex buffer
	D3D11_BUFFER_DESC bd = { 0 };
	bd.ByteWidth = sizeof(PNTVertex) * ARRAYSIZE(cubeVertices);
	bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;

	D3D11_SUBRESOURCE_DATA srd = { cubeVertices, 0, 0 };

	device->CreateBuffer(&bd, &srd, &m_vertexBuffer);


	// create the index buffer out of shorts
	short cubeIndices[] =
	{
		0, 1, 2,    // side 1
		2, 1, 3,
		4, 5, 6,    // side 2
		6, 5, 7,
		8, 9, 10,    // side 3
		10, 9, 11,
		12, 13, 14,    // side 4
		14, 13, 15,
		16, 17, 18,    // side 5
		18, 17, 19,
		20, 21, 22,    // side 6
		22, 21, 23,
	};

	// create the index buffer
	D3D11_BUFFER_DESC ibd = { 0 };
	ibd.ByteWidth = sizeof(short) * ARRAYSIZE(cubeIndices);
	ibd.BindFlags = D3D11_BIND_INDEX_BUFFER;

	D3D11_SUBRESOURCE_DATA isrd = { cubeIndices, 0, 0 };

	device->CreateBuffer(&ibd, &isrd, &m_indexBuffer);

	// Set the vertex and index counts
	m_indexCount = ARRAYSIZE(cubeIndices);
	m_vertexCount = ARRAYSIZE(cubeVertices);
}

