#include "pch.h"
#include "..\SceneManager.h"
#include "DirectXRenderer.h"

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
DirectXRenderer::DirectXRenderer(const std::shared_ptr<SceneManager>& sceneManager, const std::shared_ptr<ResourceManager>& resourceManager, Color color) :
IRenderer(sceneManager.get(), resourceManager.get(), color),

	m_loadingComplete(false),
	m_degreesPerSecond(45),
	m_tracking(false)
{

}
void DirectXRenderer::Initialize(const std::shared_ptr<DX::DeviceResources>& deviceResources)
{
	m_deviceResources = deviceResources;

	CreateDeviceDependentResources();
	CreateWindowSizeDependentResources();
}
void DirectXRenderer::SetSceneManager(SceneManager* sceneManager)
{
	m_sceneManager = sceneManager;
}

// Initializes view parameters when the window size changes.
void DirectXRenderer::CreateWindowSizeDependentResources()
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
	m_sceneManager->GetCurrentScene()->GetCamera()->SetProjParams(
		fovAngleY,
		aspectRatio,
		0.01f,
		100.0f
		);

	XMFLOAT4X4 orientation = m_deviceResources->GetOrientationTransform3D();
	XMMATRIX orientationMatrix = XMLoadFloat4x4(&orientation);

	XMStoreFloat4x4(
		&m_constantBufferChangesOnResizeData.cameraProjection,

		XMMatrixTranspose(m_sceneManager->GetCurrentScene()->GetCamera()->Projection() *
		orientationMatrix)
		);

	// Eye is at (0,0.7,1.5), looking at point (0,-0.1,0) with the up-vector along the y-axis.
	m_sceneManager->GetCurrentScene()->GetCamera()->SetViewParams(
	{ 0.0f, 0.7f, 1.5f },
	{ 0.0f, -0.1f, 0.0f },
	{ 0.0f, 1.0f, 0.0f }
	);

	XMStoreFloat4x4(&m_constantBufferChangesEveryFrameData.cameraView,
		
		XMMatrixTranspose(m_sceneManager->GetCurrentScene()->GetCamera()->View()
	));
}

// Called once per frame, rotates the cube and calculates the model and view matrices.
void DirectXRenderer::Update(DX::StepTimer const& timer, GameTimer* gameTime)
{
	if (!m_tracking)
	{
		if (m_sceneManager->GetCurrentScene()->GetSceneNode() == nullptr) return;

		// Convert degrees to radians, then convert seconds to rotation angle
		float radiansPerSecond = XMConvertToRadians(m_degreesPerSecond);
		double totalRotation = timer.GetTotalSeconds() * radiansPerSecond;

		float radians = static_cast<float>(fmod(totalRotation, XM_2PI));

		//Rotate(radians, radians);

		m_sceneManager->GetCurrentScene()->GetSceneNode()->Update(gameTime);
	}
}

// Rotate the 3D cube model a set amount of radians.
void DirectXRenderer::Rotate(float radiansX, float radiansY)
{
	if (m_sceneManager->GetCurrentScene()->GetSceneNode() == nullptr) return;

	m_sceneManager->GetCurrentScene()->GetSceneNode()->GetChild(0)->Rotation(XMFLOAT3(radiansX, radiansY, 0.0f));
}

void DirectXRenderer::StartTracking()
{
	m_tracking = true;
}

