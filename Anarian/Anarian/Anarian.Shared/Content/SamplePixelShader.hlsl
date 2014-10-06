SamplerState samplerState;
Texture2D texture1;

// Per-pixel color data passed through the pixel shader.
struct PixelShaderInput
{
	float4 position : SV_POSITION;	//	float4 position : SV_POSITION;
	float2 textureUV : TEXCOORD0;	//	float4 color : COLOR0;
	float4 diffuseColor : TEXCOORD1;//						
};

// A pass-through function for the (interpolated) color data.
float4 main(PixelShaderInput input) : SV_TARGET
{
	return input.diffuseColor;
}
