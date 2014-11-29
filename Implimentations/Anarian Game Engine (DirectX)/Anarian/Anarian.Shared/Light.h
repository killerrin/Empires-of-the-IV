#pragma once
#include "LightType.h"
#include "Color.h"

namespace Anarian
{
	struct Light
	{
	public:

		DirectX::XMFLOAT3 Position;
		float Range;

		DirectX::XMFLOAT3 Direction;
		float Cone;

		DirectX::XMFLOAT3 Att;

		/// ----------------------------------------------
		/// Pads are used to ensure the Constant Buffer fits into 16 Bytes (4D Vectors)
		float m_pad2 = 1.0f;
		/// ----------------------------------------------

		Color Ambient;
		Color Diffuse;

		// Used to determine what type of light to use in the shader
		LightType m_LightType;

	public:
		// Default Constructor
		Light();
		
		// Directional Light Constructor
		Light(DirectX::XMFLOAT3 direction, Color ambientColor, Color diffuseColor);

		// Point Light Constructor
		Light(DirectX::XMFLOAT3 position, float range, DirectX::XMFLOAT3 att, Color ambientColor, Color diffuseColor);

		// Spotlight Constructor
		Light(DirectX::XMFLOAT3 position, DirectX::XMFLOAT3 direction, float range, float cone, DirectX::XMFLOAT3 att, Color ambientColor, Color diffuseColor);

		~Light() {};

	public:
		void SetDiffuse(Color color) { Diffuse = color; };
		Color GetDiffuse() { return Diffuse; };

		void SetAmbient(Color color) { Ambient = color; };
		Color GetAmbient() { return Ambient; };
	};
}