//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

// Game:
// This is the main game class.  It controls game play logic and game state.
// Some of the key object classes used by Game are:
//     MoveLookController - for handling all input to control player movement, aiming,
//         and firing.
//     GameRenderer - for handling all graphics presentation.
//     Camera - for handling view projections.
//     Audio - for handling sound output.
// This class maintains several lists of objects:
//     m_ammo <Sphere> - is the list of the balls used to throw at targets.  Game
//         cycles through the list in a LRU fashion each time a ball is thrown by the player.
//     m_objects <GameObject> - is the list of all objects in the scene that participate in
//         game physics.  This includes m_player <Sphere> to represent the player in the scene.
//         The player object (m_player) is not visible in the scene so it is not rendered.
//     m_renderObjects <GameObject> - is the list of all objects in the scene that may be
//         rendered.  It includes both the m_ammo list, most of the m_objects list excluding m_player
//         object and the objects representing the bounding world.

#include "GameConstants.h"
#include "GameUIConstants.h"
#include "Audio.h"
#include "Camera.h"
#include "Level.h"
#include "GameObject.h"
#include "GameTimer.h"
#include "MoveLookController.h"
#include "PersistentState.h"
#include "Sphere.h"
#include "GameRenderer.h"

//--------------------------------------------------------------------------------------

enum class GameState
{
	Waiting,
	Active,
	LevelComplete,
	TimeExpired,
	GameComplete,
};

typedef struct
{
	Platform::String^ tag;
	int totalHits;
	int totalShots;
	int levelCompleted;
} HighScoreEntry;

typedef std::vector<HighScoreEntry> HighScoreEntries;

typedef struct
{
	bool isTrial;
	bool autoFire;
	bool backgroundAvailable[GameConstants::MaxBackgroundTextures];
} GameConfig;

//--------------------------------------------------------------------------------------

ref class GameRenderer;

ref class Game
{
internal:
	Game();

	void Initialize(
		_In_ MoveLookController^ controller,
		_In_ GameRenderer^ renderer
		);

	void LoadGame();
	concurrency::task<void> LoadContextAsync();
	void FinalizeLoadContext();
	void StartLevel();
	void PauseGame();
	void ContinueGame();
	GameState RunGame();
	void SetCurrentLevelToSavedState();

	void OnSuspending();
	void OnResuming();

	bool IsActivePlay()                         { return m_timer->Active(); }
	bool IsTrial()                              { return m_gameConfig.isTrial; }
	bool GameActive()                           { return m_gameActive; };
	Camera^ GameCamera()                        { return m_camera; };
	std::vector<GameObject^> RenderObjects()    { return m_renderObjects; };

	void UpdateGameConfig(Windows::ApplicationModel::Store::LicenseInformation^ licenseInformation);

private:

	void Update();

	void LoadState();
	void SaveState();
	void InitializeGameConfig();

	Audio^                                      m_audioController;
	MoveLookController^                         m_controller;
	GameRenderer^                               m_renderer;
	Camera^                                     m_camera;

	PersistentState^                            m_savedState;

	GameTimer^                                  m_timer;
	bool                                        m_gameActive;

	GameConfig                                  m_gameConfig;

	Sphere^                                     m_player;
	std::vector<GameObject^>                    m_objects;           // List of all objects to be included in intersection calculations.
	std::vector<GameObject^>                    m_renderObjects;     // List of all objects to be rendered.
};

