#include "pch.h"
#ifdef Anarian_DirectX_Mode

#include "ConstantBuffers.h"
#include "DirectXMaterial.h"
using namespace Anarian;
using namespace DirectX;

DirectXMaterial::DirectXMaterial()
	: IMaterial()
{
	m_textureRV = std::vector<Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>>();
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

	m_textureRV = std::vector<Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>>();
	m_textureRV.push_back(textureResourceView);
}


DirectXMaterial::~DirectXMaterial()
{
	// Commented out as resources are going to be managed elsewhere to keep from having multiple
	// of the same assets stored in memory

	if (m_vertexShader != nullptr) {
		//m_vertexShader.ReleaseAndGetAddressOf();
	}
	if (m_pixelShader != nullptr) {
		//m_pixelShader.ReleaseAndGetAddressOf();
	}
	if (m_textureRV.size() != 0) {
		//m_textureRV.ReleaseAndGetAddressOf();
	}
	
	//IMaterial::~IMaterial();
}

void DirectXMaterial::CreateViews(
	ID3D11VertexShader* vertexShader,
	ID3D11PixelShader* pixelShader)
{
	m_vertexShader = vertexShader;
	m_pixelShader = pixelShader;
	
	//m_textureRV.push_back(textureResourceView);
}

void DirectXMaterial::Render(ID3D11DeviceContext *context, ConstantBufferChangesEveryPrim* cBuffer, int bufferIndex)
{
	// Set constant buffers here
	if (bufferIndex == 0) {
		cBuffer->meshColor = m_meshColor;
		cBuffer->specularColor = m_specularColor;
		cBuffer->specularPower = m_specularExponent;
		cBuffer->diffuseColor = m_diffuseColor;

		context->VSSetShader(m_vertexShader.Get(), nullptr, 0);
		context->PSSetShader(m_pixelShader.Get(), nullptr, 0);
	}

	// Set shader resources here
	for (int i = 0; i < m_textureRV.size(); i++) {
		context->PSSetShaderResources(i, 1, m_textureRV[i].GetAddressOf());
	}
}

void DirectXMaterial::SetVertexShader(ID3D11VertexShader* vertexShader)
{
	m_vertexShader = vertexShader;
}
void DirectXMaterial::SetPixelShader(ID3D11PixelShader* pixelShader)
{
	m_pixelShader = pixelShader;
}
void DirectXMaterial::AddTexture(ID3D11ShaderResourceView* textureResourceView)
{
	m_textureRV.push_back(textureResourceView);
}

#endif