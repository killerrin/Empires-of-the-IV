#pragma once
#include "Color.h"

namespace Anarian {
	class IMaterial
	{
		friend class MaterialFactory;
		friend class Model;
	protected:
		IMaterial() { };
		IMaterial(
			Color meshColor,
			Color diffuseColor,
			Color specularColor,
			float specularExponent
			)
		{
			Name = "";
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
		std::string Name;

		virtual ~IMaterial() { };

		// Getter setters
		void SetMeshColor(Color color) { m_meshColor = color; };
		Color GetMeshColor() { return m_meshColor; };

		void SetDiffuseColor(Color color) { m_diffuseColor = color; };
		Color GetDiffuseColor() { return m_diffuseColor; };

		void SetSpecularColor(Color color) { m_specularColor = color; };
		Color GetSpecularColor() { return m_specularColor; };

		void SetSpecularExponent(float exponent) { m_specularExponent = exponent; };
		float GetSpecularExponent() { return m_specularExponent; };
	};
}