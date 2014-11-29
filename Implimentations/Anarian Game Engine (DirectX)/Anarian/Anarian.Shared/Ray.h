namespace Anarian
{
	struct Ray
	{
		DirectX::XMFLOAT3 startPoint;
		DirectX::XMFLOAT3 endPoint;
		DirectX::XMFLOAT3 direction;

		Ray(DirectX::XMFLOAT3 _startPoint,
			DirectX::XMFLOAT3 _endPoint,
			DirectX::XMFLOAT3 _direction);
	};
}