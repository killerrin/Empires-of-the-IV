#pragma once
#ifdef Anarian_DirectX_Mode
#include "DeviceResources.h"
#include "IRenderer.h"

namespace Anarian {
	class DirectXRenderer :
		public IRenderer
	{
	public:
		DirectXRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
		~DirectXRenderer();

		void CreateDeviceDependentResources();
		void CreateWindowSizeDependentResources();
		void CreateGameDeviceResources();

		void Render();
	private:
		// Cached pointer to device resources.
		std::shared_ptr<DX::DeviceResources>                m_deviceResources;

		bool                                                m_initialized;
		bool                                                m_gameResourcesLoaded;

		Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBufferDefault;
		Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBufferChangeOnResize;

		Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_samplerLinear;
		Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_vertexLayout;
	};
}
#endif