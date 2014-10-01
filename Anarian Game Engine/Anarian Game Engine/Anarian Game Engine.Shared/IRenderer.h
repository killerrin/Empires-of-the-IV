#pragma once
#include "SceneManager.h"

namespace Anarian {
	class IRenderer
	{
	protected:
		SceneManager* m_sceneManager;
		/*Vector4 m_backgroundColor;*/

	public:
		IRenderer();
		virtual ~IRenderer();

		void Render();
		void SetBackgroundColor(/*Vector4*/);
	};
}