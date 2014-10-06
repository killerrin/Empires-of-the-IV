#pragma once
namespace Anarian {
	class SceneNode
	{
	private:
		std::vector<SceneNode> m_children;
		std::vector<SceneNode> m_siblings;
		/*GameObject* gameObjects*/
	public:
		SceneNode();
		~SceneNode();
	};
}