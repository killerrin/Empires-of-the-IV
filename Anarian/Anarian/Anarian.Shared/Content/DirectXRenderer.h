#pragma once
#define FirstAvailableConstantBufferIndexSlot 3;


#include "..\Common\DeviceResources.h"
#include "..\ConstantBuffers.h"
#include "..\Common\StepTimer.h"
#include "..\GameTimer.h"

#include "..\GameObject.h"
#include "..\MeshFactory.h"
#include "..\MaterialFactory.h"

#include "..\IRenderer.h"

///	ToDo:
///	Create a VertexShader class (contains ID3D11InputLayout && ID3D11VertexShader
/// Create a PixelShader class (contains ID3D11PixelShader)
/// Create a ResourceView class (contains ID3D11ShaderResourceView)


namespace Anarian
{
	// This sample renderer instantiates a basic rendering pipeline.
	class DirectXRenderer : public IRenderer
	{
		friend class RendererFactory;
	public:
		DirectXRenderer(const std::shared_ptr<SceneManager>& sceneManager, const std::shared_ptr<ResourceManager>& resourceManager, Color color);
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


		void ConvertToWorldSpace(DirectX::XMFLOAT2& pointPositionInResolution, Camera* camera, _Out_ DirectX::XMFLOAT3& position, _Out_ DirectX::XMFLOAT3& direction);
		void PickRayVector(float mouseX, float mouseY, DirectX::XMVECTOR& pickRayInWorldSpacePos, DirectX::XMVECTOR& pickRayInWorldSpaceDir);
		float Pick(DirectX::XMVECTOR pickRayInWorldSpacePos,
			DirectX::XMVECTOR pickRayInWorldSpaceDir,
			std::vector<std::vector<Anarian::Verticies::PNTVertex>>& vertPosArray,
			std::vector<std::vector<unsigned short>>& indexPosArray,
			DirectX::XMMATRIX& worldSpace);
		bool DirectXRenderer::PointInTriangle(DirectX::XMVECTOR& triV1, DirectX::XMVECTOR& triV2, DirectX::XMVECTOR& triV3, DirectX::XMVECTOR& point);

	private:
		void Rotate(float radiansX, float radiansY);

	private:
		// Cached pointer to device resources.
		std::shared_ptr<DX::DeviceResources> m_deviceResources;

		// Direct3D resources for cube geometry.
		Microsoft::WRL::ComPtr<ID3D11RasterizerState> m_defaultRasterizerState;
		Microsoft::WRL::ComPtr<ID3D11SamplerState>	m_samplerState;

		///---- Ideally these two would get split off into a seperate class in order to allow multiple different shaders
		Microsoft::WRL::ComPtr<ID3D11InputLayout>	m_inputLayout;
		Microsoft::WRL::ComPtr<ID3D11VertexShader>	m_vertexShader;

		//----- These in their own classes as well
		Microsoft::WRL::ComPtr<ID3D11PixelShader>	m_pixelShader;
		Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_tyrilMap;

		// Setup the constant buffers
		Microsoft::WRL::ComPtr<ID3D11Buffer>		m_constantBufferChangesOnResize;
		Microsoft::WRL::ComPtr<ID3D11Buffer>		m_constantBufferChangesEveryFrame;
		Microsoft::WRL::ComPtr<ID3D11Buffer>		m_constantBufferChangesEveryPrim;

		// System resources for cube geometry.
		ConstantBufferChangesOnResize		m_constantBufferChangesOnResizeData;
		ConstantBufferChangesEveryFrame		m_constantBufferChangesEveryFrameData;

		// Variables used with the rendering loop.
		bool	m_loadingComplete;
		float	m_degreesPerSecond;
		bool	m_tracking;
		bool	m_isShoot;
	};
}

