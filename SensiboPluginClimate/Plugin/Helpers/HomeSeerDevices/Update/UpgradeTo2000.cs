using System;
using HomeSeerAPI;
using HSPI_SensiboClimate.LocalStorage.Models;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices.Update
{
	public class UpgradeTo2000
	{
		private readonly ILogging _log;
		private readonly IHSApplication _hs;
		private readonly IPedHandler _pedHandler;
		private readonly IHomeSeerDeviceUtils _homeSeerDeviceUtil;
		private readonly IUnitDeviceCreator _deviceCreator;

		private readonly string _versionNumberAsString = "2.0.0.0";

		public UpgradeTo2000(ILogging log, IHSApplication hs, IPedHandler pedHandler, IHomeSeerDeviceUtils homeSeerDeviceUtil, IUnitDeviceCreator deviceCreator)
		{
			_log = log;
			_hs = hs;
			_pedHandler = pedHandler;
			_homeSeerDeviceUtil = homeSeerDeviceUtil;
			_deviceCreator = deviceCreator;
		}

		public void DoUpgrade(DeviceClass rootNode, SensiboDeviceInfo sensiboDeviceInfo)
		{
			var dvRef = rootNode.get_Ref(_hs);
			var location2 = rootNode.get_Location2(_hs);
			var location= rootNode.get_Location(_hs);
			UpdateRootDevice(dvRef, rootNode, sensiboDeviceInfo);
			UpdateChildDevices(rootNode,sensiboDeviceInfo, location,location2);
			AddVersionNumber(dvRef);
			_homeSeerDeviceUtil.UpdateCachedDevices(false);
		}



		private void UpdateRootDevice(int dvRef, DeviceClass rootNode, SensiboDeviceInfo sensiboDeviceInfo)
		{
			DeleteCurrentGraphicsAndStatus(dvRef);
			AddMissingRootNodeInfo(rootNode, sensiboDeviceInfo);
			AddMissingRootPedInfo(rootNode, sensiboDeviceInfo.Id);
			_deviceCreator.AddDeviceImage(rootNode);
			_deviceCreator.AddRootGraphics(dvRef);
			_deviceCreator.AddStatusToRoot(dvRef);
			_hs.SaveEventsDevices();
		}

		private void AddMissingRootNodeInfo(DeviceClass rootNode, SensiboDeviceInfo sensiboUnitInfo)
		{
			var fullName = $"Sensibo Control Panels {sensiboUnitInfo.Room} {sensiboUnitInfo.Id}";
			rootNode.set_Device_Type_String(_hs, PluginConstants.SensiboClimateSky);
			rootNode.set_Address(_hs,sensiboUnitInfo.Id + PluginConstants.HsNameSeparator + PluginConstants.HsRootMarker);
			rootNode.set_Name(_hs,fullName);
		}

		private void AddMissingRootPedInfo(DeviceClass rootNode, string sensiboUnitId)
		{
			AddMissingPedInfo(rootNode, sensiboUnitId, PluginConstants.UnitRootDevice);
		}

		private void AddMissingPedInfo(DeviceClass rootNode, string sensiboUnitId,string deviceType)
		{
			var pluginExtraData = rootNode.get_PlugExtraData_Get(_hs);
			_deviceCreator.AddPedDataToDevice(_pedHandler, pluginExtraData, rootNode, sensiboUnitId, deviceType);
		}

		

		private void DeleteCurrentGraphicsAndStatus(int dvRef)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true); //DeviceVGP_AddPair(dvRef, vgPair);
			_hs.DeviceVSP_ClearAll(dvRef,true);
			_hs.SaveEventsDevices();
		}

		

		private void UpdateChildDevices(DeviceClass rootNode, SensiboDeviceInfo sensiboDeviceInfo, string location, string location2)
		{
			var powerDeviceFound = false;
			var childIds = rootNode.get_AssociatedDevices(_hs);
			foreach (var childId in childIds)
			{

				var associatedDevice = (DeviceClass)_hs.GetDeviceByRef(childId);
				var address = associatedDevice.get_Address(_hs);
				if (address == "Power;child")
				{
					powerDeviceFound = true;
				}

				if (associatedDevice.get_Relationship(_hs) == HomeSeerAPI.Enums.eRelationship.Child)
				{
					DoUpdateForSpecificDevice(associatedDevice,address, rootNode, sensiboDeviceInfo);
				}
				else
				{
					_log.LogError($"UpdateTo2000: Found associated device with device id: {childId} (address: {address}) that is not a child related device!!");
				}


			}

			if (!powerDeviceFound)
			{
				CreatePowerOnOffDevice(rootNode, sensiboDeviceInfo, location,location2);
			}

			
		}

		private void DoUpdateForSpecificDevice(DeviceClass associatedDevice,string address,DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			//var address = associatedDevice.get_Address(_hs);
			
			switch (address)
			{
				case "Temperature":
					UpdateTemperature(associatedDevice, sensiboUnitInfo);
					break;
				case "Current Temperature":
					UpdateCurrentTemperature(associatedDevice, sensiboUnitInfo);
					break;
				case "Mode":
					UpdateMode(associatedDevice, sensiboUnitInfo);
					break;
				case "Fan Speed":
					UpdateFanSpeed(associatedDevice, sensiboUnitInfo);
					break;
				case "Swing":
					UpdateSwing(associatedDevice, sensiboUnitInfo);
					break;
				case "Scheduled":
					UpdateScheduled(associatedDevice, sensiboUnitInfo);
					break;
				case "SmartMode":
					UpdateSmartMode(associatedDevice, sensiboUnitInfo);
					break;
				case "Current Humidity":
					UpdateCurrentHumidity(associatedDevice, sensiboUnitInfo);
					break;
			}
		}

		private void UpdateSwing(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);

			_deviceCreator.CreateChangeSwingGraphics(dvRef);
			_deviceCreator.CreateChangeSwingButtons(dvRef);



			_deviceCreator.AddDeviceImage(associatedDevice);
			AddMissingPedInfo(associatedDevice, sensiboUnitInfo.Id, PluginConstants.ChangeSwingDeviceType);
			_hs.SaveEventsDevices();
		}

		private void UpdateFanSpeed(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);

			_deviceCreator.CreateChangeFanSpeedButtons(dvRef, sensiboUnitInfo.Modes.Fan.FanLevels);



			_deviceCreator.AddDeviceImage(associatedDevice);
			AddMissingPedInfo(associatedDevice, sensiboUnitInfo.Id, PluginConstants.ChangeFanSpeedDeviceType);
			_hs.SaveEventsDevices();
		}

		private void UpdateScheduled(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);

			_deviceCreator.CreateScheduledStateDeviceGraphics(dvRef);
			_deviceCreator.AddOnOffStatusToScheduledStateDevice(dvRef);
			
			_deviceCreator.AddDeviceImage(associatedDevice);
			AddMissingPedInfo(associatedDevice, sensiboUnitInfo.Id, PluginConstants.ScheduledStateDeviceType);
			_hs.SaveEventsDevices();
		}

		private void UpdateMode(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);

			_deviceCreator.CreateChangeModeGraphics(dvRef, sensiboUnitInfo.Modes);
			_deviceCreator.CreateChangeModeButtons(dvRef, sensiboUnitInfo.Modes);

			_deviceCreator.AddDeviceImage(associatedDevice);
			AddMissingPedInfo(associatedDevice, sensiboUnitInfo.Id, PluginConstants.ChangeModeDeviceType);
			_hs.SaveEventsDevices();
		}

		private void UpdateSmartMode(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);

			_deviceCreator.CreateSmartModeStateGraphics(dvRef);
			_deviceCreator.AddOnOffStatusToDevice(dvRef, onStatusString: "Smart mode"); 
			

			_deviceCreator.AddDeviceImage(associatedDevice);
			AddMissingPedInfo(associatedDevice, sensiboUnitInfo.Id, PluginConstants.SmartModeStateDeviceType);
			_hs.SaveEventsDevices();
		}

		private void UpdateCurrentTemperature(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);
			var isCelsius = sensiboUnitInfo.TemperatureUnit == "C";
			_deviceCreator.CreateTemperatureGraphics(dvRef, isCelsius, doNotAddOff: true);
			_deviceCreator.AddDeviceImage(associatedDevice);
			AddMissingPedInfo(associatedDevice, sensiboUnitInfo.Id, PluginConstants.CurrentTemperatureDeviceType);
			_hs.SaveEventsDevices();
		}

		private void UpdateCurrentHumidity(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);
			_deviceCreator.CreateCurrentHumidityGraphics(dvRef);
			_deviceCreator.AddDeviceImage(associatedDevice);
			AddMissingPedInfo(associatedDevice, sensiboUnitInfo.Id, PluginConstants.CurrentHumidityDeviceType);
			_hs.SaveEventsDevices();
		}
		
		private void UpdateTemperature(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);
			var isCelsius = sensiboUnitInfo.TemperatureUnit == "C";
			_deviceCreator.CreateChangeTemperatureGraphics(dvRef,isCelsius);
			_deviceCreator.CreateChangeTemperatureDropDown(dvRef);
			_deviceCreator.AddDeviceImage(associatedDevice);
			AddMissingPedInfo(associatedDevice, sensiboUnitInfo.Id, PluginConstants.ChangeTemperatureDeviceType);
			_hs.SaveEventsDevices();
		}

		private void CreatePowerOnOffDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo, string location, string location2)
		{
			_deviceCreator.CreatePowerOnOffDevice(root, location, location2);
		}
		
		private void AddVersionNumber(int dvRef)
		{
			_pedHandler.AddToPedByHsRef(_versionNumberAsString, PluginConstants.PedVersionKey, dvRef);
			_hs.SaveEventsDevices();
		}
	}
}