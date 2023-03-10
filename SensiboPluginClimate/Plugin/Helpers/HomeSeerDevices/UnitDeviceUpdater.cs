using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeSeerAPI;
using HSPI_SensiboClimate.Extensions;
using HSPI_SensiboClimate.LocalStorage.Models;
using HSPI_SensiboClimate.Plugin.API;
using HSPI_SensiboClimate.Plugin.API.JSON;
using HSPI_SensiboClimate.Plugin.Helpers.Enums;
using Newtonsoft.Json;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices
{
	public interface IUnitDeviceUpdater
	{
		void SetIoMulti(List<CAPI.CAPIControl> colSend);
		void UpdateDevice(SensiboDeviceInfo info, bool allDevices = true);
	}

	public class UnitDeviceUpdater : IUnitDeviceUpdater
	{
		private readonly IHSApplication _hs;
		private readonly ILogging _logging;
		private readonly IHomeSeerDeviceUtils _hsDeviceUtils;
		private readonly IPedHandler _pedHandler;
		private readonly ISensiboRestHandler _sensiboRestHandler;
		private readonly List<string> _setAcDevices = new List<string>()
		{
			PluginConstants.PowerDeviceType,
			PluginConstants.ChangeModeDeviceType,
			PluginConstants.ChangeSwingDeviceType,
			PluginConstants.ChangeFanSpeedDeviceType,
			PluginConstants.ChangeTemperatureDeviceType,

		};

		public UnitDeviceUpdater(IHSApplication hs, ILogging logging, IHomeSeerDeviceUtils hsDeviceUtils, IPedHandler pedHandler, ISensiboRestHandler sensiboRestHandler)
		{
			_hs = hs;
			_logging = logging;
			_hsDeviceUtils = hsDeviceUtils;
			_pedHandler = pedHandler;
			_sensiboRestHandler = sensiboRestHandler;
		}
		public void SetIoMulti(List<CAPI.CAPIControl> colSend)
		{

			foreach (var control in colSend)
			{
				if (_logging.IsLoggingToFile)
				{
					_logging.LogDebug($"got click on control: {control.Ref} {control.ControlValue} {control.Label}");
				}

				var device = _hsDeviceUtils.GetDeviceByHsRef(control.Ref);
				var unitId = GetUnitIdFromParentDevice(device);
				var label = control.Label;

				var acState = GetSensiboDeviceInfoWithCurrentState(unitId);
				acState.Id = unitId;
				object newValue = null;
				var stateName = string.Empty;
				var deviceType = device.DeviceType;

				switch (deviceType)
				{
					case PluginConstants.ChangeTemperatureDeviceType:
						newValue = (int)control.ControlValue;
						stateName = "targetTemperature";
						break;

					case PluginConstants.PowerDeviceType:
						stateName = "on";
						if ((int)control.ControlValue == 1)
						{
							newValue = true;
						}
						else
						{
							newValue = false;
						}
						break;

					case PluginConstants.ChangeModeDeviceType:
						//string mode = label.ToLower();

						newValue = GetCorrectMode(control.ControlValue);
						stateName = "mode";
						break;

					case PluginConstants.ChangeFanSpeedDeviceType:// ; "Fan Speed":

						newValue = GetCorrectFanLevel(control.ControlValue);
						stateName = "fanLevel";
						break;

					case PluginConstants.ChangeSwingDeviceType:// "Swing":
						newValue = GetCorrectSwingMode(control.ControlValue);
						stateName = "swing";
						break;
				}

				if (_logging.IsLoggingToFile)
				{
					_logging.LogDebug($"Trying to update Sensibo Api with singleState unitId: {unitId}, stateName: {stateName}, newValue:{newValue}");
				}
				var result = SendAcSingleState(unitId, stateName, newValue).Result;

				if (result != null && result.Status == PluginConstants.ResponseSuccess)
				{
					if (_logging.IsLoggingDebug)
					{
						_logging.LogDebug($"Successfully changed status in Sensibo Api. Now updating HomeSeer");
					}

					UpdateDeviceFromStatusResponse(result, device, unitId);

				}
			}
			//stopWatch.Stop();
		}

		private void UpdateDeviceFromStatusResponse(StatusRequestModel result, LocalDeviceClass device, string unitId)
		{
			//something to change
			var rootDevice = _hsDeviceUtils.GetRootDeviceByUnitId(unitId);
			var childIds = rootDevice.AssociatedDevices;
			foreach (var childId in childIds)
			{


				var childDevice = _hsDeviceUtils.GetDeviceByHsRef(childId);
				var typeData = childDevice.DeviceType;

				if (_logging.IsLoggingToFile)
				{
					_logging.LogDebug($"Changing status for device {childDevice.WriteInfo()}");
				}

				switch (typeData)
				{
					case PluginConstants.PowerDeviceType:
						var onStatus = CreateBoolFromString(result.Result.AcState.On);
						UpdatePowerDevice(childId, new SensiboDeviceInfo() { On = onStatus });
						break;
					case PluginConstants.ChangeFanSpeedDeviceType:
						UpdateChangeFanSpeedDevice(childId, new SensiboDeviceInfo() { FanLevel = result.Result.AcState.FanLevel });
						break;
					case PluginConstants.ChangeSwingDeviceType:
						if(result.Result.AcState!=null && !string.IsNullOrWhiteSpace(result.Result.AcState.Swing))
						    UpdateChangeSwingDevice(childId, new SensiboDeviceInfo() { Swing = result.Result.AcState.Swing });
						break;
					case PluginConstants.ChangeTemperatureDeviceType:
                        if (result.Result.AcState != null && !string.IsNullOrWhiteSpace(result.Result.AcState.Swing))
                            UpdateChangeTemperatureDevice(childId, new SensiboDeviceInfo() { TargetTemperature = result.Result.AcState.TargetTemperature, TemperatureUnit = result.Result.AcState.TemperatureUnit });
						break;
					case PluginConstants.ChangeModeDeviceType:
						UpdateChangeModeDevice(childId, new SensiboDeviceInfo() { Mode = result.Result.AcState.Mode });
						break;
				}
			}
		}

		private bool CreateBoolFromString(string boolString)
		{
			if (boolString.Trim().ToLower() == "true")
				return true;
			return false;
		}

		private string GetCorrectMode(double controlValue)
		{
			var chosenEnum = (ModeClimate)(int)controlValue;
			return Enum.GetName(typeof(ModeClimate), chosenEnum).ToLower();
		}

		private string GetCorrectFanLevel(double controlValue)
		{
			var chosenEnum = (FanLevelsClimate)(int)controlValue;
			return Enum.GetName(typeof(FanLevelsClimate), chosenEnum).ToLower();
		}

		private string GetCorrectSwingMode(double controlValue)
		{
			var chosenEnum = (SwingState)(int)controlValue;
			return Enum.GetName(typeof(SwingState), chosenEnum).FirstCharToLower();
		}

		private SensiboDeviceInfo GetSensiboDeviceInfoWithCurrentState(string unitId)
		{
			var currentSensiboUnitWithState = new SensiboDeviceInfo();
			var currentRoot = _hsDeviceUtils.GetRootDeviceByUnitId(unitId);
			foreach (var subDeviceId in currentRoot.AssociatedDevices)
			{
				var subDevice = _hsDeviceUtils.GetDeviceByHsRef(subDeviceId);
				double doubleValue = 0;
				switch (subDevice.DeviceType)
				{
					case PluginConstants.PowerDeviceType:
						currentSensiboUnitWithState.On = true;
						doubleValue = GetHsDeviceValue(subDeviceId);
						if ((int)doubleValue != 1)
							currentSensiboUnitWithState.On = false;
						break;
					case PluginConstants.ChangeModeDeviceType:
						doubleValue = GetHsDeviceValue(subDeviceId);
						currentSensiboUnitWithState.Mode = GetModeString(doubleValue);
						break;
					case PluginConstants.ChangeFanSpeedDeviceType:
						doubleValue = GetHsDeviceValue(subDeviceId);
						currentSensiboUnitWithState.FanLevel = GetFanLevelString(doubleValue);
						break;
					case PluginConstants.ChangeTemperatureDeviceType:
						doubleValue = GetHsDeviceValue(subDeviceId);
						currentSensiboUnitWithState.TargetTemperature = (int)doubleValue;
						currentSensiboUnitWithState.TemperatureUnit =
							GetHsPedDataString(subDeviceId, PluginConstants.PedTemperatureUnitKey);
						break;
					case PluginConstants.ChangeSwingDeviceType:
						doubleValue = GetHsDeviceValue(subDeviceId);
						currentSensiboUnitWithState.Swing = GetSwingStateString(doubleValue);
						break;


				}
			}
			return currentSensiboUnitWithState;
		}

		private string GetHsPedDataString(int dvRef, string pedKey)
		{
			var dataAsString = _pedHandler.GetObjectFromPedByHsRef(pedKey, dvRef) as string;
			return dataAsString;
		}

		private string GetModeString(double doubleValue)
		{
			var modeState = (ModeClimate)(int)doubleValue;
			return Enum.GetName(typeof(ModeClimate), modeState)?.ToLower();
		}

		private string GetFanLevelString(double doubleValue)
		{
			var fanLevel = (FanLevelsClimate)(int)doubleValue;
			return Enum.GetName(typeof(FanLevelsClimate), fanLevel)?.ToLower();
		}

		private string GetSwingStateString(double doubleValue)
		{
			var swingState = (SwingState)(int)doubleValue;
			return Enum.GetName(typeof(SwingState), swingState)?.ToLower();
		}

		private double GetHsDeviceValue(int subDeviceId)
		{
			var hsDevice = (DeviceClass)_hs.GetDeviceByRef(subDeviceId);
			if (hsDevice != null) return hsDevice.get_devValue(_hs);
			return 0;
		}

		private async Task<StatusRequestModel> SendAcSingleState(string unitId, string stateName, object newValueObject)
		{
			var status = _sensiboRestHandler.SendAcSingleStateAsync(unitId, stateName, new AcSingleStateUpdate() { NewValue = newValueObject });
			var isSuccess = false;
			StatusRequestModel statusResponse = null;
			if (status != null && status.IsSuccessful && !string.IsNullOrEmpty(status.Content))
			{
				statusResponse = JsonConvert.DeserializeObject<StatusRequestModel>(status.Content);
				if (statusResponse.Status == PluginConstants.ResponseSuccess)
					isSuccess = true;
			}
			//if (isSuccess)
			//	Storage.XMLUpdate(sensiboDeviceInfo);

			return statusResponse;
		}

		private async Task<bool> SendACState(SensiboDeviceInfo sensiboDeviceInfo)
		{
			//if (sensiboDeviceInfo.Swing != null || sensiboDeviceInfo.FanLevel != null)
			//{
			//	//var mode = Storage.XMLMode(sensiboDeviceInfo.Id, sensiboDeviceInfo.Mode);
			//	var mode = sensiboDeviceInfo.;
			//	var swing = mode.Swing.Where(swg => swg.Equals(sensiboDeviceInfo.Swing, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
			//	var fanLevel = mode.FanLevels.Where(fan => fan.Equals(sensiboDeviceInfo.FanLevel, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

			//	sensiboDeviceInfo.FanLevel = sensiboDeviceInfo.FanLevel;
			//	sensiboDeviceInfo.Swing = string.IsNullOrWhiteSpace(swing) ? (bool)mode?.Swing.Any() ? mode?.Swing[0] : string.Empty : swing;
			//}

			ACStateUpdate stateUpdate = new ACStateUpdate()
			{
				ACState = new ACState()
				{
					On = sensiboDeviceInfo.On,
					Mode = sensiboDeviceInfo.Mode,
					FanLevel = sensiboDeviceInfo.FanLevel,
					TargetTemperature = (int)sensiboDeviceInfo.TargetTemperature,
					Swing = sensiboDeviceInfo.Swing,
					TemperatureUnit = sensiboDeviceInfo.TemperatureUnit
				}
			};

			var status = _sensiboRestHandler.SendACState(sensiboDeviceInfo.Id, stateUpdate);
			var isSuccess = false;
			if (status != null && status.IsSuccessful && !string.IsNullOrEmpty(status.Content))
			{
				var statusResponse = JsonConvert.DeserializeObject<StatusRequestModel>(status.Content);
				if (statusResponse.Status == PluginConstants.ResponseSuccess)
					isSuccess = true;
			}
			//if (isSuccess)
			//	Storage.XMLUpdate(sensiboDeviceInfo);

			return isSuccess;
		}

		private string GetUnitIdFromParentDevice(LocalDeviceClass device)
		{

			var unitId = GetUnitIdFromDevice(device);
			if (string.IsNullOrEmpty(unitId))
			{
				var hsDevices = _hsDeviceUtils.GetLocalHomeSeerPluginDevices();

				var associatedDeviceIds = device.AssociatedDevices;
				foreach (var associatedDeviceId in associatedDeviceIds)
				{
					var foundDevice = hsDevices.FirstOrDefault(x => x.Ref == associatedDeviceId);
					if (foundDevice != null && foundDevice.Relationship == HomeSeerAPI.Enums.eRelationship.Parent_Root)
					{
						return _pedHandler.GetObjectFromPed(PluginConstants.UnitIdKey, foundDevice.PlugExtraData) as string;
					}
				}
			}
			return unitId;
		}

		private string GetUnitIdFromDevice(LocalDeviceClass device)
		{
			return _pedHandler.GetObjectFromPed(PluginConstants.UnitIdKey, device.PlugExtraData) as string;
		}

		public void UpdateDevice(SensiboDeviceInfo sensiboInfo, bool allDevices = true)
		{
			var rootDevice = _hsDeviceUtils.GetRootDeviceByUnitId(sensiboInfo.Id);
			//var device = (DeviceClass)HS.GetDeviceByRef(refId);
			//var deviceName = device.get_Address(HS);
			if (rootDevice != null)
			{
				UpdateSensiboUnitDevices(sensiboInfo, rootDevice, allDevices);
			}
		}

		private void UpdateSensiboUnitDevices(SensiboDeviceInfo sensiboInfo, LocalDeviceClass rootDevice, bool allDevices)
		{
			UpdateDeviceWithData(sensiboInfo, rootDevice.Ref);

			var childIds = rootDevice.AssociatedDevices;
			foreach (var childId in childIds)
			{
				UpdateDeviceWithData(sensiboInfo, childId, allDevices);
			}
		}

		private void UpdateDeviceWithData(SensiboDeviceInfo sensiboDeviceInfo, int dvRef, bool allDevices = true)
		{
			var childDevice = _hsDeviceUtils.GetDeviceByHsRef(dvRef);
			if (childDevice == null) return;
			//When sending a command only update the fields we could have been changing
			if (!allDevices && !_setAcDevices.Contains(childDevice.DeviceType))
				return;
			if (_logging.IsLoggingToFile)
			{
				_logging.LogDebug($"Changing status for device {childDevice.WriteInfo()}");
			}
			switch (childDevice.DeviceType)
			{
				case PluginConstants.UnitRootDevice:
					UpdateRootDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.PowerDeviceType:
					UpdatePowerDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.ChangeTemperatureDeviceType:
					UpdateChangeTemperatureDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.ChangeModeDeviceType:
					UpdateChangeModeDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.ChangeFanSpeedDeviceType:
					UpdateChangeFanSpeedDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.ChangeSwingDeviceType:
					UpdateChangeSwingDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.SmartModeStateDeviceType:
					UpdateSmartModeStateDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.ScheduledStateDeviceType:
					UpdateScheduledStateDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.CurrentTemperatureDeviceType:
					UpdateCurrentTemperatureDevice(dvRef, sensiboDeviceInfo);
					break;
				case PluginConstants.CurrentHumidityDeviceType:
					UpdateCurrentHumidityDevice(dvRef, sensiboDeviceInfo);
					break;
			}
		}

		private void UpdateChangeSwingDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			double valueToSet = GetSwingValue(sensiboDeviceInfo.Swing);
			_hs.SetDeviceValueByRef(dvRef, valueToSet, true);
			_hs.SetDeviceString(dvRef, string.Empty, true);

		}

		private double GetSwingValue(string swingModeAsString)
		{
			foreach (var swingMode in Enum.GetValues(typeof(SwingState)).Cast<SwingState>())
			{
				if (Enum.GetName(typeof(SwingState), swingMode).ToUpper() == swingModeAsString.ToUpper())
				{
					return (double)swingMode;
				}
			}

			return 0;
		}

		private void UpdateSmartModeStateDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			double valueToSet = 0;
			if (sensiboDeviceInfo.SmartMode)
				valueToSet = 1;
			_hs.SetDeviceValueByRef(dvRef, valueToSet, true);
			//_hs.SetDeviceString(dvRef, stringValueToSet, true);
		}

		private void UpdateScheduledStateDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			double valueToSet = 0;
			if (sensiboDeviceInfo.Scheduled)
				valueToSet = 1;
			_hs.SetDeviceValueByRef(dvRef, valueToSet, true);
			//_hs.SetDeviceString(dvRef, stringValueToSet, true);
		}

		private void UpdateCurrentTemperatureDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			double valueToSet = sensiboDeviceInfo.CurrentTemperature;
			var stringValueToSet = valueToSet.ToString("F1") + PluginConstants.DegreeSymbolAsHtml + sensiboDeviceInfo.TemperatureUnit;
			_hs.SetDeviceValueByRef(dvRef, valueToSet, true);
			_hs.SetDeviceString(dvRef, stringValueToSet, true);
		}

		private void UpdateCurrentHumidityDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			double valueToSet = sensiboDeviceInfo.CurrentHumidity;
			var stringValueToSet = valueToSet.ToString("F1") + PluginConstants.PercentSignAsHtml;
			_hs.SetDeviceValueByRef(dvRef, valueToSet, true);
			_hs.SetDeviceString(dvRef, stringValueToSet, true);
		}

		private void UpdateChangeFanSpeedDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			double valueToSet = GetFanMode(sensiboDeviceInfo.FanLevel);
			_hs.SetDeviceValueByRef(dvRef, valueToSet, true);
			_hs.SetDeviceString(dvRef, string.Empty, true);
		}

		private double GetFanMode(string fanLevel)
		{
			if (!string.IsNullOrEmpty(fanLevel))
			{
				foreach (var fanLevelEnum in Enum.GetValues(typeof(FanLevelsClimate)).Cast<FanLevelsClimate>())
				{
					if (fanLevel.ToUpper() == Enum.GetName(typeof(FanLevelsClimate), fanLevelEnum).ToUpper())
						return (double)fanLevelEnum;
				}
			}
			return 0;
		}

		private void UpdateRootDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			float valueToSet = -1;
			var stringValueToSet = "Disconnected";
			if (sensiboDeviceInfo.Status)
			{
				valueToSet = 0;
				stringValueToSet = "Connected";
			}
			_hs.SetDeviceValueByRef(dvRef, valueToSet, true);
			_hs.SetDeviceString(dvRef, stringValueToSet, true);
		}

		private void UpdatePowerDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			double valueToUpdateTo = 0;
			if (sensiboDeviceInfo.On)
				valueToUpdateTo = 1;

			_hs.SetDeviceValueByRef(dvRef, valueToUpdateTo, true);
		}


		private void UpdateChangeTemperatureDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{
			var valueToUpdateTo = sensiboDeviceInfo.TargetTemperature;
			_hs.SetDeviceValueByRef(dvRef, valueToUpdateTo, true);

			var tempPrefix = PluginConstants.DegreeSymbolAsHtml + sensiboDeviceInfo.TemperatureUnit;

			_hs.SetDeviceString(dvRef, valueToUpdateTo.ToString("F0") + " " + tempPrefix, true);
		}

		private void UpdateChangeModeDevice(int dvRef, SensiboDeviceInfo sensiboDeviceInfo)
		{

			foreach (var modeClimateEnum in Enum.GetValues(typeof(ModeClimate)).Cast<ModeClimate>())
			{
				if (sensiboDeviceInfo.Mode.ToUpper() == Enum.GetName(typeof(ModeClimate), modeClimateEnum).ToUpper())
				{
					var valueToUpdateTo = (double)modeClimateEnum;
					_hs.SetDeviceValueByRef(dvRef, valueToUpdateTo, true);
					_hs.SetDeviceString(dvRef, string.Empty, true);
					break;
				}
			}

		}





		//if (!info.On)
		//{
		//	if (deviceName.Equals("Mode"))
		//	{
		//		var extraData = device.get_PlugExtraData_Get(HS);
		//		var prevState = (bool)extraData.GetNamed("PreviousState");

		//		if (prevState)
		//		{
		//			HS.SetDeviceString(refId, "Off", true);
		//			HS.SetDeviceValueByRef(refId, 0, true);
		//			var rootDevice = (DeviceClass)HS.GetDeviceByRef(HS.GetDeviceParentRefByRef(refId));
		//			UpdateDeviceOff(rootDevice);

		//			extraData.ClearAllNamed(true);
		//			extraData.AddNamed("PreviousState", false);
		//			device.set_PlugExtraData_Set(HS, extraData);
		//		}

		//		return;
		//	}
		//}
		//else
		//{
		//	if (deviceName.Equals("Mode"))
		//	{
		//		var extraData = device.get_PlugExtraData_Get(HS);
		//		var currentState = (bool)extraData.GetNamed("PreviousState");

		//		if (!currentState)
		//		{
		//			UpdateAirConStates();

		//			extraData.ClearAllNamed(true);
		//			extraData.AddNamed("PreviousState", true);
		//			device.set_PlugExtraData_Set(HS, extraData);

		//			return;
		//		}
		//	}
		//}

		//var mode = Storage.XMLMode(info.Id, info.Mode);
		//var modes = Storage.XMLModes(info.Id);

		//double value = 0;
		//var oldString = HS.DeviceString(refId);
		//if (string.IsNullOrEmpty(oldString))
		//	oldString = device.get_devValue(HS).ToString();

		//if (deviceName.Equals("Fan Speed"))
		//{
		//	if (mode.FanLevels.Any())
		//	{
		//		mode.FanLevels.ForEach(fan =>
		//		{
		//			if (fan.Equals(info.FanLevel, StringComparison.InvariantCultureIgnoreCase))
		//				value = mode.FanLevels.IndexOf(fan);
		//		});

		//		HS.SetDeviceString(refId, "", false);
		//		HS.SetDeviceString(refId, StringExtensions.FirstCharToUpper(info.FanLevel), true);

		//		//HS.SetDeviceString(refId, "", false);

		//		//if (info.FanLevel.Contains("auto"))
		//		//    HS.SetDeviceString(refId, info.FanLevel.ToUpper(), true);
		//		//if (info.FanLevel.Contains("high"))
		//		//    HS.SetDeviceString(refId, "FAN-STATE-HIGH", true);
		//		//if (info.FanLevel.Contains("low") || info.FanLevel.Contains("medium"))
		//		//    HS.SetDeviceString(refId, "FAN", true);

		//		HS.SetDeviceValueByRef(refId, value, true);
		//	}
		//}

		//if (deviceName.Equals("Mode"))
		//{
		//	modes.ForEach(lMode =>
		//	{
		//		if (lMode.Equals(info.Mode, StringComparison.InvariantCultureIgnoreCase))
		//			value = modes.IndexOf(lMode) + 1;
		//	});

		//	if (value == 0)
		//		HS.SetDeviceString(refId, "Off", true);
		//	else
		//	{
		//		var extraData = device.get_PlugExtraData_Get(HS);
		//		var tempRef = (int)extraData.GetUnNamed(0);
		//		if (tempRef > 0)
		//			HS.SetDeviceValueByRef(tempRef, info.TargetTemperature, true);

		//		HS.SetDeviceString(refId, "", false);

		//		if (info.Mode.Contains("auto"))
		//			HS.SetDeviceString(refId, "Auto-Mode", true);
		//		if (info.Mode.Contains("fan"))
		//			HS.SetDeviceString(refId, "Fan", true);
		//		if (info.Mode.Contains("cool"))
		//			HS.SetDeviceString(refId, "Cool", true);
		//		if (info.Mode.Contains("heat"))
		//			HS.SetDeviceString(refId, "Heat", true);
		//		if (info.Mode.Contains("dry"))
		//			HS.SetDeviceString(refId, "Dry", true);
		//	}

		//	var rootDevice = (DeviceClass)HS.GetDeviceByRef(HS.GetDeviceParentRefByRef(refId));
		//	var deviceList = rootDevice.get_AssociatedDevices(HS).Select(id => (DeviceClass)HS.GetDeviceByRef(id));
		//	var swingDevice = deviceList.Where(dev => dev.get_Address(HS).Contains("Swing")).FirstOrDefault();
		//	var fanSpeedDevice = deviceList.Where(dev => dev.get_Address(HS).Contains("Fan Speed")).FirstOrDefault();

		//	var listFanLevels = mode.FanLevels;
		//	var fanLevelRef = fanSpeedDevice.get_Ref(HS);
		//	HS.DeviceVSP_ClearAll(fanLevelRef, true);
		//	int i = 0;
		//	listFanLevels.ForEach(fan =>
		//	{
		//		var button = Controls.CreateButton(StringExtensions.FirstCharToUpper(fan), i);
		//		HS.DeviceVSP_AddPair(fanLevelRef, button);

		//		VSVGPairs.VGPair graphic = null;
		//		if (fan.Equals("Strong", StringComparison.InvariantCultureIgnoreCase))
		//			graphic = Controls.GetFanGraphic(i, FanLevelsClimate.High);
		//		if (fan.Equals("Quiet", StringComparison.InvariantCultureIgnoreCase))
		//			graphic = Controls.GetFanGraphic(i, FanLevelsClimate.Quiet);
		//		if (fan.Equals("Low", StringComparison.InvariantCultureIgnoreCase))
		//			graphic = Controls.GetFanGraphic(i, FanLevelsClimate.Low);
		//		if (fan.Equals("Medium_Low", StringComparison.InvariantCultureIgnoreCase))
		//			graphic = Controls.GetFanGraphic(i, FanLevelsClimate.Medium_Low);
		//		if (fan.Equals("Medium", StringComparison.InvariantCultureIgnoreCase))
		//			graphic = Controls.GetFanGraphic(i, FanLevelsClimate.Medium);
		//		if (fan.Equals("Medium_High", StringComparison.InvariantCultureIgnoreCase))
		//			graphic = Controls.GetFanGraphic(i, FanLevelsClimate.Medium_High);
		//		if (fan.Equals("High", StringComparison.InvariantCultureIgnoreCase))
		//			graphic = Controls.GetFanGraphic(i, FanLevelsClimate.High);
		//		if (fan.Equals("Auto", StringComparison.InvariantCultureIgnoreCase))
		//			graphic = Controls.GetFanGraphic(i, FanLevelsClimate.Auto);

		//		HS.DeviceVGP_AddPair(fanLevelRef, graphic);
		//		i++;
		//	});

		//	var listSwing = mode.Swing;
		//	var swingRef = swingDevice == null ? 0 : swingDevice.get_Ref(HS);
		//	if (swingRef != 0)
		//	{
		//		HS.DeviceVSP_ClearAll(swingRef, true);
		//		int j = 0;
		//		listSwing.ForEach(swing =>
		//		{
		//			var button = Controls.CreateButton(StringExtensions.FirstCharToUpper(swing), j);
		//			HS.DeviceVSP_AddPair(swingRef, button);
		//			j++;
		//		});
		//	}

		//	HS.SetDeviceValueByRef(refId, value, true);
		//}

		//if (deviceName.Contains("Swing"))
		//{
		//	if (mode.Swing.Any())
		//	{
		//		mode.Swing.ForEach(swing =>
		//		{
		//			if (swing.Equals(info.Swing, StringComparison.InvariantCultureIgnoreCase))
		//				value = mode.Swing.IndexOf(swing);
		//		});

		//		HS.SetDeviceString(refId, "", false);

		//		HS.SetDeviceValueByRef(refId, value, true);
		//		HS.SetDeviceString(refId, HTML.GetImageSwing(StringExtensions.FirstCharToUpper(info.Swing)), false);
		//	}
		//}

		//if (deviceName.Contains("Change Temperature"))
		//{
		//	var currentScale = device.get_ScaleText(HS);
		//	if (!currentScale.Contains(info.TemperatureUnit))
		//	{
		//		TemperatureScale temperatureScale = info.TemperatureUnit.Contains("C") ? TemperatureScale.Celsius : TemperatureScale.Fahrenheit;

		//		HS.DeviceVSP_ClearAll(refId, true);
		//		CreateDeviceControlScale(refId, "Temp", temperatureScale);
		//		CreateDeviceStatusScale(refId, device, "Temp", temperatureScale);
		//	}

		//	HS.SetDeviceString(refId, "", false);
		//	HS.SetDeviceValueByRef(refId, info.TargetTemperature, true);
		//}

		//if (deviceName.Contains("Current Temperature"))
		//{
		//	var currentScale = device.get_ScaleText(HS);
		//	if (!currentScale.Contains(info.TemperatureUnit))
		//	{
		//		TemperatureScale temperatureScale = info.TemperatureUnit.Contains("C") ? TemperatureScale.Celsius : TemperatureScale.Fahrenheit;

		//		HS.DeviceVSP_ClearAll(refId, true);
		//		CreateDeviceStatusScale(refId, device, "Temp", temperatureScale);
		//	}

		//	HS.SetDeviceString(refId, string.Format("{0} {1}", info.CurrentTemperature.ToString(), info.TemperatureUnit), false);
		//	HS.SetDeviceValueByRef(refId, info.CurrentTemperature, true);
		//}

		//if (deviceName.Contains("Current Humidity"))
		//{
		//	HS.SetDeviceString(refId, HTML.GetImageWater(info.CurrentHumidity.ToString()), false);
		//	HS.SetDeviceValueByRef(refId, info.CurrentHumidity, true);
		//}

		//if (deviceName.Contains("SmartMode"))
		//	HS.SetDeviceString(refId, info.SmartMode ? "Timers" : "Off", false);

		//if (deviceName.Contains("Scheduled"))
		//	HS.SetDeviceString(refId, info.Scheduled ? "Timers" : "Off", false);

		//if (device.MISC_Check(HS, Enums.dvMISC.SET_DOES_NOT_CHANGE_LAST_CHANGE))
		//{
		//	var newString = HS.DeviceString(refId);
		//	if (string.IsNullOrEmpty(newString))
		//		newString = device.get_devValue(HS).ToString();

		//	if (!oldString.Equals(newString))
		//	{
		//		device.set_Last_Change(HS, DateTime.Now);
		//		device.ChangedValueORString = true;
		//	}
		//}
		//else
		//{
		//	device.set_Last_Change(HS, DateTime.Now);
		//	device.ChangedValueORString = true;
		//}
	}
}