//===========================================================================================================
//===========================================================================================================
void DirectXRenderer::PickRayVector(float mouseX, float mouseY, DirectX::XMVECTOR& pickRayInWorldSpacePos, DirectX::XMVECTOR& pickRayInWorldSpaceDir)
{
	XMVECTOR pickRayInViewSpaceDir = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);
	XMVECTOR pickRayInViewSpacePos = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);

	float PRVecX, PRVecY, PRVecZ;

	//Transform 2D pick position on screen space to 3D ray in View space
	Size outputSize = m_deviceResources->GetOutputSize();
	XMMATRIX camProjection = m_sceneManager->GetCurrentScene()->GetCamera()->Projection();
	
	XMFLOAT4X4 camProj;
	XMStoreFloat4x4(&camProj, camProjection);

	PRVecX = (((2.0f * mouseX) / outputSize.Width) - 1) / camProj(0, 0);
	PRVecY = -(((2.0f * mouseY) / outputSize.Height) - 1) / camProj(1, 1);
	PRVecZ = -1.0f;	//View space's Z direction ranges from 0 to 1, so we set 1 since the ray goes "into" the screen

	std::string str(std::to_string(PRVecX) + ", " + std::to_string(PRVecY) + ", " + std::to_string(PRVecZ) + "\n");
	std::wstring wstr(str.begin(), str.end());
	OutputDebugString(wstr.c_str());

	pickRayInViewSpaceDir = XMVectorSet(PRVecX, PRVecY, PRVecZ, 0.0f);

	//Uncomment this line if you want to use the center of the screen (client area)
	//to be the point that creates the picking ray (eg. first person shooter)
	//pickRayInViewSpaceDir = XMVectorSet(0.0f, 0.0f, 1.0f, 0.0f);

	// Transform 3D Ray from View space to 3D ray in World space
	XMMATRIX pickRayToWorldSpaceMatrix;
	XMVECTOR matInvDeter;	//We don't use this, but the xna matrix inverse function requires the first parameter to not be null

	XMMATRIX camView = m_sceneManager->GetCurrentScene()->GetCamera()->View();
	pickRayToWorldSpaceMatrix = DirectX::XMMatrixInverse(&matInvDeter, camView);	//Inverse of View Space matrix is World space matrix

	pickRayInWorldSpacePos = XMVector3TransformCoord(pickRayInViewSpacePos, pickRayToWorldSpaceMatrix);
	pickRayInWorldSpaceDir = XMVector3TransformNormal(pickRayInViewSpaceDir, pickRayToWorldSpaceMatrix);
}

