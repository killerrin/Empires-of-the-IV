#include "pch.h"

#if Anarian_DirectX_Mode
#include "DirectXMesh.h"
using namespace DirectX;
#endif

#include "PNTVertex.h"

#include "MeshFactory.h"
using namespace Anarian;


MeshFactory* MeshFactory::m_instance;
MeshFactory* MeshFactory::Instance()
{
	if (m_instance == nullptr) m_instance = new MeshFactory();
	return m_instance;
}

MeshFactory::MeshFactory()
{
}

MeshFactory::~MeshFactory()
{
	//delete m_instance;
}

IMeshObject* MeshFactory::ConstructCube()
{
	IMeshObject* cubeMesh;

	// Construct the MeshObject
#if Anarian_DirectX_Mode
	cubeMesh = new DirectXMesh();
#endif

	// Create the Vertices
	std::vector<Anarian::Verticies::PNTVertex> cubeVertices = std::vector<Anarian::Verticies::PNTVertex>();
	cubeVertices.reserve(24);

	//						 Position							Normal							Texture Coordinates
	cubeVertices.push_back({ XMFLOAT3(-0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(0.0f, 0.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(0.0f, 1.0f) });
	cubeVertices.push_back({ XMFLOAT3(-0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(1.0f, 0.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(1.0f, 1.0f) });

	cubeVertices.push_back({ XMFLOAT3(-0.5f, -0.5f, -0.5f),		XMFLOAT3(0.0f, 0.0f, -1.0f),	XMFLOAT2(0.0f, 0.0f) });   // side 2
	cubeVertices.push_back({ XMFLOAT3(-0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 0.0f, -1.0f),	XMFLOAT2(0.0f, 1.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, -0.5f, -0.5f),		XMFLOAT3(0.0f, 0.0f, -1.0f),	XMFLOAT2(1.0f, 0.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 0.0f, -1.0f),	XMFLOAT2(1.0f, 1.0f) });

	cubeVertices.push_back({ XMFLOAT3(-0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(0.0f, 0.0f) });   // side 3
	cubeVertices.push_back({ XMFLOAT3(-0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(0.0f, 1.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(1.0f, 0.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(1.0f, 1.0f) });

	cubeVertices.push_back({ XMFLOAT3(-0.5f, -0.5f, -0.5f),		XMFLOAT3(0.0f, -1.0f, 0.0f),	XMFLOAT2(0.0f, 0.0f) });    // side 4
	cubeVertices.push_back({ XMFLOAT3(0.5f, -0.5f, -0.5f),		XMFLOAT3(0.0f, -1.0f, 0.0f),	XMFLOAT2(0.0f, 1.0f) });
	cubeVertices.push_back({ XMFLOAT3(-0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, -1.0f, 0.0f),	XMFLOAT2(1.0f, 0.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, -1.0f, 0.0f),	XMFLOAT2(1.0f, 1.0f) });

	cubeVertices.push_back({ XMFLOAT3(0.5f, -0.5f, -0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(0.0f, 0.0f) });   // side 5
	cubeVertices.push_back({ XMFLOAT3(0.5f, 0.5f, -0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(0.0f, 1.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, -0.5f, 0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(1.0f, 0.0f) });
	cubeVertices.push_back({ XMFLOAT3(0.5f, 0.5f, 0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(1.0f, 1.0f) });

	cubeVertices.push_back({ XMFLOAT3(-0.5f, -0.5f, -0.5f),		XMFLOAT3(-1.0f, 0.0f, 0.0f),	XMFLOAT2(0.0f, 0.0f) });   // side 6
	cubeVertices.push_back({ XMFLOAT3(-0.5f, -0.5f, 0.5f),		XMFLOAT3(-1.0f, 0.0f, 0.0f),	XMFLOAT2(0.0f, 1.0f) });
	cubeVertices.push_back({ XMFLOAT3(-0.5f, 0.5f, -0.5f),		XMFLOAT3(-1.0f, 0.0f, 0.0f),	XMFLOAT2(1.0f, 0.0f) });
	cubeVertices.push_back({ XMFLOAT3(-0.5f, 0.5f, 0.5f),		XMFLOAT3(-1.0f, 0.0f, 0.0f),	XMFLOAT2(1.0f, 1.0f) });

	// Create the Indices
	std::vector<short> cubeIndices = std::vector<short>();
	cubeIndices.reserve(36);
	cubeIndices.push_back(0);	cubeIndices.push_back(1);	cubeIndices.push_back(2); // side 1
	cubeIndices.push_back(2);	cubeIndices.push_back(1);	cubeIndices.push_back(3);
	cubeIndices.push_back(4);	cubeIndices.push_back(5);	cubeIndices.push_back(6); // side 2
	cubeIndices.push_back(6);	cubeIndices.push_back(5);	cubeIndices.push_back(7);
	cubeIndices.push_back(8);	cubeIndices.push_back(9);	cubeIndices.push_back(10); // side 3
	cubeIndices.push_back(10);	cubeIndices.push_back(9);	cubeIndices.push_back(11);
	cubeIndices.push_back(12);	cubeIndices.push_back(13);	cubeIndices.push_back(14); // side 4
	cubeIndices.push_back(14);	cubeIndices.push_back(13);	cubeIndices.push_back(15);
	cubeIndices.push_back(16);	cubeIndices.push_back(17);	cubeIndices.push_back(18); // side 5
	cubeIndices.push_back(18);	cubeIndices.push_back(17);	cubeIndices.push_back(19);
	cubeIndices.push_back(20);	cubeIndices.push_back(21);	cubeIndices.push_back(22); // side 6
	cubeIndices.push_back(22);	cubeIndices.push_back(21);	cubeIndices.push_back(23);

	// Apply cross-platform variables
	cubeMesh->m_vertices = cubeVertices;
	cubeMesh->m_indices = cubeIndices;

	cubeMesh->m_vertexCount = cubeVertices.size();
	cubeMesh->m_indexCount = cubeIndices.size();

	return cubeMesh;
}
