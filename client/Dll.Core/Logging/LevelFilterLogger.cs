using System;

namespace XFX.Core.Logging
{
	public abstract class LevelFilterLogger : ILogger
	{
		private LoggerLevel level;

		private string name = "unnamed";

		public LoggerLevel Level
		{
			get
			{
				return level;
			}
			set
			{
				level = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("logger new name");
				}
				name = value;
			}
		}

		public bool IsTraceEnabled => Level >= LoggerLevel.Trace;

		public bool IsDebugEnabled => level >= LoggerLevel.Debug;

		public bool IsInfoEnabled => Level >= LoggerLevel.Info;

		public bool IsWarnEnabled => Level >= LoggerLevel.Warn;

		public bool IsErrorEnabled => Level >= LoggerLevel.Error;

		public bool IsFatalEnabled => Level >= LoggerLevel.Fatal;

		protected LevelFilterLogger()
		{
		}

		protected LevelFilterLogger(string name)
		{
			this.name = name;
		}

		protected LevelFilterLogger(LoggerLevel level)
		{
			this.level = level;
		}

		protected LevelFilterLogger(string name, LoggerLevel level)
		{
			this.name = name;
			this.level = level;
		}

		public void Trace(string message, params object[] args)
		{
			if (IsTraceEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Trace, message, null);
				}
				else
				{
					Log(LoggerLevel.Trace, string.Format(message, args), null);
				}
			}
		}

		public void Trace(Exception exception, string message, params object[] args)
		{
			if (IsTraceEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Trace, message, exception);
				}
				else
				{
					Log(LoggerLevel.Trace, string.Format(message, args), exception);
				}
			}
		}

		public void Debug(string message, params object[] args)
		{
			if (IsDebugEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Debug, message, null);
				}
				else
				{
					Log(LoggerLevel.Debug, string.Format(message, args), null);
				}
			}
		}

		public void Debug(Exception exception, string message, params object[] args)
		{
			if (IsDebugEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Debug, message, exception);
				}
				else
				{
					Log(LoggerLevel.Debug, string.Format(message, args), exception);
				}
			}
		}

		public void Info(string message, params object[] args)
		{
			if (IsInfoEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Info, message, null);
				}
				else
				{
					Log(LoggerLevel.Info, string.Format(message, args), null);
				}
			}
		}

		public void Info(Exception exception, string message, params object[] args)
		{
			if (IsInfoEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Info, message, exception);
				}
				else
				{
					Log(LoggerLevel.Info, string.Format(message, args), exception);
				}
			}
		}

		public void Warn(string message, params object[] args)
		{
			if (IsWarnEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Warn, message, null);
				}
				else
				{
					Log(LoggerLevel.Warn, string.Format(message, args), null);
				}
			}
		}

		public void Warn(Exception exception, string message, params object[] args)
		{
			if (IsWarnEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Warn, message, exception);
				}
				else
				{
					Log(LoggerLevel.Warn, string.Format(message, args), exception);
				}
			}
		}

		public void Error(string message, params object[] args)
		{
			if (IsErrorEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Error, message, null);
				}
				else
				{
					Log(LoggerLevel.Error, string.Format(message, args), null);
				}
			}
		}

		public void Error(Exception exception, string message, params object[] args)
		{
			if (IsErrorEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Error, message, exception);
				}
				else
				{
					Log(LoggerLevel.Error, string.Format(message, args), exception);
				}
			}
		}

		public void Fatal(string message, params object[] args)
		{
			if (IsFatalEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Fatal, message, null);
				}
				else
				{
					Log(LoggerLevel.Fatal, string.Format(message, args), null);
				}
			}
		}

		public void Fatal(Exception exception, string message, params object[] args)
		{
			if (IsFatalEnabled)
			{
				if (args.Length == 0)
				{
					Log(LoggerLevel.Fatal, message, exception);
				}
				else
				{
					Log(LoggerLevel.Fatal, string.Format(message, args), exception);
				}
			}
		}

		protected abstract void Log(LoggerLevel logger_level, string logger_name, string message, Exception exception);

		private void Log(LoggerLevel logger_level, string message, Exception exception)
		{
			Log(logger_level, Name, message, exception);
		}

		public abstract ILogger ForkChild(string logger_name);
	}
}
