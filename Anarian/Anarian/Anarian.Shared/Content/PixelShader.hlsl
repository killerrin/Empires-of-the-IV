#include "..\ConstantBuffers.hlsli"

// A pass-through function for the (interpolated) color data.
float4 main(PixelShaderInput input) : SV_TARGET
{
	//Load diffuse from diffuse map
	float4 diffuseMap = texture0.Sample(samplerState, input.textureUV);

	//Load bump from bump map
	float4 bumpMap = texture1.Sample(samplerState, input.textureUV);

	//Load gloss from gloss map
	float4 glossMap = texture2.Sample(samplerState, input.textureUV);

	// Perform Calculations
	// Expand the range of the normal value from (0, +1) to (-1, +1).
	//bumpMap = (bumpMap * 2.0f) - 1.0f;

	float4 finalColor = glossMap * input.diffuseColor;

	return finalColor;
}
