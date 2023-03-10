using System;
using HomeSeerAPI;
using HSPI_SensiboClimate.Plugin.Helpers.Requests;
using Newtonsoft.Json.Linq;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
	public interface IIniSettings
	{
		string ApiKey { get; set; }
		int TimerIntervalInSeconds { get; set; }
		bool RunFake { get; }
		LogLevel LogLevel { get; set; }
		void ReloadIni();
	}
	public class IniSettings : IIniSettings , IDisposable, IHandler
	{
		//const

		private const int DefaultTimerInterval = 60;

		private bool _disposed;
		private IHSApplication _hs;
		private readonly IMediator _mediator;
		private string _apiKey;
		private int _timerIntervalInSeconds;
		private bool _runFake;
		private LogLevel _logLevel;

		public IniSettings(IHSApplication hs, IMediator mediator)
		{
			_hs = hs;
			_mediator = mediator;
			_mediator.AddHandler(this);
			LoadSettings();
		}

		private void LoadSettings()
		{
			_apiKey = _hs.GetINISetting(PluginConstants.ConfigSection, PluginConstants.ApiIniKey, "", PluginConstants.IniFileName);
			_timerIntervalInSeconds = GetPositiveIntFromString(PluginConstants.ConfigSection, PluginConstants.TimerIntervalInSecondsIniKey, DefaultTimerInterval);
			_runFake = GetBool(_hs.GetINISetting(PluginConstants.ConfigSection, PluginConstants.UseFakeIniKey, "false", PluginConstants.IniFileName));
			_logLevel = GetLogLevel();
		}

		private LogLevel GetLogLevel()
		{
			var debugLevelAsString = _hs.GetINISetting(PluginConstants.ConfigSection, PluginConstants.LogLevelIniKey, "None", PluginConstants.IniFileName);
			LogLevel logLevelToReturn;
			if (!Enum.TryParse(debugLevelAsString, true, out logLevelToReturn))
			{
				logLevelToReturn = LogLevel.Normal;
			}
			return logLevelToReturn;
		}

		private bool GetBool(string initSetting)
		{
			if (initSetting.ToLower() == "true")
				return true;
			return false;
		}

		public string ApiKey
		{
			get => _apiKey;
			set
			{
				_apiKey = value;
				SaveApiKey();
				_mediator.Send(new ApiUpdatedRequest(), this);
			}
		}

		private void SaveApiKey()
		{
			_hs.SaveINISetting(PluginConstants.ConfigSection, PluginConstants.ApiIniKey, _apiKey, PluginConstants.IniFileName);
		}



		public int TimerIntervalInSeconds
		{
			get => _timerIntervalInSeconds;
			set
			{
				_timerIntervalInSeconds = value;
				SaveTimerIntervalInSeconds();
				_mediator.Send(new TimerIntervalUpdatedRequest(), this);
			}
		}

		private void SaveTimerIntervalInSeconds()
		{
			_hs.SaveINISetting(PluginConstants.ConfigSection, PluginConstants.TimerIntervalInSecondsIniKey, _timerIntervalInSeconds.ToString(), PluginConstants.IniFileName);
		}


		private int GetPositiveIntFromString(string configSection, string configKey, int defaultValue = -1)
		{
			var intAsString = _hs.GetINISetting(configSection, configKey, "", PluginConstants.IniFileName);
			if (int.TryParse(intAsString, out var intValue))
			{
				if (intValue > 0)
					return intValue;
			}
			return defaultValue;
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
			//Enter all request this class should handle from the Mediator
		}

		public string Name => this.GetType().FullName;
		public bool RunFake => _runFake;
		public LogLevel LogLevel
		{
			get => _logLevel;
			set
			{
				_logLevel = value;
				SaveLogLevel();
			}
		}

		public void ReloadIni()
		{
			LoadSettings();
		}

		private void SaveLogLevel()
		{

			var logLevelToSave = Enum.GetName(typeof(LogLevel), _logLevel);

			_hs.SaveINISetting(PluginConstants.ConfigSection, PluginConstants.LogLevelIniKey, logLevelToSave, PluginConstants.IniFileName);
			_mediator.Send(new LogLevelChangedRequest(),this);
		}

	}
}
