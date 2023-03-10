using System.IO;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
	public class PluginConstants
	{
		public const string PluginName = "SensiboClimate";
		public const string IniFileName = "SensiboClimate.ini";

		public const string IsoFormatting = "yyyy-MM-dd HH:mm:ss";
		public const string TimerIntervalInputKey = "oTimerIntervalInput";
		public const string TimerIntervalValueKey = "timerInterval";
		public const string ResponseSuccess = "success";
		public static string ExePath = Directory.GetCurrentDirectory();
		public const string LogLevelKey = "LogLevel";

		public const string HsRootMarker = "root";
		public static string HsNameSeparator = ";";

		public const string UnitIdKey = "UnitIdKey";
		public static string PluginInstanceName = "";

		public static string PedIsSensiboDataKey => "IsSensiboData";
		public static string DeviceTypeKey => "SensiboPluginDeviceType";
		public static string PedVersionKey => "SensiboPluginVersion";
		public static string PedTemperatureUnitKey => "SensiboPluginTemperatureUnit";

		public const string ChangeTemperatureDeviceType = "SensiboPluginDeviceType_ChangeTemperature";
		public const string ChangeModeDeviceType = "SensiboPluginDeviceType_ChangeMode";
		public const string ChangeFanSpeedDeviceType = "SensiboPluginDeviceType_ChangeFanSpeed";
		public const string ChangeSwingDeviceType = "SensiboPluginDeviceType_ChangeSwing";
		public const string SmartModeStateDeviceType = "SensiboPluginDeviceType_SmartModeState";
		public const string ScheduledStateDeviceType = "SensiboPluginDeviceType_ScheduledState";
		public const string CurrentTemperatureDeviceType = "SensiboPluginDeviceType_CurrentTemperature";
		public const string CurrentHumidityDeviceType = "SensiboPluginDeviceType_CurrentHumidity";
		public const string PowerDeviceType = "SensiboPluginDeviceType_Power";
		public const string UnitRootDevice = "SensiboPluginDeviceType_RootDevice";

		public const string DegreeSymbolAsHtml = "&deg;";
		public static string PercentSignAsHtml => "&#37;";
		

		public const string SensiboClimateSky = "SensiboClimate Sky";

		//Update version information
		public static int UpdatedFromNoVersion = 2000000;
		public static int UpdateIniFile = 2000001;
		public static int UpdateFanAndSwingModes= 2000005;

		//Ini file constants
		public const string ConfigSection = "Config";
		public const string TimerIntervalInSecondsIniKey = "TimeIntervalInSeconds";
		public const string LogLevelIniKey = "Loglevel";
		public const string ApiIniKey = "ApiKey";
		public const string OldApiIniKey = "OldApiKey";
		public const string UseFakeIniKey = "RunFake";
	}
}