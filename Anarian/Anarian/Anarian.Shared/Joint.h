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

			Weight() {};
			Weight(int _jointID, float _bias, DirectX::XMFLOAT3 _position)
			{
				JointID = _jointID;
				Bias = _bias;
				Position = _position;
			};
		};
	}
}