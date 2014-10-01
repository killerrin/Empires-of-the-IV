#include "pch.h"

#include <fstream>
#include <iomanip>
#include <string>

#include "LogFileManager.h"
using namespace Anarian;

LogFileManager*	LogFileManager::m_instance;

LogFileManager::LogFileManager()
{
	m_outStream = nullptr;
	m_defaultFileName = "logfile.txt";
	m_levelOfSeverity = LogSeverity::LOG_ERROR;
	m_traceToConsole = false;
}

LogFileManager& LogFileManager::Instance()
{
	if (m_instance == nullptr) m_instance = new LogFileManager();
	return *m_instance;
}


LogFileManager::~LogFileManager()
{
	CloseLogFile();
}


void LogFileManager::CloseLogFile()
{
	if (m_outStream == nullptr) return;

	m_outStream->close();
	delete m_outStream;
	m_outStream = nullptr;
}

void LogFileManager::SetLogFile(std::string filename)
{
	CloseLogFile();
	m_outStream = new std::ofstream(filename.c_str());
	m_levelOfSeverity = LogSeverity::LOG_ERROR;
}

void LogFileManager::Log(LogSeverity severity, std::string message)
{
	if (severity == LogSeverity::LOG_NONE ||
		m_levelOfSeverity == LogSeverity::LOG_NONE)
	{
		return;
	}

	if (severity >= m_levelOfSeverity)
	{
		// Store the value of severity so we can set it back to normal later
		LogSeverity t = m_levelOfSeverity;

		// If the m_outStream is null, we will open a new file.
		// Followed by writing our message out with a line delimiter
		if (m_outStream == nullptr)  { SetLogFile(m_defaultFileName); }
		(*m_outStream) << message << "\n";
		m_outStream->flush();


		// If able, print the message to the console we we can read it.
		if (m_traceToConsole){
			printf(message.c_str());
			printf("\n");
		}

		// Because m_outStream is going nullptr again every call, 
		// Reset m_levelOfSeverity back to the temp variable
		m_levelOfSeverity = t;
	}
}

void LogFileManager::Error(std::string message)
{
	Log(LogSeverity::LOG_ERROR, message);
}
void LogFileManager::Warn(std::string message)
{
	Log(LogSeverity::LOG_WARN, message);
}
void LogFileManager::Trace(std::string message)
{
	Log(LogSeverity::LOG_TRACE, message);
}
void LogFileManager::Info(std::string message)
{
	Log(LogSeverity::LOG_INFO, message);
}