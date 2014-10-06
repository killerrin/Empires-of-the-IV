#include "pch.h"
#include "IMaterial.h"
#include "IMeshObject.h"
#include "GameObject.h"
using namespace Anarian;

GameObject::GameObject(): 
	IRenderable(),
	IUpdatable()
{
	m_materials = std::vector < IMaterial* >();
	m_meshes = std::vector<IMeshObject*>();
}


GameObject::~GameObject()
{
	//IRenderable::~IRenderable();
	//IUpdatable::~IUpdatable();
}

void GameObject::Update(GameTimer* gameTime)
{
	IUpdatable::Update(gameTime);
}

void GameObject::Render()
{
	IRenderable::Render();
}
