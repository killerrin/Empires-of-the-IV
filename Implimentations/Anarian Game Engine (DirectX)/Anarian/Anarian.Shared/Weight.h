#pragma once
namespace Anarian
{
	namespace Verticies
	{
		struct Weight
		{
			int						JointID;
			float					Bias;
			DirectX::XMFLOAT3       Position;
			DirectX::XMFLOAT3       Normal;

			Weight() {};
			Weight(int _jointID, float _bias, DirectX::XMFLOAT3 _position, DirectX::XMFLOAT3 _normal)
			{
				JointID = _jointID;
				Bias = _bias;
				Position = _position;
				Normal = _normal;
			};
		};
	}
}