#pragma once
#ifdef Anarian_DirectX_Mode
#include "IRenderer.h"

namespace Anarian {
	class DirectXRenderer :
		public IRenderer
	{
	public:
		DirectXRenderer();
		~DirectXRenderer();

		void Render();
		void SetBackgroundColor(Color color);
	};
}
#endif