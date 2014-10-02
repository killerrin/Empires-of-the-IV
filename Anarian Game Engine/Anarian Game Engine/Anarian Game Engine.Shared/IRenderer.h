#pragma once
#include "SceneManager.h"
#include "Color.h"

namespace Anarian {
	class IRenderer
	{
	protected:
		SceneManager* m_sceneManager;
		Color m_backgroundColor;

	public:
		IRenderer();
		virtual ~IRenderer();

		virtual void Render();
		void SetBackgroundColor(Color color);
	};
}