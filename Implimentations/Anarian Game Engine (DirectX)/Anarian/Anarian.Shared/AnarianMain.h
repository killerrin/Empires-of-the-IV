#pragma once

#include "ResourceManager.h"
#include "SceneManager.h"
#include "GameTimer.h"

#include "Common\StepTimer.h"
#include "Common\DeviceResources.h"
#include "Content\DirectXRenderer.h"
#include "Content\SampleFpsTextRenderer.h"

// Renders Direct2D and 3D content on the screen.
namespace Anarian
{
	class AnarianMain : public DX::IDeviceNotify
	{
	public:
		AnarianMain(const std::shared_ptr<DX::DeviceResources>& deviceResources);
		~AnarianMain();
		void CreateWindowSizeDependentResources();
		void StartTracking() { ((DirectXRenderer*)m_sceneRenderer)->StartTracking(); }
		void TrackingUpdate(float positionX, float positionY) { m_pointerLocationX = positionX; m_pointerLocationY = positionY; }
		void StopTracking() { ((DirectXRenderer*)m_sceneRenderer)->StopTracking(); }

		bool IsTracking() { return ((DirectXRenderer*)m_sceneRenderer)->IsTracking(); }
		void StartRenderLoop();
		void StopRenderLoop();
		Concurrency::critical_section& GetCriticalSection() { return m_criticalSection; }

		// IDeviceNotify
		virtual void OnDeviceLost();
		virtual void OnDeviceRestored();

	private:
		void ProcessInput();
		void Update();
		bool Render();

		// Cached pointer to device resources.
		std::shared_ptr<DX::DeviceResources> m_deviceResources;
		
		// Managers
		std::shared_ptr<ResourceManager> m_resourceManager;
		std::shared_ptr<SceneManager> m_sceneManager;

		// TODO: Replace with your own content renderers.
		IRenderer* m_sceneRenderer;
		std::unique_ptr<SampleFpsTextRenderer> m_fpsTextRenderer;

		Windows::Foundation::IAsyncAction^ m_renderLoopWorker;
		Concurrency::critical_section m_criticalSection;

		// Rendering loop timer.
		DX::StepTimer m_timer;
		GameTimer m_gameTime;

		// Track current input pointer position.
		float m_pointerLocationX, m_pointerLocationY;
	};
}