#pragma once

namespace Anarian
{
	class TinyObjectLoaderConverter
	{
	public:
		static bool LoadObj(
			_In_ std::string filename,
			_Out_ Anarian::IMeshObject** m_mesh,
			_Out_ Anarian::IMaterial** m_material,
			_In_opt_ std::string basePath = "");
	};
}