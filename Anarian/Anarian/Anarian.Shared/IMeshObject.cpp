#include "pch.h"
#include "IMeshObject.h"

using namespace Anarian;

void IMeshObject::CalculateModelVectors()
{
	for (int x = 0; x < m_vertices.size(); x++) {
		int faceCount, i, index;
		Anarian::Verticies::PNTVertex vertex1, vertex2, vertex3;
		DirectX::XMFLOAT3 tangent, binormal, normal;

		// Calculate the number of faces in the model.
		faceCount = m_vertices[x].size() / 3;

		// Initialize the index to the model data.
		index = 0;

		// Go through all the faces and calculate the the tangent, binormal, and normal vectors.
		for (i = 0; i < faceCount; i++) {
			// Get the three vertices for this face from the model.
			vertex1 = m_vertices[x][index];
			index++;

			vertex2 = m_vertices[x][index];
			index++;

			vertex3 = m_vertices[x][index];
			index++;

			// Calculate the tangent and binormal of that face.
			CalculateTangentBinormal(vertex1, vertex2, vertex3, tangent, binormal);

			// Calculate the new normal using the tangent and binormal.
			CalculateNormal(tangent, binormal, normal);

			// Store the normal, tangent, and binormal for this face back in the model structure.
			m_vertices[x][index - 1].tangent = tangent;
			m_vertices[x][index - 2].tangent = tangent;
			m_vertices[x][index - 3].tangent = tangent;

			m_vertices[x][index - 1].binormal = binormal;
			m_vertices[x][index - 2].binormal = binormal;
			m_vertices[x][index - 3].binormal = binormal;

			//m_vertices[x][index - 1].normal = normal;
			//m_vertices[x][index - 2].normal = normal;
			//m_vertices[x][index - 3].normal = normal;
		}
	}
}

void IMeshObject::CalculateTangentBinormal(
	Anarian::Verticies::PNTVertex vertex1, Anarian::Verticies::PNTVertex vertex2, Anarian::Verticies::PNTVertex vertex3,
	DirectX::XMFLOAT3& tangent, DirectX::XMFLOAT3& binormal)
{
	float vector1[3], vector2[3];
	float tuVector[2], tvVector[2];
	float den;
	float length;

	// Calculate the two vectors for this face.
	vector1[0] = vertex2.position.x - vertex1.position.x;
	vector1[1] = vertex2.position.y - vertex1.position.y;
	vector1[2] = vertex2.position.z - vertex1.position.z;

	vector2[0] = vertex3.position.x - vertex1.position.x;
	vector2[1] = vertex3.position.y - vertex1.position.y;
	vector2[2] = vertex3.position.z - vertex1.position.z;

	// Calculate the tu and tv texture space vectors.
	tuVector[0] = vertex2.textureCoordinate.x - vertex1.textureCoordinate.x;
	tvVector[0] = vertex2.textureCoordinate.y - vertex1.textureCoordinate.y;

	tuVector[1] = vertex3.textureCoordinate.x - vertex1.textureCoordinate.x;
	tvVector[1] = vertex3.textureCoordinate.y - vertex1.textureCoordinate.y;

	// Calculate the denominator of the tangent/binormal equation.
	den = 1.0f / (tuVector[0] * tvVector[1] - tuVector[1] * tvVector[0]);

	// Calculate the cross products and multiply by the coefficient to get the tangent and binormal.
	tangent.x = (tvVector[1] * vector1[0] - tvVector[0] * vector2[0]) * den;
	tangent.y = (tvVector[1] * vector1[1] - tvVector[0] * vector2[1]) * den;
	tangent.z = (tvVector[1] * vector1[2] - tvVector[0] * vector2[2]) * den;

	binormal.x = (tuVector[0] * vector2[0] - tuVector[1] * vector1[0]) * den;
	binormal.y = (tuVector[0] * vector2[1] - tuVector[1] * vector1[1]) * den;
	binormal.z = (tuVector[0] * vector2[2] - tuVector[1] * vector1[2]) * den;

	// Calculate the length of this normal.
	length = sqrt((tangent.x * tangent.x) + (tangent.y * tangent.y) + (tangent.z * tangent.z));

	// Normalize the normal and then store it
	tangent.x = tangent.x / length;
	tangent.y = tangent.y / length;
	tangent.z = tangent.z / length;

	// Calculate the length of this normal.
	length = sqrt((binormal.x * binormal.x) + (binormal.y * binormal.y) + (binormal.z * binormal.z));

	// Normalize the normal and then store it
	binormal.x = binormal.x / length;
	binormal.y = binormal.y / length;
	binormal.z = binormal.z / length;

	return;
}

void IMeshObject::CalculateNormal(DirectX::XMFLOAT3 tangent, DirectX::XMFLOAT3 binormal, DirectX::XMFLOAT3& normal)
{
	float length;

	// Calculate the cross product of the tangent and binormal which will give the normal vector.
	normal.x = (tangent.y * binormal.z) - (tangent.z * binormal.y);
	normal.y = (tangent.z * binormal.x) - (tangent.x * binormal.z);
	normal.z = (tangent.x * binormal.y) - (tangent.y * binormal.x);

	// Calculate the length of the normal.
	length = sqrt((normal.x * normal.x) + (normal.y * normal.y) + (normal.z * normal.z));

	// Normalize the normal.
	normal.x = normal.x / length;
	normal.y = normal.y / length;
	normal.z = normal.z / length;

	return;
}