float DirectXRenderer::Pick(XMVECTOR pickRayInWorldSpacePos,
	XMVECTOR pickRayInWorldSpaceDir,
	std::vector<std::vector<Anarian::Verticies::PNTVertex>>& vertPosArray,
	std::vector<std::vector<unsigned short>>& indexPosArray,
	XMMATRIX& worldSpace)
{
	//Loop through each triangle in the object
	for (int x = 0; x < indexPosArray.size(); x++) {
		for (int i = 0; i < indexPosArray[x].size() / 3; i++) {
			//Triangle's vertices V1, V2, V3
			XMVECTOR tri1V1 = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);
			XMVECTOR tri1V2 = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);
			XMVECTOR tri1V3 = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);

			//Temporary 3d floats for each vertex
			XMFLOAT3 tV1, tV2, tV3;

			//Get triangle 
			tV1 = vertPosArray[x][indexPosArray[x][(i * 3) + 0]].position;
			tV2 = vertPosArray[x][indexPosArray[x][(i * 3) + 1]].position;
			tV3 = vertPosArray[x][indexPosArray[x][(i * 3) + 2]].position;

			tri1V1 = XMVectorSet(tV1.x, tV1.y, tV1.z, 0.0f);
			tri1V2 = XMVectorSet(tV2.x, tV2.y, tV2.z, 0.0f);
			tri1V3 = XMVectorSet(tV3.x, tV3.y, tV3.z, 0.0f);

			//Transform the vertices to world space
			tri1V1 = XMVector3TransformCoord(tri1V1, worldSpace);
			tri1V2 = XMVector3TransformCoord(tri1V2, worldSpace);
			tri1V3 = XMVector3TransformCoord(tri1V3, worldSpace);

			//Find the normal using U, V coordinates (two edges)
			XMVECTOR U = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);
			XMVECTOR V = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);
			XMVECTOR faceNormal = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);

			U = tri1V2 - tri1V1;
			V = tri1V3 - tri1V1;

			//Compute face normal by crossing U, V
			//faceNormal = XMVector3Cross(U, V);
			//faceNormal = XMVector3Normalize(faceNormal);

			faceNormal = XMLoadFloat3(&(vertPosArray[x][indexPosArray[x][(i * 3) + 1]].normal));
			faceNormal = XMVector3TransformCoord(faceNormal, worldSpace);

			//Calculate a point on the triangle for the plane equation
			XMVECTOR triPoint = tri1V1;

			//Get plane equation ("Ax + By + Cz + D = 0") Variables
			float tri1A = XMVectorGetX(faceNormal);
			float tri1B = XMVectorGetY(faceNormal);
			float tri1C = XMVectorGetZ(faceNormal);
			float tri1D = (-tri1A*XMVectorGetX(triPoint) - tri1B*XMVectorGetY(triPoint) - tri1C*XMVectorGetZ(triPoint));

			//Now we find where (on the ray) the ray intersects with the triangles plane
			float ep1, ep2, t = 0.0f;
			float planeIntersectX, planeIntersectY, planeIntersectZ = 0.0f;
			XMVECTOR pointInPlane = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);

			ep1 = (XMVectorGetX(pickRayInWorldSpacePos) * tri1A) + (XMVectorGetY(pickRayInWorldSpacePos) * tri1B) + (XMVectorGetZ(pickRayInWorldSpacePos) * tri1C);
			ep2 = (XMVectorGetX(pickRayInWorldSpaceDir) * tri1A) + (XMVectorGetY(pickRayInWorldSpaceDir) * tri1B) + (XMVectorGetZ(pickRayInWorldSpaceDir) * tri1C);

			//Make sure there are no divide-by-zeros
			if (ep2 != 0.0f)
				t = -(ep1 + tri1D) / (ep2);

			if (t > 0.0f)    //Make sure you don't pick objects behind the camera
			{
				//Get the point on the plane
				planeIntersectX = XMVectorGetX(pickRayInWorldSpacePos) + XMVectorGetX(pickRayInWorldSpaceDir) * t;
				planeIntersectY = XMVectorGetY(pickRayInWorldSpacePos) + XMVectorGetY(pickRayInWorldSpaceDir) * t;
				planeIntersectZ = XMVectorGetZ(pickRayInWorldSpacePos) + XMVectorGetZ(pickRayInWorldSpaceDir) * t;

				pointInPlane = XMVectorSet(planeIntersectX, planeIntersectY, planeIntersectZ, 0.0f);

				//Call function to check if point is in the triangle
				if (PointInTriangle(tri1V1, tri1V2, tri1V3, pointInPlane)) {
					//Return the distance to the hit, so you can check all the other pickable objects in your scene
					//and choose whichever object is closest to the camera
					return t / 2.0f;
				}
			}
		}
	}
	//return the max float value (near infinity) if an object was not picked
	return FLT_MAX;
}

bool DirectXRenderer::PointInTriangle(XMVECTOR& triV1, XMVECTOR& triV2, XMVECTOR& triV3, XMVECTOR& point)
{
	//To find out if the point is inside the triangle, we will check to see if the point
	//is on the correct side of each of the triangles edges.

	XMVECTOR cp1 = XMVector3Cross((triV3 - triV2), (point - triV2));
	XMVECTOR cp2 = XMVector3Cross((triV3 - triV2), (triV1 - triV2));
	if (XMVectorGetX(XMVector3Dot(cp1, cp2)) >= 0) {
		cp1 = XMVector3Cross((triV3 - triV1), (point - triV1));
		cp2 = XMVector3Cross((triV3 - triV1), (triV2 - triV1));
		if (XMVectorGetX(XMVector3Dot(cp1, cp2)) >= 0) {
			cp1 = XMVector3Cross((triV2 - triV1), (point - triV1));
			cp2 = XMVector3Cross((triV2 - triV1), (triV3 - triV1));
			if (XMVectorGetX(XMVector3Dot(cp1, cp2)) >= 0) {
				return true;
			}
			else
				return false;
		}
		else
			return false;
	}
	return false;
}

