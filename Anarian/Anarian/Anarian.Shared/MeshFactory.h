#pragma once
#include "IMeshObject.h"

namespace Anarian {
	class MeshFactory
	{
		MeshFactory();
	private:
		static MeshFactory* m_instance;
	public:
		static MeshFactory* Instance();
		~MeshFactory();

		// Constructions
		IMeshObject* ConstructCube();
	};
}