using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HomeSeerAPI;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices
{
	public interface IHomeSeerDeviceUtils
	{
		bool DeviceExists(string name);
		int GetRootDeviceId(string name);
		//int GetDeviceIdByIpAddress(string ipAddress);
		int GetRootDeviceIdByUnitId(string unitId);
		LocalDeviceClass GetRootDeviceByUnitId(string unitId);
		List<LocalDeviceClass> GetAssociatedDevices(int deviceId);

		List<LocalDeviceClass> GetLocalHomeSeerPluginDevices();
		//Use next with care!!!
		List<DeviceClass> GetHomeSeerPluginDevices();
		LocalDeviceClass GetDeviceByHsRef(int dvRef);
		void UpdateCachedDevices(bool useNewThread=true);
	}

	public class HomeSeerDeviceUtils : IHomeSeerDeviceUtils
	{
		private readonly IHSApplication _hs;
		private readonly ILogging _log;
		private readonly IPedHandler _pedHandler;

		private DateTime _lastFetchDevicesDateTime;
		private List<LocalDeviceClass> _localDevices;
		

		public HomeSeerDeviceUtils(IHSApplication hs, ILogging log)
		{
			_hs = hs;
			_log = log;
			if (_pedHandler == null)
			{
				_pedHandler = new PedHandler(hs, log);
			}
		}

		public List<LocalDeviceClass> GetLocalHomeSeerPluginDevices()
		{
			if (_localDevices == null)
			{
				_lastFetchDevicesDateTime = SystemTime.Now();
				CreateLocalDevices();
			}


			return _localDevices;
		}

		public LocalDeviceClass GetDeviceByHsRef(int dvRef)
		{
			var devices = GetLocalHomeSeerPluginDevices();
			if (devices.Any())
			{
				var foundDevice = devices.FirstOrDefault(x => x.Ref == dvRef);
				return foundDevice;
			}
			return null;
		}

		public void UpdateCachedDevices(bool useNewThread=true)
		{
			_log.LogDebug("Fetching devices to local cache");
			if(useNewThread){
				var updateLocalDevicesThread = new Thread(CreateLocalDevices);
				updateLocalDevicesThread.Start();
			}
			else
			{
				CreateLocalDevices();
			}
		}

		private void CreateLocalDevices(object obj = null)
		{
			_log.LogDebug("CreateLocalDevices: Trying to update local devices");
			var stopWatch=new Stopwatch();
			stopWatch.Start();
			bool lockTaken = false;
			var timeout = new TimeSpan(0, 1, 0);
			try
			{
				Monitor.TryEnter(PluginLocks.HsFetchDevicesLock, timeout, ref lockTaken);
				if (lockTaken)
				{
					_log.LogDebug("CreateLocalDevices: Locking and fetching HomeSeer devices");
					var hsDevices = GetHomeSeerPluginDevices();
					_localDevices = MapHsDeviceClassToLocalDeviceClass(hsDevices);
					_lastFetchDevicesDateTime = SystemTime.Now();
					stopWatch.Stop();
					_log.LogDebug($"CreateLocalDevices: Done fetching and mapping HS devices to local cache (time used: {stopWatch.ElapsedMilliseconds:# ###} ms");
				}
				else
				{
					_log.LogDebug("CreateLocalDevices: Could not fetch hs devices due to lock for more than 1 minute");
				}
			}
			finally
			{
				if (lockTaken) Monitor.Exit(PluginLocks.HsFetchDevicesLock);
			}
		}

		private List<LocalDeviceClass> MapHsDeviceClassToLocalDeviceClass(List<DeviceClass> hsDevices)
		{
			var localDeviceList = new List<LocalDeviceClass>();
			foreach (var deviceClass in hsDevices)
			{
				var newLocalDevice = new LocalDeviceClass();
				newLocalDevice.Ref = deviceClass.get_Ref(_hs);
				newLocalDevice.AssociatedDevices = deviceClass.get_AssociatedDevices(_hs);
				newLocalDevice.DeviceTypeString = deviceClass.get_Device_Type_String(_hs);
				newLocalDevice.Relationship = deviceClass.get_Relationship(_hs);
				newLocalDevice.PlugExtraData = deviceClass.get_PlugExtraData_Get(_hs);
				newLocalDevice.ScaleText = deviceClass.get_ScaleText(_hs);
				newLocalDevice.DeviceType =(string)
					_pedHandler.GetObjectFromPed(PluginConstants.DeviceTypeKey, newLocalDevice.PlugExtraData);
				newLocalDevice.Address = deviceClass.get_Address(_hs);
				//newLocalDevice.Location = deviceClass.get_Location(_hs);
				//newLocalDevice.Location = deviceClass.get_Location2(_hs);
				//newLocalDevice.Location = deviceClass.get_Name(_hs);

				localDeviceList.Add(newLocalDevice);
				if(_log.IsLoggingDebug)
					_log.LogDebug($"added device with data: {newLocalDevice.WriteInfo()}");
			}

			return localDeviceList;
		}

		public bool DeviceExists(string unitId)
		{
			if (GetRootDeviceIdByUnitId(unitId) > 0) return true;
			return false;
		}

		public int GetRootDeviceIdByUnitId(string unitId)
		{
			var device = GetRootDeviceByUnitId(unitId);
			if (device != null)
				return device.Ref;

			return -1;
		}

		public LocalDeviceClass GetRootDeviceByUnitId(string unitId)
		{
			var devices = GetLocalHomeSeerPluginDevices();
			var foundDevices = devices.Where(x => x.Address.Contains(unitId)).ToList();
			if (foundDevices.Any())
			{
				var rootDevice = devices.FirstOrDefault(x =>
					x.Address.Contains(unitId) &&
					x.Address.Contains(PluginConstants.HsRootMarker));
				if (rootDevice != null) return rootDevice;
			}

			return null;
		}

		public int GetRootDeviceId(string name)
		{
			var devices = GetLocalHomeSeerPluginDevices();
			var foundDevices = devices.Where(x => x.Address.Contains(name)).ToList();
			if (foundDevices.Any())
			{
				var rootDevice = devices.FirstOrDefault(x =>
					x.Address.Contains(name) &&
					x.Address.Contains(PluginConstants.HsRootMarker));
				if (rootDevice != null) return rootDevice.Ref;
			}

			return -1;
		}
		
		public List<DeviceClass> GetHomeSeerPluginDevices()
		{
			var deviceList = new List<Scheduler.Classes.DeviceClass>();
			var deviceEnumerator = (clsDeviceEnumeration) _hs.GetDeviceEnumerator();
			while (!deviceEnumerator.Finished)
			{
				var foundDevice = deviceEnumerator.GetNext();
				deviceList.Add(foundDevice);
			}

			return deviceList.Where(x => x.get_Interface(_hs) == PluginConstants.PluginName).ToList();
		}


		public List<LocalDeviceClass> GetAssociatedDevices(int deviceId)
		{
			var allDevices = GetLocalHomeSeerPluginDevices();
			return allDevices.Where(x => x.Relationship == HomeSeerAPI.Enums.eRelationship.Child &&
			                             x.AssociatedDevices.Length == 1 &&
			                             x.AssociatedDevices.Contains(deviceId)).ToList();
		}

	}
}