using System;
using System.Collections.Generic;
using System.Linq;
using HomeSeerAPI;
using HSPI_SensiboClimate.LocalStorage.Models;
using HSPI_SensiboClimate.Plugin.API;
using HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices;
using HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices.Update;
using HSPI_SensiboClimate.Plugin.Helpers.Requests;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
	public interface IDeviceHandler
	{
		List<DeviceClass> GetAllPluginDevices(bool reload=false);
		void CreateUnit(string unitId, SensiboDeviceInfo units);
		void DeleteUnit(string id);
		int GetRootDeviceById(string deviceId);
		void UpdateControlPanelOffline(List<LocalDeviceClass> deviceList, string status = "No api key, check config");
		void UpdateControlPanel(List<SensiboDeviceInfo> devicesInfo);
		List<LocalDeviceClass> GetLocalDevices();
		LocalDeviceClass GetDeviceByHsRef(int controlRef);
		void SetIoMulti(List<CAPI.CAPIControl> colSend);
		void DoUpdates(List<SensiboDeviceInfo> sensiboDevices);
	}

	public class DeviceHandler : IHandler, IDeviceHandler
	{
		private readonly IMediator _mediator;
		private readonly IHSApplication _hs;
		private readonly ILogging _logging;
		private List<DeviceClass> _lastFetchedDevicesData;
		private DateTime _lastFetchedDevicesDateTime;
		private IHomeSeerDeviceUtils _hsDeviceUtils;
		private IPedHandler _pedHandler;
		private IUnitDeviceUpdater unitDeviceUpdater;
		private IUnitDeviceUpdater _unitDeviceUpdater;

		public DeviceHandler(IMediator mediator, IHSApplication hs, ILogging logging,IHomeSeerDeviceUtils hsDeviceUtils, ISensiboRestHandler sensiboRestHandler)
		{
			_mediator = mediator;
			_hs = hs;
			_logging = logging;
			_mediator.AddHandler(this);
			_hsDeviceUtils = hsDeviceUtils;
			//_hsDeviceUtils = new HomeSeerDeviceUtils(_hs, _logging);
			_pedHandler = new PedHandler(_hs, _logging);

			_unitDeviceUpdater = new UnitDeviceUpdater(_hs, _logging, _hsDeviceUtils,_pedHandler,sensiboRestHandler);
		}


		public string Name => this.GetType().FullName;
		public List<DeviceClass> GetAllPluginDevices(bool reload=false)
		{
			if (_lastFetchedDevicesData != null && (SystemTime.Now() - _lastFetchedDevicesDateTime).TotalSeconds < 30)
			{
				return _lastFetchedDevicesData;
			}

			var devices = new List<DeviceClass>();

			clsDeviceEnumeration deviceEnumeration = (clsDeviceEnumeration)_hs.GetDeviceEnumerator();
			while (!deviceEnumeration.Finished)
			{
				var device = deviceEnumeration.GetNext();
				if (device.get_Interface(_hs) == PluginConstants.PluginName)
					devices.Add(device);
			}

			_lastFetchedDevicesData = devices;
			_lastFetchedDevicesDateTime = SystemTime.Now();

			return devices.ToList();
		}

		public void CreateUnit(string unitId, SensiboDeviceInfo sensiboUnit)
		{
			var unitDeviceCreator = new UnitDeviceCreator(_hs, _logging,_hsDeviceUtils);
			unitDeviceCreator.Execute(sensiboUnit);
			//Update cachend devices
			_hsDeviceUtils.UpdateCachedDevices(useNewThread: false);
		}

		public void DeleteUnit(string sensiboUnitId)
		{
			var unitDeviceDeleter = new UnitDeviceDeleter(_hs, _logging,_hsDeviceUtils);
			unitDeviceDeleter.Delete(sensiboUnitId);
			_hsDeviceUtils.UpdateCachedDevices(useNewThread:false);
			
		}

		public int GetRootDeviceById(string unitId)
		{
			return _hsDeviceUtils.GetRootDeviceIdByUnitId(unitId);

		}

		public void UpdateControlPanelOffline(List<LocalDeviceClass> deviceList, string status = "No api key, check config")
		{
			if (deviceList == null || deviceList.Count == 0)
				return;
			foreach (var hsDevice in deviceList)
			{

				_hs.SetDeviceValueByRef(hsDevice.Ref, 0, true);
				_hs.SetDeviceString(hsDevice.Ref, status, true);
			}
		}

		public void UpdateControlPanel(List<SensiboDeviceInfo> sensiboUnits)
		{
			//Run through every unit and update its devices
			foreach (var sensiboUnit in sensiboUnits)
			{
				_unitDeviceUpdater.UpdateDevice(sensiboUnit);
				//Find the root id, then update every sub device with relevant data
			}
		}

		public List<LocalDeviceClass> GetLocalDevices()
		{
			return _hsDeviceUtils.GetLocalHomeSeerPluginDevices();
		}

		public LocalDeviceClass GetDeviceByHsRef(int controlRef)
		{
			return _hsDeviceUtils.GetDeviceByHsRef(controlRef);
		}

		public void SetIoMulti(List<CAPI.CAPIControl> colSend)
		{
			
			_unitDeviceUpdater.SetIoMulti(colSend);
		}

		public void DoUpdates(List<SensiboDeviceInfo> sensiboDevices)
		{
			var updater=new HomeSeerDeviceUpgrader(_logging,_hs,_hsDeviceUtils,_pedHandler,_mediator);
			updater.DoUpdate(sensiboDevices);
		}

		private void UpdateSensiboUnit(SensiboDeviceInfo sensiboUnit, int rootId)
		{
			float valueToSet = -1;
			var stringValueToSet = "Disconnected";
			if (sensiboUnit.Status)
			{
				valueToSet = 0;
				stringValueToSet = "Connected";
			}

			//Todo - Add possibillity to show errors in root device
			//if (eventArgs.ErrorNumber > 0)
			//{
			//	valueToSet = eventArgs.ErrorNumber;
			//	stringValueToSet = $"Connected<br> Error {eventArgs.ErrorNumber}";
			//}

			_hs.SetDeviceValueByRef(rootId, valueToSet, true);
			_hs.SetDeviceString(rootId, stringValueToSet, true);

			UpdateSubDevices(sensiboUnit, rootId);
		}

		private void UpdateSubDevices(SensiboDeviceInfo sensiboUnit, int rootId)
		{
			var rootDevice = (DeviceClass)_hs.GetDeviceByRef(rootId);// DeviceUtils.GetRootDeviceIdByUnitId(eventArgs.IpAddress);

			if (rootDevice != null)
			{

				//get child devices
				var childDevices = _hsDeviceUtils.GetAssociatedDevices(rootId);
				foreach (var childDevice in childDevices)
				{
					UpdateSubDevice(childDevice, sensiboUnit);

				}
			}
		}

		private void UpdateSubDevice(LocalDeviceClass childDevice, SensiboDeviceInfo sensiboUnit)
		{
			if (childDevice != null)
			{
				var deviceType = GetDeviceTypeFromPedData(childDevice);
				UpdateByDeviceType(deviceType, childDevice, sensiboUnit);
			}
		}

		private string GetDeviceTypeFromPedData(LocalDeviceClass device)
		{
			var result = _pedHandler.GetObjectFromPed(PluginConstants.DeviceTypeKey, device.PlugExtraData) as string;
			if (!string.IsNullOrEmpty(result))
			{
				return result;
			}
			return string.Empty;
		}

		private void UpdateByDeviceType(string deviceType, LocalDeviceClass childDevice, SensiboDeviceInfo sensiboUnit)
		{
			double valueToUpdateTo = 0;
			string stringToUpdateTo = "Disconnected";
			bool updateDevice = true;
			switch (deviceType)
			{
				case PluginConstants.PowerDeviceType:
					if (sensiboUnit.On)
						valueToUpdateTo = 1;
					stringToUpdateTo = string.Empty;
					break;
				case PluginConstants.CurrentHumidityDeviceType:
					valueToUpdateTo = sensiboUnit.CurrentHumidity;
					if (valueToUpdateTo > -300)
						stringToUpdateTo = valueToUpdateTo.ToString("0.0") + PluginConstants.PercentSignAsHtml ;
					break;
				case PluginConstants.CurrentTemperatureDeviceType:
					valueToUpdateTo = sensiboUnit.CurrentTemperature;
					if (sensiboUnit.TemperatureUnit == "F")
						valueToUpdateTo = (valueToUpdateTo * 9 / 5) + 32;
					if (valueToUpdateTo > -300)
					{
						stringToUpdateTo = valueToUpdateTo.ToString("0.0") + PluginConstants.DegreeSymbolAsHtml + sensiboUnit.TemperatureUnit;
					}
					break;
				case PluginConstants.ScheduledStateDeviceType:
					if (sensiboUnit.Scheduled)
					{
						valueToUpdateTo = 1;
						stringToUpdateTo = "Timers";
					}
					else
					{
						valueToUpdateTo = 0;
						stringToUpdateTo = "Off";
					}
					break;
				case PluginConstants.SmartModeStateDeviceType:
					if (sensiboUnit.SmartMode)
					{
						valueToUpdateTo = 1;
						stringToUpdateTo = "Timers";
					}
					else
					{
						valueToUpdateTo = 0;
						stringToUpdateTo = "Off";
					}
					break;
				case PluginConstants.ChangeSwingDeviceType:
					//Todo-Get more control over how to set this up correctly
					valueToUpdateTo = 1;
					stringToUpdateTo = sensiboUnit.Swing;
					//valueToUpdateTo = double.Parse(eventArgs.FanPosition2.ToString());
					//if (valueToUpdateTo > -300)
					//	stringToUpdateTo = string.Empty;
					break;
				case PluginConstants.ChangeFanSpeedDeviceType:
					//Todo-Get more control over how to set this up correctly
					valueToUpdateTo = 1;
					stringToUpdateTo = sensiboUnit.FanLevel;
					//valueToUpdateTo = double.Parse(eventArgs.FanSpeed.ToString());
					//if (valueToUpdateTo > -300)
					//	stringToUpdateTo = string.Empty;
					break;
				case PluginConstants.ChangeModeDeviceType:
					//Todo-Get more control over how to set this up correctly
					valueToUpdateTo = 1;
					stringToUpdateTo = sensiboUnit.Mode;
					//sensiboUnit.Modes.Fan.
					//valueToUpdateTo = double.Parse(eventArgs.CompressorUse.ToString());
					//if (valueToUpdateTo > -300)
					//	stringToUpdateTo = valueToUpdateTo.ToString("0.0") + PluginConstants.PercentSignAsHtml;
					break;
				default:
					updateDevice = false;
					break;
			}

			if (updateDevice)
			{
				var childDeviceId = childDevice.Ref;
				_hs.SetDeviceValueByRef(childDeviceId, valueToUpdateTo, true);
				_hs.SetDeviceString(childDeviceId, stringToUpdateTo, true);

			}


		}

		public void Handle(IRequest request)
		{
			//Add a new ac unit
			if (request is AddUnitRequest)
			{

			}

			//Update an ac unit
			if (request is UpdateUnitRequest)
			{

			}

			if (request is ResetLastFetchedDevicesDateTimeRequest)
			{
				_lastFetchedDevicesDateTime = DateTime.MinValue;
			}
		}
	}
}