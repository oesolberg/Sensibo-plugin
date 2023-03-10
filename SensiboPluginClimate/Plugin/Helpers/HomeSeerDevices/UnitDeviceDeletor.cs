using System.Collections.Generic;
using System.Linq;
using HomeSeerAPI;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices
{
	public class UnitDeviceDeleter
	{
			private readonly IHSApplication _hs;
			private readonly ILogging _log;
			private readonly PedHandler _pedHandler;
			private readonly IHomeSeerDeviceUtils _hsDeviceUtils;

			public UnitDeviceDeleter(IHSApplication hs, ILogging log,IHomeSeerDeviceUtils hsDeviceUtils)
			{
				_hs = hs;
				_log = log;
				if (_pedHandler == null)
					_pedHandler = new PedHandler(_hs, _log);
				_hsDeviceUtils = hsDeviceUtils;
				//if (_hsDeviceUtils == null) _hsDeviceUtils = new HomeSeerDeviceUtils(_hs, _log);
			}

			public void Delete(string unitId)
			{
				
				if (_hsDeviceUtils.DeviceExists(unitId))
				{
					var dvRef = _hsDeviceUtils.GetRootDeviceId(unitId);
					if (dvRef > 0)
					{
						var parentDevice = (DeviceClass)_hs.GetDeviceByRef(dvRef);
						var associatedDevices = parentDevice.get_AssociatedDevices(_hs);
						foreach (var deviceId in associatedDevices)
						{
							_hs.DeleteDevice(deviceId);
						}
						_hs.DeleteDevice(dvRef);
						_hs.SaveEventsDevices();
					}
				}
			}

			//public bool DeleteSingleDevice(string ipAddress, string deviceType)
			//{
			//	var nodeRoot = GetRootNodeFromIp(ipAddress);
			//	if (nodeRoot == null)
			//		return false;

			//	return DeleteDeviceFromRoot(nodeRoot, deviceType);

			//	return true;
			//}

			private bool DeleteDeviceFromRoot(DeviceClass nodeRoot, string deviceType)
			{
				var subNodeIds = nodeRoot.get_AssociatedDevices(_hs);
				foreach (var subNodeId in subNodeIds)
				{
					var subNode = (DeviceClass)_hs.GetDeviceByRef(subNodeId);
					if (subNode.get_Relationship(_hs) == HomeSeerAPI.Enums.eRelationship.Child)
					{
						var subNodeDeviceType = GetDeviceTypeFromPedData(subNodeId);
						if (deviceType == subNodeDeviceType)
						{
							//Delete reference to root node
							nodeRoot.AssociatedDevice_Remove(_hs, subNodeId);
							//delete device
							return _hs.DeleteDevice(subNodeId);
						}
					}
				}

				return false;
			}

			private string GetDeviceTypeFromPedData(int dvRef)
			{
				var result = _pedHandler.GetObjectFromPedByHsRef(PluginConstants.DeviceTypeKey, dvRef) as string;
				if (!string.IsNullOrEmpty(result))
				{
					return result;
				}
				return string.Empty;
			}

			//public DeviceClass GetRootNodeFromIp(string ipAddress)
			//{
			//	var devices = _hsDeviceUtils.GetAllPluginDevices();
			//	var foundDevices = devices.Where(x => x.get_Device_Type_String(_hs).Contains(ipAddress)).ToList();
			//	if (foundDevices.Any())
			//	{
			//		var rootDevice = devices.FirstOrDefault(x => x.get_Device_Type_String(_hs)
			//			.Contains(ipAddress) && x.get_Device_Type_String(_hs).Contains(PluginConstants.HsRootMarker));
			//		if (rootDevice != null) return rootDevice;
			//	}
			//	return null;
			//}

			//private List<DeviceClass> GetHomeSeerDevices()
			//{
			//	var deviceList = new List<Scheduler.Classes.DeviceClass>();
			//	var deviceEnumerator = (clsDeviceEnumeration)_hs.GetDeviceEnumerator();
			//	while (!deviceEnumerator.Finished)
			//	{
			//		var foundDevice = deviceEnumerator.GetNext();
			//		deviceList.Add(foundDevice);
			//	}
			//	return deviceList.Where(x => x.get_Interface(_hs) == PluginConstants.PluginName).ToList();
			//}
		}
}