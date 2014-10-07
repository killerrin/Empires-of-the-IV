#include "pch.h"
#include "..\SceneManager.h"
#include "Sample3DSceneRenderer.h"

#include "..\Common\DirectXHelper.h"
#include "..\Common\BasicLoader.h"
#include "..\WICTextureLoader.h"

#include "..\Color.h"
#include "..\PNTVertex.h"

#include "..\DirectXMesh.h"
#include "..\DirectXMaterial.h"

#include "..\ConstantBuffers.h"

using namespace Anarian;
using namespace DirectX;
using namespace Windows::Foundation;

// Loads vertex and pixel shaders from files and instantiates the cube geometry.
Sample3DSceneRenderer::Sample3DSceneRenderer(Color color) :
	IRenderer(color),

	m_loadingComplete(false),
	m_degreesPerSecond(45),
	m_tracking(false)
{

}
void Sample3DSceneRenderer::Initialize(const std::shared_ptr<DX::DeviceResources>& deviceResources, SceneManager* sceneManager)
{
	m_deviceResources = deviceResources;
	m_sceneManager = sceneManager;

	CreateDeviceDependentResources();
	CreateWindowSizeDependentResources();
}
void Sample3DSceneRenderer::SetSceneManager(SceneManager* sceneManager)
{
	m_sceneManager = sceneManager;
}

// Initializes view parameters when the window size changes.
void Sample3DSceneRenderer::CreateWindowSizeDependentResources()
{
	Size outputSize = m_deviceResources->GetOutputSize();
	float aspectRatio = outputSize.Width / outputSize.Height;
	float fovAngleY = 70.0f * XM_PI / 180.0f;

	// This is a simple example of change that can be made when the app is in
	// portrait or snapped view.
	if (aspectRatio < 1.0f)
	{
		fovAngleY *= 2.0f;
	}

	// Note that the OrientationTransform3D matrix is post-multiplied here
	// in order to correctly orient the scene to match the display orientation.
	// This post-multiplication step is required for any draw calls that are
	// made to the swap chain render target. For draw calls to other targets,
	// this transform should not be applied.

	// This sample makes use of a right-handed coordinate system using row-major matrices.
	XMMATRIX perspectiveMatrix = XMMatrixPerspectiveFovRH(
		fovAngleY,
		aspectRatio,
		0.01f,
		100.0f
		);

	XMFLOAT4X4 orientation = m_deviceResources->GetOrientationTransform3D();

	XMMATRIX orientationMatrix = XMLoadFloat4x4(&orientation);

	XMStoreFloat4x4(
		&m_constantBufferData.projection,
		XMMatrixTranspose(perspectiveMatrix * orientationMatrix)
		);

	// Eye is at (0,0.7,1.5), looking at point (0,-0.1,0) with the up-vector along the y-axis.
	static const XMVECTORF32 eye = { 0.0f, 0.7f, 1.5f, 0.0f };
	static const XMVECTORF32 at = { 0.0f, -0.1f, 0.0f, 0.0f };
	static const XMVECTORF32 up = { 0.0f, 1.0f, 0.0f, 0.0f };

	XMStoreFloat4x4(&m_constantBufferData.view, XMMatrixTranspose(XMMatrixLookAtRH(eye, at, up)));
}

// Called once per frame, rotates the cube and calculates the model and view matrices.
void Sample3DSceneRenderer::Update(DX::StepTimer const& timer, GameTimer* gameTime)
{
	if (!m_tracking)
	{
		if (gameObject == nullptr) return;

		// Convert degrees to radians, then convert seconds to rotation angle
		float radiansPerSecond = XMConvertToRadians(m_degreesPerSecond);
		double totalRotation = timer.GetTotalSeconds() * radiansPerSecond;

		float radians = static_cast<float>(fmod(totalRotation, XM_2PI));

		Rotate(radians, radians);

		gameObject->Update(gameTime);
	}
}

// Rotate the 3D cube model a set amount of radians.
void Sample3DSceneRenderer::Rotate(float radiansX, float radiansY)
{
	if (gameObject == nullptr) return;
	XMFLOAT3 rotation(radiansX, radiansY, 0.0f);
	gameObject->Rotation(rotation);

	// Prepare to pass the updated model matrix to the shader
	XMStoreFloat4x4(&m_constantBufferData.model,
		XMMatrixTranspose(XMMatrixRotationX(radiansX) * XMMatrixRotationY(radiansY))
		);
}

