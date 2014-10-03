//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "GameRenderer.h"

#include "Game.h"

#include "Level1.h"
#include "Level2.h"
#include "Level3.h"
#include "Level4.h"
#include "Level5.h"
#include "Level6.h"
#include "Animate.h"
#include "Sphere.h"
#include "Cylinder.h"
#include "Face.h"
#include "MediaReader.h"

using namespace concurrency;
using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel::Store;
using namespace Windows::Storage;
using namespace Windows::UI::Core;


//----------------------------------------------------------------------

Game::Game():
	m_gameActive(false)
{
}

//----------------------------------------------------------------------

void Game::Initialize(
	_In_ MoveLookController^ controller,
	_In_ GameRenderer^ renderer
	)
{
	// This method is expected to be called as an asynchronous task.
	// Care should be taken to not call rendering methods on the
	// m_renderer as this would result in the D3D Context being
	// used in multiple threads, which is not allowed.

	m_controller = controller;
	m_renderer = renderer;

	m_audioController = ref new Audio;
	m_audioController->CreateDeviceIndependentResources();

	InitializeGameConfig();

	m_objects = std::vector<GameObject^>();
	m_renderObjects = std::vector<GameObject^>();

	m_savedState = ref new PersistentState();
	m_savedState->Initialize(ApplicationData::Current->LocalSettings->Values, "Game");

	m_timer = ref new GameTimer();

	// Create a sphere primitive to represent the player.
	// The sphere will be used to handle collisions and constrain the player in the world.
	// It is not rendered so it is not added to the list of render objects.
	// It is added to the object list so it will be included in intersection calculations.
	m_player = ref new Sphere(XMFLOAT3(0.0f, -1.3f, 4.0f), 0.2f);
	m_objects.push_back(m_player);
	m_player->Active(true);

	m_camera = Camera::MainCamera();
	m_camera->SetProjParams(XM_PI / 2, 1.0f, 0.01f, 100.0f);
	m_camera->SetViewParams(
		m_player->Position(),            // Eye point in world coordinates.
		XMFLOAT3 (0.0f, 0.7f, 0.0f),     // Look at point in world coordinates.
		XMFLOAT3 (0.0f, 1.0f, 0.0f)      // The Up vector for the camera.
		);

	m_controller->Pitch(m_camera->Pitch());
	m_controller->Yaw(m_camera->Yaw());

	// Instantiate a set of primitives to represent the containing world. These objects
	// maintain the geometry and material properties of the walls, floor and ceiling.
	// The TargetId is used to identify the world objects so that the right geometry
	// and textures can be associated with them later after those resources have
	// been created.
	GameObject^ world = ref new GameObject();
	world->TargetId(GameConstants::WorldFloorId);
	world->Active(true);
	m_renderObjects.push_back(world);

	world = ref new GameObject();
	world->TargetId(GameConstants::WorldCeilingId);
	world->Active(true);
	m_renderObjects.push_back(world);

	world = ref new GameObject();
	world->TargetId(GameConstants::WorldWallsId);
	world->Active(true);
	m_renderObjects.push_back(world);

	// Instantiate the Cylinders for use in the various game levels.
	// Each cylinder has a different initial position, radius and direction vector,
	// but share a common set of material properties.
	for (int a = 0; a < GameConstants::MaxCylinders; a++)
	{
		Cylinder^ cylinder;
		switch (a)
		{
		case 0:
			cylinder = ref new Cylinder(XMFLOAT3(-2.0f, -3.0f, 0.0f), 0.25f, XMFLOAT3(0.0f, 6.0f, 0.0f));
			break;
		case 1:
			cylinder = ref new Cylinder(XMFLOAT3(2.0f, -3.0f, 0.0f), 0.25f, XMFLOAT3(0.0f, 6.0f, 0.0f));
			break;
		case 2:
			cylinder = ref new Cylinder(XMFLOAT3(0.0f, -3.0f, -2.0f), 0.25f, XMFLOAT3(0.0f, 6.0f, 0.0f));
			break;
		case 3:
			cylinder = ref new Cylinder(XMFLOAT3(-1.5f, -3.0f, -4.0f), 0.25f, XMFLOAT3(0.0f, 6.0f, 0.0f));
			break;
		case 4:
			cylinder = ref new Cylinder(XMFLOAT3(1.5f, -3.0f, -4.0f), 0.50f, XMFLOAT3(0.0f, 6.0f, 0.0f));
			break;
		}
		cylinder->Active(true);
		m_objects.push_back(cylinder);
		m_renderObjects.push_back(cylinder);
	}

	MediaReader^ mediaReader = ref new MediaReader;

	//auto targetHitSound = mediaReader->LoadMedia("Assets\\hit.mp3");
	auto targetHitSound = mediaReader->LoadMedia("Assets\\hit.wav");

	// Instantiate the targets for use in the game.
	// Each target has a different initial position, size and orientation,
	// but share a common set of material properties.
	// The target is defined by a position and two vectors that define both
	// the plane of the target in world space and the size of the parallelogram
	// based on the lengths of the vectors.
	// Each target is assigned a number for identification purposes.
	// The Target ID number is 1 based.
	// All targets have the same material properties.
	for (int a = 1; a < GameConstants::MaxTargets; a++)
	{
		Face^ target;
		switch (a)
		{
		case 1:
			target = ref new Face(XMFLOAT3(-2.5f, -1.0f, -1.5f), XMFLOAT3(-1.5f, -1.0f, -2.0f), XMFLOAT3(-2.5f, 1.0f, -1.5f));
			break;
		case 2:
			target = ref new Face(XMFLOAT3(-1.0f, 1.0f, -3.0f), XMFLOAT3(0.0f, 1.0f, -3.0f), XMFLOAT3(-1.0f, 2.0f, -3.0f));
			break;
		case 3:
			target = ref new Face(XMFLOAT3(1.5f, 0.0f, -3.0f), XMFLOAT3(2.5f, 0.0f, -2.0f), XMFLOAT3(1.5f, 2.0f, -3.0f));
			break;
		case 4:
			target = ref new Face(XMFLOAT3(-2.5f, -1.0f, -5.5f), XMFLOAT3(-0.5f, -1.0f, -5.5f), XMFLOAT3(-2.5f, 1.0f, -5.5f));
			break;
		case 5:
			target = ref new Face(XMFLOAT3(0.5f, -2.0f, -5.0f), XMFLOAT3(1.5f, -2.0f, -5.0f), XMFLOAT3(0.5f, 0.0f, -5.0f));
			break;
		case 6:
			target = ref new Face(XMFLOAT3(1.5f, -2.0f, -5.5f), XMFLOAT3(2.5f, -2.0f, -5.0f), XMFLOAT3(1.5f, 0.0f, -5.5f));
			break;
		case 7:
			target = ref new Face(XMFLOAT3(0.0f, 0.0f, 0.0f), XMFLOAT3(0.5f, 0.0f, 0.0f), XMFLOAT3(0.0f, 0.5f, 0.0f));
			break;
		case 8:
			target = ref new Face(XMFLOAT3(0.0f, 0.0f, 0.0f), XMFLOAT3(0.5f, 0.0f, 0.0f), XMFLOAT3(0.0f, 0.5f, 0.0f));
			break;
		case 9:
			target = ref new Face(XMFLOAT3(0.0f, 0.0f, 0.0f), XMFLOAT3(0.5f, 0.0f, 0.0f), XMFLOAT3(0.0f, 0.5f, 0.0f));
			break;
		}

		target->Target(true);
		target->TargetId(a);
		target->Active(true);
		target->HitSound(ref new SoundEffect());
		target->HitSound()->Initialize(
			m_audioController->SoundEffectEngine(),
			mediaReader->GetOutputWaveFormatEx(),
			targetHitSound
			);

		m_objects.push_back(target);
		m_renderObjects.push_back(target);
	}

	if (!m_gameConfig.isTrial)
	{

	}

	// Load the currentScore for saved state if it exists.
	LoadState();

	m_controller->Active(false);
}

