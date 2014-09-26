#pragma once

#include "targetver.h"

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <windows.h>


//-- DirectX PCH
#include <wrl.h>
#include <wrl/client.h>
#include <d3d11_2.h>
#include <d2d1_2.h>
#include <d2d1effects_1.h>
#include <dwrite_2.h>
#include <wincodec.h>
#include <DirectXMath.h>

#include <xaudio2.h>
#include <xaudio2fx.h>

#include <mmreg.h>
#include <mfidl.h>
#include <mfapi.h>
#include <mfreadwrite.h>

#include <stdio.h>
#include <vector>
#include <memory>

#include <ppltasks.h>
#include <agile.h>
#include <concrt.h>