void Sample3DSceneRenderer::StartTracking()
{
	m_tracking = true;
}

// When tracking, the 3D cube can be rotated around its Y axis by tracking pointer position relative to the output screen width.
void Sample3DSceneRenderer::TrackingUpdate(float positionX, float positionY)
{
	if (m_tracking)
	{
		float radiansX = XM_2PI * 2.0f * positionX / m_deviceResources->GetOutputSize().Width;
		float radiansY = XM_2PI * 2.0f * positionY / m_deviceResources->GetOutputSize().Height;
		Rotate(radiansX, radiansY);
	}
}

void Sample3DSceneRenderer::StopTracking()
{
	m_tracking = false;
}

// Renders one frame using the vertex and pixel shaders.
void Sample3DSceneRenderer::Render()
{
	// Loading is asynchronous. Only draw geometry after it's loaded.
	if (!m_loadingComplete)
	{
		return;
	}

	IRenderer::Render();

	auto context = m_deviceResources->GetD3DDeviceContext();

	// Reset the viewport to target the whole screen.
	auto viewport = m_deviceResources->GetScreenViewport();
	context->RSSetViewports(1, &viewport);

	// Reset render targets to the screen.
	ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetView() };
	context->OMSetRenderTargets(1, targets, m_deviceResources->GetDepthStencilView());

	// Clear the back buffer and depth stencil view.
	context->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetView(), m_backgroundColor[0]);
	context->ClearDepthStencilView(m_deviceResources->GetDepthStencilView(), D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);

	// Set the SamplerState
	context->PSSetSamplers(0, 1, m_samplerState.GetAddressOf());

	// Prepare the constant buffer to send it to the graphics device.
	context->UpdateSubresource(
		m_constantBuffer.Get(),
		0,
		NULL,
		&m_constantBufferData,
		0,
		0
		);

	context->IASetInputLayout(m_inputLayout.Get());

	// Send the constant buffer to the graphics device.
	context->VSSetConstantBuffers(
		0,
		1,
		m_constantBuffer.GetAddressOf()
		);

	context->VSSetConstantBuffers(
		1,
		1,
		m_constantBufferChangesEveryPrim.GetAddressOf()
		);

	// Set the Rasterizer State
	context->RSSetState(m_defaultRasterizerState.Get());

	// Draw the Mesh
	gameObject->Render(context, m_constantBufferChangesEveryPrim.Get());
}

