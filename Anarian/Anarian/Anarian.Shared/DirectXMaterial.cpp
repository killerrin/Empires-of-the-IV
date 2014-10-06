#include "pch.h"
#ifdef Anarian_DirectX_Mode

#include "ConstantBuffers.h"
#include "DirectXMaterial.h"
using namespace Anarian;
using namespace DirectX;

DirectXMaterial::DirectXMaterial()
	: IMaterial()
{

}

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
	// Commented out as resources are going to be managed elsewhere to keep from having multiple
	// of the same assets stored in memory

	if (m_textureRV != nullptr) {
		//m_textureRV.ReleaseAndGetAddressOf();
	}
	if (m_pixelShader != nullptr) {
		//m_pixelShader.ReleaseAndGetAddressOf();
	}
	if (m_textureRV != nullptr) {
		//m_textureRV.ReleaseAndGetAddressOf();
	}
	
	//IMaterial::~IMaterial();
}

void DirectXMaterial::CreateViews(
	ID3D11ShaderResourceView* textureResourceView,
	ID3D11VertexShader* vertexShader,
	ID3D11PixelShader* pixelShader)
{
	m_vertexShader = vertexShader;
	m_pixelShader = pixelShader;
	m_textureRV = textureResourceView;
}

void DirectXMaterial::Render(ID3D11DeviceContext *context, ConstantBufferChangesEveryPrim* cBuffer, int bufferIndex)
{
	// Set constant buffers here
	cBuffer->meshColor = m_meshColor;
	cBuffer->specularColor = m_specularColor;
	cBuffer->specularPower = m_specularExponent;
	cBuffer->diffuseColor = m_diffuseColor;

	// Set shader resources here
	context->PSSetShaderResources(bufferIndex, 1, m_textureRV.GetAddressOf());
	context->VSSetShader(m_vertexShader.Get(), nullptr, 0);
	context->PSSetShader(m_pixelShader.Get(), nullptr, 0);
}

void DirectXMaterial::Texture(ID3D11ShaderResourceView* textureResourceView)
{
	//m_textureRV.ReleaseAndGetAddressOf( );
	m_textureRV = textureResourceView;
}
void DirectXMaterial::VertexShader(ID3D11VertexShader* vertexShader)
{
	m_vertexShader = vertexShader;
}
void DirectXMaterial::PixelShader(ID3D11PixelShader* pixelShader)
{
	m_pixelShader = pixelShader;
}
#endif