//----------------------------------------------------------------------

void Game::LoadGame()
{
	m_player->Position(XMFLOAT3 (0.0f, -1.3f, 4.0f));

	m_camera->SetViewParams(
		m_player->Position(),            // Eye point in world coordinates.
		XMFLOAT3 (0.0f, 0.7f, 0.0f),     // Look at point in world coordinates.
		XMFLOAT3 (0.0f, 1.0f, 0.0f)      // The Up vector for the camera.
		);

	m_controller->Pitch(m_camera->Pitch());
	m_controller->Yaw(m_camera->Yaw());
	m_gameActive = false;
	m_timer->Reset();
}

//----------------------------------------------------------------------

task<void> Game::LoadContextAsync()
{
	// Initialize the level and spin up the async loading of the rendering
	// resources for the level.
	// This will run in a separate thread, so for Direct3D 11, only Device
	// methods are allowed.  Any DeviceContext method calls need to be
	// done in FinalizeLoadLevel.

	return m_renderer->LoadContextResourcesAsync();
}

//----------------------------------------------------------------------

void Game::FinalizeLoadContext()
{
	// This method is called on the main thread, so Direct3D 11 DeviceContext
	// method calls are allowable here.

	// Finalize the Level loading.
	m_renderer->FinalizeLoadContextResources();
}

