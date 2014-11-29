#pragma once
#include "GameTimer.h"
namespace Anarian
{
	class IUpdatable
	{
	protected:
		IUpdatable() {};
	public:
		virtual ~IUpdatable() {};

		virtual void Update(GameTimer* gameTime) {};
	};
}