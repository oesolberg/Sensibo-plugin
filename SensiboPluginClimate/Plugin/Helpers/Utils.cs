using HomeSeerAPI;
using Scheduler.Classes;
using HSPI_SensiboClimate.Plugin.Enums;
using System.Collections.Generic;
using System.Linq;

namespace HSPI_SensiboClimate.Plugin
{
    public static class Utils
    {
        private static string PluginName { get => HSPI.PluginName; }
        private static IHSApplication HS { get => HSPI.HSApp; }

  //      public static void Log(string message, LogType logLevel = LogType.Normal)
		//{
		//	switch (logLevel)
		//	{
  //              case LogType.Debug:
  //                  HS.WriteLog(PluginName + " Debug", message);
		//			break;

		//		case LogType.Normal:
  //                  HS.WriteLog(PluginName, message);
		//			break;

		//		case LogType.Warning:
  //                  HS.WriteLog(PluginName + " Warning", message);
		//			break;

		//		case LogType.Error:
  //                  HS.WriteLog(PluginName + " ERROR", message);
		//			break;
		//	}
		//}

        public static List<DeviceClass> DevicesOnlyForPlugin()
        {
            var devices = new List<DeviceClass>();

            clsDeviceEnumeration deviceEnumeration = (clsDeviceEnumeration)HS.GetDeviceEnumerator();
            while (!deviceEnumeration.Finished)
            {
                var device = deviceEnumeration.GetNext();
                if (device.get_Interface(HS) == PluginName)
                    devices.Add(device);
            }

            return devices;
        }

        public static string GetACId(int deviceRef)
        {
            var rootRef = HS.GetDeviceParentRefByRef(deviceRef);

            var root = (DeviceClass)HS.GetDeviceByRef(rootRef);
            var acId = root.get_Address(HS);

            return acId;
        }

    }
}