//===========================================================================================================
//===========================================================================================================
// When tracking, the 3D cube can be rotated around its Y axis by tracking pointer position relative to the output screen width.
void DirectXRenderer::TrackingUpdate(float positionX, float positionY)
{
	if (m_tracking)
	{
		//float radiansX = XM_2PI * 2.0f * positionX / m_deviceResources->GetOutputSize().Width;
		//float radiansY = XM_2PI * 2.0f * positionY / m_deviceResources->GetOutputSize().Height;
		//Rotate(radiansX, radiansY);
		m_isClicking = true;

		float tempDist;
		float closestDist = FLT_MAX;
		int hitIndex;

		XMVECTOR prwsPos, prwsDir;
		PickRayVector(positionX, positionY, prwsPos, prwsDir);
		
		for (int i = 0; i < m_sceneManager->GetCurrentScene()->GetSceneNode()->ChildCount(); i++) { // Number of things on screen to search
			if (true) //Use this If Statement to shorten the list of raycasts
			{
				m_sceneManager->GetCurrentScene()->GetSceneNode()->GetChild(i)->UpdatePosition();

				tempDist = Pick(prwsPos, prwsDir,
					*m_sceneManager->GetCurrentScene()->GetSceneNode()->GetChild(i)->GetMesh()->Vertices(),
					*m_sceneManager->GetCurrentScene()->GetSceneNode()->GetChild(i)->GetMesh()->Indices(),
					m_sceneManager->GetCurrentScene()->GetSceneNode()->GetChild(i)->ModelMatrix() // If it doesn't work, try XMTransposing this
					);
				if (tempDist < closestDist) {
					closestDist = tempDist;
					hitIndex = i;
				}
			}
		}
		
		if (closestDist < FLT_MAX) {
			// It was hit
			std::string str("Hit!");
			std::wstring wstr(str.begin(), str.end());
			OutputDebugString(wstr.c_str());

			m_isShoot = true;
			m_sceneManager->GetCurrentScene()->GetSceneNode()->GetChild(0)->GetMaterial()->SetDiffuseColor(Color::RandomColor());
		}

		m_isClicking = false;
	}
}

void DirectXRenderer::StopTracking()
{
	m_tracking = false;
}

// Renders one frame using the vertex and pixel shaders.
void DirectXRenderer::Render()
{
	// Loading is asynchronous. Only draw geometry after it's loaded.
	if (!m_loadingComplete) { return; }

	//while (m_isClicking) {};
	
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

	// Check early return on the current scene
	// If the scene is null, exit
	if (m_sceneManager->GetCurrentScene() == nullptr) return;

	// If the early return checks pass successfully, then we are clear to begin
	// the process of setting buffers and rendering the scene starting from the
	// main scene node.

	// Set the SamplerState
	context->PSSetSamplers(0, 1, m_samplerState.GetAddressOf());
	context->IASetInputLayout(m_inputLayout.Get());

	// Set the constant buffer to the graphics device.
	context->VSSetConstantBuffers(0, 1,	m_constantBufferChangesOnResize.GetAddressOf());
	context->VSSetConstantBuffers(1, 1,	m_constantBufferChangesEveryFrame.GetAddressOf());
	context->VSSetConstantBuffers(2, 1,	m_constantBufferChangesEveryPrim.GetAddressOf());

	// Send the data to the Constant Buffer
	m_deviceResources->GetD3DDeviceContext()->UpdateSubresource(
		m_constantBufferChangesOnResize.Get(),
		0,
		NULL,
		&m_constantBufferChangesOnResizeData,
		0,
		0
		);
	context->UpdateSubresource(
		m_constantBufferChangesEveryFrame.Get(),
		0,
		NULL,
		&m_constantBufferChangesEveryFrameData,
		0,
		0
		);

	// Set the Rasterizer State
	context->RSSetState(m_defaultRasterizerState.Get());

	// Draw the Mesh
	m_sceneManager->GetCurrentScene()->GetSceneNode()->Render(context, m_constantBufferChangesEveryPrim.Get());
}

