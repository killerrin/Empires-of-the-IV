#pragma once
#include "GameTimer.h"
namespace Anarian
{
	class IUpdatable
	{
	public:
		IUpdatable();
		virtual ~IUpdatable();

		virtual void Update(GameTimer* gameTime);
	};
}