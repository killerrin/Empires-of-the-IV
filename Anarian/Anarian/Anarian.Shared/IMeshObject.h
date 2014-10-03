#pragma once
#include "PNTVertex.h"

namespace Anarian{
	class IMeshObject
	{
	protected:
		int m_vertexCount;
		int m_indexCount;
		std::vector<short> m_indices;
		std::vector<PNTVertex> m_vertices;
	public:
		IMeshObject(){};
		virtual ~IMeshObject(){};
	};
}