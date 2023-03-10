using System;
using System.Collections.Generic;
using System.Diagnostics;
using HomeSeerAPI;
using HSPI_SensiboClimate.LocalStorage.Models;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices.Update
{
	public class UpgradeTo2001
	{
		private readonly ILogging _log;
		private readonly IHSApplication _hs;
		private readonly IPedHandler _pedHandler;
		private readonly IHomeSeerDeviceUtils _homeSeerDeviceUtil;
		private readonly IUnitDeviceCreator _deviceCreator;

		private readonly string _versionNumberAsString = "2.0.0.1";

		public UpgradeTo2001(ILogging log, IHSApplication hs, IPedHandler pedHandler, IHomeSeerDeviceUtils homeSeerDeviceUtil, IUnitDeviceCreator deviceCreator)
		{
			_log = log;
			_hs = hs;
			_pedHandler = pedHandler;
			_homeSeerDeviceUtil = homeSeerDeviceUtil;
			_deviceCreator = deviceCreator;
		}

		public void DoUpgrade(List<int> rootNodeDvRefs)
		{
			//load old ini if any
			var stopWatch=new Stopwatch();
			stopWatch.Start();
			var resultOldFileSettings = _hs.GetINISectionEx("Settings", "SensiboPlugin.ini");


			//load new ini if any
			var newFileSettings = _hs.GetINISectionEx(PluginConstants.ConfigSection, PluginConstants.IniFileName);
			var oldApiKey = string.Empty;
			if (resultOldFileSettings.Length > 0)
			{
				foreach (var resultOldFileSetting in resultOldFileSettings)
				{
					if (resultOldFileSetting.Contains("APIKey"))
					{
						var split = resultOldFileSetting.Split('=');
						if (split.Length > 1)
						{
							oldApiKey = split[1];
						}
						
					}
				}
			}

			var newApiKey = string.Empty;
			//Do updates to new if data not already in
			if (newFileSettings.Length > 0)
			{
				foreach (var resultOldFileSetting in resultOldFileSettings)
				{
					if (resultOldFileSetting.Contains("APIKey"))
					{
						var split = resultOldFileSetting.Split('=');
						if (split.Length > 1)
						{
							newApiKey = split[1];
						}

					}
				}
			}

			if (!string.IsNullOrWhiteSpace(oldApiKey) && string.IsNullOrWhiteSpace(newApiKey))
			{
				_hs.SaveINISetting(PluginConstants.ConfigSection,PluginConstants.ApiIniKey,oldApiKey,PluginConstants.IniFileName);
				_log.Log($"Storing the old api key in the new ini-file {PluginConstants.IniFileName}");
			}
			if (!string.IsNullOrWhiteSpace(oldApiKey) && !string.IsNullOrWhiteSpace(newApiKey))
			{
				//_hs.SaveINISetting(PluginConstants.ConfigSection, PluginConstants.ApiIniKey, oldApiKey, PluginConstants.IniFileName);
				_hs.SaveINISetting(PluginConstants.ConfigSection, PluginConstants.OldApiIniKey, oldApiKey, PluginConstants.IniFileName);
				_log.LogWarning($"Found api key in old and new ini-file. Storing old api key as 'OldApiKey' in ini-file {PluginConstants.IniFileName}");
			}

			AddVersionToRootNodes(rootNodeDvRefs);

			stopWatch.Stop();
			if (_log.LogLevel == LogLevel.Debug)
			{
				_log.LogDebug($"Done with upgrade to {_versionNumberAsString} in {stopWatch.ElapsedMilliseconds.ToString("# ###")} ms");
			}
		}

		private void AddVersionToRootNodes(List<int> rootNodeDvRefs)
		{
			foreach (var dvRef in rootNodeDvRefs)
			{
				AddVersionNumber(dvRef);
			}
		}

		private void AddVersionNumber(int dvRef)
		{
			_pedHandler.AddToPedByHsRef(_versionNumberAsString, PluginConstants.PedVersionKey, dvRef);
			_hs.SaveEventsDevices();
		}
	}
}