#include "pch.h"
#include "IMaterial.h"
#include "IMeshObject.h"
#include "GameObject.h"

#ifdef Anarian_DirectX_Mode
#include "DirectXMaterial.h"
#include "DirectXMesh.h"
#include "ConstantBuffers.h"
#endif

using namespace Anarian;
using namespace DirectX;

GameObject::GameObject(): 
	IRenderable(),
	IUpdatable()
{
	m_active = true;

	m_parent = nullptr;
	m_children = std::vector<GameObject>();

	m_material = nullptr;
	m_mesh = nullptr;

	m_position = XMFLOAT3(0.0f, 0.0f, 0.0f);
	m_scale = XMFLOAT3(1.0f, 1.0f, 1.0f);
	m_rotation = XMFLOAT3(0.0f, 0.0f, 0.0f);

	XMStoreFloat4x4(&m_modelMatrix, XMMatrixIdentity());
}


GameObject::~GameObject()
{
	m_children.clear();
}

void GameObject::UpdatePosition() 
{
	XMStoreFloat4x4(
		&m_modelMatrix,

		ScaleMatrix() *
		RotationMatrix() *
		TranslationMatrix()
		);
}

void GameObject::Update(GameTimer* gameTime)
{
	// If its not active, skip the rest
	if (!m_active) { return; }

	IUpdatable::Update(gameTime);
	UpdatePosition();

	// Finally, render the children
	for (int i = 0; i < m_children.size(); i++)
	{
		m_children[i].Update(gameTime);
	}
}

void GameObject::Render(
	ID3D11DeviceContext *context,
	ID3D11Buffer *primitiveConstantBuffer)
{
	// If its not active, skip the rest
	if (!m_active) { return; }
	
	// If there is no mesh or material, skip this object and move onto the children
	if ((m_mesh == nullptr) || (m_material == nullptr)) { }
	else
	{
		IRenderable::Render();

		ConstantBufferChangesEveryPrim constantBuffer;

		XMStoreFloat4x4(
			&constantBuffer.worldMatrix,
			XMMatrixTranspose(ModelMatrix())
			);

		((DirectXMaterial*)m_material)->Render(context, &constantBuffer);
		context->UpdateSubresource(primitiveConstantBuffer, 0, nullptr, &constantBuffer, 0, 0);

		((DirectXMesh*)m_mesh)->Render(context);
	}

	// Finally, render the children
	for (int i = 0; i < m_children.size(); i++)
	{
		m_children[i].Render(context, primitiveConstantBuffer);
	}
}
