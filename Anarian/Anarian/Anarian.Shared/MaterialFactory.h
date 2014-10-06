#pragma once
#include "IMaterial.h"

namespace Anarian {
	class MaterialFactory
	{
	private:
		static MaterialFactory* m_instance;
		MaterialFactory();
	public:
		static MaterialFactory* Instance();
		~MaterialFactory();

		// Constructions
		IMaterial* ConstructMaterial(
			Color meshColor,
			Color diffuseColor,
			Color specularColor,
			float specularExponent);
	};
}