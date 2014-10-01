#pragma once
#include "Camera.h"
#include "SceneNode.h"

namespace Anarian {
	class IScene
	{
	private:
		Camera	  m_camera;
		SceneNode m_sceneNode;
	public:
		IScene();
		virtual ~IScene();

		Camera* GetCamera();
		SceneNode* GetSceneNode();
	};
}