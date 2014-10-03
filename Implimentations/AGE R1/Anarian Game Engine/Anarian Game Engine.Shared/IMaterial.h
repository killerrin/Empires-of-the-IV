#pragma once
#include "Color.h"

namespace Anarian {
	class IMaterial
	{
	protected:
		Color m_meshColor;
		Color m_diffuseColor;
		Color m_specularColor;
		float m_specularExponent;
	public:
		IMaterial(
			Color meshColor,
			Color diffuseColor,
			Color specularColor,
			float specularExponent
			)
		{
			m_meshColor = meshColor;
			m_diffuseColor = diffuseColor;
			m_specularColor = specularColor;
			m_specularExponent = specularExponent;
		}
		virtual ~IMaterial();
	};
}