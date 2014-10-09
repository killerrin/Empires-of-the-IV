#pragma once
#include "PNTVertex.h"

namespace Anarian{
	class IMeshObject
	{
		friend class MeshFactory;
	protected:
		IMeshObject() { 
			m_indices	= std::vector<std::vector<short>> ();
			m_vertices	= std::vector<std::vector<Anarian::Verticies::PNTVertex>> ();
		};

		//int m_vertexCount;
		//int m_indexCount;
		std::vector<std::vector<short>> m_indices;
		std::vector<std::vector<Anarian::Verticies::PNTVertex>> m_vertices;
	public:
		virtual ~IMeshObject() { };

		int VertexCount(int index) { return m_indices[index].size(); };
		int IndexCount(int index) { return m_vertices[index].size(); };
	};
}