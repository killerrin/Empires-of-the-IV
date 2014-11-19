#include "pch.h"

#include "PNTVertex.h"
#include "Weight.h"
#include "Joint.h"
#include "ModelAnimation.h"

#include "IMeshObject.h"
#include "IMaterial.h"
#include "DirectXMesh.h"
#include "DirectXMaterial.h"
#include "Model.h"

#include "Common\BasicLoader.h"

#include "ModelRawDataType.h"

#include "MD5LoaderConverter.h"
#include "MD5Loader.h"

using namespace Anarian;
using namespace Verticies;
using namespace DirectX;

bool MD5LoaderConverter::LoadMD5MeshAnim(
	_In_ std::string filename,
	_Out_ Anarian::Model** m_model,
	_Out_ Anarian::Verticies::ModelAnimation** m_anim,
	_In_ BasicLoader^ basicLoader
	)
{
	std::string meshName = filename + ".md5mesh";
	std::string animName = filename + ".md5anim";

	Model* loadModel = nullptr;
	Anarian::Verticies::ModelAnimation* loadAnim = nullptr;

	// Load the Mesh
	if (MD5LoaderConverter::LoadMD5Mesh(meshName, &loadModel, basicLoader)) {
		{
			std::string str = "MD5 Model Successfully Loaded: " + meshName + " \n";
			std::wstring wstr(str.begin(), str.end());
			OutputDebugString(wstr.c_str());
		}

		// Then load the Animation
		if (MD5LoaderConverter::LoadMD5Animation(animName, &loadAnim, basicLoader, loadModel->GetRawData())) {
			{
				std::string str = "MD5 Animation Successfully Loaded: " + animName + "  \n";
				std::wstring wstr(str.begin(), str.end());
				OutputDebugString(wstr.c_str());
			}

			// Then Set the values and return
			*m_model = loadModel;
			*m_anim = loadAnim;

			return true;
		}
	}

	return false;
}

bool MD5LoaderConverter::LoadMD5Mesh(
	_In_ std::string filename,
	_Out_ Anarian::Model** m_model,
	_In_ BasicLoader^ basicLoader)
{
	std::wstring wfilename(filename.begin(), filename.end());
	MD5Loader::Model3D* loadedModel3DRaw = new MD5Loader::Model3D();
	bool loadedSuccessfully = MD5Loader::LoadMD5Model(wfilename, *loadedModel3DRaw);
	
	if (loadedSuccessfully) {
		IMeshObject* mesh = MeshFactory::Instance()->ConstructEmpty();
		IMaterial* material = MaterialFactory::Instance()->ConstructEmpty();

		std::vector<Joint> joints = std::vector<Joint>();
		std::vector<Weight> weights = std::vector<Weight>();

		MD5Loader::Model3D loadedModel3D = *loadedModel3DRaw;

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
			
			// Weights
			{
				std::string str = "WeightCount: " + std::to_string(loadedModel3D.subsets[i].weights.size()) + " \n";
				std::wstring wstr(str.begin(), str.end());
				OutputDebugString(wstr.c_str());
			}
			
			for (int w = 0; w < loadedModel3D.subsets[i].weights.size(); w++) {
				Weight weight = Weight();
				weight.Bias =		loadedModel3D.subsets[i].weights[w].bias;
				weight.JointID =	loadedModel3D.subsets[i].weights[w].jointID;
				weight.Position =	loadedModel3D.subsets[i].weights[w].pos;
				weight.Normal =		loadedModel3D.subsets[i].weights[w].normal;

				weights.push_back(weight);
			}


			// Joints
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
		for (int i = 0; i < weights.size(); i++) { model->AddWeight(weights[i]); }

		model->SetRawData(loadedModel3DRaw, Anarian::ModelRawDataType::MD5);

		// Set the value and return
		*m_model = model;

		return true;
	}
	
	// If it wasn't successful, Set it to a blank object and return false
	*m_model = new Model();
	return false;
}

