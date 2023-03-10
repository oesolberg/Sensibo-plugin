using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HomeSeerAPI;
using HSPI_SensiboClimate.LocalStorage.Models;
using HSPI_SensiboClimate.Plugin.Helpers.Requests;
using Scheduler.Classes;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices.Update
{
	public class HomeSeerDeviceUpgrader
	{
		private readonly ILogging _log;
		private readonly IHSApplication _hs;
		private readonly IHomeSeerDeviceUtils _hsDeviceUtils;
		private readonly IPedHandler _pedHandler;
		private readonly IMediator _mediator;
		private readonly GetVersionIntFromVersionString _getVersionFromString;
		private IUnitDeviceCreator _unitDeviceCreator;

		public HomeSeerDeviceUpgrader(ILogging log, IHSApplication hs, IHomeSeerDeviceUtils hsDeviceUtils, IPedHandler pedHandler, IMediator mediator)
		{
			_log = log;
			_hs = hs;
			_hsDeviceUtils = hsDeviceUtils;
			_pedHandler = pedHandler;
			_mediator = mediator;
			//_homeSeerDeviceUtil = new HomeSeerDeviceUtils(_hs, _log);
			_getVersionFromString = new GetVersionIntFromVersionString();
			_unitDeviceCreator = new UnitDeviceCreator(_hs, _log, _hsDeviceUtils);
		}

		public void DoUpdate(List<SensiboDeviceInfo> sensiboDevices)
		{
			_log.LogDebug("Starting running HomeSeerDeviceUpgrader");
			var rootNodes = GetRootDevices();
			int versionNumber = 0;
			bool sendHsDevicesUpdate = false;
			List<int> rootNodeDvRefs = new List<int>();
			if (rootNodes.Any())
			{
				//Upgrade of rootnodes
				foreach (var rootNode in rootNodes)
				{
					//Up to 2.0.0.0
					versionNumber = GetVersionNumber(rootNode);

					if (versionNumber < PluginConstants.UpdatedFromNoVersion || versionNumber < PluginConstants.UpdateFanAndSwingModes)
					{
						var rootDvRef = rootNode.get_Ref(_hs);

						var unitId = rootNode.get_Address(_hs);
						var sensiboUnitData = sensiboDevices.FirstOrDefault(x => x.Id == unitId);
						if (sensiboUnitData == null)
						{
							sensiboUnitData = sensiboDevices.FirstOrDefault(x => unitId.StartsWith(x.Id));
						}
						if (sensiboUnitData == null)
						{
							_log.LogError($"Could not run update for unitId {unitId} with dvRef {rootDvRef}");
							continue;
						}
						else
						{


							if (versionNumber < PluginConstants.UpdatedFromNoVersion)
							{
								_log.LogDebug(
									$"Doing upgrade from version {versionNumber} to version {PluginConstants.UpdatedFromNoVersion} for device {rootDvRef}");
								var upgrader = new UpgradeTo2000(_log, _hs, _pedHandler, _hsDeviceUtils, _unitDeviceCreator);
								upgrader.DoUpgrade(rootNode, sensiboUnitData);
								sendHsDevicesUpdate = true;
								
							}
							//Upgrades for the whole application 2.0.0.5
							if (versionNumber < PluginConstants.UpdateFanAndSwingModes)
							{
								_log.LogDebug(
									$"Doing upgrade from version {versionNumber} to version {PluginConstants.UpdateFanAndSwingModes} for device {rootDvRef}");
								var upgrader2005 = new UpgradeTo2005(_log, _hs, _pedHandler, _hsDeviceUtils, _unitDeviceCreator);
								upgrader2005.DoUpgrade(rootNode, sensiboUnitData);
								sendHsDevicesUpdate = true;
							}

							rootNodeDvRefs.Add(rootDvRef);
						}

					}
					
				}

				//Upgrades for the whole application 2.0.0.1
				if (versionNumber < PluginConstants.UpdateIniFile)
				{
					//Try to load existing ini file and transfer to new ini file
					var upgrader20001 = new UpgradeTo2001(_log, _hs, _pedHandler, _hsDeviceUtils, _unitDeviceCreator);
					upgrader20001.DoUpgrade(rootNodeDvRefs);
				}



				if (sendHsDevicesUpdate)
				{
					_mediator.Send(new HsDevicesUpdatedEvent(), null);
				}
			}

			_mediator.Send(new UpdateDoneEvent(), null);
			_log.LogDebug("Done running HomeSeerDeviceUpgrader");
		}

		private List<DeviceClass> GetRootDevices()
		{
			var foundDevices = _hsDeviceUtils.GetHomeSeerPluginDevices();
			if (foundDevices.Any())
			{
				return foundDevices
					.Where(x => x.get_Relationship(_hs) == HomeSeerAPI.Enums.eRelationship.Parent_Root)
					.ToList();
			}
			return new List<DeviceClass>();
		}

		private int GetVersionNumber(DeviceClass rootDevice)
		{
			var pedData = rootDevice.get_PlugExtraData_Get(_hs);
			var foundVersionAsString = _pedHandler.GetObjectFromPed(PluginConstants.PedVersionKey, pedData) as string;
			if (foundVersionAsString == null)
			{
				return 0;

			}
			return _getVersionFromString.GetVersionFromString(foundVersionAsString);

		}


	}
}