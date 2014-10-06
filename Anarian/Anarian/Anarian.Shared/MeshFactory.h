#pragma once
#include "IMeshObject.h"

namespace Anarian {
	class MeshFactory
	{
	private:
		static MeshFactory* m_instance;
		MeshFactory();
	public:
		static MeshFactory* Instance();
		~MeshFactory();

		// Constructions
		IMeshObject* ConstructCube();
	};
}