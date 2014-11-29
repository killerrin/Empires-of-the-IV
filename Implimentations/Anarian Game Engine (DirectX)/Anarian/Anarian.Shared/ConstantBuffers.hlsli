#include "ShaderInputs.hlsli"
#include "GenericStructures.hlsli"

SamplerState samplerState : register(s0);
Texture2D texture0 : register(t0);
Texture2D texture1 : register(t1);
Texture2D texture2 : register(t2);
Texture2D texture3 : register(t3);

cbuffer ConstantBufferChangesOnResize : register(b0)
{
	matrix cameraProjection;
};

cbuffer ConstantBufferChangesEveryFrame : register(b1)
{
	matrix cameraView;
};

cbuffer ConstantBufferChangesEveryPrim : register (b2)
{
	matrix modelWorldPosition;
	float4 meshColor;
	float4 diffuseColor;
	float4 specularColor;
	float  specularExponent;
};

cbuffer ConstantBufferChangesEveryLevel : register(b3)
{
	Light globalLight;
}


