#include "pch.h"

#if Anarian_DirectX_Mode
#include "Content\Sample3DSceneRenderer.h"
using namespace DirectX;
#endif

#include "RendererFactory.h"
using namespace Anarian;

RendererFactory* RendererFactory::m_instance;
RendererFactory* RendererFactory::Instance()
{
	if (m_instance == nullptr) m_instance = new RendererFactory();
	return m_instance;
}

RendererFactory::RendererFactory()
{
}

RendererFactory::~RendererFactory()
{
	//delete m_instance;
}

IRenderer* RendererFactory::ConstructRenderer(SceneManager* sceneManager, Color backgroundColor)
{
	IRenderer* renderer;

#if Anarian_DirectX_Mode
	renderer = new Sample3DSceneRenderer(sceneManager, backgroundColor);
#endif

	return renderer;
}