#pragma once
#include "Color.h"

namespace Anarian {
	class IRenderer
	{
		friend class RendererFactory;
	protected:
		IRenderer(Color color) { m_backgroundColor = color; };

		Color m_backgroundColor;
	public:
		virtual ~IRenderer() { };
		virtual void Render() { };

		void SetBackgroundColor(Color color) { m_backgroundColor = color; };
	};
}