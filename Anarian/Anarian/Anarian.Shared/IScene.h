#pragma once
#include "Camera.h"
#include "GameObject.h"

namespace Anarian {
	class IScene
	{
	protected:


		Camera		m_camera;
		GameObject* m_sceneNode;

	public:
		IScene() {

		};

		virtual ~IScene() { delete m_sceneNode; };

		Camera*		GetCamera() { return &m_camera; };
		void		SetCamera(Camera camera) { m_camera = camera; };

		GameObject*	GetSceneNode() { return m_sceneNode; };
		void		SetSceneNode(GameObject* sceneNode) { m_sceneNode = sceneNode; };
	};
}