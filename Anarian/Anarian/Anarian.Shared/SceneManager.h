#pragma once
#include "IScene.h"

namespace Anarian {
	class SceneManager
	{
		IScene* m_currentScene;
	public:
		SceneManager();
		~SceneManager();

		IScene* GetCurrentScene();
		void	SetCurrentScene(IScene* currentScene);

	};
}