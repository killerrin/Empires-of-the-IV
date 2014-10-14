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

		// Load Bump Texture
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

void TinyObjectLoaderConverter::CalculateTangentBinormal(
	DirectX::XMFLOAT3 vertex1, DirectX::XMFLOAT3 vertex2, DirectX::XMFLOAT3 vertex3,
	DirectX::XMFLOAT3& tangent, DirectX::XMFLOAT3& binormal)
{
	float vector1[3], vector2[3];
	float tuVector[2], tvVector[2];
	float den;
	float length;


	// Calculate the two vectors for this face.
	vector1[0] = vertex2.x - vertex1.x;
	vector1[1] = vertex2.y - vertex1.y;
	vector1[2] = vertex2.z - vertex1.z;

	vector2[0] = vertex3.x - vertex1.x;
	vector2[1] = vertex3.y - vertex1.y;
	vector2[2] = vertex3.z - vertex1.z;

	// Calculate the tu and tv texture space vectors.
	tuVector[0] = vertex2.x - vertex1.x;
	tvVector[0] = vertex2.y - vertex1.y;

	tuVector[1] = vertex3.x - vertex1.x;
	tvVector[1] = vertex3.y - vertex1.y;

	// Calculate the denominator of the tangent/binormal equation.
	den = 1.0f / (tuVector[0] * tvVector[1] - tuVector[1] * tvVector[0]);

	// Calculate the cross products and multiply by the coefficient to get the tangent and binormal.
	tangent.x = (tvVector[1] * vector1[0] - tvVector[0] * vector2[0]) * den;
	tangent.y = (tvVector[1] * vector1[1] - tvVector[0] * vector2[1]) * den;
	tangent.z = (tvVector[1] * vector1[2] - tvVector[0] * vector2[2]) * den;

	binormal.x = (tuVector[0] * vector2[0] - tuVector[1] * vector1[0]) * den;
	binormal.y = (tuVector[0] * vector2[1] - tuVector[1] * vector1[1]) * den;
	binormal.z = (tuVector[0] * vector2[2] - tuVector[1] * vector1[2]) * den;

	// Calculate the length of this normal.
	length = sqrt((tangent.x * tangent.x) + (tangent.y * tangent.y) + (tangent.z * tangent.z));

	// Normalize the normal and then store it
	tangent.x = tangent.x / length;
	tangent.y = tangent.y / length;
	tangent.z = tangent.z / length;

	// Calculate the length of this normal.
	length = sqrt((binormal.x * binormal.x) + (binormal.y * binormal.y) + (binormal.z * binormal.z));

	// Normalize the normal and then store it
	binormal.x = binormal.x / length;
	binormal.y = binormal.y / length;
	binormal.z = binormal.z / length;

	return;
}