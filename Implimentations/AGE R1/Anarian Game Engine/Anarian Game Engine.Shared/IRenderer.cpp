#include "pch.h"
#include "IRenderer.h"
using namespace Anarian;

IRenderer::IRenderer()
{
	m_backgroundColor = Color::CornFlowerBlue();
}


IRenderer::~IRenderer()
{
	//m_sceneManager = nullptr;
}

void IRenderer::Render()
{

}

void IRenderer::SetBackgroundColor(Color color)
{
	m_backgroundColor = color;
}
