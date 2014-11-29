#pragma once
#include "Camera.h"
#include "GameObject.h"
#include "Light.h"

namespace Anarian {
	class IScene
	{
	protected:

		Camera		m_camera;
		Light		m_globalLight;

		GameObject* m_sceneNode;

	public:
		IScene() {
			m_camera = Camera();
			m_globalLight = Light();

			m_sceneNode = new GameObject();
		};

		virtual ~IScene() { delete m_sceneNode; };

		Camera*		GetCamera() { return &m_camera; };
		void		SetCamera(Camera camera) { m_camera = camera; };

		GameObject*	GetSceneNode() { return m_sceneNode; };
		void		SetSceneNode(GameObject* sceneNode) { m_sceneNode = sceneNode; };

		Light*		GetGlobalLight() { return &m_globalLight; };
		void		SetGlobalLight(Light light) { m_globalLight = light; };
	};
}