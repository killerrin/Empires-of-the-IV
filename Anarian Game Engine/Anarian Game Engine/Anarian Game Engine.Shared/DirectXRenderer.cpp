#ifdef Anarian_DirectX_Mode
#include "pch.h"
#include "DirectXRenderer.h"
using namespace Anarian;
using namespace DirectX;

DirectXRenderer::DirectXRenderer()
	:IRenderer()
{

}


DirectXRenderer::~DirectXRenderer()
{
	IRenderer::~IRenderer();
}

void DirectXRenderer::Render()
{
	IRenderer::Render();
}

#endif