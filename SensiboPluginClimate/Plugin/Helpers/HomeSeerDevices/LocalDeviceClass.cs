using HomeSeerAPI;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices
{
	public class LocalDeviceClass
	{
		public int[] AssociatedDevices { get; set; }
		public int Ref { get; set; }
		public string DeviceTypeString { get; set; }
		public HomeSeerAPI.Enums.eRelationship Relationship { get; set; }
		public PlugExtraData.clsPlugExtraData PlugExtraData { get; set; }
		public string ScaleText { get; set; }
		public string DeviceType { get; set; }
		public string Address { get; set; }


		
		public string WriteInfo()
		{
			return $"Type: {DeviceType}, Ref:{Ref}, Address {Address}";
		}
	}
}