#include "pch.h"
#include "Model.h"

using namespace Anarian;
using namespace Verticies;

Model::Model(IMeshObject* mesh, IMaterial* material):
	IUpdatable(),
	IRenderable()
{
	m_mesh = mesh;
	m_material = material;

	m_joints = std::vector<Anarian::Verticies::Joint>();
}

Model::~Model()
{
	//delete m_mesh;
	//delete m_material;

	m_joints.clear();
}

void Model::Update(GameTimer* gameTime)
{

}

void Model::Render(
	ID3D11DeviceContext *context,
	ID3D11Buffer *primitiveConstantBuffer,
	ConstantBufferChangesEveryPrim* constantBuffer)
{
	// If one is null, we return early to save the calls
	if ((m_material == nullptr) ||
		(m_mesh == nullptr))
		return;

#ifdef Anarian_DirectX_Mode
	((DirectXMaterial*)m_material)->Render(context, constantBuffer);
	context->UpdateSubresource(primitiveConstantBuffer, 0, nullptr, constantBuffer, 0, 0);

	((DirectXMesh*)m_mesh)->Render(context);
#endif
}