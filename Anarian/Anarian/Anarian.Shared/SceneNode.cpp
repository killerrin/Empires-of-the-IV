#include "pch.h"
#include "SceneNode.h"
using namespace Anarian;

SceneNode::SceneNode()
{
	m_children = std::vector<SceneNode>();
	m_siblings = std::vector<SceneNode>();
}

SceneNode::~SceneNode()
{
	m_children.clear();
	m_siblings.clear();
}
