using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HomeSeerAPI;
using HSPI_SensiboClimate.Plugin.Helpers.Requests;
using Serilog;
using Serilog.Core;
using HSPI_SensiboClimate.Plugin.Enums;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
	public interface ILogging : IDisposable,IHandler
	{
		void LogToHsNoMatterSettings(string logMessage);
		void Log(string logMessage);
		void Log(string logMessage, LogLevel logLevel);
		void Log(string logMessage, HSPI_SensiboClimate.Plugin.Enums.LogType logType);
		void LogDebug(string logMessage);
		LogLevel LogLevel { get; }
		void LogException(Exception ex);
		void LogError(string logMessage);
		void LogWarning(string logMessage);
		void LogIfLogToFileEnabled(string logMessage);
		void IniSettingHasChanged(object sender, EventArgs eventArgs);
		bool IsLoggingToFile { get; }
		bool IsLoggingDebug { get; }
	}

	public class Logging : ILogging
	{
		private const string OrangeColor = "#FFA500";
		private const string RedColor = "#FF0000";
		private static object _lockObject = new object();
		private readonly IIniSettings _iniSettings;
		private readonly IHSApplication _hs;
		private readonly IMediator _mediator;
		private bool _disposed;
		private Logger _seriLogger = null;



		public Logging(IIniSettings iniSettings, IHSApplication hs, IMediator mediator)
		{
			_iniSettings = iniSettings;
			_hs = hs;
			_mediator = mediator;
			_mediator.AddHandler(this);
			var logPath = Path.Combine(PluginConstants.ExePath, "Logs");
			if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
			var logFile = Path.Combine(logPath, PluginConstants.PluginName + "Debug.log");
			if (_iniSettings.LogLevel == LogLevel.DebugToFile || _iniSettings.LogLevel == LogLevel.DebugToFileAndLog)
			{
				CreateLogFile();
			}
		}



		public void LogToHsNoMatterSettings(string logMessage)
		{
			DoLog(logMessage, LogLevel.IgnoreSettings);
		}

		public void LogError(string message)
		{
			_hs.WriteLogEx(PluginConstants.PluginName + "-Error", message, RedColor);
		}

		public void LogWarning(string message)
		{
			_hs.WriteLogEx(PluginConstants.PluginName + "-Warn", message, OrangeColor);
		}

		public void Log(string message)
		{
			DoLog(message, LogLevel.Normal);
		}

		public void Log(string message, LogLevel logLevel)
		{
			if (logLevel == _iniSettings.LogLevel)
				DoLog(message, logLevel);
		}

		public void Log(string logMessage, LogType logType)
		{
			switch (logType)
			{
				case LogType.Error:LogError(logMessage);
					break;
				case LogType.Warning:
					LogWarning(logMessage);
					break;
				case LogType.Debug:
					LogDebug(logMessage);
					break;
				case LogType.Normal:
					Log(logMessage);
					break;
			}
		}

		public void LogDebug(string message)
		{
			DoLog(message, LogLevel.Debug);
		}

		public void LogIfLogToFileEnabled(string message)
		{
			if (_iniSettings.LogLevel == LogLevel.DebugToFile || _iniSettings.LogLevel == LogLevel.DebugToFileAndLog)
			{
				lock (_lockObject)
				{
					_seriLogger.Debug($"{message}");
					Serilog.Log.CloseAndFlush();
				}
			}
		}

		public LogLevel LogLevel => _iniSettings.LogLevel;
		public bool IsLoggingToFile => (_iniSettings.LogLevel == LogLevel.DebugToFile ||
		                                _iniSettings.LogLevel == LogLevel.DebugToFileAndLog);

		public bool IsLoggingDebug => (IsLoggingToFile || _iniSettings.LogLevel == LogLevel.Debug);

		public void LogException(Exception ex)
		{
			if (ex != null)
			{
				_hs.WriteLog(PluginConstants.PluginName, "Error: " + ex.Message);
				_hs.WriteLog(PluginConstants.PluginName, ex.StackTrace);
			}
		}

		private void DoLog(string message, LogLevel logLevel = LogLevel.Normal)
		{
			if (_iniSettings.LogLevel == LogLevel.None && logLevel != LogLevel.IgnoreSettings) return; // Logging = off in config
																									   //Write to Homseseer log

			if (_iniSettings.LogLevel == LogLevel.Normal && logLevel == LogLevel.Normal)
			{
				_hs.WriteLog(PluginConstants.PluginName, message);
			}

			if ((_iniSettings.LogLevel == LogLevel.Debug || _iniSettings.LogLevel == LogLevel.DebugToFileAndLog) &&
				(logLevel == LogLevel.Normal || logLevel == LogLevel.Debug))
			{
				_hs.WriteLog(PluginConstants.PluginName, message);
			}

			if (logLevel == LogLevel.IgnoreSettings)  //Log that will be written no matter what ini setting))
			{
				_hs.WriteLog(PluginConstants.PluginName, message);
			}

			//Write to logfile
			if (_iniSettings.LogLevel == LogLevel.DebugToFile || _iniSettings.LogLevel == LogLevel.DebugToFileAndLog)
			{
				LogIfLogToFileEnabled(message);
			}
		}

		public void IniSettingHasChanged(object sender, EventArgs eventArgs)
		{
			if (_iniSettings.LogLevel != LogLevel.DebugToFileAndLog && _iniSettings.LogLevel != LogLevel.DebugToFile)
			{
				CloseLogFile();
			}

			if (_iniSettings.LogLevel == LogLevel.DebugToFileAndLog || _iniSettings.LogLevel == LogLevel.DebugToFile)
			{
				CreateLogFile();
			}
		}

		private void CreateLogFile()
		{
			if (_seriLogger != null) return;
			var logPath = Path.Combine(PluginConstants.ExePath, "Logs");
			if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
			var logFile = Path.Combine(logPath, PluginConstants.PluginName + "Debug.log");
			lock (_lockObject)
			{
				_seriLogger = new LoggerConfiguration()
					.MinimumLevel.Debug()

					.WriteTo.File(path: logFile, rollingInterval: RollingInterval.Day, shared: true)
					.CreateLogger();
				var version = PluginConstants.PluginName + " version: " + Assembly.GetExecutingAssembly().GetName().Version;
				_seriLogger.Information($"{version}");
			}
		}

		private void CloseLogFile()
		{
			if (_seriLogger != null)
			{
				lock (_lockObject)
				{
					_seriLogger.Dispose();
					_seriLogger = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);

			// Use SupressFinalize in case a subclass 
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				// Indicate that the instance has been disposed.
				_disposed = true;
			}
		}

		public void Handle(IRequest request)
		{
			if (request is LogLevelChangedRequest)
			{
				if (_iniSettings.LogLevel != LogLevel.DebugToFileAndLog && _iniSettings.LogLevel != LogLevel.DebugToFile)
				{
					CloseLogFile();
				}

				if (_iniSettings.LogLevel == LogLevel.DebugToFileAndLog || _iniSettings.LogLevel == LogLevel.DebugToFile)
				{
					CreateLogFile();
				}
			}
		}

		public string Name => this.GetType().FullName;
	}

	public enum LogLevel
	{
		None = 0,
		Normal = 1,
		Debug = 2,
		DebugToFile = 3,
		DebugToFileAndLog = 4,
		IgnoreSettings
	}
}