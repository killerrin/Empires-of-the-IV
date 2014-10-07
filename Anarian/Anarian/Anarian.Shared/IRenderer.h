#pragma once
#include "Color.h"
#include "SceneManager.h"

namespace Anarian {
	class IRenderer
	{
		friend class RendererFactory;
	protected:
		IRenderer(SceneManager* sceneManager, Color color) { m_sceneManager = sceneManager; m_backgroundColor = color; };

		SceneManager* m_sceneManager;
		Color m_backgroundColor;
	public:
		virtual ~IRenderer() { };
		virtual void Render() { };

		SceneManager* GetSceneManager() { return m_sceneManager; };
		void SetSceneManager(SceneManager* sceneManager) { m_sceneManager = sceneManager; };

		Color GetBackgroundColor() { return m_backgroundColor; };
		void SetBackgroundColor(Color color) { m_backgroundColor = color; };
	};
}