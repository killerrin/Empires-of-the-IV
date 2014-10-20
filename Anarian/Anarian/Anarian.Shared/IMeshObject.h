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

		void CalculateTangentBinormal(
			Anarian::Verticies::PNTVertex vertex1, Anarian::Verticies::PNTVertex vertex2, Anarian::Verticies::PNTVertex vertex3,
			DirectX::XMFLOAT3& tangent,
			DirectX::XMFLOAT3& binormal);
		void CalculateNormal(DirectX::XMFLOAT3 tangent, DirectX::XMFLOAT3 binormal, DirectX::XMFLOAT3& normal);

	public:
		virtual ~IMeshObject() { };

		void CalculateModelVectors();

		int VertexCount(int index) { return m_indices[index].size(); };
		int IndexCount(int index) { return m_vertices[index].size(); };
	};
}