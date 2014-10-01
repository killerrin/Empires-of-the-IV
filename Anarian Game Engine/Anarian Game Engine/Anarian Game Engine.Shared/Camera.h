#pragma once

namespace Anarian{
	class Camera
	{
	private:
		static Camera* m_mainCamera;

	public:
		static Camera* MainCamera()
		{
			if (m_mainCamera == nullptr) m_mainCamera = new Camera();
			return m_mainCamera;
		}
		Camera();
		~Camera();
	};
}
