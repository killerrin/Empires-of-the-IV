#include "PrimitiveType.h"

namespace Anarian
{
	class ICollider
	{
	protected:
		PrimitiveType m_type;

		ICollider(PrimitiveType type) { m_type = type; };
		~ICollider() {};

	public:
		bool Collides(ICollider collider) {};
	};
}