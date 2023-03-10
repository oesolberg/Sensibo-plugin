using System;
using System.Collections.Generic;
using System.Linq;
using HomeSeerAPI;
using HSPI_SensiboClimate.Extensions;
using HSPI_SensiboClimate.LocalStorage.Models;
using HSPI_SensiboClimate.Plugin.API.JSON;
using HSPI_SensiboClimate.Plugin.Helpers.Enums;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices
{
	public interface IUnitDeviceCreator
	{
		bool Execute(SensiboDeviceInfo sensiboUnit);
		void CreatePowerOnOffDevice(DeviceClass root, string room, string location2);

		void AddPedDataToDevice(IPedHandler pedHandler, PlugExtraData.clsPlugExtraData pluginExtraData, DeviceClass dv,
			string unitId, string deviceType);

		void AddDeviceImage(DeviceClass device);
		void AddRootGraphics(int dvRef);
		void AddStatusToRoot(int dvRef);

		void CreateChangeTemperatureGraphics(int dvRef, bool isCelsius = true);
		void CreateChangeTemperatureDropDown(int dvRef);
		void SetTemperatureUnitInfoInPed(string temperatureUnit, int dvRef);

		void CreateCurrentHumidityGraphics(int dvRef);

		void CreateTemperatureGraphics(int dvRef, bool isCelsius, bool doNotAddOff);

		void CreateChangeModeGraphics(int dvRef, Modes modes);
		void CreateChangeModeButtons(int dvRef, Modes modes);

		void CreateSmartModeStateGraphics(int dvRef);

		void CreateScheduledStateDeviceGraphics(int dvRef);
		void AddOnOffStatusToScheduledStateDevice(int dvRef);

		void AddOnOffStatusToDevice(int dvRef, string offStatusString = "Off", string onStatusString = "On", string offStatusImagePath = "", string onStatusImagePath = "");

		void CreateChangeSwingGraphics(int dvRef);
		void CreateChangeSwingButtons(int dvRef);
		void CreateChangeSwingButtonsAndGraphics(int dvRef, List<string> swingModes);

		void CreateChangeFanSpeedButtons(int dvRef, List<string> fanLevels);
	}

	public class UnitDeviceCreator : IUnitDeviceCreator
	{
		private readonly IHSApplication _hs;
		private readonly ILogging _logging;
		private readonly IPedHandler _pedHandler;
		private readonly IHomeSeerDeviceUtils _hsDeviceUtils;
		private readonly IGetVersionIntFromVersionString _getVersionFromString;

		private readonly List<TempRange> _rangesCelsius = new List<TempRange>()
		{
			new TempRange(29d, 150, "110"),
			new TempRange(27, 28.999999, "100"),
			new TempRange(25, 26.999999, "90"),
			new TempRange(23, 24.999999, "80"),
			new TempRange(21, 22.999999, "70"),
			new TempRange(19, 20.999999, "60"),
			new TempRange(16, 18.999999, "50"),
			new TempRange(13, 15.999999, "40"),
			new TempRange(10, 12.999999, "30"),
			new TempRange(7, 9.999999, "20"),
			new TempRange(4, 6.999999, "10"),
			new TempRange(-50, 3.999999, "00"),
			new TempRange(double.MinValue, -50, "Off")

		};

		private readonly List<TempRange> _rangesFarenheit = new List<TempRange>()
		{
			new TempRange(84d, 302, "110"),
			new TempRange(80, 84.999999, "100"),
			new TempRange(77, 79.999999, "90"),
			new TempRange(73, 76.999999, "80"),
			new TempRange(70, 72.999999, "70"),
			new TempRange(66, 69.999999, "60"),
			new TempRange(60, 65.999999, "50"),
			new TempRange(55, 59.999999, "40"),
			new TempRange(50, 54.999999, "30"),
			new TempRange(44, 49.999999, "20"),
			new TempRange(39, 43.999999, "10"),
			new TempRange(-60, 38.999999, "00"),
			new TempRange(double.MinValue, -59.9999, "Off")
		};



		public UnitDeviceCreator(IHSApplication hs, ILogging logging, IHomeSeerDeviceUtils hsDeviceUtils)
		{
			_hs = hs;
			_logging = logging;
			_pedHandler = new PedHandler(_hs, _logging);
			_hsDeviceUtils = hsDeviceUtils;
			_getVersionFromString = new GetVersionIntFromVersionString();
		}

		public bool Execute(SensiboDeviceInfo sensiboUnit)
		{
			var rootId = CreateRootDevice(sensiboUnit);
			if (rootId <= 0) return false;

			var root = (DeviceClass)_hs.GetDeviceByRef(rootId);
			_logging.LogDebug("Starting to create devices");
			_logging.LogDebug("Starting to create power");
			CreatePowerOnOffDevice(root, sensiboUnit.Room);
			_logging.LogDebug("Starting to create fan speed");
			CreateChangeFanSpeedDevice(root, sensiboUnit);
			_logging.LogDebug("Starting to create change mode");
			CreateChangeModeDevice(root, sensiboUnit);
			_logging.LogDebug("Starting to create swing");
			CreateChangeSwingDevice(root, sensiboUnit);
			_logging.LogDebug("Starting to create temperature");
			CreateChangeTemperatureDevice(root, sensiboUnit);
			_logging.LogDebug("Starting to create humidity");
			CreateShowCurrentHumidityDevice(root, sensiboUnit);
			_logging.LogDebug("Starting to create current temp");
			CreateShowCurrentTemperatureDevice(root, sensiboUnit);
			_logging.LogDebug("Starting to create smart mode");
			CreateShowSmartModeStateDevice(root, sensiboUnit);
			_logging.LogDebug("Starting to create scheduled state");
			CreateShowScheduledStateDevice(root, sensiboUnit);


			return true;
		}

		//public bool CreatePowerOnOffDevice(DeviceClass root, string room, string location2)
		//{
		//	CreatePowerOnOffDevice(root, room, location2);
		//	return true;
		//}


		private int CreateRootDevice(SensiboDeviceInfo sensiboUnitInfo)
		{
			var fullName = $"Sensibo Control Panels {sensiboUnitInfo.Room} {sensiboUnitInfo.Id}";
			//Return null if the unit already exists
			if (_hsDeviceUtils.DeviceExists(sensiboUnitInfo.Id))
			{
				_logging.Log($"Unit already exists: '{sensiboUnitInfo.Id}' ");
				return -1;
			}

			var root = CreateRootDeviceNode(sensiboUnitInfo.Id, fullName, sensiboUnitInfo.Room, PluginConstants.PluginName, sensiboUnitInfo);
			var rootId = root.get_Ref(_hs);

			//AddStatusToRoot(rootId);
			SetRootPedVersion(rootId);

			//Device image
			AddDeviceImage(root);
			AddRootGraphics(rootId);
			AddStatusToRoot(rootId);

			_hs.SaveEventsDevices();
			return rootId;
		}

		public void AddRootGraphics(int dvRef)
		{
			CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/notConnected.png", -1);
			CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/Connected.png", 0);
			//CreateRangeGraphics(dvRef, $"images/{Utility.PluginName}/ConnectedWithError.png", 1, double.MaxValue);
		}

		public void AddStatusToRoot(int dvRef)
		{
			var svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Status)
			{
				PairType = VSVGPairs.VSVGPairType.SingleValue,
				Value = -1,
				Status = "Disconnected",
				IncludeValues = false
			};
			_hs.DeviceVSP_AddPair(dvRef, svPair);
			svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Status)
			{
				PairType = VSVGPairs.VSVGPairType.SingleValue,
				Value = 0,
				Status = "Connected",
				IncludeValues = false
			};
			_hs.DeviceVSP_AddPair(dvRef, svPair);
			svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Status)
			{
				PairType = VSVGPairs.VSVGPairType.Range,
				RangeStart = 1,
				RangeEnd = 99999,
				Status = "Error",
				IncludeValues = false
			};
			_hs.DeviceVSP_AddPair(dvRef, svPair);

			_hs.SaveEventsDevices();
		}

		private void SetRootPedVersion(int dvRef)
		{
			var versionNumberAsString = _getVersionFromString.GetVersionStringFromAssembly();
			_pedHandler.AddToPedByHsRef(versionNumberAsString, PluginConstants.PedVersionKey, dvRef);
		}

		public void AddDeviceImage(DeviceClass device)
		{

			device.set_ImageLarge(_hs, $"/images/{PluginConstants.PluginName}/Sensibo-Sky.png");
			device.set_Image(_hs, "/images/Devices/Image-Not-Selected_small.png");

		}

		public DeviceClass CreateRootDeviceNode(string sensiboUnitId, string rootDeviceName, string location, string location2, SensiboDeviceInfo unitInfo)
		{
			try
			{
				//Creating a brand new device, and get the actual device from the device reference
				var rootDevice = (DeviceClass)_hs.GetDeviceByRef(CreateBasicDevice(sensiboUnitId, location, location2, deviceType: PluginConstants.UnitRootDevice, unitId: unitInfo.Id, deviceTypeInfo: DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Thermostat));
				rootDevice.set_Address(_hs, sensiboUnitId + PluginConstants.HsNameSeparator + PluginConstants.HsRootMarker);

				var deviceType = new DeviceTypeInfo_m.DeviceTypeInfo()
				{
					Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Thermostat,
					Device_Type = (int)DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Operating_Mode
				};
				rootDevice.set_DeviceType_Set(_hs, deviceType);
				//DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Thermostat;
				rootDevice.set_Relationship(_hs, HomeSeerAPI.Enums.eRelationship.Parent_Root);
				var dvRef = rootDevice.get_Ref(_hs);
				_hs.SaveEventsDevices();
				return rootDevice; //Return the reference
			}
			catch (Exception ex)
			{
				_logging.LogException(ex);
				throw new Exception("Error creating root device: " + ex.Message, ex);
			}
		}

		public int CreateBasicDevice(string name, string location, string location2, string deviceType = null, string unitId = null, DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI deviceTypeInfo = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Plug_In, string address = "")
		{
			try
			{
				//Creating a brand new device, and get the actual device from the device reference
				var fullName = name;
				var dv = (DeviceClass)_hs.GetDeviceByRef(_hs.NewDeviceRef(fullName));
				int dvRef = dv.get_Ref(_hs);

				//Setting the type to plugin device
				DeviceTypeInfo_m.DeviceTypeInfo typeInfo = new DeviceTypeInfo_m.DeviceTypeInfo();
				typeInfo.Device_Type = (int)deviceTypeInfo;
				typeInfo.Device_API = deviceTypeInfo;
				typeInfo.Device_SubType_Description = name;
				dv.set_DeviceType_Set(_hs, typeInfo);

				//Ped info
				PlugExtraData.clsPlugExtraData pluginExtraData = new PlugExtraData.clsPlugExtraData();

				AddPedDataToDevice(_pedHandler, pluginExtraData, dv, unitId, deviceType);



				dv.set_Interface(_hs, PluginConstants.PluginName); //Don't change this, or the device won't be associated with your plugin
				dv.set_InterfaceInstance(_hs, PluginConstants.PluginInstanceName); //Don't change this, or the device won't be associated with that particular instance

				dv.set_Device_Type_String(_hs, PluginConstants.SensiboClimateSky);
				dv.set_Can_Dim(_hs, false);

				//Setting the name and locations
				dv.set_Name(_hs, name);
				//dv.set_Address(_hs, name.Contains("Change") ? name.Replace("Change ", "") : name);
				if (string.IsNullOrEmpty(address))
					address = name;
				dv.set_Address(_hs, address);
				dv.set_Location(_hs, location);
				dv.set_Location2(_hs, location2);

				//Misc options
				dv.set_Status_Support(_hs, false); //Set to True if the devices can be polled, False if not. (See PollDevice in hspi.vb)
				dv.MISC_Set(_hs, HomeSeerAPI.Enums.dvMISC.SHOW_VALUES); //If not set, device control options will not be displayed.
				dv.MISC_Set(_hs, HomeSeerAPI.Enums.dvMISC.NO_LOG); //As default, we don't want to Log every device value change to the Log




				//Committing to the database, clear value-status-pairs and graphic-status pairs
				_hs.SaveEventsDevices();

				_hs.DeviceVSP_ClearAll(dvRef, true);
				_hs.DeviceVGP_ClearAll(dvRef, true);
				_hs.SaveEventsDevices();
				return dvRef; //Return the reference
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating basic device: " + ex.Message, ex);
			}
		}

		public void AddPedDataToDevice(IPedHandler pedHandler, PlugExtraData.clsPlugExtraData pluginExtraData, DeviceClass dv, string unitId, string deviceType)
		{
			pluginExtraData = pedHandler.AddToPed(true, PluginConstants.PedIsSensiboDataKey, pluginExtraData);

			if (!string.IsNullOrEmpty(unitId))
			{
				pluginExtraData = pedHandler.AddToPed(unitId, PluginConstants.UnitIdKey, pluginExtraData);
			}

			if (!string.IsNullOrEmpty(deviceType))
			{
				pluginExtraData = pedHandler.AddToPed(deviceType, PluginConstants.DeviceTypeKey, pluginExtraData);
			}

			dv.set_PlugExtraData_Set(_hs, pluginExtraData);
			_hs.SaveEventsDevices();
		}

		public void CreatePowerOnOffDevice(DeviceClass root, string location, string location2 = PluginConstants.PluginName)
		{
			var dvRef = CreateChildDevice("Power", location,
				location2, root, PluginConstants.PowerDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);

			var dv = (DeviceClass)_hs.GetDeviceByRef(dvRef);
			var deviceTypeInfo = dv.get_DeviceType_Get(_hs);
			deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Thermostat;
			deviceTypeInfo.Device_Type = (int)DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Operating_Mode;
			dv.set_DeviceType_Set(_hs, deviceTypeInfo);

			CreatePowerOnOffGraphics(dvRef);
			CreatePowerOnOffButtons(dvRef);
			_hs.SaveEventsDevices();
		}

		private void CreatePowerOnOffGraphics(int dvRef)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fan-state-off.png", -300);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/off.gif", 0);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/on.gif", 1);
		}

		private void CreatePowerOnOffButtons(int dvRef)
		{
			_hs.DeviceVSP_ClearAll(dvRef, true);
			CreateButton(dvRef, 1, "On", controlUse: ePairControlUse._On);
			CreateButton(dvRef, 0, "Off", controlUse: ePairControlUse._Off);
		}

		private void CreateChangeTemperatureDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = CreateChildDevice("Change Temperature", sensiboUnitInfo.Room,
				PluginConstants.PluginName, root, PluginConstants.ChangeTemperatureDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);

			var isCelsius = sensiboUnitInfo.TemperatureUnit.Trim().ToUpper() == "C";
			CreateChangeTemperatureGraphics(dvRef, isCelsius);
			CreateChangeTemperatureDropDown(dvRef);
			SetTemperatureUnitInfoInPed(sensiboUnitInfo.TemperatureUnit, dvRef);
			_hs.SaveEventsDevices();
		}

		public void SetTemperatureUnitInfoInPed(string temperatureUnit, int dvRef)
		{
			_pedHandler.AddToPedByHsRef(temperatureUnit, PluginConstants.PedTemperatureUnitKey, dvRef);
		}

		public void CreateChangeTemperatureGraphics(int dvRef, bool isCelsius = true)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);
			CreateTemperatureGraphics(dvRef, isCelsius, doNotAddOff: false);
		}

		private void CreateForGivenTemperatureRange(double startOfRange, double endOfRange, string imageSuffix, int dvRef)
		{
			var graphicsFilePath = $"images/HomeSeer/status/Thermometer-{imageSuffix}.png";
			if (imageSuffix.ToLower() == "off")
			{
				graphicsFilePath = "images/HomeSeer/status/off.gif";
			}
			var vgPair = new VSVGPairs.VGPair
			{
				PairType = VSVGPairs.VSVGPairType.Range,
				RangeStart = startOfRange,
				RangeEnd = endOfRange,
				Graphic = graphicsFilePath
			};
			_hs.DeviceVGP_AddPair(dvRef, vgPair);
		}

		public void CreateChangeTemperatureDropDown(int dvRef)
		{
			_hs.DeviceVSP_ClearAll(dvRef, true);

			//Add status
			HomeSeerAPI.VSVGPairs.VSPair statusPair = new HomeSeerAPI.VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Status);
			statusPair.PairType = HomeSeerAPI.VSVGPairs.VSVGPairType.Range;
			statusPair.RangeStart = -2147483648;
			statusPair.RangeEnd = 2147483647;
			statusPair.RangeStatusSuffix = "@S@";
			statusPair.RangeStatusDecimals = 0;
			statusPair.ControlUse = ePairControlUse.Not_Specified;
			statusPair.HasScale = true;
			statusPair.IncludeValues = true;
			_hs.DeviceVSP_AddPair(dvRef, statusPair);


			//Add control

			HomeSeerAPI.VSVGPairs.VSPair controlPair = new HomeSeerAPI.VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Control);
			controlPair.PairType = HomeSeerAPI.VSVGPairs.VSVGPairType.Range;
			controlPair.RangeStart = 0;
			controlPair.RangeEnd = 30;
			//controlPair.RangeStatusSuffix = "  @S@";
			controlPair.RangeStatusDecimals = 0;
			controlPair.ControlUse = ePairControlUse._HeatSetPoint;
			controlPair.Render = HomeSeerAPI.Enums.CAPIControlType.ValuesRange;
			controlPair.HasScale = true;
			controlPair.IncludeValues = true;

			_hs.DeviceVSP_AddPair(dvRef, controlPair);
		}


		private void CreateChangeModeDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = CreateChildDevice("Change Mode", sensiboUnitInfo.Room,
				PluginConstants.PluginName, root, PluginConstants.ChangeModeDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);

			var dv = (DeviceClass)_hs.GetDeviceByRef(dvRef);
			var deviceTypeInfo = dv.get_DeviceType_Get(_hs);
			deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Thermostat;
			deviceTypeInfo.Device_Type = (int)DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Mode_Set;
			dv.set_DeviceType_Set(_hs, deviceTypeInfo);

			CreateChangeModeGraphics(dvRef, sensiboUnitInfo.Modes);
			CreateChangeModeButtons(dvRef, sensiboUnitInfo.Modes);
			_hs.SaveEventsDevices();
		}

		public void CreateChangeModeGraphics(int dvRef, Modes modes)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);

			//CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/off.gif", 0);
			if (modes.Auto != null)
				CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/auto-mode.png", (int)ModeClimate.Auto);
			if (modes.Cool != null)
				CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/Cool.png", (int)ModeClimate.Cool);
			if (modes.Dry != null)
				CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/water.gif", (int)ModeClimate.Dry);
			if (modes.Fan != null)
				CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fanonly.png", (int)ModeClimate.Fan);
			if (modes.Heat != null)
				CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/Heat.png", (int)ModeClimate.Heat);
		}

		public void CreateChangeModeButtons(int dvRef, Modes modes)
		{
			_hs.DeviceVSP_ClearAll(dvRef, true);
			//CreateButton(dvRef, 0, "Off");
			if (modes.Auto != null)
				CreateButton(dvRef, (int)ModeClimate.Auto, "Auto");
			if (modes.Cool != null)
				CreateButton(dvRef, (int)ModeClimate.Cool, "Cool");
			if (modes.Dry != null)
				CreateButton(dvRef, (int)ModeClimate.Dry, "Dry");
			if (modes.Fan != null)
				CreateButton(dvRef, (int)ModeClimate.Fan, "Fan");
			if (modes.Heat != null)
				CreateButton(dvRef, (int)ModeClimate.Heat, "Heat");
		}

		private void CreateChangeFanSpeedDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = CreateChildDevice("Change Fan Speed", sensiboUnitInfo.Room,
				PluginConstants.PluginName, root, PluginConstants.ChangeFanSpeedDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);
			List<string> fanLevels = null;
			if (sensiboUnitInfo.Modes.Fan?.FanLevels != null)
			{
				fanLevels = sensiboUnitInfo.Modes.Fan.FanLevels;
			}
			else
			{
				fanLevels = sensiboUnitInfo.Modes.Auto.FanLevels;
			}
			CreateChangeFanSpeedButtons(dvRef, fanLevels);
			_hs.SaveEventsDevices();
		}

		public void CreateChangeFanSpeedButtons(int dvRef, List<string> fanLevels)
		{
			_hs.DeviceVSP_ClearAll(dvRef, true);
			_hs.DeviceVGP_ClearAll(dvRef, true);

			foreach (var fanLevel in fanLevels)
			{
				//FanLevelsClimate.
				var buttonValue = GetIntValueFromEnumMatchingString(typeof(FanLevelsClimate), fanLevel);
				if (buttonValue == int.MinValue)
					continue;
				var valueAsEnum = (FanLevelsClimate)buttonValue;
				//var buttonValue = (int)Enum.Parse(typeof(FanLevelsClimate), fanLevel.FirstCharToUpper());
				CreateButton(dvRef, buttonValue, Enum.GetName(typeof(FanLevelsClimate), valueAsEnum));
				if (fanLevel.Equals("Strong", StringComparison.InvariantCultureIgnoreCase))
					CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fan-state-high.png", buttonValue);
				if (fanLevel.Equals("Quiet", StringComparison.InvariantCultureIgnoreCase))
					CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/quiet.png", buttonValue);
				if (fanLevel.Equals("Low", StringComparison.InvariantCultureIgnoreCase))
					CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fan-state-low.png", buttonValue);
				if (fanLevel.Equals("Medium_Low", StringComparison.InvariantCultureIgnoreCase))
					CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/fan.png", buttonValue);
				if (fanLevel.Equals("Medium", StringComparison.InvariantCultureIgnoreCase))
					CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/fan.png", buttonValue);
				if (fanLevel.Equals("Medium_High", StringComparison.InvariantCultureIgnoreCase))
					CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fan-state-high.png", buttonValue);
				if (fanLevel.Equals("High", StringComparison.InvariantCultureIgnoreCase))
					CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fan-state-high.png", buttonValue);
				if (fanLevel.Equals("Auto", StringComparison.InvariantCultureIgnoreCase))
					CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fan-auto.png", buttonValue);
			}
		}

		private int GetIntValueFromEnumMatchingString(Type enumType, string enumAsString)
		{
			if (enumType == typeof(FanLevelsClimate))
			{
				if (Enum.TryParse(enumAsString, true, out FanLevelsClimate foundFanLevelsClimate))
				{
					return (int)foundFanLevelsClimate;
				}
			}

			if (enumType == typeof(SwingState))
			{
				if (Enum.TryParse(enumAsString, true, out SwingState foundSwingState))
				{
					return (int)foundSwingState;
				}
			}
			//if (fanLevel.Equals("Strong", StringComparison.InvariantCultureIgnoreCase))
			return int.MinValue;
		}

		private void CreateChangeSwingDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = CreateChildDevice("Change Swing", sensiboUnitInfo.Room,
				PluginConstants.PluginName, root, PluginConstants.ChangeSwingDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);
			//CreateChangeSwingGraphics(dvRef);
			//CreateChangeSwingButtons(dvRef);
			List<string> swing = null;
			if (sensiboUnitInfo.Modes.Heat?.FanLevels != null)
			{
				swing = sensiboUnitInfo.Modes.Heat.Swing;
			}
			else
			{
				swing = sensiboUnitInfo.Modes.Auto.Swing;
			}
			CreateChangeSwingButtonsAndGraphics(dvRef, swing);
			_hs.SaveEventsDevices();
		}

		public void CreateChangeSwingButtonsAndGraphics(int dvRef, List<string> swingModes)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);
			_hs.DeviceVSP_ClearAll(dvRef, true);
			//foreach (var swingMode in Enum.GetValues(typeof(SwingState)).Cast<SwingState>())
			foreach (var swingMode in swingModes)
			{
				var buttonValue = GetIntValueFromEnumMatchingString(typeof(SwingState), swingMode);
				if (buttonValue == int.MinValue)
					continue;
				var valueAsEnum = (SwingState)buttonValue;
				if (buttonValue == (int)SwingState.Stopped)
				{
					CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/swingOff.png", buttonValue);
				}
				else
				{
					CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/swing.png", buttonValue);
				}
				CreateButton(dvRef, buttonValue, Enum.GetName(typeof(SwingState), valueAsEnum).FirstCharToUpper());
			}
		}

		public void CreateChangeSwingGraphics(int dvRef)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);
			_hs.DeviceVSP_ClearAll(dvRef, true);
			foreach (var swingMode in Enum.GetValues(typeof(SwingState)).Cast<SwingState>())
			{
				if (swingMode == SwingState.Stopped)
				{
					CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/swingOff.png", (int)swingMode);
				}
				else
				{
					CreateSingleValueGraphics(dvRef, $"images/{PluginConstants.PluginName}/swing.png", (int)swingMode);
				}
			}
		}

		private void CreateRangeValueGraphics(int dvRef, string graphicsFilePath, double startOfRange, double endOfRange)
		{
			var vgPair = new VSVGPairs.VGPair
			{
				PairType = VSVGPairs.VSVGPairType.Range,
				RangeStart = startOfRange,
				RangeEnd = endOfRange,
				Graphic = graphicsFilePath
			};
			_hs.DeviceVGP_AddPair(dvRef, vgPair);
		}

		public void CreateChangeSwingButtons(int dvRef)
		{
			_hs.DeviceVSP_ClearAll(dvRef, true);
			//Todo - create only the swing status that the device has
			foreach (var swingMode in Enum.GetValues(typeof(SwingState)).Cast<SwingState>())
			{
				CreateButton(dvRef, (int)swingMode, Enum.GetName(typeof(SwingState), swingMode).FirstCharToUpper());

			}
		}

		private void CreateShowSmartModeStateDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			if(!sensiboUnitInfo.SmartMode)return;

			var dvRef = CreateChildDevice("SmartMode", sensiboUnitInfo.Room,
				PluginConstants.PluginName, root, PluginConstants.SmartModeStateDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);
			CreateSmartModeStateGraphics(dvRef);
			AddOnOffStatusToDevice(dvRef, onStatusString: "Smart mode");
			_hs.SaveEventsDevices();
		}

		public void CreateSmartModeStateGraphics(int dvRef)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fan-state-off.png", -300);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/off.gif", 0);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/on.gif", 1);
		}

		private void CreateShowScheduledStateDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = CreateChildDevice("Scheduled state", sensiboUnitInfo.Room,
				PluginConstants.PluginName, root, PluginConstants.ScheduledStateDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);
			CreateScheduledStateDeviceGraphics(dvRef);
			AddOnOffStatusToScheduledStateDevice(dvRef);
			_hs.SaveEventsDevices();
		}

		public void AddOnOffStatusToScheduledStateDevice(int dvRef)
		{
			AddOnOffStatusToDevice(dvRef, onStatusString: "Scheduled",
						onStatusImagePath: "images/HomeSeer/status/timers.png", offStatusImagePath: $"images/{PluginConstants.PluginName}/timersOff.png");
		}

		public void CreateScheduledStateDeviceGraphics(int dvRef)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/fan-state-off.png", -300);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/off.gif", 0);
			CreateSingleValueGraphics(dvRef, "images/HomeSeer/status/on.gif", 1);
		}

		private void CreateShowCurrentTemperatureDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = CreateChildDevice("Current Temperature", sensiboUnitInfo.Room,
				PluginConstants.PluginName, root, PluginConstants.CurrentTemperatureDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);

			var dv = (DeviceClass)_hs.GetDeviceByRef(dvRef);
			var deviceType = new DeviceTypeInfo_m.DeviceTypeInfo()
			{
				Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Thermostat,
				Device_Type = (int)DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Temperature,
				Device_SubType = (int)DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Temperature.Temperature
			};
			dv.set_DeviceType_Set(_hs, deviceType);
			var isCelsius = sensiboUnitInfo.TemperatureUnit.Trim().ToUpper() == "C";
			CreateCurrentTemperatureGraphics(dvRef, isCelsius);
			_hs.SaveEventsDevices();
		}

		private void CreateCurrentTemperatureGraphics(int dvRef, bool isCelsius = true)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);
			CreateTemperatureGraphics(dvRef, isCelsius, doNotAddOff: true);
		}

		public void CreateTemperatureGraphics(int dvRef, bool isCelsius, bool doNotAddOff)
		{
			var temperatureRange = _rangesCelsius;
			if (!isCelsius)
			{
				temperatureRange = _rangesFarenheit;
			}

			foreach (var tempRange in temperatureRange)
			{
				if (doNotAddOff && tempRange.Suffix.ToUpper() == "OFF")
				{
					continue;
				}
				CreateForGivenTemperatureRange(tempRange.StartOfRange, tempRange.EndOfRange, tempRange.Suffix, dvRef);
			}

		}

		private void CreateShowCurrentHumidityDevice(DeviceClass root, SensiboDeviceInfo sensiboUnitInfo)
		{
			var dvRef = CreateChildDevice("Current Humidity", sensiboUnitInfo.Room,
				PluginConstants.PluginName, root, PluginConstants.CurrentHumidityDeviceType);
			root.AssociatedDevice_Add(_hs, dvRef);

			

			CreateCurrentHumidityGraphics(dvRef);
			_hs.SaveEventsDevices();
		}

		public void CreateCurrentHumidityGraphics(int dvRef)
		{
			_hs.DeviceVGP_ClearAll(dvRef, true);
			CreateRangeValueGraphics(dvRef, "images/HomeSeer/status/water.gif", double.MinValue, double.MaxValue);

		}

		public int CreateChildDevice(string childDeviceName, string location, string location2, DeviceClass rootDevice, string deviceType = null, DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI deviceTypeInfo = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Plug_In)
		{
			try
			{
				//Creating a brand new device, and get the actual device from the device reference
				var childDevice = (DeviceClass)_hs.GetDeviceByRef(CreateBasicDevice(childDeviceName, location, location2, deviceType, deviceTypeInfo: deviceTypeInfo));
				//childDevice.set_Device_Type_String(_hs, childDeviceName + PluginConstants.HsNameSeparator + "child");
				childDevice.set_Relationship(_hs, HomeSeerAPI.Enums.eRelationship.Child);
				childDevice.AssociatedDevice_Add(_hs, rootDevice.get_Ref(_hs));

				int dvRef = childDevice.get_Ref(_hs);

				rootDevice.AssociatedDevice_Add(_hs, dvRef); //Then associated that child reference with the root.

				AddDeviceImage(childDevice);

				//Committing to the database, clear value-status-pairs and graphic-status pairs
				_hs.SaveEventsDevices();

				return dvRef; //Return the reference
			}
			catch (Exception ex)
			{
				_logging.LogException(ex);
				throw new Exception("Error creating child device: " + ex.Message, ex);
			}
		}

		protected void CreateButton(int dvRef, int value, string statusText,
			bool asStatus = false, int renderLocationRow = 0, int renderLocationColumn = 0, ePairControlUse controlUse = ePairControlUse.Not_Specified)
		{
			var renderLocation = new HomeSeerAPI.Enums.CAPIControlLocation() { Column = 0, Row = 0, ColumnSpan = 0 };
			if (renderLocationRow > 0)
			{
				renderLocation.Row = renderLocationRow;
				renderLocation.Column = renderLocationColumn;
			}

			var svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Both)
			{
				PairType = VSVGPairs.VSVGPairType.SingleValue,
				Value = value,
				Status = statusText,
				Render_Location = renderLocation,
				ControlUse = controlUse,//'For IFTTT/HStouch support
				Render = HomeSeerAPI.Enums.CAPIControlType.Button,
				IncludeValues = true
			};
			_hs.DeviceVSP_AddPair(dvRef, svPair);
			if (asStatus)
				_hs.DeviceVSP_ChangePair(dvRef, svPair, ePairStatusControl.Status);
		}

		protected void CreateSingleValueGraphics(int dvRef, string imagePath, int setValue)
		{
			var vgPair = new VSVGPairs.VGPair
			{
				PairType = VSVGPairs.VSVGPairType.SingleValue,
				Set_Value = setValue,
				Graphic = imagePath
			};
			_hs.DeviceVGP_AddPair(dvRef, vgPair);
		}

		public void AddOnOffStatusToDevice(int dvRef, string offStatusString = "Off", string onStatusString = "On", string offStatusImagePath = "", string onStatusImagePath = "")
		{
			var svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Status)
			{
				PairType = VSVGPairs.VSVGPairType.SingleValue,
				Value = 0,
				Status = offStatusString,
				IncludeValues = false
			};
			_hs.DeviceVSP_AddPair(dvRef, svPair);
			svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Status)
			{
				PairType = VSVGPairs.VSVGPairType.SingleValue,
				Value = 1,
				Status = onStatusString,
				IncludeValues = false
			};
			_hs.DeviceVSP_AddPair(dvRef, svPair);

			if (!string.IsNullOrEmpty(offStatusImagePath))
			{
				CreateSingleValueGraphics(dvRef, offStatusImagePath, 0);
			}
			if (!string.IsNullOrEmpty(onStatusImagePath))
			{
				CreateSingleValueGraphics(dvRef, onStatusImagePath, 1);
			}

			//svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Status)
			//{
			//	PairType = VSVGPairs.VSVGPairType.Range,
			//	RangeStart = 1,
			//	RangeEnd = 99999,
			//	Status = "Error",
			//	IncludeValues = false
			//};
			//_hs.DeviceVSP_AddPair(dvRef, svPair);


		}

	}

	internal struct TempRange
	{
		public TempRange(double startOfRange, double endOfRange, string suffix)
		{
			StartOfRange = startOfRange;
			EndOfRange = endOfRange;
			Suffix = suffix;
		}
		public double StartOfRange { get; set; }
		public double EndOfRange { get; set; }
		public string Suffix { get; set; }
	}
}