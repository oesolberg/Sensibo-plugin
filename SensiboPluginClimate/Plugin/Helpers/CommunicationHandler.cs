using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HSPI_SensiboClimate.LocalStorage.Models;
using HSPI_SensiboClimate.Plugin.API;
using HSPI_SensiboClimate.Plugin.API.JSON;
using Newtonsoft.Json;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
	public interface ICommunicationHandler
	{
		List<SensiboDeviceInfo> GetAllDevices(bool overrideCaching = false);
	}

	//Class for keeping the communication controlled. Also a place for caching the previous values
	public class CommunicationHandler : ICommunicationHandler
	{
		private DateTime _lastFetchDateTime;
		private List<SensiboDeviceInfo> _lastFetchData;

		private readonly ISensiboRestHandler _sensiboRestHandler;
		private readonly ILogging _log;

		public CommunicationHandler(ISensiboRestHandler sensiboRestHandler, ILogging log)
		{
			_sensiboRestHandler = sensiboRestHandler;
			_log = log;
		}
		public List<SensiboDeviceInfo> GetAllDevices(bool overrideCaching = false)
		{


			bool lockTaken = false;
			var sensiboDevices = new List<SensiboDeviceInfo>();
			if (overrideCaching)
			{
				_lastFetchDateTime = DateTime.MinValue;
			}

			//Return cached data if any and we are less than x seconds since last request
			if (_lastFetchData != null && (SystemTime.Now() - _lastFetchDateTime).TotalSeconds <= 30)
			{
				return _lastFetchData;
			}
			var timeout = new TimeSpan(0, 1, 0);
			try
			{
				var stopWatch = new Stopwatch();
				stopWatch.Start();
				_log.LogDebug("GetAllDevices: Trying to update local units");

				Monitor.TryEnter(PluginLocks.SensiboFetchDevicesLock, timeout, ref lockTaken);
				if (lockTaken)
				{
					_log.LogDebug("GetAllDevices: Locking and fetching sensibo units");
					var foundDevices = _sensiboRestHandler.GetAllDevices();
					if (foundDevices != null && foundDevices.IsSuccessful && !string.IsNullOrWhiteSpace(foundDevices.Content))
					{
						var returnedDevices = JsonConvert.DeserializeObject<APIGetAllModel>(foundDevices.Content);
						_lastFetchDateTime = SystemTime.Now();
						foreach (var acInfo in returnedDevices.ACInfoList)
						{

							bool scheduledState = GetScheduleState(acInfo.Id);
							bool smartMode = GetSmartMode(acInfo.Id);
							sensiboDevices.Add(new SensiboDeviceInfo()
							{
								Id = acInfo.Id,
								Mac = acInfo.MacAddress,
								CurrentHumidity = acInfo.Measurements.Humidity,
								CurrentTemperature = acInfo.Measurements.Temperature,
								TargetTemperature = acInfo.AcState.TargetTemperature,
								TemperatureUnit = acInfo.TemperatureUnit.Equals("C") ? "C" : "F",
								FanLevel = acInfo.AcState.FanLevel != null ? acInfo.AcState.FanLevel : null,
								Mode = acInfo.AcState.Mode,
								Room = acInfo.Room.Name,
								Swing = acInfo.AcState.Swing != null ? acInfo.AcState.Swing : null,
								On = acInfo.AcState.On,
								Status = acInfo.ConnectionStatus.IsAlive,
								Modes = acInfo.RemoteCapabilities.Modes,
								SmartMode = smartMode,
								Scheduled = scheduledState,
								Address = acInfo.Location.Address.FirstOrDefault()
							});

						}
					}
					_lastFetchData = sensiboDevices;
					stopWatch.Stop();
					_log.LogDebug($"GetAllDevices: Done fetching Sensibo units (time used: {stopWatch.ElapsedMilliseconds.ToString("# ###")}) ms");

				}
				else
				{
					_log.LogDebug("GetAllDevices: Could not fetch Sensibo units due to lock for more than 1 minute");
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(PluginLocks.SensiboFetchDevicesLock);
				}
			}

			return _lastFetchData;
		}

		private bool GetSmartMode(string acId)
		{
			var smartModeResponse = _sensiboRestHandler.GetSmartModeState(acId);
			if (smartModeResponse != null && smartModeResponse.IsSuccessful && !string.IsNullOrEmpty(smartModeResponse.Content))
			{
				var returnedData= JsonConvert.DeserializeObject<SmartModeModel>(smartModeResponse.Content);
				if(returnedData.Result!=null && returnedData.Result.Status == PluginConstants.ResponseSuccess &&
				   returnedData.Result.On)
				return true;
			}
			return false;
		}

		private bool GetScheduleState(string acId)
		{
			var scheduledStateResponse = _sensiboRestHandler.GetScheduledState(acId);
			if (scheduledStateResponse != null && scheduledStateResponse.IsSuccessful && !string.IsNullOrEmpty(scheduledStateResponse.Content))
			{
				var returnedData = JsonConvert.DeserializeObject<ScheduledStateModel>(scheduledStateResponse.Content);
				if(returnedData!=null && returnedData.Status == PluginConstants.ResponseSuccess &&
				returnedData.Result != null && returnedData.Result.Count > 0)
					return true;
			}

			return false;
		}
	}
}