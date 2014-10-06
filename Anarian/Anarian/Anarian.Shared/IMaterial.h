#pragma once
#include "Color.h"

namespace Anarian {
	class IMaterial
	{
		friend class MaterialFactory;
	protected:
		IMaterial() { };
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

		Color m_meshColor;
		Color m_diffuseColor;
		Color m_specularColor;
		float m_specularExponent;
	public:
		virtual ~IMaterial() { };
	};
}