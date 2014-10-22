#pragma once
#include "IUpdatable.h"
#include "IRenderable.h"

namespace Anarian{
	class GameObject : IUpdatable, IRenderable
	{
	private:

	protected:
		bool m_active;

		GameObject* m_parent;
		std::vector<GameObject*> m_children;

		IMaterial* m_material;
		IMeshObject* m_mesh;

		DirectX::XMFLOAT3   m_position;
		DirectX::XMFLOAT3   m_scale;
		DirectX::XMFLOAT3   m_rotation;
		DirectX::XMFLOAT4X4 m_modelMatrix;

	public:
		GameObject();
		virtual ~GameObject();

		virtual void UpdatePosition();
		virtual void Update(GameTimer* gameTime);
		virtual void Render(
			ID3D11DeviceContext *context,
			ID3D11Buffer *primitiveConstantBuffer);

		//----------------------\\

		void SetActive(bool active) { m_active = active; };
		bool GetActive() { return m_active; };

		//----------------------\\

		void SetParent(GameObject* parent) { m_parent = parent; };
		GameObject* GetParent() { return m_parent; };

		void AddChild(GameObject* gameObject) {
			gameObject->SetParent(this);
			m_children.push_back(gameObject);
		};
		GameObject* GetChild(int index) { return m_children[index]; };

		int ChildCount() { return m_children.size(); }

		void SetMesh(IMeshObject* mesh) { m_mesh = mesh; };
		IMeshObject* GetMesh() { return m_mesh; };
		
		void SetMaterial(IMaterial* material) { m_material = material; };
		IMaterial* GetMaterial() { return m_material; };

		//----------------------\\

		DirectX::XMMATRIX ModelMatrix()	{ return DirectX::XMLoadFloat4x4(&m_modelMatrix); };
		DirectX::XMMATRIX ScaleMatrix() { 
			DirectX::XMFLOAT3 sca = WorldScale();
			return DirectX::XMMatrixScaling(sca.x, sca.y, sca.z);
		};
		DirectX::XMMATRIX RotationMatrix() {
			DirectX::XMFLOAT3 rot = WorldRotation();

			DirectX::XMMATRIX xRot = DirectX::XMMatrixRotationX(rot.x);
			DirectX::XMMATRIX yRot = DirectX::XMMatrixRotationY(rot.y);
			DirectX::XMMATRIX zRot = DirectX::XMMatrixRotationZ(rot.z);
			return xRot * yRot * zRot;
		};
		DirectX::XMMATRIX TranslationMatrix() { 
			DirectX::XMFLOAT3 pos = WorldPosition();
			return DirectX::XMMatrixTranslation(pos.x, pos.y, pos.z);
		};

		//----------------------\\
		
		void Position(DirectX::XMFLOAT3 position) {
			m_position = position;
			UpdatePosition();
		};
		DirectX::XMFLOAT3 Position() { return m_position; };
		DirectX::XMFLOAT3 WorldPosition() {
			DirectX::XMFLOAT3 pos = m_position;

			if (m_parent != nullptr) {
				DirectX::XMFLOAT3 parentPos = m_parent->WorldPosition();
				pos.x += parentPos.x;
				pos.y += parentPos.y;
				pos.z += parentPos.z;
			}

			return pos;
		}

		void Position(DirectX::XMVECTOR position) {
			XMStoreFloat3(&m_position, position);
			UpdatePosition();
		};
		DirectX::XMVECTOR VectorPosition() { return DirectX::XMLoadFloat3(&m_position); };

		void Scale(DirectX::XMFLOAT3 scale){
			m_scale = scale;
			UpdatePosition();
		}
		DirectX::XMFLOAT3 Scale() { return m_scale; };
		DirectX::XMFLOAT3 WorldScale() {
			DirectX::XMFLOAT3 sca = m_scale;

			if (m_parent != nullptr) {
				DirectX::XMFLOAT3 parentSca = m_parent->WorldScale();
				sca.x *= parentSca.x;
				sca.y *= parentSca.y;
				sca.z *= parentSca.z;
			}

			return sca;
		}
		
		void Scale(DirectX::XMVECTOR scale) {
			XMStoreFloat3(&m_scale, scale);
			UpdatePosition();
		}
		DirectX::XMVECTOR VectorScale() { return DirectX::XMLoadFloat3(&m_scale); };

		void Rotation(DirectX::XMFLOAT3 rotation) {
			m_rotation = rotation;

			UpdatePosition();
		}
		DirectX::XMFLOAT3 Rotation() { return m_rotation; };
		DirectX::XMFLOAT3 WorldRotation() {
			DirectX::XMFLOAT3 rot = m_rotation;

			if (m_parent != nullptr) {
				DirectX::XMFLOAT3 parentRot = m_parent->WorldRotation();
				rot.x += parentRot.x;
				rot.y += parentRot.y;
				rot.z += parentRot.z;
			}

			return rot;
		}

		void Rotation(DirectX::XMVECTOR rotation) {
			XMStoreFloat3(&m_rotation, rotation);
			UpdatePosition();
		}
		DirectX::XMVECTOR VectorRotation() { return DirectX::XMLoadFloat3(&m_rotation); };
	};
}