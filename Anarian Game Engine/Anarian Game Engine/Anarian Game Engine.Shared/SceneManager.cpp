#include "pch.h"
#include "SceneManager.h"
using namespace Anarian;

SceneManager::SceneManager()
{
}


SceneManager::~SceneManager()
{
}

IScene* SceneManager::CurrentScene()
{
	return m_currentScene;
}
void SceneManager::CurrentScene(IScene* scene)
{
	delete m_currentScene;
	m_currentScene = scene;
}