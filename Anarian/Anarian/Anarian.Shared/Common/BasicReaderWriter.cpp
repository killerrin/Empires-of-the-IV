//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "BasicReaderWriter.h"

using namespace Microsoft::WRL;
using namespace Windows::Storage;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation;
using namespace Windows::ApplicationModel;
using namespace concurrency;

BasicReaderWriter::BasicReaderWriter()
{
	m_location = Package::Current->InstalledLocation;
}

BasicReaderWriter::BasicReaderWriter(
	_In_ Windows::Storage::StorageFolder^ folder
	)
{
	m_location = folder;
	Platform::String^ path = m_location->Path;
	if (path->Length() == 0)
	{
		// Applications are not permitted to access certain
		// folders, such as the Documents folder, using this
		// code path.  In such cases, the Path property for
		// the folder will be an empty string.
		throw ref new Platform::FailureException();
	}
}

Platform::Array<byte>^ BasicReaderWriter::ReadData(
	_In_ Platform::String^ filename
	)
{
	CREATEFILE2_EXTENDED_PARAMETERS extendedParams = {0};
	extendedParams.dwSize = sizeof(CREATEFILE2_EXTENDED_PARAMETERS);
	extendedParams.dwFileAttributes = FILE_ATTRIBUTE_NORMAL;
	extendedParams.dwFileFlags = FILE_FLAG_SEQUENTIAL_SCAN;
	extendedParams.dwSecurityQosFlags = SECURITY_ANONYMOUS;
	extendedParams.lpSecurityAttributes = nullptr;
	extendedParams.hTemplateFile = nullptr;

	Wrappers::FileHandle file(
		CreateFile2(
			filename->Data(),
			GENERIC_READ,
			FILE_SHARE_READ,
			OPEN_EXISTING,
			&extendedParams
			)
		);
	if (file.Get() == INVALID_HANDLE_VALUE)
	{
		throw ref new Platform::FailureException();
	}

	FILE_STANDARD_INFO fileInfo = {0};
	if (!GetFileInformationByHandleEx(
		file.Get(),
		FileStandardInfo,
		&fileInfo,
		sizeof(fileInfo)
		))
	{
		throw ref new Platform::FailureException();
	}

	if (fileInfo.EndOfFile.HighPart != 0)
	{
		throw ref new Platform::OutOfMemoryException();
	}

	Platform::Array<byte>^ fileData = ref new Platform::Array<byte>(fileInfo.EndOfFile.LowPart);

	if (!ReadFile(
		file.Get(),
		fileData->Data,
		fileData->Length,
		nullptr,
		nullptr
		))
	{
		throw ref new Platform::FailureException();
	}

	return fileData;
}

task<Platform::Array<byte>^> BasicReaderWriter::ReadDataAsync(
	_In_ Platform::String^ filename
	)
{
	return task<StorageFile^>(m_location->GetFileAsync(filename)).then([=](StorageFile^ file)
	{
		return FileIO::ReadBufferAsync(file);
	}).then([=](IBuffer^ buffer)
	{
		auto fileData = ref new Platform::Array<byte>(buffer->Length);
		DataReader::FromBuffer(buffer)->ReadBytes(fileData);
		return fileData;
	});
}

Platform::String^ BasicReaderWriter::ReadTextFile(_In_ Platform::String^ filename)
{
	///http://kooksta.wordpress.com/2013/02/01/ccx-async-reading-from-txt-files-that-are-in-the-xapinstalled-folder/
	using namespace Windows::Storage;
	auto folder = Windows::ApplicationModel::Package::Current->InstalledLocation;

	Platform::Array<byte>^ data = nullptr;
	std::wstring dir = folder->Path->ToString()->Data();
	auto fullpath = dir.append(L"/").append(filename->Data());

	std::ifstream file(fullpath, std::ios::in | std::ios::ate);
	// if opened read it in
	if (file.is_open()) {
		int length = (int)file.tellg();
		data = ref new Platform::Array<byte>(length);
		file.seekg(0, std::ios::beg);
		file.read(reinterpret_cast<char*>(data->Data), length);
		file.close();
	}

	std::wstring output;
	for (int i = 0; i < data->Length; i++)
		output += data[i];

	return ref new Platform::String(output.c_str());
}


uint32 BasicReaderWriter::WriteData(
	_In_ Platform::String^ filename,
	_In_ const Platform::Array<byte>^ fileData
	)
{
	CREATEFILE2_EXTENDED_PARAMETERS extendedParams = {0};
	extendedParams.dwSize = sizeof(CREATEFILE2_EXTENDED_PARAMETERS);
	extendedParams.dwFileAttributes = FILE_ATTRIBUTE_NORMAL;
	extendedParams.dwFileFlags = FILE_FLAG_SEQUENTIAL_SCAN;
	extendedParams.dwSecurityQosFlags = SECURITY_ANONYMOUS;
	extendedParams.lpSecurityAttributes = nullptr;
	extendedParams.hTemplateFile = nullptr;

	Wrappers::FileHandle file(
		CreateFile2(
			filename->Data(),
			GENERIC_WRITE,
			0,
			CREATE_ALWAYS,
			&extendedParams
			)
		);
	if (file.Get() == INVALID_HANDLE_VALUE)
	{
		throw ref new Platform::FailureException();
	}

	DWORD numBytesWritten;
	if (
		!WriteFile(
			file.Get(),
			fileData->Data,
			fileData->Length,
			&numBytesWritten,
			nullptr
			) ||
		numBytesWritten != fileData->Length
		)
	{
		throw ref new Platform::FailureException();
	}

	return numBytesWritten;
}

task<void> BasicReaderWriter::WriteDataAsync(
	_In_ Platform::String^ filename,
	_In_ const Platform::Array<byte>^ fileData
	)
{
	return task<StorageFile^>(m_location->CreateFileAsync(filename, CreationCollisionOption::ReplaceExisting)).then([=](StorageFile^ file)
	{
		FileIO::WriteBytesAsync(file, fileData);
	});
}
