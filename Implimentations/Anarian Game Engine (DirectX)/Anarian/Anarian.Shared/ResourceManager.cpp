#include "pch.h"
#include "ResourceManager.h"
using namespace Anarian;

//ResourceManager* ResourceManager::m_instance;
//ResourceManager* ResourceManager::Instance()
//{
//	if (m_instance == nullptr) m_instance = new ResourceManager();
//	return m_instance;
//}

ResourceManager::ResourceManager()
{
	m_materials = std::unordered_map<std::string, IMaterial*>();
	m_meshes = std::unordered_map<std::string, IMeshObject*>();

#ifdef Anarian_DirectX_Mode
#endif
}

ResourceManager::~ResourceManager()
{
	for (auto kv : m_meshes)
	{
		delete kv.second;
	}
	m_meshes.clear();

	for (auto kv : m_materials)
	{
		delete kv.second;
	}
	m_materials.clear();

#ifdef Anarian_DirectX_Mode

#endif
}


void ResourceManager::AddMaterial(std::string key, IMaterial* material) 
{
	m_materials.emplace(key, material);
}
IMaterial* ResourceManager::GetMaterial(std::string key)
{
	return m_materials.at(key);
}

void ResourceManager::AddMesh(std::string key, IMeshObject* mesh)
{
	m_meshes.emplace(key, mesh);
}
IMeshObject* ResourceManager::GetMesh(std::string key)
{
	return m_meshes.at(key);
}

#ifdef Anarian_DirectX_Mode

#endif