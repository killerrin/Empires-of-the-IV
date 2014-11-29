// Generic Structures
struct Light
{
	// Directional Light utilizes
	//// direction, ambient, diffuse

	// Point Light utilizes
	//// position, range, att, ambient, diffuse

	// Spotlight utilizes
	//// position, range, direction, cone, att, ambient, diffuse

	/// ----------------------------------------------
	/// ----------------------------------------------
	/// ----------------------------------------------

	float3	position;
	float	range;

	float3	direction;
	float	cone;

	float3	att;

	/// ----------------------------------------------
	/// Pads are used to ensure the Constant Buffer fits into 16 Bytes (4D Vectors)
	float	m_pad2;
	/// ----------------------------------------------

	float4	ambient;
	float4	diffuse;

	// Look at LightType.h for type values
	int		lightType;
};
