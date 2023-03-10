using System.Collections.Specialized;
using HomeSeerAPI;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
	public interface IPedHandler
	{
		void AddToPedByHsRef(ListDictionary dataListDictionary, int hsRef);
		void AddToPedByHsRef(object objectToAdd, string pedName, int hsRef);
		PlugExtraData.clsPlugExtraData AddToPed(ListDictionary dataListDictionary, PlugExtraData.clsPlugExtraData plugExtraData);
		PlugExtraData.clsPlugExtraData AddToPed(object objectToAdd, string pedName, PlugExtraData.clsPlugExtraData plugExtraData);
		object GetObjectFromPedByHsRef(string pedName, int hsRef);
		ListDictionary GetListDictionaryOfObjectsFromPedByHsRef(int hsRef);
		object GetObjectFromPed(string pedName, PlugExtraData.clsPlugExtraData plugExtraData);
		ListDictionary GetListDictionaryOfObjectsFromPed(PlugExtraData.clsPlugExtraData plugExtraData);
		//void UpdatePedDataForUnitByRefId(int refId, string pedPortKey, object pedObject);
		object GetObjectFromPedInDevice(string pedName, DeviceClass device);
	}

	public class PedHandler : IPedHandler
	{
		//private ILogging _log;
		private IHSApplication _hs;
		private readonly ILogging _logging;

		public PedHandler(IHSApplication hs, ILogging logging)
		{
			_hs = hs;
			_logging = logging;
		}

		public void AddToPedByHsRef(ListDictionary dataListDictionary, int hsRef)
		{

			var device = (DeviceClass)_hs.GetDeviceByRef(hsRef);
			if (device != null)
			{
				var plugExtraData = device.get_PlugExtraData_Get(_hs);
				plugExtraData = AddToPed(dataListDictionary, plugExtraData);
				device.set_PlugExtraData_Set(_hs, plugExtraData);
			}
		}

		public void AddToPedByHsRef(object objectToAdd, string pedName, int hsRef)
		{
			var device = (DeviceClass)_hs.GetDeviceByRef(hsRef);
			if (device != null)
			{
				var plugExtraData = device.get_PlugExtraData_Get(_hs);
				plugExtraData = AddToPed(objectToAdd, pedName, plugExtraData);
				device.set_PlugExtraData_Set(_hs, plugExtraData);
			}
		}

		public PlugExtraData.clsPlugExtraData AddToPed(ListDictionary dataListDictionary, PlugExtraData.clsPlugExtraData plugExtraData)
		{
			var dataEnumerator = dataListDictionary.GetEnumerator();
			while (dataEnumerator.MoveNext())
			{
				if (dataEnumerator.Key is string)
					plugExtraData = AddToPed(dataEnumerator.Value, dataEnumerator.Key as string, plugExtraData);
			}
			return plugExtraData;
		}

		public PlugExtraData.clsPlugExtraData AddToPed(object objectToAdd, string pedName, PlugExtraData.clsPlugExtraData plugExtraData)
		{
			byte[] byteArray = null;
			if (plugExtraData == null)
			{
				plugExtraData = new PlugExtraData.clsPlugExtraData();
			}
			if (HsCollectionFactory.SerializeObject(ref objectToAdd, ref byteArray, _hs))
			{

				if (!plugExtraData.AddNamed(pedName, byteArray))
				{
					plugExtraData.RemoveNamed(pedName);
					plugExtraData.AddNamed(pedName, byteArray);
				}

				return plugExtraData;
			}
			return null;
		}

		public object GetObjectFromPedByHsRef(string pedName, int hsRef)
		{
			var device = (DeviceClass)_hs.GetDeviceByRef(hsRef);
			if (device != null)
			{
				var plugExtraData = device.get_PlugExtraData_Get(_hs);
				return GetObjectFromPed(pedName, plugExtraData);
			}
			return null;
		}

		public object GetObjectFromPedInDevice(string pedName, DeviceClass device)
		{
			if (device != null)
			{
				var plugExtraData = device.get_PlugExtraData_Get(_hs);
				return GetObjectFromPed(pedName, plugExtraData);
			}
			return null;
		}

		public ListDictionary GetListDictionaryOfObjectsFromPedByHsRef(int hsRef)
		{
			var device = (DeviceClass)_hs.GetDeviceByRef(hsRef);
			if (device != null)
			{
				var plugExtraData = device.get_PlugExtraData_Get(_hs);
				return GetListDictionaryOfObjectsFromPed(plugExtraData);
			}
			return null;
		}

		public object GetObjectFromPed(string pedName, PlugExtraData.clsPlugExtraData plugExtraData)
		{
			var pedObject = plugExtraData?.GetNamed(pedName);
			if (pedObject == null) return null;
			if (pedObject is byte[])
			{
				byte[] pedObjectAsByteArray = (byte[])pedObject;
				object objectToReturn = new object();
				if (HsCollectionFactory.DeSerializeObject(ref pedObjectAsByteArray, ref objectToReturn, _hs))
				{
					return objectToReturn;
				}
			}

			return null;
		}

		public ListDictionary GetListDictionaryOfObjectsFromPed(PlugExtraData.clsPlugExtraData plugExtraData)
		{
			var dataDictionary = new ListDictionary();
			if (plugExtraData == null) return null;

			var dataKeys = plugExtraData.GetNamedKeys();
			foreach (var dataKey in dataKeys)
			{
				dataDictionary.Add(dataKey, GetObjectFromPed(dataKey, plugExtraData));
			}
			return dataDictionary;
		}

		//public void UpdatePedDataForUnitByRefId(int refId, string pedPortKey, object pedObject)
		//{
		//	var pedData = GetListDictionaryOfObjectsFromPedByHsRef(refId);
		//	if (pedData.Contains(pedPortKey))
		//	{
		//		pedData[pedPortKey] = pedObject;
		//	}
		//	else
		//	{
		//		pedData.Add(pedPortKey,pedObject);
		//	}

		//}
	}
}