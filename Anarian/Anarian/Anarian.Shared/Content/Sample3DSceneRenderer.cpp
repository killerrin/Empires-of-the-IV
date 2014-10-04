#include "pch.h"
#include "Sample3DSceneRenderer.h"

#include "..\Common\DirectXHelper.h"
#include "..\Color.h"
#include "..\PNTVertex.h"
#include "..\DirectXMesh.h"

using namespace Anarian;
using namespace DirectX;
using namespace Windows::Foundation;

// Loads vertex and pixel shaders from files and instantiates the cube geometry.
Sample3DSceneRenderer::Sample3DSceneRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
	IRenderer(Color::CornFlowerBlue()),

	m_loadingComplete(false),
	m_degreesPerSecond(45),
	m_indexCount(0),
	m_tracking(false),
	m_deviceResources(deviceResources)
{
	CreateDeviceDependentResources();
	CreateWindowSizeDependentResources();
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
void Sample3DSceneRenderer::Update(DX::StepTimer const& timer)
{
	if (!m_tracking)
	{
		// Convert degrees to radians, then convert seconds to rotation angle
		float radiansPerSecond = XMConvertToRadians(m_degreesPerSecond);
		double totalRotation = timer.GetTotalSeconds() * radiansPerSecond;
		float radians = static_cast<float>(fmod(totalRotation, XM_2PI));

		Rotate(radians);
	}
}

// Rotate the 3D cube model a set amount of radians.
void Sample3DSceneRenderer::Rotate(float radians)
{
	// Prepare to pass the updated model matrix to the shader
	XMStoreFloat4x4(&m_constantBufferData.model, XMMatrixTranspose(XMMatrixRotationY(radians)));
}

void Sample3DSceneRenderer::StartTracking()
{
	m_tracking = true;
}

// When tracking, the 3D cube can be rotated around its Y axis by tracking pointer position relative to the output screen width.
void Sample3DSceneRenderer::TrackingUpdate(float positionX)
{
	if (m_tracking)
	{
		float radians = XM_2PI * 2.0f * positionX / m_deviceResources->GetOutputSize().Width;
		Rotate(radians);
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


	// Prepare the constant buffer to send it to the graphics device.
	context->UpdateSubresource(
		m_constantBuffer.Get(),
		0,
		NULL,
		&m_constantBufferData,
		0,
		0
		);

	//// Each vertex is one instance of the Anarian::Verticies::PNTVertex struct.
	//UINT stride = sizeof(Anarian::Verticies::PNTVertex);
	//UINT offset = 0;
	//context->IASetVertexBuffers(
	//	0,
	//	1,
	//	((DirectXMesh*)mesh)->VertexBuffer().GetAddressOf(),//m_vertexBuffer.GetAddressOf(),
	//	&stride,
	//	&offset
	//	);
	//
	//context->IASetIndexBuffer(
	//	((DirectXMesh*)mesh)->IndexBuffer().Get(), //m_indexBuffer.Get(),
	//	DXGI_FORMAT_R16_UINT, // Each index is one 16-bit unsigned integer (short).
	//	0
	//	);
	//
	//context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

	context->IASetInputLayout(m_inputLayout.Get());

	// Attach our vertex shader.
	context->VSSetShader(
		m_vertexShader.Get(),
		nullptr,
		0
		);

	// Send the constant buffer to the graphics device.
	context->VSSetConstantBuffers(
		0,
		1,
		m_constantBuffer.GetAddressOf()
		);

	// Attach our pixel shader.
	context->PSSetShader(
		m_pixelShader.Get(),
		nullptr,
		0
		);

	//// Draw the objects.
	//context->DrawIndexed(
	//	mesh->IndexCount(),//m_indexCount,
	//	0,
	//	0
	//	);

	((DirectXMesh*)mesh)->Render(context);
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
	});

	// Once both shaders are loaded, create the mesh.
	auto createCubeTask = (createPSTask && createVSTask).then([this] () {
		mesh = MeshFactory::Instance()->ConstructCube();
		((DirectXMesh*)mesh)->CreateBuffers(m_deviceResources->GetD3DDevice());

		// Load mesh vertices. Each vertex has a position and a color.
		std::vector<Anarian::Verticies::PNTVertex> cubeVertices = std::vector<Anarian::Verticies::PNTVertex>();
			//						 Position							Normal							Texture Coordinates
			cubeVertices.push_back({ XMFLOAT3(-0.5f, -0.5f, -0.5f),		XMFLOAT3(0.0f, 0.0f, 0.0f),		XMFLOAT2(1.0f, 0.0f) });
			cubeVertices.push_back({ XMFLOAT3(-0.5f, -0.5f, 0.5f),		XMFLOAT3(0.0f, 0.0f, 1.0f),		XMFLOAT2(1.0f, 0.0f) });
			cubeVertices.push_back({ XMFLOAT3(-0.5f, 0.5f, -0.5f),		XMFLOAT3(0.0f, 1.0f, 0.0f),		XMFLOAT2(1.0f, 0.0f) });
			cubeVertices.push_back({ XMFLOAT3(-0.5f, 0.5f, 0.5f),		XMFLOAT3(0.0f, 1.0f, 1.0f),		XMFLOAT2(1.0f, 0.0f) });
			cubeVertices.push_back({ XMFLOAT3(0.5f, -0.5f, -0.5f),		XMFLOAT3(1.0f, 0.0f, 0.0f),		XMFLOAT2(1.0f, 0.0f) });
			cubeVertices.push_back({ XMFLOAT3(0.5f, -0.5f, 0.5f),		XMFLOAT3(1.0f, 0.0f, 1.0f),		XMFLOAT2(1.0f, 0.0f) });
			cubeVertices.push_back({ XMFLOAT3(0.5f, 0.5f, -0.5f),		XMFLOAT3(1.0f, 1.0f, 0.0f),		XMFLOAT2(1.0f, 0.0f) });
			cubeVertices.push_back({ XMFLOAT3(0.5f, 0.5f, 0.5f),		XMFLOAT3(1.0f, 1.0f, 1.0f),		XMFLOAT2(1.0f, 0.0f) });

		D3D11_SUBRESOURCE_DATA vertexBufferData = { 0 };
		vertexBufferData.pSysMem = &cubeVertices[0];
		vertexBufferData.SysMemPitch = 0;
		vertexBufferData.SysMemSlicePitch = 0;
		CD3D11_BUFFER_DESC vertexBufferDesc(sizeof(Anarian::Verticies::PNTVertex) * cubeVertices.size(), D3D11_BIND_VERTEX_BUFFER);
		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateBuffer(
			&vertexBufferDesc,
			&vertexBufferData,
			&m_vertexBuffer
			)
			);

		std::vector<short> cubeIndices = std::vector<short>();
			cubeIndices.push_back(0);	 cubeIndices.push_back(2);	 cubeIndices.push_back(1); // -x
			cubeIndices.push_back(1);	 cubeIndices.push_back(2);	 cubeIndices.push_back(3);
			cubeIndices.push_back(4);	 cubeIndices.push_back(5);	 cubeIndices.push_back(6); // +x
			cubeIndices.push_back(5);	 cubeIndices.push_back(7);	 cubeIndices.push_back(6);
			cubeIndices.push_back(0);	 cubeIndices.push_back(1);	 cubeIndices.push_back(5); // -y
			cubeIndices.push_back(0);	 cubeIndices.push_back(5);	 cubeIndices.push_back(4);
			cubeIndices.push_back(2);	 cubeIndices.push_back(6);	 cubeIndices.push_back(7); // +y
			cubeIndices.push_back(2);	 cubeIndices.push_back(7);	 cubeIndices.push_back(3);
			cubeIndices.push_back(0);	 cubeIndices.push_back(4);	 cubeIndices.push_back(6); // -z
			cubeIndices.push_back(0);	 cubeIndices.push_back(6);	 cubeIndices.push_back(2);
			cubeIndices.push_back(1);	 cubeIndices.push_back(3);	 cubeIndices.push_back(7); // +z
			cubeIndices.push_back(1);	 cubeIndices.push_back(7);	 cubeIndices.push_back(5);

		m_indexCount = cubeIndices.size();

		D3D11_SUBRESOURCE_DATA indexBufferData = { 0 };
		indexBufferData.pSysMem = &cubeIndices[0];
		indexBufferData.SysMemPitch = 0;
		indexBufferData.SysMemSlicePitch = 0;
		CD3D11_BUFFER_DESC indexBufferDesc(sizeof(short) * cubeIndices.size(), D3D11_BIND_INDEX_BUFFER);
		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateBuffer(
				&indexBufferDesc,
				&indexBufferData,
				&m_indexBuffer
				)
			);
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
	m_constantBuffer.Reset();
	m_vertexBuffer.Reset();
	m_indexBuffer.Reset();
}