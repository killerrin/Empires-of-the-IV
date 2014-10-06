#pragma once
#include "Camera.h"
#include "SceneNode.h"

namespace Anarian {
	class IScene
	{
	protected:
		IScene() { 
		
		};

		Camera	  m_camera;
		SceneNode m_sceneNode;

	public:
		virtual ~IScene() {};

		Camera*		GetCamera() { return &m_camera; };
		void		SetCamera(Camera camera) { m_camera = camera; };

		SceneNode*	GetSceneNode() { return &m_sceneNode; };
		void		SetSceneNode(SceneNode sceneNode) { m_sceneNode = sceneNode; };
	};
}