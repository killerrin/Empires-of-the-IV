#include "pch.h"

#include "Joint.h"
#include "ModelAnimation.h"

#include "Model.h"
#include "Common\DeviceResources.h"

using namespace Anarian;
using namespace Anarian::Verticies;
using namespace DirectX;

Model::Model(IMeshObject* mesh, IMaterial* material) :
	IUpdatable(),
	IRenderable()
{
	m_mesh = mesh;
	m_material = material;

	m_joints = std::vector<Anarian::Verticies::Joint>();
	m_weights = std::vector<Anarian::Verticies::Weight>();
}

Model::~Model()
{
	//delete m_mesh;
	//delete m_material;

	m_joints.clear();
	m_weights.clear();
}

void Model::Update(GameTimer* gameTime, AnimationState* animationState)
{
	if (animationState != nullptr) { UpdateAnimation(gameTime, animationState); }
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

//-----------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------

void Model::UpdateAnimation(GameTimer* gameTime, AnimationState* animationState)
{
	if (!animationState->HasAnimation()) return;
	if (!animationState->IsPlaying()) return;

	// Update AnimationState here
	animationState->Update(gameTime);

	// Update the mesh
	std::vector<Joint>  interpolatedSkeleton = animationState->GetInterpolatedSkeleton();
	
	for (int x = 0; x < m_mesh->m_vertices.size(); x++) {
		for (int k = 0; k < m_mesh->m_vertices[x].size(); k++) {
			PNTVertex tempVert = m_mesh->m_vertices[x][k];
			tempVert.position = XMFLOAT3(0, 0, 0);	// Make sure the vertex's pos is cleared first
			tempVert.normal = XMFLOAT3(0, 0, 0);	// Clear vertices normal
	
			// Sum up the joints and weights information to get vertex's position and normal
			for (int j = 0; j < tempVert.m_weightCount; ++j) {
				Weight tempWeight = m_weights[tempVert.m_startWeight + j];
				Joint tempJoint = interpolatedSkeleton[tempWeight.JointID];
	
				// Convert joint orientation and weight pos to vectors for easier computation
				XMVECTOR tempJointOrientation = XMVectorSet(tempJoint.Orientation.x, tempJoint.Orientation.y, tempJoint.Orientation.z, tempJoint.Orientation.w);
				XMVECTOR tempWeightPos = XMVectorSet(tempWeight.Position.x, tempWeight.Position.y, tempWeight.Position.z, 0.0f);
	
				// We will need to use the conjugate of the joint orientation quaternion
				XMVECTOR tempJointOrientationConjugate = XMQuaternionInverse(tempJointOrientation);
	
				// Calculate vertex position (in joint space, eg. rotate the point around (0,0,0)) for this weight using the joint orientation quaternion and its conjugate
				// We can rotate a point using a quaternion with the equation "rotatedPoint = quaternion * point * quaternionConjugate"
				XMFLOAT3 rotatedPoint;
				XMStoreFloat3(&rotatedPoint, XMQuaternionMultiply(XMQuaternionMultiply(tempJointOrientation, tempWeightPos), tempJointOrientationConjugate));
	
				// Now move the verices position from joint space (0,0,0) to the joints position in world space, taking the weights bias into account
				tempVert.position.x += (tempJoint.Position.x + rotatedPoint.x) * tempWeight.Bias;
				tempVert.position.y += (tempJoint.Position.y + rotatedPoint.y) * tempWeight.Bias;
				tempVert.position.z += (tempJoint.Position.z + rotatedPoint.z) * tempWeight.Bias;
	
				// Compute the normals for this frames skeleton using the weight normals from before
				// We can comput the normals the same way we compute the vertices position, only we don't have to translate them (just rotate)
				XMVECTOR tempWeightNormal = XMVectorSet(tempWeight.Normal.x, tempWeight.Normal.y, tempWeight.Normal.z, 0.0f);
	
				// Rotate the normal
				XMStoreFloat3(&rotatedPoint, XMQuaternionMultiply(XMQuaternionMultiply(tempJointOrientation, tempWeightNormal), tempJointOrientationConjugate));
	
				// Add to vertices normal and ake weight bias into account
				tempVert.normal.x -= rotatedPoint.x * tempWeight.Bias;
				tempVert.normal.y -= rotatedPoint.y * tempWeight.Bias;
				tempVert.normal.z -= rotatedPoint.z * tempWeight.Bias;
			}
	
			m_mesh->m_vertices[x][k].position = tempVert.position;				// Store the vertices position in the position vector instead of straight into the vertex vector
			m_mesh->m_vertices[x][k].normal = tempVert.normal;		// Store the vertices normal
			XMStoreFloat3(&m_mesh->m_vertices[x][k].normal, XMVector3Normalize(XMLoadFloat3(&m_mesh->m_vertices[x][k].normal)));
		}
	
		//// Put the positions into the vertices for this subset
		//for (int i = 0; i < MD5Model.subsets[k].vertices.size(); i++) {
		//	MD5Model.subsets[k].vertices[i].pos = MD5Model.subsets[k].positions[i];
		//}
		//// Update the subsets vertex buffer
		//// First lock the buffer
		//D3D11_MAPPED_SUBRESOURCE mappedVertBuff;
		//hr = d3d11DevCon->Map(MD5Model.subsets[k].vertBuff, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedVertBuff);
		//
		//// Copy the data into the vertex buffer.
		//memcpy(mappedVertBuff.pData, &MD5Model.subsets[k].vertices[0], (sizeof(Vertex) * MD5Model.subsets[k].vertices.size()));
		//
		//d3d11DevCon->Unmap(MD5Model.subsets[k].vertBuff, 0);
		//
		//// The line below is another way to update a buffer. You will use this when you want to update a buffer less
		//// than once per frame, since the GPU reads will be faster (the buffer was created as a DEFAULT buffer instead
		//// of a DYNAMIC buffer), and the CPU writes will be slower. You can try both methods to find out which one is faster
		//// for you. if you want to use the line below, you will have to create the buffer with D3D11_USAGE_DEFAULT instead
		//// of D3D11_USAGE_DYNAMIC
		////d3d11DevCon->UpdateSubresource( MD5Model.subsets[k].vertBuff, 0, NULL, &MD5Model.subsets[k].vertices[0], 0, 0 );
	}

	// Recreate the buffers to update for the new animation
#ifdef Anarian_DirectX_Mode
	//((DirectXMesh*)m_mesh)->CreateBuffers(DX::DeviceResources::Instance()->GetD3DDevice(), D3D11_CPU_ACCESS_FLAG::D3D11_CPU_ACCESS_WRITE);
#endif
}