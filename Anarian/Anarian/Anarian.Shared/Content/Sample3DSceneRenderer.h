#pragma once

#include "..\Common\DeviceResources.h"
#include "ShaderStructures.h"
#include "..\Common\StepTimer.h"
#include "..\GameTimer.h"

#include "..\GameObject.h"
#include "..\MeshFactory.h"
#include "..\MaterialFactory.h"

#include "IRenderer.h"

namespace Anarian
{
	// This sample renderer instantiates a basic rendering pipeline.
	class Sample3DSceneRenderer : public IRenderer
	{
		friend class RendererFactory;
	public:
		Sample3DSceneRenderer(SceneManager* sceneManager, Color color);
		void Initialize(const std::shared_ptr<DX::DeviceResources>& deviceResources);
		void SetSceneManager(SceneManager* sceneManager);

		void CreateDeviceDependentResources();
		void CreateWindowSizeDependentResources();
		void ReleaseDeviceDependentResources();
		void Update(DX::StepTimer const& timer, GameTimer* gameTime);
		void Render();
		void StartTracking();
		void TrackingUpdate(float positionX, float positionY);
		void StopTracking();
		bool IsTracking() { return m_tracking; }


	private:
		void Rotate(float radiansX, float radiansY);

	private:
		// Cached pointer to device resources.
		std::shared_ptr<DX::DeviceResources> m_deviceResources;

		// Direct3D resources for cube geometry.
		Microsoft::WRL::ComPtr<ID3D11InputLayout>	m_inputLayout;
		Microsoft::WRL::ComPtr<ID3D11RasterizerState> m_defaultRasterizerState;

		Microsoft::WRL::ComPtr<ID3D11VertexShader>	m_vertexShader;
		Microsoft::WRL::ComPtr<ID3D11PixelShader>	m_pixelShader;

		Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_tyrilMap;
		Microsoft::WRL::ComPtr<ID3D11Buffer>		m_constantBuffer;
		Microsoft::WRL::ComPtr<ID3D11Buffer>		m_constantBufferChangesEveryPrim;


		Microsoft::WRL::ComPtr<ID3D11SamplerState>	m_samplerState;

		// Meshes and Materials
		//GameObject* gameObject;
		IMaterial* material;
		IMeshObject* mesh;

		// System resources for cube geometry.
		ModelViewProjectionConstantBuffer	m_constantBufferData;

		// Variables used with the rendering loop.
		bool	m_loadingComplete;
		float	m_degreesPerSecond;
		bool	m_tracking;
	};
}

