using System;

namespace XFX.Core.Logging
{
	public class NullLogger : ILogger
	{
		public static readonly NullLogger Instance = new NullLogger();

		public bool IsTraceEnabled => false;

		public bool IsDebugEnabled => false;

		public bool IsInfoEnabled => false;

		public bool IsWarnEnabled => false;

		public bool IsErrorEnabled => false;

		public bool IsFatalEnabled => false;

		public void Trace(string message, params object[] args)
		{
		}

		public void Trace(Exception exception, string message, params object[] args)
		{
		}

		public void Debug(string message, params object[] args)
		{
		}

		public void Debug(Exception exception, string message, params object[] args)
		{
		}

		public void Info(string message, params object[] args)
		{
		}

		public void Info(Exception exception, string message, params object[] args)
		{
		}

		public void Warn(string message, params object[] args)
		{
		}

		public void Warn(Exception exception, string message, params object[] args)
		{
		}

		public void Error(string message, params object[] args)
		{
		}

		public void Error(Exception exception, string message, params object[] args)
		{
		}

		public void Fatal(string message, params object[] args)
		{
		}

		public void Fatal(Exception exception, string message, params object[] args)
		{
		}

		public ILogger ForkChild(string logger_name)
		{
			return this;
		}
	}
}
