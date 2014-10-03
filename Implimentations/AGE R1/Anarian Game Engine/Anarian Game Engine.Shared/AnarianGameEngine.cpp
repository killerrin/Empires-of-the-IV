#include "pch.h"

#include "GameTimer.h"
#include "LogFileManager.h"

#include "IRenderer.h"
#include "ResourceManager.h"
#include "SceneManager.h"

#include "AnarianGameEngine.h"

using namespace Anarian;
using namespace DirectX;

AnarianGameEngine::AnarianGameEngine()
{
	m_gameTime = GameTimer();
}
AnarianGameEngine::~AnarianGameEngine()
{

}

void AnarianGameEngine::OnStart()
{
	m_gameTime.Start();
}
void AnarianGameEngine::OnEnd()
{

}

void AnarianGameEngine::Update()
{
	m_gameTime.Update();
}

void AnarianGameEngine::PreRender()
{

}
void AnarianGameEngine::Render()
{

}
void AnarianGameEngine::PostRender()
{

}
