#pragma once
#include "Color.h"

namespace Anarian {
	class IRenderer
	{
	protected:
		Color m_backgroundColor;
	public:
		IRenderer(Color color);
		virtual ~IRenderer();

		virtual void Render();
		void SetBackgroundColor(Color color);
	};
}