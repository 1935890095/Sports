using System;

namespace ZF.Core.Logging
{
	public interface ILogger
	{
		bool IsTraceEnabled { get; }

		bool IsDebugEnabled { get; }

		bool IsInfoEnabled { get; }

		bool IsWarnEnabled { get; }

		bool IsErrorEnabled { get; }

		bool IsFatalEnabled { get; }

		void Trace(string message, params object[] args);

		void Trace(Exception exception, string message, params object[] args);

		void Debug(string message, params object[] args);

		void Debug(Exception exception, string message, params object[] args);

		void Info(string message, params object[] args);

		void Info(Exception exception, string message, params object[] args);

		void Warn(string message, params object[] args);

		void Warn(Exception exception, string message, params object[] args);

		void Error(string message, params object[] args);

		void Error(Exception exception, string message, params object[] args);

		void Fatal(string message, params object[] args);

		void Fatal(Exception exception, string message, params object[] args);

		ILogger ForkChild(string logger_name);
	}
}
