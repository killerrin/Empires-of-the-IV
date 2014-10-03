#pragma once
#include "IScene.h"

namespace Anarian {
	class SceneManager
	{
	private:
		IScene* m_currentScene;

	public:
		SceneManager();
		~SceneManager();

		IScene* CurrentScene();
		void CurrentScene(IScene* scene);
	};
}