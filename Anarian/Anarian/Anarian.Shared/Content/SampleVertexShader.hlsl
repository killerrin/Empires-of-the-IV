// A constant buffer that stores the three basic column-major matrices for composing geometry.
cbuffer ModelViewProjectionConstantBuffer : register(b0)
{
	matrix model;
	matrix view;
	matrix projection;
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
	float4 position : SV_POSITION;
	float4 color : COLOR0;		//float2 textureUV : TEXCOORD0; 
								//float4 diffuseColor : TEXCOORD1;
};

// Simple shader to do vertex processing on the GPU.
PixelShaderInput main(VertexShaderInput input)
{
	PixelShaderInput output;
	float4 pos = float4(input.position, 1.0f);

	// Transform the vertex position into projected space.
	pos = mul(pos, model);
	pos = mul(pos, view);
	pos = mul(pos, projection);
	output.position = pos;

	// Pass the color through without modification.
	output.color = float4(1.0f, 0.0f, 0.0f, 1.0f); // input.normal, 1.0f);

	return output;
}
