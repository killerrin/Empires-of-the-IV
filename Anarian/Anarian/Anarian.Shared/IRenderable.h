#pragma once
namespace Anarian {
	class IRenderable 
	{
	protected:
		IRenderable() {};

	public:
		virtual ~IRenderable() {};
		virtual void Render() {};
	};
}