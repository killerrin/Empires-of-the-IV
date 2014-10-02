#ifdef Anarian_DirectX_Mode
#include "pch.h"
#include "DirectXMaterial.h"
using namespace Anarian;
using namespace DirectX;

DirectXMaterial::DirectXMaterial(
	Color meshColor,
	Color diffuseColor,
	Color specularColor,
	float specularExponent,
	ID3D11ShaderResourceView* textureResourceView,
	ID3D11VertexShader* vertexShader,
	ID3D11PixelShader* pixelShader
	)	
	:IMaterial(meshColor, diffuseColor, specularColor, specularExponent)
{
	m_vertexShader = vertexShader;
	m_pixelShader = pixelShader;
	m_textureRV = textureResourceView;
}


DirectXMaterial::~DirectXMaterial()
{
	if (m_textureRV != nullptr)
		m_textureRV.ReleaseAndGetAddressOf();
	if (m_pixelShader != nullptr)
		m_pixelShader.ReleaseAndGetAddressOf();
	if (m_textureRV != nullptr)
		m_textureRV.ReleaseAndGetAddressOf();
	
	IMaterial::~IMaterial();
}

void DirectXMaterial::SetTexture(ID3D11ShaderResourceView* textureResourceView)
{
	m_textureRV.ReleaseAndGetAddressOf( );
	m_textureRV = textureResourceView;
}
#endif