#pragma once
namespace Anarian {
	class IMoveable
	{
	protected:
		IMoveable() {};

	public:
		virtual ~IMoveable() {};

		virtual void MoveUp(float ammount) {};
		virtual void MoveDown(float ammount) {};
		virtual void MoveLeft(float ammount) {};
		virtual void MoveRight(float ammount) {};

		virtual void MoveUpLeft(float ammountX, float ammountY) {};
		virtual void MoveUpRight(float ammountX, float ammountY) {};
		virtual void MoveDownLeft(float ammountX, float ammountY) {};
		virtual void MoveDownRight(float ammountX, float ammountY) {};
	};
}