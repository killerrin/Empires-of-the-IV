#pragma once
namespace Anarian
{
	class MD5LoaderConverter
	{
	public:
		static bool LoadMD5Mesh(
			_In_ std::string filename,
			_Out_ Anarian::Model** m_model,
			_In_ BasicLoader^ basicLoader);

		//static bool LoadMD5Animation(
		//	_In_ BasicLoader^ basicLoader
		//	) {};
	};
}
