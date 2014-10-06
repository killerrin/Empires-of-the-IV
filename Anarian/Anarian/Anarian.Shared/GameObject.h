#pragma once
#include "IUpdatable.h"
#include "IRenderable.h"

namespace Anarian{
	class GameObject : IUpdatable, IRenderable
	{
	protected:
		std::vector<IMaterial*> m_materials;
		std::vector<IMeshObject*> m_meshes;

	public:
		GameObject();
		virtual ~GameObject();

		virtual void Update(GameTimer* gameTime);
		virtual void Render();
	};
}