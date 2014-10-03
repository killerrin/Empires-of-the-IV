#pragma once

namespace Anarian {
	class LogFileManager
	{
	public:
		enum LogSeverity { LOG_NONE = 0, LOG_INFO = 1, LOG_TRACE = 2, LOG_WARN = 3, LOG_ERROR = 4 };

		static LogFileManager& Instance();
		~LogFileManager();

		void CloseLogFile();
		void SetLogFile(std::string filename);

		void Log(LogSeverity severity, std::string message);
		void Error(std::string message);
		void Warn(std::string message);
		void Trace(std::string message);
		void Info(std::string message);

	private:
		static LogFileManager*	m_instance;
		std::ofstream*			m_outStream;

		bool					m_traceToConsole;

		std::string				m_defaultFileName;
		LogSeverity				m_levelOfSeverity;

		LogFileManager();

	public:
		bool			TraceToConsole()						{ return m_traceToConsole; };
		void			TraceToConsole(bool b)					{ m_traceToConsole = b; };

		std::string		DefaultFileName()						{ return m_defaultFileName; };
		void			DefaultFileName(std::string filename)	{ m_defaultFileName = filename; };

		LogSeverity		LevelOfSeverity()						{ return m_levelOfSeverity; };
		void			LevelOfSeverity(LogSeverity severity)	{ m_levelOfSeverity = severity; };


	};
}