//----------------------------------------------------------------------

void Game::StartLevel()
{
	m_timer->Reset();
	m_timer->Start();

	m_controller->Active(true);
}

//----------------------------------------------------------------------

void Game::PauseGame()
{
	m_timer->Stop();
	SaveState();
}

//----------------------------------------------------------------------

void Game::ContinueGame()
{
	m_timer->Start();
	m_controller->Active(true);
}

//----------------------------------------------------------------------

GameState Game::RunGame()
{
	// This method is called to execute a single time interval for active game play.
	// It returns the resulting state of game play after the interval has been executed.

	m_timer->Update();

	// Time has not expired, so run one frame of game play.
	m_player->Velocity(m_controller->Velocity());
	m_camera->LookDirection(m_controller->LookDirection());

	Update();

	// Update the Camera with the player position updates from the dynamics calculations.
	m_camera->Eye(m_player->Position());
	m_camera->LookDirection(m_controller->LookDirection());

	return GameState::Active;
}

//----------------------------------------------------------------------

void Game::OnSuspending()
{
	m_audioController->SuspendAudio();
}

//----------------------------------------------------------------------

void Game::OnResuming()
{
	m_audioController->ResumeAudio();
}

//----------------------------------------------------------------------

void Game::Update()
{
	float timeTotal = m_timer->PlayingTime();
	float timeFrame = m_timer->DeltaTime();

#pragma region Animate Objects
	// Walk the list of objects looking for any objects that have an animation associated with it.
	// Update the position of the object based on evaluating the animation object with the current time.
	// Once the current time (timeTotal) is past the end of the animation time remove
	// the animation object since it is no longer needed.
	for (uint32 i = 0; i < m_objects.size(); i++)
	{
		if (m_objects[i]->AnimatePosition())
		{
			m_objects[i]->Position(m_objects[i]->AnimatePosition()->Evaluate(timeTotal));
			if (m_objects[i]->AnimatePosition()->IsFinished(timeTotal))
			{
				m_objects[i]->AnimatePosition(nullptr);
			}
		}
	}
#pragma endregion

	// If the elapsed time is too long, we slice up the time and handle physics over several
	// smaller time steps to avoid missing collisions.
	float timeLeft = timeFrame;
	float elapsedFrameTime;
	while (timeLeft > 0.0f)
	{
		elapsedFrameTime = min(timeLeft, GameConstants::Physics::FrameLength);
		timeLeft -= elapsedFrameTime;

		// Update the player position.
		m_player->Position(m_player->VectorPosition() + m_player->VectorVelocity() * elapsedFrameTime);

		// Do m_player / object intersections.
		for (uint32 a = 0; a < m_objects.size(); a++)
		{
			if (m_objects[a]->Active() && m_objects[a] != m_player)
			{
				XMFLOAT3 contact;
				XMFLOAT3 normal;

				if (m_objects[a]->IsTouching(m_player->Position(), m_player->Radius(), &contact, &normal))
				{
					// Player is in contact with m_objects[a].
					XMVECTOR oneToTwo;
					oneToTwo = -XMLoadFloat3(&normal);

					float impact = XMVectorGetX(
						XMVector3Dot (oneToTwo, m_player->VectorVelocity())
						);
					// Make sure that the player is actually headed towards the object. At grazing angles there
					// could appear to be an impact when the player is actually already hit and moving away.
					if (impact > 0.0f)
					{
						// Compute the normal and tangential components of the player's velocity.
						XMVECTOR velocityOneNormal = XMVector3Dot(oneToTwo, m_player->VectorVelocity()) * oneToTwo;
						XMVECTOR velocityOneTangent = m_player->VectorVelocity() - velocityOneNormal;

						// Compute post-collision velocity.
						m_player->Velocity(velocityOneTangent - velocityOneNormal);

						// Fix the positions so that the player is just touching the object.
						float distanceToMove = m_player->Radius();
						m_player->Position(XMLoadFloat3(&contact) - (oneToTwo * distanceToMove));
					}
				}
			}
		}
		{
			// Do collision detection of the player with the bounding world.
			XMFLOAT3 position = m_player->Position();
			XMFLOAT3 velocity = m_player->Velocity();
			float radius = m_player->Radius();

			m_player->Position(position);
			m_player->Velocity(velocity);
		}
	}
}

