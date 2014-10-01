#pragma once
namespace Anarian {
	class IMoveable
	{
	public:
		IMoveable();
		virtual ~IMoveable();

		virtual void MoveUp(float ammount);
		virtual void MoveDown(float ammount);
		virtual void MoveLeft(float ammount);
		virtual void MoveRight(float ammount);

		virtual void MoveUpLeft(float ammountx, float ammounty);
		virtual void MoveUpRight(float ammountx, float ammounty);
		virtual void MoveDownLeft(float ammountx, float ammounty);
		virtual void MoveDownRight(float ammountx, float ammounty);
	};
}