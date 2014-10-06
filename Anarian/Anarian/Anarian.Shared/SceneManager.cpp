#include "pch.h"
#include "SceneManager.h"
using namespace Anarian;

SceneManager::SceneManager()
{
	m_currentScene = nullptr;
}

SceneManager::~SceneManager()
{
	delete m_currentScene;
}

IScene* SceneManager::GetCurrentScene()
{
	return m_currentScene;
}
void SceneManager::SetCurrentScene(IScene* currentScene)
{
	m_currentScene = currentScene;
}
