#pragma once
#ifdef Anarian_DirectX_Mode
#include "IMaterial.h"

namespace Anarian {
	class DirectXMaterial :
		public IMaterial
	{
	private:
		Microsoft::WRL::ComPtr<ID3D11VertexShader>       m_vertexShader;
		Microsoft::WRL::ComPtr<ID3D11PixelShader>        m_pixelShader;
		Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_textureRV;

	public:
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

		void SetTexture(ID3D11ShaderResourceView* textureResourceView);
	};
}
#endif