void Sample3DSceneRenderer::CreateDeviceDependentResources()
{
	// Load shaders asynchronously.
	auto loadVSTask = DX::ReadDataAsync(L"SampleVertexShader.cso");
	auto loadPSTask = DX::ReadDataAsync(L"SamplePixelShader.cso");

	// After the vertex shader file is loaded, create the shader and input layout.
	auto createVSTask = loadVSTask.then([this](const std::vector<byte>& fileData) {
		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateVertexShader(
				&fileData[0],
				fileData.size(),
				nullptr,
				&m_vertexShader
				)
			);

		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateInputLayout(
				Anarian::Verticies::PNTVertexLayout,
				ARRAYSIZE(Anarian::Verticies::PNTVertexLayout),
				&fileData[0],
				fileData.size(),
				&m_inputLayout
				)
			);
	});

	// After the pixel shader file is loaded, create the shader and constant buffer.
	auto createPSTask = loadPSTask.then([this](const std::vector<byte>& fileData) {
		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreatePixelShader(
				&fileData[0],
				fileData.size(),
				nullptr,
				&m_pixelShader
				)
			);

		CD3D11_BUFFER_DESC constantBufferDesc(sizeof(ModelViewProjectionConstantBuffer) , D3D11_BIND_CONSTANT_BUFFER);
		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateBuffer(
				&constantBufferDesc,
				nullptr,
				&m_constantBuffer
				)
			);

		CD3D11_BUFFER_DESC constantBufferCEPDesc((sizeof(ConstantBufferChangesEveryPrim) + 15) / 16 * 16,
												 D3D11_BIND_CONSTANT_BUFFER);
		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateBuffer(
			&constantBufferCEPDesc,
			nullptr,
			&m_constantBufferChangesEveryPrim
			)
			);
	});

	auto createStatesTask = (createPSTask && createVSTask).then([this]() {
		// Filter sampler
		D3D11_SAMPLER_DESC sd;
		sd.Filter = D3D11_FILTER_ANISOTROPIC;
		sd.MaxAnisotropy = 16;							// use Anisotropic x 8. Available values = 1-16
		sd.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;		// horizontally the texture is repeated
		sd.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;		// vertically the texture is mirrored
		sd.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;		// if it's a 3D texture, it is clamped
		sd.BorderColor[0] = 0.0f;
		sd.BorderColor[1] = 0.0f;
		sd.BorderColor[2] = 0.0f;
		sd.BorderColor[3] = 0.0f;
		sd.MinLOD = 0.0f;
		sd.MaxLOD = FLT_MAX;
		sd.MipLODBias = 0.0f;							// decrease mip level of detail by 2

		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateSamplerState(&sd, &m_samplerState)
			);

		// Rasterizer State
		D3D11_RASTERIZER_DESC rd;
		rd.FillMode = D3D11_FILL_SOLID;
		rd.CullMode = D3D11_CULL_FRONT;
		rd.FrontCounterClockwise = FALSE;
		rd.DepthClipEnable = TRUE;
		rd.ScissorEnable = FALSE;
		rd.AntialiasedLineEnable = FALSE;
		rd.MultisampleEnable = FALSE;
		rd.DepthBias = 0;
		rd.DepthBiasClamp = 0.0f;
		rd.SlopeScaledDepthBias = 0.0f;

		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateRasterizerState(&rd, &m_defaultRasterizerState)
			);
	});

	// Once both shaders are loaded, create the cube
	auto createCubeTask = (createStatesTask).then([this]() {
		// Create the Mesh
		mesh = MeshFactory::Instance()->ConstructCube();
		((DirectXMesh*)mesh)->CreateBuffers(m_deviceResources->GetD3DDevice());

		// Create Material
		material = MaterialFactory::Instance()->ConstructMaterial(
														Color(0.5f, 1.0f, 0.4f, 0.5f),
														Color(0.0f, 1.0, 0.5f, 0.5f),
														Color(0.5f, 0.5f, 0.5f, 0.5f),
														1.0f);

		// Start the async tasks to load the shaders and textures.
		//BasicLoader^ loader = ref new BasicLoader(m_deviceResources->GetD3DDevice());
		//loader->LoadTextureAsync("Assets\\TyrilMap.png", nullptr, &m_tyrilMap);
		// load the texture
		HRESULT hr = CreateWICTextureFromFile(
			m_deviceResources->GetD3DDevice(),
			nullptr,
			L"Assets\\TyrilMap.png",
			nullptr,
			&m_tyrilMap,
			0);

		((DirectXMaterial*)material)->CreateViews(m_tyrilMap.Get(), m_vertexShader.Get(), m_pixelShader.Get());
	
		// Create the Game Object
		gameObject = new GameObject();
		gameObject->SetMaterial(material);
		gameObject->SetMesh(mesh);

		GameObject g2 = GameObject();
		g2.SetMaterial(material);
		g2.SetMesh(mesh);
		g2.Position(DirectX::XMFLOAT3(0.5f, 0.5f, 0.0f));
		g2.Scale(DirectX::XMFLOAT3(0.5f, 0.5f, 0.5f));

		gameObject->AddChild(g2);
	});

	// Once the cube is loaded, the object is ready to be rendered.
	createCubeTask.then([this] () {
		m_loadingComplete = true;
	});
}

void Sample3DSceneRenderer::ReleaseDeviceDependentResources()
{
	m_loadingComplete = false;

	m_vertexShader.Reset();
	m_inputLayout.Reset();
	m_pixelShader.Reset();

	m_samplerState.Reset();
	m_defaultRasterizerState.Reset();

	m_constantBuffer.Reset();
	m_constantBufferChangesEveryPrim.Reset();
}