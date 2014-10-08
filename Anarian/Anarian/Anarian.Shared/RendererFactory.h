#pragma once
#include "IRenderer.h"

namespace Anarian {
	class RendererFactory
	{
	private:
		static RendererFactory* m_instance;
		RendererFactory();
	public:
		static RendererFactory* Instance();
		~RendererFactory();

		// Constructions
		IRenderer* ConstructRenderer(const std::shared_ptr<SceneManager>& sceneManager, const std::shared_ptr<ResourceManager>& resourceManager, Color backgroundColor);
	};
}