void DirectXRenderer::CreateDeviceDependentResources()
{
	// Load shaders asynchronously.
	auto loadVSTask = DX::ReadDataAsync(L"VertexShader.cso");
	auto loadPSTask = DX::ReadDataAsync(L"PixelShader.cso");

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
	});

	auto createConstantBuffers = (createPSTask && createVSTask).then([this]() {
		// Make the constant buffers
		CD3D11_BUFFER_DESC constantBufferCORDesc((sizeof(ConstantBufferChangesOnResize) + 15) / 16 * 16,
			D3D11_BIND_CONSTANT_BUFFER);
		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateBuffer(
			&constantBufferCORDesc,
			nullptr,
			&m_constantBufferChangesOnResize
			)
			);

		CD3D11_BUFFER_DESC constantBufferCEFDesc((sizeof(ConstantBufferChangesEveryFrame) + 15) / 16 * 16,
			D3D11_BIND_CONSTANT_BUFFER);
		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateBuffer(
			&constantBufferCEFDesc,
			nullptr,
			&m_constantBufferChangesEveryFrame
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

	auto createStatesTask = (createConstantBuffers).then([this]() {
		// Filter sampler
		D3D11_SAMPLER_DESC sd;
		sd.Filter = D3D11_FILTER_ANISOTROPIC;
		sd.MaxAnisotropy = 16;																// use Anisotropic x 8. Available values = 1-16
		sd.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;		//D3D11_TEXTURE_ADDRESS_CLAMP;		// horizontally the texture is repeated
		sd.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;		//D3D11_TEXTURE_ADDRESS_CLAMP;		// vertically the texture is mirrored
		sd.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;		//D3D11_TEXTURE_ADDRESS_CLAMP;		// if it's a 3D texture, it is clamped
		sd.BorderColor[0] = 0.0f;
		sd.BorderColor[1] = 0.0f;
		sd.BorderColor[2] = 0.0f;
		sd.BorderColor[3] = 0.0f;
		sd.MinLOD = 0.0f;
		sd.MaxLOD = FLT_MAX;
		sd.MipLODBias = 0.0f;							// decrease mip level of detail by 0

		DX::ThrowIfFailed(
			m_deviceResources->GetD3DDevice()->CreateSamplerState(&sd, &m_samplerState)
			);

		// Rasterizer State
		D3D11_RASTERIZER_DESC rd;
		rd.FillMode = D3D11_FILL_SOLID;
		rd.CullMode = D3D11_CULL_BACK;
		rd.FrontCounterClockwise = TRUE;
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
		// Start the async tasks to load the shaders and textures.
		BasicLoader^ loader = ref new BasicLoader(m_deviceResources->GetD3DDevice());
		loader->LoadTexture("Assets\\TyrilMap.png", nullptr, &m_tyrilMap);

		// Add the Shaders
		((DirectXMaterial*)m_sceneManager->GetCurrentScene()->GetSceneNode()->GetChild(0)->GetMaterial())->
			CreateViews(
			m_vertexShader.Get(),
			m_pixelShader.Get());

		m_sceneManager->GetCurrentScene()->GetSceneNode()->GetChild(0)->SetActive(true);
	});

	// Once the cube is loaded, the object is ready to be rendered.
	createCubeTask.then([this] () {
		m_loadingComplete = true;
	});
}

void DirectXRenderer::ReleaseDeviceDependentResources()
{
	m_loadingComplete = false;

	m_defaultRasterizerState.Reset();
	m_samplerState.Reset();

	m_inputLayout.Reset();
	m_vertexShader.Reset();
	m_pixelShader.Reset();

	m_constantBufferChangesOnResize.Reset();
	m_constantBufferChangesEveryFrame.Reset();
	m_constantBufferChangesEveryPrim.Reset();

	// Release texture resource
	m_tyrilMap.Reset();
}