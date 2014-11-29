// Per-vertex data used as input to the vertex shader.
struct VertexShaderInput
{
	float3 position : POSITION;

	float3 normal : NORMAL;
	float3 tangent : TANGENT;
	float3 binormal : BINORMAL;

	float2 textureUV : TEXCOORD0;
};

// Per-pixel color data passed through the pixel shader.
struct PixelShaderInput
{
	float4 position : SV_POSITION;

	float3 positionMod : POSITION;
	float3 normal : NORMAL;
	float3 tangent : TANGENT;
	float3 binormal : BINORMAL;

	float2 textureUV : TEXCOORD0;
	float4 diffuseColor : TEXCOORD1;
};