#pragma once
namespace MD5Loader
{
	struct Vertex	//Overloaded Vertex Structure
	{
		Vertex(){}
		Vertex(float x, float y, float z,
			float u, float v,
			float nx, float ny, float nz,
			float tx, float ty, float tz)
			: pos(x, y, z), texCoord(u, v), normal(nx, ny, nz),
			tangent(tx, ty, tz){}

		DirectX::XMFLOAT3 pos;
		DirectX::XMFLOAT2 texCoord;
		DirectX::XMFLOAT3 normal;
		DirectX::XMFLOAT3 tangent;
		DirectX::XMFLOAT3 biTangent;

		// Will not be sent to shader
		int StartWeight;
		int WeightCount;
	};

	struct Joint
	{
		std::wstring name;
		int parentID;

		DirectX::XMFLOAT3 pos;
		DirectX::XMFLOAT4 orientation;
	};

	struct BoundingBox
	{
		DirectX::XMFLOAT3 min;
		DirectX::XMFLOAT3 max;
	};

	struct FrameData
	{
		int frameID;
		std::vector<float> frameData;
	};
	struct AnimJointInfo
	{
		std::wstring name;
		int parentID;

		int flags;
		int startIndex;
	};

	struct ModelAnimation
	{
		int numFrames;
		int numJoints;
		int frameRate;
		int numAnimatedComponents;

		float frameTime;
		float totalAnimTime;
		float currAnimTime;

		std::vector<AnimJointInfo> jointInfo;
		std::vector<BoundingBox> frameBounds;
		std::vector<Joint>	baseFrameJoints;
		std::vector<FrameData>	frameData;
		std::vector<std::vector<Joint>> frameSkeleton;
	};
	///////////////**************new**************////////////////////

	struct Weight
	{
		int jointID;
		float bias;
		DirectX::XMFLOAT3 pos;
		///////////////**************new**************////////////////////
		DirectX::XMFLOAT3 normal;
		///////////////**************new**************////////////////////
	};

	struct ModelSubset
	{
		int numTriangles;

		std::vector<Vertex> vertices;
		std::vector<DirectX::XMFLOAT3> jointSpaceNormals;
		std::vector<DWORD> indices;
		std::vector<Weight> weights;

		std::vector<DirectX::XMFLOAT3> positions;

		std::vector<std::wstring> texFileNameArray;
		
		ID3D11Buffer* vertBuff;
		ID3D11Buffer* indexBuff;
	};

	struct Model3D
	{
		int numSubsets;
		int numJoints;

		std::vector<Joint> joints;
		std::vector<ModelSubset> subsets;

		///////////////**************new**************////////////////////
		std::vector<ModelAnimation> animations;
		///////////////**************new**************////////////////////
	};

	bool LoadMD5Anim(std::wstring filename, Model3D& MD5Model);
	bool LoadMD5Model(std::wstring filename, Model3D& MD5Model);
}