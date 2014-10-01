#pragma once
namespace Anarian {
	class IMaterial
	{
	protected:
		/*Vector4 m_meshColor;*/
		/*Vector4 m_diffuseColor;*/
		/*Vector4 m_specularColor;*/
		float m_specularExponent;
	public:
		IMaterial();
		virtual ~IMaterial();
	};
}