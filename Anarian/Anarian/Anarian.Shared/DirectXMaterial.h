#pragma once
#ifdef Anarian_DirectX_Mode
#include "IMaterial.h"
#include "ConstantBuffers.h"

namespace Anarian {
	class DirectXMaterial :
		public IMaterial
	{
		friend class MaterialFactory;
	private:
		Microsoft::WRL::ComPtr<ID3D11VertexShader>       m_vertexShader;
		Microsoft::WRL::ComPtr<ID3D11PixelShader>        m_pixelShader;
		Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_textureRV;

	public:
		DirectXMaterial();
		DirectXMaterial(
			Color meshColor,
			Color diffuseColor,
			Color specularColor,
			float specularExponent,
			ID3D11ShaderResourceView* textureResourceView,
			ID3D11VertexShader* vertexShader,
			ID3D11PixelShader* pixelShader
			);
		~DirectXMaterial();

		void CreateViews(
			ID3D11ShaderResourceView* textureResourceView,
			ID3D11VertexShader* vertexShader,
			ID3D11PixelShader* pixelShader);

		void Render(ID3D11DeviceContext *context, ConstantBufferChangesEveryPrim* cBuffer, int bufferIndex = 0);

		void Texture(ID3D11ShaderResourceView* textureResourceView);
		void VertexShader(ID3D11VertexShader* vertexShader);
		void PixelShader(ID3D11PixelShader* pixelShader);
	};
}
#endif