#pragma once
#include "LightType.h"
#include "Color.h"

namespace Anarian
{
	struct Light
	{
	public:
		DirectX::XMFLOAT3 Direction;

		/// Pads are used to ensure the Constant Buffer fits into 16 Bytes (4D Vectors)
		float m_pad = 1.0f;
		
		Color Ambient;
		Color Diffuse;

		// Used to determine what type of light to use in the shader
		LightType m_LightType;

	public:
		Light() {
			m_LightType = LightType::None;

			Direction = DirectX::XMFLOAT3(0.0f, -1.0f, 0.0f);
			Ambient = Color::White();
			Diffuse = Color::White();
		};
		
		// Directional Light Constructor
		Light(DirectX::XMFLOAT3 direction, Color ambientColor, Color diffuseColor)
		{
			m_LightType = LightType::DirectionalLight;

			Direction = direction;
			Ambient = ambientColor;
			Diffuse = diffuseColor;
		}

		~Light() {};

	public:
		void SetDirection(DirectX::XMFLOAT3 dir) { Direction = dir; };
		void SetDiffuse(Color color) { Diffuse = color; };
		void SetAmbient(Color color) { Ambient = color; };
	};
}