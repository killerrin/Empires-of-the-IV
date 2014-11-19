#include "pch.h"
#include "Light.h"

using namespace Anarian;
using namespace DirectX;

// Default Constructor
Light::Light() {
	ZeroMemory(this, sizeof(Light));

	m_LightType = LightType::NoLight;

	Direction = DirectX::XMFLOAT3(0.0f, -1.0f, 0.0f);
	Ambient = Color::White();
	Diffuse = Color::White();
};

// Directional Light Constructor
Light::Light(DirectX::XMFLOAT3 direction, Color ambientColor, Color diffuseColor)
{
	ZeroMemory(this, sizeof(Light));

	m_LightType = LightType::DirectionalLight;

	Direction = direction;
	Ambient = ambientColor;
	Diffuse = diffuseColor;
}

// Point Light Constructor
Light::Light(DirectX::XMFLOAT3 position, float range, DirectX::XMFLOAT3 att, Color ambientColor, Color diffuseColor)
{
	ZeroMemory(this, sizeof(Light));

	m_LightType = LightType::PointLight;

	Position = position;
	Range = range;
	Att = att;

	Ambient = ambientColor;
	Diffuse = diffuseColor;
}

// Spotlight Constructor
Light::Light(DirectX::XMFLOAT3 position, DirectX::XMFLOAT3 direction, float range, float cone, DirectX::XMFLOAT3 att, Color ambientColor, Color diffuseColor)
{
	ZeroMemory(this, sizeof(Light));

	m_LightType = LightType::SpotLight;

	Position = position;
	Direction = direction;

	Range = range;
	Cone = cone;

	Att = att;

	Ambient = ambientColor;
	Diffuse = diffuseColor;
}