#pragma once
namespace Anarian
{
	namespace Verticies
	{
		struct Joint
		{
			std::wstring				Name;
			int						ParentID;
			DirectX::XMFLOAT3		Position;
			DirectX::XMFLOAT4		Orientation;

			Joint(){};
			Joint(std::wstring _name,
				int _parentID,
				DirectX::XMFLOAT3 _position,
				DirectX::XMFLOAT4 _orientation)
			{
				Name = _name;
				ParentID = _parentID;
				Position = _position;
				Orientation = _orientation;
			}
		};
	}
}