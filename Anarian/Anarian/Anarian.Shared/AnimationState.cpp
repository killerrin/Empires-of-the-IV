#include "pch.h"
#include "GameTimer.h"

#include "Joint.h"
#include "ModelAnimation.h"
#include "AnimationState.h"

#include "Common/DeviceResources.h"

using namespace Anarian;
using namespace Anarian::Verticies;
using namespace DirectX;

AnimationState::AnimationState(Anarian::Verticies::ModelAnimation* _anim)
{
	Reset();
	m_currentAnimation =		_anim;
}

AnimationState::~AnimationState()
{
}

void AnimationState::Reset()
{
	m_currentAnimation = nullptr;
	m_isLooping = false;
	m_isPlaying = false;
	m_currentFrame = 0;
	m_currentAnimationTime = 0.0f;
}

void AnimationState::Update(GameTimer* gameTime)
{
	m_currentAnimationTime += gameTime->DeltaTime();			// Update the current animation time

	if (m_currentAnimationTime > MaxAnimationTime()) {
		if (IsLooping())
			m_currentAnimationTime = 0.0f;
		else return;
	}

	// Which frame are we on
	float currentFrame = m_currentAnimationTime * FrameRate();
	int frame0 = floorf(currentFrame);
	int frame1 = frame0 + 1;

	// Make sure we don't go over the number of frames	
	if (frame0 == MaxFrames() - 1)
		frame1 = 0;

	float interpolation = currentFrame - frame0;	// Get the remainder (in time) between frame0 and frame1 to use as interpolation factor

	// Clear the interpolated skeleton before we recreate it
	m_interpolatedSkeleton.clear();
	
	// Compute the interpolated skeleton
	for (int i = 0; i < m_currentAnimation->baseFrameJoints.size(); i++) {
		Joint tempJoint;
		Joint joint0 = m_currentAnimation->frameSkeleton[frame0][i];		// Get the i'th joint of frame0's skeleton
		Joint joint1 = m_currentAnimation->frameSkeleton[frame1][i];		// Get the i'th joint of frame1's skeleton

		tempJoint.ParentID = joint0.ParentID;											// Set the tempJoints parent id
		
		// Turn the two quaternions into XMVECTORs for easy computations
		XMVECTOR joint0Orient = XMVectorSet(joint0.Orientation.x, joint0.Orientation.y, joint0.Orientation.z, joint0.Orientation.w);
		XMVECTOR joint1Orient = XMVectorSet(joint1.Orientation.x, joint1.Orientation.y, joint1.Orientation.z, joint1.Orientation.w);

		// Interpolate positions
		tempJoint.Position.x = joint0.Position.x + (interpolation * (joint1.Position.x - joint0.Position.x));
		tempJoint.Position.y = joint0.Position.y + (interpolation * (joint1.Position.y - joint0.Position.y));
		tempJoint.Position.z = joint0.Position.z + (interpolation * (joint1.Position.z - joint0.Position.z));

		// Interpolate orientations using spherical interpolation (Slerp)
		XMStoreFloat4(&tempJoint.Orientation, XMQuaternionSlerp(joint0Orient, joint1Orient, interpolation));

		m_interpolatedSkeleton.push_back(tempJoint);		// Push the joint back into our interpolated skeleton
	}

	m_currentFrame = frame1;
}