//----------------------------------------------------------------------

void Game::SaveState()
{
	// Save basic state of the game.
	m_savedState->SaveBool(":GameActive", m_gameActive);
	m_savedState->SaveXMFLOAT3(":PlayerPosition", m_player->Position());
	m_savedState->SaveXMFLOAT3(":PlayerLookDirection", m_controller->LookDirection());
}

//----------------------------------------------------------------------

void Game::LoadState()
{
	m_gameActive = m_savedState->LoadBool(":GameActive", m_gameActive);

	if (m_gameActive)
	{
		// Reload the current player position and set both the camera and the controller
		// with the current Look Direction.
		m_player->Position(
			m_savedState->LoadXMFLOAT3(":PlayerPosition", XMFLOAT3(0.0f, 0.0f, 0.0f))
			);
		m_camera->Eye(m_player->Position());
		m_camera->LookDirection(
			m_savedState->LoadXMFLOAT3(":PlayerLookDirection", XMFLOAT3(0.0f, 0.0f, 1.0f))
			);
		m_controller->Pitch(m_camera->Pitch());
		m_controller->Yaw(m_camera->Yaw());
	}
	else
	{
		// The game was not being played when it was last saved, so initialize to the beginning.
	}
}

//----------------------------------------------------------------------

void Game::SetCurrentLevelToSavedState()
{
	if (m_gameActive)
	{

	}
}

//----------------------------------------------------------------------

void Game::InitializeGameConfig()
{
	m_gameConfig.isTrial = true;
	m_gameConfig.autoFire = false;
	m_controller->AutoFire(false);
	m_gameConfig.backgroundAvailable[0] = true;
	for (int i = 1; i < GameConstants::MaxBackgroundTextures; i++)
	{
		m_gameConfig.backgroundAvailable[i] = false;
	}
}

//--------------------------------------------------------------------------------------

void Game::UpdateGameConfig(LicenseInformation^ licenseInformation)
{
	if (licenseInformation->IsActive)
	{
		m_gameConfig.isTrial = licenseInformation->IsTrial;
		if (!m_gameConfig.isTrial && licenseInformation->ProductLicenses->Lookup("AutoFire")->IsActive)
		{
			m_gameConfig.autoFire = true;
			m_controller->AutoFire(true);
		}
		else
		{
			m_gameConfig.autoFire = false;
			m_controller->AutoFire(false);
		}
		if (!m_gameConfig.isTrial && licenseInformation->ProductLicenses->Lookup("NightBackground")->IsActive)
		{
			m_gameConfig.backgroundAvailable[1] = true;
		}
		else
		{
			m_gameConfig.backgroundAvailable[1] = false;
		}
		if (!m_gameConfig.isTrial && licenseInformation->ProductLicenses->Lookup("DayBackground")->IsActive)
		{
			m_gameConfig.backgroundAvailable[2] = true;
		}
		else
		{
			m_gameConfig.backgroundAvailable[2] = false;
		}
	}
	else
	{
		// If no active license then default back to trial version.
		InitializeGameConfig();
	}

	if (m_gameConfig.isTrial)
	{
		
	}
	else
	{
		
	}
}

//--------------------------------------------------------------------------------------

