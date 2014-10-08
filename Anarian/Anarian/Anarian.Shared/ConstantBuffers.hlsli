SamplerState samplerState : register(s0);
Texture2D texture1 : register(t0);

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

// Per-vertex data used as input to the vertex shader.
struct VertexShaderInput
{
	float3 position : POSITION;	//float4 position : POSITION;	
	float3 normal : NORMAL;		//float4 normal : NORMAL;		
	float2 textureUV : TEXCOORD0;
};

// Per-pixel color data passed through the pixel shader.
struct PixelShaderInput
{
	float4 position : SV_POSITION;	//	float4 position : SV_POSITION;
	float2 textureUV : TEXCOORD0;	//	float4 color : COLOR0;
	float4 diffuseColor : TEXCOORD1;//						
};