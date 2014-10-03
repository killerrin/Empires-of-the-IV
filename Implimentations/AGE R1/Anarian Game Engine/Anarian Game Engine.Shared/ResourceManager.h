#pragma once
#include "IMeshObject.h"
#include "IMaterial.h"

namespace Anarian {
	class ResourceManager
	{
	public:
		ResourceManager();
		~ResourceManager();

		void AddMaterial(IMaterial material);
		void AddMesh(IMeshObject meshObject);
	};
}