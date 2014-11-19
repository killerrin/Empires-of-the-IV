#pragma once
namespace Anarian
{
	class MD5LoaderConverter
	{
	public:
		static bool LoadMD5MeshAnim(
			_In_ std::string filename,
			_Out_ Anarian::Model** m_model,
			_Out_ Anarian::Verticies::ModelAnimation** m_anim,
			_In_ BasicLoader^ basicLoader
			);

		static bool LoadMD5Mesh(
			_In_ std::string filename,
			_Out_ Anarian::Model** m_model,
			_In_ BasicLoader^ basicLoader
			);

		static bool LoadMD5Animation(
			_In_ std::string filename,
			_Out_ Anarian::Verticies::ModelAnimation** m_anim,
			_In_ BasicLoader^ basicLoader,
			_In_ void* loadedModel3DRaw
			);
	};
}
