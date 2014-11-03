#pragma once
#include "PNTVertex.h"
#include "Weight.h"
#include "Joint.h"

namespace Anarian{
	class IMeshObject
	{
		friend class MeshFactory;
		friend class Model;
	protected:
		IMeshObject() { 
			Name = "";
			m_indices = std::vector<std::vector<unsigned short>>();
			m_vertices	= std::vector<std::vector<Anarian::Verticies::PNTVertex>>();
			m_weights = std::vector<std::vector<Anarian::Verticies::Weight>>();
		};
		
		std::vector<std::vector<Anarian::Verticies::PNTVertex>> m_vertices;
		std::vector<std::vector<unsigned short>> m_indices;
		std::vector<std::vector<Anarian::Verticies::Weight>> m_weights;

		void CalculateTangentBinormal(
			Anarian::Verticies::PNTVertex vertex1, Anarian::Verticies::PNTVertex vertex2, Anarian::Verticies::PNTVertex vertex3,
			DirectX::XMFLOAT3& tangent,
			DirectX::XMFLOAT3& binormal);
		void CalculateNormal(DirectX::XMFLOAT3 tangent, DirectX::XMFLOAT3 binormal, DirectX::XMFLOAT3& normal);

	public:
		std::string Name;

		virtual ~IMeshObject() { };
		void CalculateModelVectors();

		int VertexCount(int index) { return m_indices[index].size(); };
		int IndexCount(int index) { return m_vertices[index].size(); };
		std::vector<std::vector<unsigned short>>* Indices() { return &m_indices; };
		std::vector<std::vector<Anarian::Verticies::PNTVertex>>* Vertices() { return &m_vertices; };
	};
}