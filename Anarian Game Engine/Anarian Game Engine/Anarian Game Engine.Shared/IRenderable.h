#pragma once
namespace Anarian{
	class IRenderable
	{
	public:
		IRenderable();
		virtual ~IRenderable();

		virtual void Render();
	};
}
