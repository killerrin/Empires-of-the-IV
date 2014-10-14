#include "pch.h"

// Factories
#include "MeshFactory.h"
#include "MaterialFactory.h"

// Data Structures
#include "PNTVertex.h"

#include "DirectXMesh.h"
#include "DirectXMaterial.h"
#include "IMeshObject.h"
#include "IMaterial.h"

#include "Common\BasicLoader.h"
#include "tiny_obj_loader.h"

#include "TinyObjectLoaderConverter.h"
using namespace Anarian;

bool TinyObjectLoaderConverter::LoadObj(
	_In_ std::string filename,
	_Out_ Anarian::IMeshObject** m_mesh,
	_Out_ Anarian::IMaterial** m_material,
	_In_opt_ std::string basePath)
{
	IMeshObject* mesh = MeshFactory::Instance()->ConstructEmpty();
	IMaterial* material = MaterialFactory::Instance()->ConstructEmpty();

	std::vector<tinyobj::shape_t> shapes;
	std::vector<tinyobj::material_t> materials;
	std::string error = tinyobj::LoadObj(shapes, materials, filename.c_str(), basePath.c_str());
	
	if (!error.empty()) {
		*m_mesh = mesh;
		*m_material = material;
		return false;
	}

	// Go through each data structure from TinyObjLoader and convert it to internal formats
	for (size_t i = 0; i < shapes.size(); i++) {
		// Indices
		std::vector<unsigned short> indices = std::vector<unsigned short>();
		assert((shapes[i].mesh.indices.size() % 3) == 0);
		for (size_t f = 0; f < shapes[i].mesh.indices.size() / 3; f++) {
			indices.push_back(shapes[i].mesh.indices[3 * f + 0]);
			indices.push_back(shapes[i].mesh.indices[3 * f + 1]);
			indices.push_back(shapes[i].mesh.indices[3 * f + 2]);
		}
		MeshFactory::Instance()->AddToIndexVector(mesh, indices);

		// Vertices
		assert((shapes[i].mesh.positions.size() % 3) == 0);
		std::vector<DirectX::XMFLOAT3> positions = std::vector<DirectX::XMFLOAT3>();
		for (size_t v = 0; v < shapes[i].mesh.positions.size() / 3; v++) {
			positions.push_back({
				shapes[i].mesh.positions[3 * v + 0],
				shapes[i].mesh.positions[3 * v + 1],
				shapes[i].mesh.positions[3 * v + 2]// * -1.0f
			});
		}

		assert((shapes[i].mesh.normals.size() % 3) == 0);
		std::vector<DirectX::XMFLOAT3> normals = std::vector<DirectX::XMFLOAT3>();
		for (size_t v = 0; v < shapes[i].mesh.normals.size() / 3; v++) {
			normals.push_back({
				shapes[i].mesh.normals[3 * v + 0],
				shapes[i].mesh.normals[3 * v + 1],
				shapes[i].mesh.normals[3 * v + 2]// * -1.0f
			});
		}

		assert((shapes[i].mesh.texcoords.size() % 2) == 0);
		std::vector<DirectX::XMFLOAT2> textureCoords = std::vector<DirectX::XMFLOAT2>();
		for (size_t v = 0; v < shapes[i].mesh.texcoords.size() / 2; v++) {
			textureCoords.push_back({
				shapes[i].mesh.texcoords[2 * v + 0],
				shapes[i].mesh.texcoords[2 * v + 1]
			});
		}

		// Convert to PNTVertex's
		std::vector<Anarian::Verticies::PNTVertex> vertices = std::vector<Anarian::Verticies::PNTVertex>();
		for (size_t i = 0; i < positions.size(); i++) {
			vertices.push_back({ positions[i], normals[i], textureCoords[i] });
		}
		MeshFactory::Instance()->AddToVertexVector(mesh, vertices);
	}

	*m_mesh = mesh;
	*m_material = material;
	return true;
}