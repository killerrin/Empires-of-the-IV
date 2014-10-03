#include "pch.h"
#ifdef Anarian_DirectX_Mode

#include "DirectXConstantBuffers.h"
#include "DirectXRenderer.h"
using namespace Anarian;
using namespace DirectX;

DirectXRenderer::DirectXRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources)
	:IRenderer()
{
	m_deviceResources = deviceResources;
	m_initialized = false;
	m_gameResourcesLoaded = false;

	CreateDeviceDependentResources();
	CreateWindowSizeDependentResources();
	CreateGameDeviceResources();
}


DirectXRenderer::~DirectXRenderer()
{
	IRenderer::~IRenderer();
}

void DirectXRenderer::CreateDeviceDependentResources()
{
	m_gameResourcesLoaded = false;
}

void DirectXRenderer::CreateWindowSizeDependentResources()
{
	auto d3dContext = m_deviceResources->GetD3DDeviceContext();
	auto renderTargetSize = m_deviceResources->GetRenderTargetSize();

	// Update the Projection Matrix and the associated Constant Buffer.
	Camera::MainCamera()->SetProjParams(
		XM_PI / 2, renderTargetSize.Width / renderTargetSize.Height,
		0.01f,
		100.0f
		);

	XMFLOAT4X4 orientation = m_deviceResources->GetOrientationTransform3D();

	ConstantBufferChangeOnResize changesOnResize;
	XMStoreFloat4x4(
		&changesOnResize.projection,
		XMMatrixMultiply(
		XMMatrixTranspose(Camera::MainCamera()->Projection()),
		XMMatrixTranspose(XMLoadFloat4x4(&orientation))
		)
		);

	d3dContext->UpdateSubresource(
		m_constantBufferChangeOnResize.Get(),
		0,
		nullptr,
		&changesOnResize,
		0,
		0
		);
}

void DirectXRenderer::CreateGameDeviceResources()
{
	auto d3dDevice = m_deviceResources->GetD3DDevice();

	D3D11_BUFFER_DESC bd;
	ZeroMemory(&bd, sizeof(bd));

	// Create the constant buffers.
	bd.Usage = D3D11_USAGE_DEFAULT;
	bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
	bd.CPUAccessFlags = 0;
	bd.ByteWidth = (sizeof(m_constantBufferDefault) + 12);
	DX::ThrowIfFailed(
		d3dDevice->CreateBuffer(&bd, nullptr, &m_constantBufferDefault)
		);
	bd.ByteWidth = (sizeof(ConstantBufferChangeOnResize) + 15) / 16 * 16;
	DX::ThrowIfFailed(
		d3dDevice->CreateBuffer(&bd, nullptr, &m_constantBufferChangeOnResize)
		);

	D3D11_SAMPLER_DESC sampDesc;
	ZeroMemory(&sampDesc, sizeof(sampDesc));

	sampDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
	sampDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
	sampDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
	sampDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
	sampDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
	sampDesc.MinLOD = 0;
	sampDesc.MaxLOD = FLT_MAX;
	DX::ThrowIfFailed(
		d3dDevice->CreateSamplerState(&sampDesc, &m_samplerLinear)
		);
}

//----------------------------------------------------------------------------------------------------------------\\
//----------------------------------------------------------------------------------------------------------------\\

void DirectXRenderer::Render()
{
	IRenderer::Render();

	bool stereoEnabled = m_deviceResources->GetStereoState();

	auto d3dContext = m_deviceResources->GetD3DDeviceContext();
	auto d2dContext = m_deviceResources->GetD2DDeviceContext();

	int renderingPasses = 1;
	if (stereoEnabled)
	{
		renderingPasses = 2;
	}

	for (int i = 0; i < renderingPasses; i++)
	{
		// First of all, we need to clear the background
		const float ClearColor[4] = { 0.5f, 0.5f, 0.8f, 1.0f };
		if (i > 0)
		{
			// Doing the Right Eye View.
			d3dContext->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetViewRight(), m_backgroundColor[0]);
		}
		else
		{
			// Doing the Mono or Left Eye View.
			d3dContext->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetView(), m_backgroundColor[0]);
		}


		// Iterate through the number of rendering passes to be completed.
		if (i > 0)
		{
			// Doing the Right Eye View.
			ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetViewRight() };

			d3dContext->OMSetRenderTargets(1, targets, m_deviceResources->GetDepthStencilView());
			d3dContext->ClearDepthStencilView(m_deviceResources->GetDepthStencilView(), D3D11_CLEAR_DEPTH, 1.0f, 0);
			d2dContext->SetTarget(m_deviceResources->GetD2DTargetBitmapRight());
		}
		else
		{
			// Doing the Mono or Left Eye View.
			ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetView() };

			d3dContext->OMSetRenderTargets(1, targets, m_deviceResources->GetDepthStencilView());
			d3dContext->ClearDepthStencilView(m_deviceResources->GetDepthStencilView(), D3D11_CLEAR_DEPTH, 1.0f, 0);
			d2dContext->SetTarget(m_deviceResources->GetD2DTargetBitmap());
		}

		if (m_gameResourcesLoaded)
		{
			// This section is only used after the game state has been initialized and all device
			// resources needed for the game have been created and associated with the game objects.
			if (stereoEnabled)
			{
				// When doing stereo, it is necessary to update the projection matrix once per rendering pass.
				auto orientation = m_deviceResources->GetOrientationTransform3D();

				ConstantBufferChangeOnResize changesOnResize;
				XMStoreFloat4x4(
					&changesOnResize.projection,
					XMMatrixMultiply(
					XMMatrixTranspose(
					i == 0 ?
					Camera::MainCamera()->LeftEyeProjection() :
					Camera::MainCamera()->RightEyeProjection()
					),
					XMMatrixTranspose(XMLoadFloat4x4(&orientation))
					)
					);

				d3dContext->UpdateSubresource(
					m_constantBufferChangeOnResize.Get(),
					0,
					nullptr,
					&changesOnResize,
					0,
					0
					);
			}

			// Setup the graphics pipeline. This sample uses the same InputLayout and set of
			// constant buffers for all shaders, so they only need to be set once per frame.

			//d3dContext->IASetInputLayout(m_vertexLayout.Get());
			d3dContext->VSSetConstantBuffers(0, 1, m_constantBufferDefault.GetAddressOf());
			d3dContext->VSSetConstantBuffers(1, 1, m_constantBufferChangeOnResize.GetAddressOf());

			d3dContext->PSSetSamplers(0, 1, m_samplerLinear.GetAddressOf());

			// Render Objects Here

		}
		else
		{

		}

		d2dContext->BeginDraw();

		// To handle the swapchain being pre-rotated, set the D2D transformation to include it.
		d2dContext->SetTransform(m_deviceResources->GetOrientationTransform2D());

		if (m_gameResourcesLoaded)
		{
			// This is only used after the game state has been initialized.
			//m_gameHud->Render(m_game);
		}

		//--Added this
		d2dContext->EndDraw();

		// We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
		// is lost. It will be handled during the next call to Present.
		HRESULT hr = d2dContext->EndDraw();
		if (hr != D2DERR_RECREATE_TARGET)
		{
			DX::ThrowIfFailed(hr);
		}
	}
}

#endif