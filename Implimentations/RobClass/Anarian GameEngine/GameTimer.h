#pragma once
#ifndef GAME_TIMER_H
#define GAME_TIMER_H

#include <Windows.h>

namespace Anarian {
	class GameTimer
	{
	public:
		GameTimer();
		~GameTimer();

		float PlayingTime();            // Return the Elapsed time the Game has been active in seconds since Reset
		void PlayingTime(float time);   // Set the Elapsed playing time -- used for restarting in the middle of a game
		float DeltaTime();              // Return the Delta time between the last two updates

		LARGE_INTEGER CurrentTime() { return m_currentTime; };
		bool Active()				{ return m_active; };


		float TicksToSeconds(LARGE_INTEGER ticks) { return ((float)ticks.QuadPart) / m_secondsPerCount; }
		LARGE_INTEGER SecondsToTicks(float seconds) {
			LARGE_INTEGER result;
			result.QuadPart = (LONGLONG)(seconds * m_secondsPerCount);
			return result;
		}

		void Reset();
		void Start();
		void Stop();
		void Update();

	private:
		float			m_secondsPerCount;  // 1.0 / Frequency
		float			m_deltaTime;

		LARGE_INTEGER	m_baseTime;
		LARGE_INTEGER	m_pausedTime;
		LARGE_INTEGER	m_stopTime;
		LARGE_INTEGER	m_previousTime;
		LARGE_INTEGER	m_currentTime;

		bool			m_active;
	};
}
#endif