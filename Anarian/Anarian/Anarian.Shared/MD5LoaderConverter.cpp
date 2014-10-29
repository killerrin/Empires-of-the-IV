#include "pch.h"

#include "PNTVertex.h"

#include "IMeshObject.h"
#include "IMaterial.h"
#include "DirectXMesh.h"
#include "DirectXMaterial.h"
#include "Model.h"

#include "Common\BasicLoader.h"

#include "MD5LoaderConverter.h"
#include "MD5Loader.h"

using namespace Anarian;
using namespace DirectX;

bool MD5LoaderConverter::LoadMD5Mesh(
	_In_ std::string filename,
	_Out_ Anarian::Model** m_model,
	_In_ BasicLoader^ basicLoader)
{
	std::wstring wfilename(filename.begin(), filename.end());
	MD5Loader::Model3D model3D;
	bool loadedSuccessfully = MD5Loader::LoadMD5Model(wfilename, model3D);
	
	Model* model = new Model();

	// Set the values and return
	m_model = &model;

	if (loadedSuccessfully)
		return true;
	return false;
}