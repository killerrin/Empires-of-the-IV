#include "..\ConstantBuffers.hlsli"

// Simple shader to do vertex processing on the GPU.
PixelShaderInput main(VertexShaderInput input)
{
	PixelShaderInput output;
	float4 pos = float4(input.position, 1.0f);

	float3 norm = input.normal; // float4(input.normal, 1.0f);
	float3 tang = input.tangent; // float4(input.tangent, 1.0f);
	float3 bitang = input.binormal; // float4(input.binormal, 1.0f);

	float4x4 world = modelWorldPosition;
	world = mul(world, cameraView);
	world = mul(world, cameraProjection);

	// Transform the vertex position into projected space.
	pos = mul(pos, world);

	norm = mul(norm, (float3x3)world);
	tang = mul(tang, (float3x3)world);
	bitang = mul(bitang, (float3x3)world);

	// Fill out the pixel shader input and pass it.
	output.position = pos;
	output.positionMod = pos;

	output.normal = normalize(norm);
	output.tangent = normalize(tang);
	output.binormal = normalize(bitang);

	output.textureUV = input.textureUV;
	output.diffuseColor = diffuseColor;

	return output;
}
