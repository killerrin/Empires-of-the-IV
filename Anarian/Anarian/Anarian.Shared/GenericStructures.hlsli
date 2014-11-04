// Generic Structures
struct Light
{
	float3 direction;
	float  m_pad;

	float4 ambient;
	float4 diffuse;

	// Look at Light.h for type values
	int lightType;
};