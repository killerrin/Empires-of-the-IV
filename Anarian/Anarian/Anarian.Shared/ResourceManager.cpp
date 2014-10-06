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
	m_materials = std::unordered_map<std::string, IMaterial>();
	m_meshes = std::unordered_map<std::string, IMeshObject>();
}


ResourceManager::~ResourceManager()
{
	m_meshes.clear();
	m_materials.clear();
}


void ResourceManager::AddMaterial(std::string key, IMaterial material) 
{
	m_materials.emplace(key, material);
}
IMaterial* ResourceManager::GetMaterial(std::string key)
{
	return &m_materials.at(key);
}

void ResourceManager::AddMesh(std::string key, IMeshObject mesh)
{
	m_meshes.emplace(key, mesh);
}
IMeshObject* ResourceManager::GetMesh(std::string key)
{
	return &m_meshes.at(key);
}