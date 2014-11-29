#include "pch.h"
#include "Ray.h"

using namespace Anarian;

Ray::Ray(DirectX::XMFLOAT3 _startPoint,
	DirectX::XMFLOAT3 _endPoint,
	DirectX::XMFLOAT3 _direction)
{
	startPoint = _startPoint;
	endPoint = _endPoint;
	direction = _direction;
}