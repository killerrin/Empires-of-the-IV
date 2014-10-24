#include "..\ConstantBuffers.hlsli"

// A pass-through function for the (interpolated) color data.
float4 main(PixelShaderInput input) : SV_TARGET
{
	//Load diffuse from diffuse map (Diffuse Map)
	float4 diffuseMap = texture0.Sample(samplerState, input.textureUV);

	//Load Specular from Specular Map
	float specularMap = texture1.Sample(samplerState, input.textureUV);

	//Load bump from bump map (Normal Map)
	float4 bumpMap = texture2.Sample(samplerState, input.textureUV);

	//Load gloss from gloss map
	float4 glossMap = texture3.Sample(samplerState, input.textureUV);

	//---------------------------------------------------------------
	//-- Perform Calculations
	// Diffuse Calculations
	float4 diffuse = diffuseMap * input.diffuseColor;

	// Bump Calculations
	// Expand the range of the normal value from (0, +1) to (-1, +1).
	bumpMap = (bumpMap * 2.0f) - 1.0f;

	float3 bumpNormal;
	float3 lightDir;
	float lightIntensity;
	float4 color;

	// Calculate the normal from the data in the bump map.
	bumpNormal = input.normal + bumpMap.x * input.tangent + bumpMap.y * input.binormal;
	
	// Normalize the resulting bump normal.
	bumpNormal = normalize(bumpNormal);
	
	// Invert the light direction for calculations.
	float3 lightDirection = float3(input.positionMod.x, input.positionMod.y, input.positionMod.z) - float3(20.0f, 50.0f, 30.0f);
	lightDir = -lightDirection;
	
	// Calculate the amount of light on this pixel based on the bump map normal value.
	lightIntensity = saturate(dot(bumpNormal, lightDir));
	
	// Determine the final diffuse color based on the diffuse color and the amount of light intensity.
	color = saturate(input.diffuseColor * lightIntensity);

	//---------------------------------------------------------------
	//-- Set the final color
	//float4 finalColor = glossMap * input.diffuseColor;
	float4 finalColor = diffuseMap;

		// For Pick Testing
	finalColor = input.diffuseColor;

	return finalColor;
}
