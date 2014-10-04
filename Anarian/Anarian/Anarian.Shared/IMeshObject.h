#pragma once
#include "PNTVertex.h"

namespace Anarian{
	class IMeshObject
	{
		friend class MeshFactory;
	protected:
		int m_vertexCount;
		int m_indexCount;
		std::vector<short> m_indices;
		std::vector<Anarian::Verticies::PNTVertex> m_vertices;
	public:
		IMeshObject(){};
		virtual ~IMeshObject(){};

		int VertexCount() { return m_vertexCount; };
		int IndexCount() { return m_indexCount; };
	};
}