#include "pch.h"

#if Anarian_DirectX_Mode
#include "DirectXMaterial.h"
using namespace DirectX;
#endif

#include "MaterialFactory.h"
using namespace Anarian;

MaterialFactory* MaterialFactory::m_instance;
MaterialFactory* MaterialFactory::Instance()
{
	if (m_instance == nullptr) m_instance = new MaterialFactory();
	return m_instance;
}

MaterialFactory::MaterialFactory()
{
}


MaterialFactory::~MaterialFactory()
{
	//delete m_instance;
}

IMaterial* MaterialFactory::ConstructEmpty()
{
	IMaterial* material;

	// Construct the Material
#if Anarian_DirectX_Mode
	material = new DirectXMaterial();
#endif

	return material;
}

IMaterial* MaterialFactory::ConstructMaterial(
	Color meshColor,
	Color diffuseColor,
	Color specularColor,
	float specularExponent)
{
	IMaterial* material = ConstructEmpty();

	// Assign the cross platform values
	material->m_meshColor = meshColor;
	material->m_diffuseColor = diffuseColor;
	material->m_specularColor = specularColor;
	material->m_specularExponent = specularExponent;

	return material;
}

