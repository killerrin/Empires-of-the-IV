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
		IRenderer* ConstructRenderer(SceneManager* sceneManager, Color backgroundColor);
	};
}