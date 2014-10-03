#include "pch.h"
#include "IRenderer.h"
using namespace Anarian;

IRenderer::IRenderer(Color color)
{
	m_backgroundColor = color;
}


IRenderer::~IRenderer()
{
}

void IRenderer::Render()
{

}


void IRenderer::SetBackgroundColor(Color color)
{
	m_backgroundColor = color;
}