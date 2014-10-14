#pragma once

namespace Anarian
{
	class TinyObjectLoaderConverter
	{
	public:
		static bool LoadObj(
			_In_ std::string basePath,
			_In_ std::string filename,
			_Out_ Anarian::IMeshObject** m_mesh,
			_Out_ Anarian::IMaterial** m_material,
			_In_ BasicLoader^ basicLoader
			);

	private:
		static void CalculateTangentBinormal(
			DirectX::XMFLOAT3 vertex1, DirectX::XMFLOAT3 vertex2, DirectX::XMFLOAT3 vertex3,
			DirectX::XMFLOAT3& tangent, DirectX::XMFLOAT3& binormal);
	};
}