bool MD5LoaderConverter::LoadMD5Animation(
	_In_ std::string filename,
	_Out_ Anarian::Verticies::ModelAnimation** m_anim,
	_In_ BasicLoader^ basicLoader,
	_In_ void* loadedModel3DRaw
	)
{
	std::wstring wfilename(filename.begin(), filename.end());
	MD5Loader::Model3D loadedModel3D = *((MD5Loader::Model3D*)loadedModel3DRaw);
	bool loadedSuccessfully = MD5Loader::LoadMD5Anim(wfilename, loadedModel3D);

	if (loadedSuccessfully) {
		Anarian::Verticies::ModelAnimation* tempAnim = new Anarian::Verticies::ModelAnimation();

		// Convert the Objects
		for (int i = 0; i < loadedModel3D.animations.size(); i++) {
			tempAnim->numFrames = loadedModel3D.animations[i].numFrames;
			tempAnim->numJoints = loadedModel3D.animations[i].numJoints;;
			tempAnim->frameRate = loadedModel3D.animations[i].frameRate;;
			tempAnim->numAnimatedComponents = loadedModel3D.animations[i].numAnimatedComponents;;

			tempAnim->frameTime = loadedModel3D.animations[i].frameTime;
			tempAnim->totalAnimTime = loadedModel3D.animations[i].totalAnimTime;
			tempAnim->currAnimTime = loadedModel3D.animations[i].currAnimTime;

			for (int x = 0; x < loadedModel3D.animations[i].jointInfo.size(); x++) {
				AnimJointInfo tmp;
				tmp.name = loadedModel3D.animations[i].jointInfo[x].name;
				tmp.flags = loadedModel3D.animations[i].jointInfo[x].flags;
				tmp.parentID = loadedModel3D.animations[i].jointInfo[x].parentID;
				tmp.startIndex = loadedModel3D.animations[i].jointInfo[x].startIndex;

				tempAnim->jointInfo.push_back(tmp);
			}

			for (int x = 0; x < loadedModel3D.animations[i].frameBounds.size(); x++) {
				BoundingBox tmp;
				tmp.min = loadedModel3D.animations[i].frameBounds[x].min;
				tmp.max = loadedModel3D.animations[i].frameBounds[x].max;

				tempAnim->frameBounds.push_back(tmp);
			}

			for (int x = 0; x < loadedModel3D.animations[i].baseFrameJoints.size(); x++) {
				Joint tmp;
				tmp.Name = loadedModel3D.animations[i].baseFrameJoints[x].name;
				tmp.Orientation = loadedModel3D.animations[i].baseFrameJoints[x].orientation;
				tmp.ParentID = loadedModel3D.animations[i].baseFrameJoints[x].parentID;
				tmp.Position = loadedModel3D.animations[i].baseFrameJoints[x].pos;

				tempAnim->baseFrameJoints.push_back(tmp);
			}

			for (int x = 0; x < loadedModel3D.animations[i].frameData.size(); x++) {
				FrameData tmp;
				tmp.frameData = loadedModel3D.animations[i].frameData[x].frameData;
				tmp.frameID = loadedModel3D.animations[i].frameData[x].frameID;

				tempAnim->frameData.push_back(tmp);
			}

			for (int x = 0; x < loadedModel3D.animations[i].frameSkeleton.size(); x++) {
				std::vector<Joint> tmpV;
				for (int y = 0; y < loadedModel3D.animations[i].frameSkeleton[x].size(); y++) {
					Joint tmp;
					tmp.Name =			loadedModel3D.animations[i].frameSkeleton[x][y].name;
					tmp.Orientation =	loadedModel3D.animations[i].frameSkeleton[x][y].orientation;
					tmp.ParentID =		loadedModel3D.animations[i].frameSkeleton[x][y].parentID;
					tmp.Position =		loadedModel3D.animations[i].frameSkeleton[x][y].pos;

					tmpV.push_back(tmp);
				}

				tempAnim->frameSkeleton.push_back(tmpV);
			}
		}

		{
			//std::string str = "z2: " + std::to_string(tempAnim->frameSkeleton[0].size()) + " \n";
			//std::wstring wstr(str.begin(), str.end());
			//OutputDebugString(wstr.c_str());
		}

		// Set the value and return
		*m_anim = tempAnim;

		return true;
	}

	// If it wasn't successful, set to an empty ModelAnimation and return false
	*m_anim = new ModelAnimation();
	return false;
}