#include "..\ConstantBuffers.hlsli"

// Simple shader to do vertex processing on the GPU.
PixelShaderInput main(VertexShaderInput input)
{
	PixelShaderInput output;
	float4 pos = float4(input.position, 1.0f);

	// Transform the vertex position into projected space.
	pos = mul(pos, modelWorldPosition);
	pos = mul(pos, cameraView);
	pos = mul(pos, cameraProjection);
	output.position = pos;

	// Fill out the pixel shader input and pass it.
	output.position = pos;
	output.textureUV = input.textureUV;
	output.diffuseColor = diffuseColor;

	return output;
}
