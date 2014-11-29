#pragma once
#include "IMeshObject.h"
#include "IMaterial.h"

namespace Anarian {
	class ResourceManager {
	private:
		//static ResourceManager* m_instance;

		std::unordered_map<std::string, IMaterial*> m_materials;
		std::unordered_map<std::string, IMeshObject*> m_meshes;

#ifdef Anarian_DirectX_Mode
#endif
	public:
		//static ResourceManager* Instance();
		ResourceManager();
		~ResourceManager();

		void AddMaterial(std::string key, IMaterial* material);
		IMaterial* GetMaterial(std::string key);

		void AddMesh(std::string key, IMeshObject* mesh);
		IMeshObject* GetMesh(std::string key);


#ifdef Anarian_DirectX_Mode
#endif
	};
}                                                                                                                                                                                                                                                                                                                                                                                                   