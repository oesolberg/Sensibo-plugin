using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using HomeSeerAPI;
using HSPI_SensiboClimate.LocalStorage.Models;
using Scheduler;
using HSPI_SensiboClimate.Plugin;
using HSPI_SensiboClimate.Plugin.API;
using HSPI_SensiboClimate.Plugin.Helpers;

namespace HSPI_SensiboClimate.Pages
{
	public class MainPage : PageBuilderAndMenu.clsPageBuilder
	{
		private readonly IIniSettings _iniSettings;
		private readonly ICommunicationHandler _communicationHandler;
		private readonly IDeviceHandler _deviceHandler;
		private readonly IHSApplication _hs;
		private readonly ILogging _logging;
		private string PluginName { get => HSPI.PluginName; }
		private IHSApplication HS { get => HSPI.HSApp; }
		private IAppCallbackAPI Callback { get => HSPI.CallbackApp; }

		private string MainPageName { get; set; }

		public MainPage(string pageName, IIniSettings iniSettings, ICommunicationHandler communicationHandler, 
			IDeviceHandler deviceHandler, IHSApplication hs, ILogging logging) : base(pageName)
		{
			_iniSettings = iniSettings;
			_communicationHandler = communicationHandler;
			_deviceHandler = deviceHandler;
			_hs = hs;
			_logging = logging;
			MainPageName = pageName;
		}

		public override string GetPage(ref StateObject state, string queryString)
		{
			return base.GetPage(ref state, queryString);
		}

		public override string postBackProc(string page, string data, string user, int userRights)
		{
			_logging.LogDebug($"Page:{page}, data:{data}, user:{user}");
			var parts = HttpUtility.ParseQueryString(data);

			if (parts["id"].Contains("remove_") || parts["id"].Contains("add_"))
			{
				var id = parts["id"].Replace("remove_", "").Replace("add_", "");
				if (parts["id"].Contains("remove_"))
					RemoveUnit(id);
				if (parts["id"].Contains("add_"))
					AddUnit(id);
			}

			switch (parts["id"])
			{
				case "oResetKey":
					ResetKey();
					break;

				case "oApiKeyInput":
					AddApiKey(parts["apiKeyInput"]);
					break;
				case PluginConstants.TimerIntervalInputKey:
					SetTimerInterval(parts[PluginConstants.TimerIntervalValueKey]);
					break;
				case PluginConstants.LogLevelKey:
					ChangeLogLevel(parts[PluginConstants.LogLevelKey]);
					break;
			}

			data = parts.ToString();

			return base.postBackProc(page, data, user, userRights);
		}

		private void ChangeLogLevel(string part)
		{
			var newLogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), part);
			_iniSettings.LogLevel = newLogLevel;
		}

		private void SetTimerInterval(string timerIntervalAsString)
		{
			if (int.TryParse(timerIntervalAsString, out int resultingInterval))
			{
				_iniSettings.TimerIntervalInSeconds = resultingInterval;
			}
		}

		private void AddApiKey(string key)
		{
			UpdateKey(key);
			if (!string.IsNullOrEmpty(key))
			{
				//Fetch all AC units and start building the tabs
				UpdateDevicesTab();
			}
		}

		private void ResetKey()
		{
			UpdateKey(string.Empty);
		}

		private void UpdateKey(string key = "")
		{
			_iniSettings.ApiKey = key;
			pageCommands.Add("refresh", "true");

			//HS.SaveINISetting("Settings", "APIKey", key, "SensiboPlugin.ini");
			//Storage.APIKey = key;
		}


		private void AddUnit(string id)
		{
			_logging.LogDebug($"trying to add unit with id {id}");

			var sensiboUnits = _communicationHandler.GetAllDevices();
			
			
			var newUnit = GetUnitById(id, sensiboUnits);
			if(newUnit!=null)
				_deviceHandler.CreateUnit(id, newUnit);
			else
			{
				//Should add some error message since we could not create a new unit by Id
				var numberOfSensiboUnits = -1;
				if (sensiboUnits != null)
				{
					numberOfSensiboUnits = sensiboUnits.Count;
				}
				_logging.LogError($"Could not add new unit with id '{id}'. Number of returned units from SensiboSky={numberOfSensiboUnits}");
			}

			UpdateDevicesTab();
		}


		private SensiboDeviceInfo GetUnitById(string id, List<SensiboDeviceInfo> sensiboUnits)
		{
			if (sensiboUnits != null && sensiboUnits.Count > 0)
			{
				var newUnit = sensiboUnits.FirstOrDefault(x => x.Id == id);
				return newUnit;
			}
			return null;
		}


		private void RemoveUnit(string id)
		{
			//Todo - Delete all devices connected with this unit
			_deviceHandler.DeleteUnit(id);
			//var done = DeleteDevices(id);
			//if (done)
			//    Storage.XMLRemoveDevice(id);
			
			UpdateDevicesTab();
		}

		//private bool DeleteDevices(string id)
		//{
		//	var deviceList = Utils.DevicesOnlyForPlugin();
		//	var root = deviceList.Where(x => x.get_Address(HS).Contains(id)).SingleOrDefault();
		//	var rootRef = root.get_Ref(HS);

		//	var childrenRef = root.get_AssociatedDevices(HS);
		//	childrenRef.ToList().ForEach(refDev => HS.DeleteDevice(refDev));

		//	var done = HS.DeleteDevice(rootRef);

		//	return done;
		//}

		private void UpdateDevicesTab()
		{
			//var deviceInfo = _sensiboApi.GetAllDevices();

			divToUpdate.Add("oTabDevices", HTML.DevicesUpdateUI(_iniSettings,_communicationHandler, _deviceHandler, _hs));
		}

		private string BuildContent()
		{
			reset();

			var html = new StringBuilder();

			//add menus and headers
			html.Append(HS.GetPageHeader(MainPageName, PluginName, "", "", false, false));

			//add styles
			html.Append(HTML.GetStyles());

			//add content
			html.Append(HTML.GetMainTemplate(PageName, _iniSettings, _communicationHandler,  _deviceHandler, _hs));

			return html.ToString();
		}

		internal string GetPagePlugin(object pageName, string user, int userRights, string queryString)
		{
			NameValueCollection parts = null;
			if (!string.IsNullOrEmpty(queryString))
			{
				parts = HttpUtility.ParseQueryString(queryString);
			};

			var html = new StringBuilder();

			html.Append(DivStart("pluginpage", ""));
			_communicationHandler.GetAllDevices(overrideCaching:true);
			html.Append(BuildContent());
			html.Append(DivEnd());

			AddBody(html.ToString());
			return BuildPage();
		}
	}
}
