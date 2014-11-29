#include "IUpdatable.h"
#include "IRenderable.h"

#include "IMeshObject.h"
#include "DirectXMesh.h"

#include "IMaterial.h"
#include "DirectXMaterial.h"

#include "AnimationState.h"

#include "PNTVertex.h"
#include "Weight.h"
#include "Joint.h"

#include "ModelRawDataType.h"

namespace Anarian
{
	class Model :
		public IUpdatable, 
		public IRenderable
	{
		friend IMeshObject;
		friend IMaterial;

#ifdef Anarian_DirectX_Mode
		friend DirectXMesh;
		friend DirectXMaterial;
#endif
	private:
		ModelRawDataType m_rawDataType;
		void* m_rawData;

	private:
		IMeshObject*								m_mesh;
		IMaterial*									m_material;

		std::vector<Anarian::Verticies::Joint>		m_joints;
		std::vector<Anarian::Verticies::Weight>		m_weights;

	public:
		Model(IMeshObject* mesh = nullptr, IMaterial* material = nullptr);
		~Model();

		/// Basic Getter Setters
		IMeshObject* GetMesh() { return m_mesh; };
		IMaterial* GetMaterial() { return m_material; };

		void* GetRawData() { return m_rawData; };
		ModelRawDataType GetRawDataType() { return m_rawDataType; };
		void  SetRawData(void* raw, ModelRawDataType type) { 
			m_rawData = raw;
			m_rawDataType = type;
		};

		void SetMesh(IMeshObject** mesh) { m_mesh = *mesh; };
		void SetMaterial(IMaterial** material) { m_material = *material; };
		
		Anarian::Verticies::Joint* GetJoint(int index) { return &m_joints[index]; };
		void AddJoint(Anarian::Verticies::Joint joint) { m_joints.push_back(joint); };

		Anarian::Verticies::Weight* GetWeight(int index) { return &m_weights[index]; };
		void AddWeight(Anarian::Verticies::Weight weight) { m_weights.push_back(weight); };

		// Helper Methods
		bool HasJoints() { 
			if (m_joints.size() > 0) return true;
			return false;
		}
		bool HasWeights() {
			if (m_weights.size() > 0) return true;
			return false;
		}


		/// Method Calls
		virtual void Update(GameTimer* gameTime, AnimationState* animationState = nullptr);
		virtual void Render(
			ID3D11DeviceContext *context,
			ID3D11Buffer *primitiveConstantBuffer,
			ConstantBufferChangesEveryPrim* constantBuffer);

	private:
		void UpdateAnimation(GameTimer* gameTime, AnimationState* animationState);
	};
}