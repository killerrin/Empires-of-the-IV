#include "pch.h"

#include "PNTVertex.h"
#include "Weight.h"
#include "Joint.h"

#include "IMeshObject.h"
#include "IMaterial.h"
#include "DirectXMesh.h"
#include "DirectXMaterial.h"
#include "Model.h"

#include "Common\BasicLoader.h"

#include "MD5LoaderConverter.h"
#include "MD5Loader.h"

using namespace Anarian;
using namespace Verticies;
using namespace DirectX;

bool MD5LoaderConverter::LoadMD5Mesh(
	_In_ std::string filename,
	_Out_ Anarian::Model** m_model,
	_In_ BasicLoader^ basicLoader)
{
	std::wstring wfilename(filename.begin(), filename.end());
	MD5Loader::Model3D loadedModel3D;
	bool loadedSuccessfully = MD5Loader::LoadMD5Model(wfilename, loadedModel3D);
	
	IMeshObject* mesh = MeshFactory::Instance()->ConstructEmpty();
	IMaterial* material = MaterialFactory::Instance()->ConstructEmpty();
	std::vector<Joint> joints = std::vector<Joint>();

	// Convert the Objects
	for (int i = 0; i < loadedModel3D.subsets.size(); i++) {
		// Indices
		std::vector<unsigned short> indices = std::vector<unsigned short>();
		assert((loadedModel3D.subsets[i].indices.size() % 3) == 0);
		for (int x = loadedModel3D.subsets[i].indices.size() - 1; x >= 0; x--) {
			indices.push_back(loadedModel3D.subsets[i].indices[x]);
		}

		MeshFactory::Instance()->AddToIndexVector(mesh, indices);

		// Vertices
		std::vector<PNTVertex> subsetVertices = std::vector<PNTVertex>();
		for (int x = 0; x < loadedModel3D.subsets[i].vertices.size(); x++) {
			PNTVertex vert = PNTVertex();
			
			vert.binormal = loadedModel3D.subsets[i].vertices[x].biTangent;
			vert.m_startWeight = loadedModel3D.subsets[i].vertices[x].StartWeight;
			vert.m_weightCount = loadedModel3D.subsets[i].vertices[x].WeightCount;
			vert.normal = loadedModel3D.subsets[i].vertices[x].normal;
			vert.position = loadedModel3D.subsets[i].vertices[x].pos;
			vert.tangent = loadedModel3D.subsets[i].vertices[x].tangent;
			vert.textureCoordinate = loadedModel3D.subsets[i].vertices[x].texCoord;

			subsetVertices.push_back(vert);
		}
		MeshFactory::Instance()->AddToVertexVector(mesh, subsetVertices);

		for (int j = 0; j < loadedModel3D.joints.size(); j++) {
			Joint joint = Joint();
			joint.Name = loadedModel3D.joints[j].name;
			joint.Orientation = loadedModel3D.joints[j].orientation;
			joint.ParentID = loadedModel3D.joints[j].parentID;
			joint.Position = loadedModel3D.joints[j].pos;

			joints.push_back(joint);
		}

		// Load all the textures and add them to the material
		for (int t = 0; t < loadedModel3D.subsets[i].texFileNameArray.size(); t++) {
			ID3D11ShaderResourceView* assetTextureView;

			basicLoader->LoadTexture(ref new Platform::String(loadedModel3D.subsets[i].texFileNameArray[t].c_str()), nullptr, &assetTextureView);
			((DirectXMaterial*)material)->AddTexture(assetTextureView);
		}
	}

	// Create the Model and add the Joints
	Model* model = new Model(
		mesh,
		material
		);

	for (int i = 0; i < joints.size(); i++) { model->AddJoint(joints[i]); }

	// Set the value and return
	*m_model = model;

	if (loadedSuccessfully)
		return true;
	return false;
}