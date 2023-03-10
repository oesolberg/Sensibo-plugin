using System;
using System.Collections.Generic;
using System.Diagnostics;
using HomeSeerAPI;
using HSPI_SensiboClimate.LocalStorage.Models;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices.Update
{
	public class UpgradeTo2005
	{
		private readonly ILogging _log;
		private readonly IHSApplication _hs;
		private readonly IPedHandler _pedHandler;
		private readonly IHomeSeerDeviceUtils _homeSeerDeviceUtil;
		private readonly IUnitDeviceCreator _deviceCreator;

		private readonly string _versionNumberAsString = "2.0.0.5";

		public UpgradeTo2005(ILogging log, IHSApplication hs, IPedHandler pedHandler, IHomeSeerDeviceUtils homeSeerDeviceUtil, IUnitDeviceCreator deviceCreator)
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
			var location = rootNode.get_Location(_hs);
			UpdateChildDevices(rootNode, sensiboDeviceInfo, location, location2);
			AddVersionNumber(dvRef);
			_homeSeerDeviceUtil.UpdateCachedDevices(false);
		}

		private void UpdateChildDevices(DeviceClass rootNode, SensiboDeviceInfo sensiboDeviceInfo, string location, string location2)
		{
			var childIds = rootNode.get_AssociatedDevices(_hs);
			foreach (var childId in childIds)
			{
				var associatedDevice = (DeviceClass)_hs.GetDeviceByRef(childId);
				var address = associatedDevice.get_Address(_hs);

				if (associatedDevice.get_Relationship(_hs) == HomeSeerAPI.Enums.eRelationship.Child)
				{
					DoUpdateForSpecificDevice(associatedDevice, address, rootNode, sensiboDeviceInfo);
				}
				else
				{
					_log.LogError($"UpdateTo2005: Found associated device with device id: {childId} (address: {address}) that is not a child related device!!");
				}
			}
		}

		private void DoUpdateForSpecificDevice(DeviceClass associatedDevice, string address, DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			var deviceType =
				_pedHandler.GetObjectFromPedInDevice(PluginConstants.DeviceTypeKey, associatedDevice) as string;
			if (deviceType == null)
				return;

			switch (deviceType)
			{

				case PluginConstants.ChangeFanSpeedDeviceType:
					UpdateFanSpeed(associatedDevice, sensiboUnitInfo);
					break;
				case PluginConstants.ChangeSwingDeviceType:
					UpdateSwing(associatedDevice, sensiboUnitInfo);
					break;
				
				
			}
		}

		private void UpdateFanSpeed(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);

			_deviceCreator.CreateChangeFanSpeedButtons(dvRef, sensiboUnitInfo.Modes.Fan.FanLevels);
			
			
			_hs.SaveEventsDevices();
		}

		private void UpdateSwing(DeviceClass associatedDevice, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = associatedDevice.get_Ref(_hs);

			_deviceCreator.CreateChangeSwingButtonsAndGraphics(dvRef, sensiboUnitInfo.Modes.Heat.Swing);
			
			_hs.SaveEventsDevices();
		}
		
		private void AddVersionNumber(int dvRef)
		{
			_pedHandler.AddToPedByHsRef(_versionNumberAsString, PluginConstants.PedVersionKey, dvRef);
			_hs.SaveEventsDevices();
		}
	}
}