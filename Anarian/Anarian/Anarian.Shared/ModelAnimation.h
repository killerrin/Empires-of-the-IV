#pragma once
#include "Joint.h"

namespace Anarian
{
	namespace Verticies
	{
		struct BoundingBox
		{
			DirectX::XMFLOAT3 min;
			DirectX::XMFLOAT3 max;
		};

		struct FrameData
		{
			int frameID;
			std::vector<float> frameData;
		};
		struct AnimJointInfo
		{
			std::wstring name;
			int parentID;

			int flags;
			int startIndex;
		};

		struct ModelAnimation
		{
			int numFrames;
			int numJoints;
			int frameRate;
			int numAnimatedComponents;

			float frameTime;
			float totalAnimTime;
			float currAnimTime;

			std::vector<AnimJointInfo> jointInfo;
			std::vector<BoundingBox> frameBounds;
			std::vector<Joint>	baseFrameJoints;
			std::vector<FrameData>	frameData;
			std::vector<std::vector<Joint>> frameSkeleton;
		};
	}
}