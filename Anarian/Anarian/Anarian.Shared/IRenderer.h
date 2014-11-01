#pragma once
#include "Color.h"
#include "SceneManager.h"

namespace Anarian {
	class IRenderer
	{
		friend class RendererFactory;
	protected:
		IRenderer(SceneManager* sceneManager, ResourceManager* resourceManager, Color color) {
			m_sceneManager = sceneManager; 
			m_backgroundColor = color; };

		SceneManager* m_sceneManager;
		ResourceManager* m_resourceManager;
		Color m_backgroundColor;
	public:
		virtual ~IRenderer() { };

		virtual void PreRender() { };
		virtual void Render() { };
		virtual void PostRender() { };

		SceneManager* GetSceneManager() { return m_sceneManager; };
		void SetSceneManager(SceneManager* sceneManager) { m_sceneManager = sceneManager; };

		ResourceManager* GetResourceManager() { return m_resourceManager; };
		void SetResourceManager(ResourceManager* resourceManager) { m_resourceManager = resourceManager; };

		Color GetBackgroundColor() { return m_backgroundColor; };
		void SetBackgroundColor(Color color) { m_backgroundColor = color; };
	};
}