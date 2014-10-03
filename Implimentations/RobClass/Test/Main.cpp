#include <iostream>
using namespace std;

#include "Clock.h"
#include "GameTimer.h"
using namespace Anarian;

#include "LogFileManager.h"

int main()
{
	Clock::init();
	LogFileManager::Instance();

	GameTimer gameTime = GameTimer();
	gameTime.Start();

	for (int i = 0; i < 500; i++)
	{
		gameTime.Update();
		cout << "Clock: " << Clock::getCurrentTime() << endl;
		printf("PlayingTime: %f \n", gameTime.PlayingTime());
		printf("DeltaTime: %f \n", gameTime.DeltaTime());
		printf("\n");
	}


	cout << gameTime.SecondsToTicks(3.0f).QuadPart << endl;

	getchar();
	

	LogFileManager::Instance().LevelOfSeverity(LogFileManager::LogSeverity::LOG_NONE);
	LogFileManager::Instance().TraceToConsole(true);

	LogFileManager::Instance().Log(LogFileManager::LogSeverity::LOG_NONE, "None!");
	LogFileManager::Instance().Error("Error!");
	LogFileManager::Instance().Warn("Warn!");
	LogFileManager::Instance().Trace("Trace!");
	LogFileManager::Instance().Info("Info!");

	return 0;
}