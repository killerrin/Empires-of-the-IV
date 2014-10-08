#include "..\ConstantBuffers.hlsli"

// A pass-through function for the (interpolated) color data.
float4 main(PixelShaderInput input) : SV_TARGET
{
	return input.diffuseColor * texture1.Sample(samplerState, input.textureUV);
}
