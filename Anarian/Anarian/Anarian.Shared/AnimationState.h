#pragma once
#include "ModelAnimation.h"
using namespace Anarian::Verticies;

namespace Anarian
{
	class AnimationState
	{
	private:
		ModelAnimation*		m_currentAnimation;

		std::vector<Joint>  m_interpolatedSkeleton;

		bool				m_isPlaying;
		bool				m_isLooping;

		int					m_currentFrame;
		float				m_currentAnimationTime;

	public:
		AnimationState(ModelAnimation* _anim = nullptr);
		~AnimationState();

		void Reset();


		// Helper Methods
		void				Play() { m_isPlaying = true; };
		void				Pause() { m_isPlaying = false; };
		bool				IsPlaying() { return m_isPlaying; };
		
		void				BeginLoop() { m_isLooping = true; };
		void				EndLoop() { m_isLooping = false; };
		void				SetLoop(bool loop) { m_isLooping = loop; };
		bool				IsLooping() { return m_isLooping; };

		int					MaxFrames() { return m_currentAnimation->numFrames; };
		float				FrameRate() { return m_currentAnimation->frameRate; };
		float				MaxAnimationTime() { return m_currentAnimation->totalAnimTime; };

		// Getter Setter
		float				GetCurrentAnimationTime() { return m_currentAnimationTime; };
		std::vector<Joint>	GetInterpolatedSkeleton() { return m_interpolatedSkeleton; };

		int					GetCurrentFrame() { return m_currentFrame; };
		void				SetCurrentFrame(int frame) { if (frame < MaxFrames()) m_currentFrame = frame; };

		ModelAnimation*		GetAnimation() { return m_currentAnimation; };
		void				SetAnimation(ModelAnimation* anim) { m_currentAnimation = anim; };
		bool				HasAnimation() {
			if (m_currentAnimation != nullptr) return true;
			return false;
		};

		// Methods
		void Update(GameTimer* gameTime);
	};
}