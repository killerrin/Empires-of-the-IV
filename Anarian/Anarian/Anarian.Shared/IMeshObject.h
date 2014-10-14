#pragma once
#include "PNTVertex.h"

namespace Anarian{
	class IMeshObject
	{
		friend class MeshFactory;
	protected:
		IMeshObject() { 
			m_indices = std::vector<std::vector<unsigned short>>();
			m_vertices	= std::vector<std::vector<Anarian::Verticies::PNTVertex>> ();
		};

		std::vector<std::vector<unsigned short>> m_indices;
		std::vector<std::vector<Anarian::Verticies::PNTVertex>> m_vertices;
	public:
		virtual ~IMeshObject() { };

		int VertexCount(int index) { return m_indices[index].size(); };
		int IndexCount(int index) { return m_vertices[index].size(); };

		void AddToVertexVector(std::vector<Anarian::Verticies::PNTVertex> vertexList) { m_vertices.push_back(vertexList); }
		void AddToIndexVector(std::vector<unsigned short> indexList) { m_indices.push_back(indexList); }
	};
}