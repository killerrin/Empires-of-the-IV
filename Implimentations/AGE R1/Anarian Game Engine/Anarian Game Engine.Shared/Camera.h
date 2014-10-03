#pragma once

namespace Anarian{
	class Camera
	{
	private:
		static Camera* m_mainCamera;

		DirectX::XMFLOAT4X4 m_viewMatrix;
		DirectX::XMFLOAT4X4 m_projectionMatrix;
		DirectX::XMFLOAT4X4 m_projectionMatrixLeft;
		DirectX::XMFLOAT4X4 m_projectionMatrixRight;

		DirectX::XMFLOAT4X4 m_inverseView;

		DirectX::XMFLOAT3 m_eye;
		DirectX::XMFLOAT3 m_lookAt;
		DirectX::XMFLOAT3 m_up;
		float             m_cameraYawAngle;
		float             m_cameraPitchAngle;

		float             m_fieldOfView;
		float             m_aspectRatio;
		float             m_nearPlane;
		float             m_farPlane;

	public:
		static Camera* MainCamera();
		Camera();
		~Camera();

		void SetViewParams(DirectX::XMFLOAT3 eye, DirectX::XMFLOAT3 lookAt, DirectX::XMFLOAT3 up);
		void SetProjParams(float fieldOfView, float aspectRatio, float nearPlane, float farPlane);

		void LookDirection(DirectX::XMFLOAT3 lookDirection);
		void Eye(DirectX::XMFLOAT3 position);

		DirectX::XMMATRIX View();
		DirectX::XMMATRIX Projection();
		DirectX::XMMATRIX LeftEyeProjection();
		DirectX::XMMATRIX RightEyeProjection();
		DirectX::XMMATRIX World();
		DirectX::XMFLOAT3 Eye();
		DirectX::XMFLOAT3 LookAt();
		DirectX::XMFLOAT3 Up();
		float NearClipPlane();
		float FarClipPlane();
		float Pitch();
		float Yaw();
	};
}
