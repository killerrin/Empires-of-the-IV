#pragma once
namespace Anarian {
	namespace Verticies {

		// A Generic Vertex Point which contains all potential 
		// Values needed for rendering.
		// ~~~~~~~~~~~~~l~~~~~~~~~~~~~
		// By default, a Shader Input Description is only created for 
		// Position, Normal, Tangent, Binormal and Texture Coordinates
		// Use of the other values in the shader will require a custom Shader Input Description
		struct PNTVertex
		{
			DirectX::XMFLOAT3 position;

			DirectX::XMFLOAT3 normal;
			DirectX::XMFLOAT3 tangent;
			DirectX::XMFLOAT3 binormal;

			DirectX::XMFLOAT2 textureCoordinate;


			//* Not Sent to Shader *//

			int m_startWeight;
			int m_weightCount;

			///--------- Constructors and Methods ---------\\\

			PNTVertex() {};
			PNTVertex(DirectX::XMFLOAT3 pos, DirectX::XMFLOAT3 norm, DirectX::XMFLOAT2 texCoord) 
			{
				position = pos;
				normal = norm;
				textureCoordinate = texCoord;
			};
			PNTVertex(DirectX::XMFLOAT3 pos, DirectX::XMFLOAT3 norm, DirectX::XMFLOAT3 tan, DirectX::XMFLOAT3 binorm, DirectX::XMFLOAT2 texCoord)
			{
				position = pos;

				normal = norm;
				tangent = tan;
				binormal = binorm;

				textureCoordinate = texCoord;
			};
		};

#ifdef 	Anarian_DirectX_Mode
		// The Shader Input Description for PNTVertex utilizing
		// Position, Normal, Tangent, Binormal, Texture Coordinates
		// Use of the other values in the shader will require a custom Shader Input Description
		static const D3D11_INPUT_ELEMENT_DESC PNTVertexLayout[] =
		{
			{ "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
			
			{ "NORMAL", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
			{ "TANGENT", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 24, D3D11_INPUT_PER_VERTEX_DATA, 0 },
			{ "BINORMAL", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 36, D3D11_INPUT_PER_VERTEX_DATA, 0 },

			{ "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 48, D3D11_INPUT_PER_VERTEX_DATA, 0 },
		};
#endif
	}
}

