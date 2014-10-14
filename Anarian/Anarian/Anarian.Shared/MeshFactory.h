#pragma once
#include "IMeshObject.h"

namespace Anarian {
	class MeshFactory
	{
	private:
		static MeshFactory* m_instance;
		MeshFactory();
	public:
		static MeshFactory* Instance();
		~MeshFactory();

		// Constructions
		IMeshObject* ConstructEmpty();
		IMeshObject* ConstructCube(IMeshObject* parent = nullptr);
		IMeshObject* ConstructFace(IMeshObject* parent = nullptr);
		IMeshObject* ConstructCylinder(uint32 segments, IMeshObject* parent = nullptr);
		IMeshObject* ConstructSphere(uint32 segments, IMeshObject* parent = nullptr);

		// External Mesh Creation Helper Methods
		void AddToVertexVector(IMeshObject* mesh, std::vector<Anarian::Verticies::PNTVertex> vertexList);
		void AddToIndexVector(IMeshObject* mesh, std::vector<unsigned short> indexList);
	};
}