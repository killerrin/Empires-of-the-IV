#include "PrimitiveType.h"

namespace Anarian
{
	class ICollider
	{
	protected:
		PrimitiveType m_type;

		ICollider(PrimitiveType type) { m_type = type; };

	public:
		~ICollider() {};

		bool Collides(ICollider collider) {};
	};
}