using System;
using UnityEngine;

namespace XFX.Core.Logging
{
	public class UnityLogger : LevelFilterLogger
	{
		public UnityLogger()
			: this(string.Empty, LoggerLevel.Debug)
		{
		}

		public UnityLogger(LoggerLevel level)
			: this(string.Empty, level)
		{
		}

		public UnityLogger(string name)
			: this(name, LoggerLevel.Debug)
		{
		}

		public UnityLogger(string name, LoggerLevel level)
			: base(name, level)
		{
		}

		protected override void Log(LoggerLevel logger_level, string logger_name, string message, Exception exception)
		{
			switch (logger_level)
			{
			case LoggerLevel.Info:
			case LoggerLevel.Debug:
			case LoggerLevel.Trace:
				UnityEngine.Debug.Log((object)$"[{logger_level}] '{logger_name}' {message}");
				break;
			case LoggerLevel.Warn:
				UnityEngine.Debug.LogWarning((object)$"[{logger_level}] '{logger_name}' {message}");
				break;
			case LoggerLevel.Fatal:
			case LoggerLevel.Error:
				UnityEngine.Debug.LogError((object)$"[{logger_level}] '{logger_name}' {message}");
				break;
			}
			if (exception != null)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}

		public override ILogger ForkChild(string logger_name)
		{
			if (logger_name == null)
			{
				throw new ArgumentNullException("logger_name", "To create a child logger you must supply a non null name");
			}
			return new UnityLogger(base.Name + "." + logger_name, base.Level);
		}
	}
}
