using System;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
	public class PluginLocks
	{
		public static Object HsFetchDevicesLock = new Object();
		public static Object SensiboFetchDevicesLock = new Object();
	}
}