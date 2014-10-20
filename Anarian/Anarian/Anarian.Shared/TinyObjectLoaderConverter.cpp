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
	_In_ std::string basePath,
	_In_ std::string filename,
	_Out_ Anarian::IMeshObject** m_mesh,
	_Out_ Anarian::IMaterial** m_material,
	_In_ BasicLoader^ basicLoader)
{
	IMeshObject* mesh = MeshFactory::Instance()->ConstructEmpty();
	IMaterial* material = MaterialFactory::Instance()->ConstructEmpty();

	std::vector<tinyobj::shape_t> shapes;
	std::vector<tinyobj::material_t> materials;

	std::string filenameFull = basePath + filename;
	std::string error = tinyobj::LoadObj(shapes, materials, filenameFull.c_str(), basePath.c_str());
	
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

	for (size_t i = 0; i < materials.size(); i++) {

		material->SetMeshColor(Color(materials[i].ambient[0], materials[i].ambient[1], materials[i].ambient[2]));
		material->SetDiffuseColor(Color(materials[i].diffuse[0], materials[i].diffuse[1], materials[i].diffuse[2]));

		material->SetSpecularColor(Color(materials[i].specular[0], materials[i].specular[1], materials[i].specular[2]));
		material->SetSpecularExponent(materials[i].shininess);

		// Load Diffuse Texture
		ID3D11ShaderResourceView* diffuseTextureView;

		std::string diffTexFull = basePath + materials[i].diffuse_texname;
		std::wstring diffuseTextureFullPath(diffTexFull.begin(), diffTexFull.end());

		basicLoader->LoadTexture(ref new Platform::String(diffuseTextureFullPath.c_str()), nullptr, &diffuseTextureView);
		((DirectXMaterial*)material)->AddTexture(diffuseTextureView);

		// Load Specular Texture
		ID3D11ShaderResourceView* specularTextureView;

		std::string specTexFull = basePath + materials[i].specular_texname;
		std::wstring specularTextureFullPath(specTexFull.begin(), specTexFull.end());

		basicLoader->LoadTexture(ref new Platform::String(specularTextureFullPath.c_str()), nullptr, &specularTextureView);
		((DirectXMaterial*)material)->AddTexture(specularTextureView);

		// Load Bump (Normal) Texture
		std::map<std::string, std::string>::const_iterator it(materials[i].unknown_parameter.begin());
		std::map<std::string, std::string>::const_iterator itEnd(materials[i].unknown_parameter.end());
		for (; it != itEnd; it++) {
			if (it->first == "bump") {
				ID3D11ShaderResourceView* bumpTextureView;

				std::string bumpTexFull = basePath + it->second;
				std::wstring bumpTextureFullPath(bumpTexFull.begin(), bumpTexFull.end());

				basicLoader->LoadTexture(ref new Platform::String(bumpTextureFullPath.c_str()), nullptr, &bumpTextureView);
				((DirectXMaterial*)material)->AddTexture(bumpTextureView);
			}
		}

		// Load NS Texture
		ID3D11ShaderResourceView* glossTextureView;

		std::string glossText = basePath + materials[i].normal_texname;
		std::wstring glossTextureFullPath(glossText.begin(), glossText.end());

		basicLoader->LoadTexture(ref new Platform::String(glossTextureFullPath.c_str()), nullptr, &glossTextureView);
		((DirectXMaterial*)material)->AddTexture(glossTextureView);
	}

	*m_mesh = mesh;
	*m_material = material;
	return true;
}