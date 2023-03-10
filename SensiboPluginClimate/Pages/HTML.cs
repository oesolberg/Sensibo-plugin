using System.Collections.Generic;
using Scheduler;
using System.Linq;
using System.Text;
using HomeSeerAPI;
using HSPI_SensiboClimate.Plugin.API;
using HSPI_SensiboClimate.Plugin.Helpers;

namespace HSPI_SensiboClimate.Pages
{
    public static class HTML
    {
        private static string PageName { get; set; }

        public static string GetStyles()
        {
            var cssStyles = new StringBuilder();

            #region BUILD STYLES
            cssStyles.Append("<style>");

            //add css for buttons
            cssStyles.Append(".custom{background-color: #008CBA; border-radius: 8px; border: none; color: white; width:200px; height:40px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; -webkit-transition-duration: 0.4s; /* Safari */ transition-duration: 0.4s;} .custom:hover{background-color: #007095; box-shadow: 0 12px 16px 0 rgba(0,0,0,0.24),0 17px 50px 0 rgba(0,0,0,0.19);}");
            //add css for table
            cssStyles.Append(".tcustom{border-collapse: collapse; border: 4px double #C2C2C2; width:100%;} .ttcustom{text-align: center; padding: 8px; border: 1px solid #C2C2C2; height:50px; width:50%;}tr:nth-child(even){background-color: #f2f2f2;}");

            cssStyles.Append("</style>");
            #endregion

            return cssStyles.ToString();
        }

        public static string GetMainTemplate(string pageName,IIniSettings iniSettings,ICommunicationHandler communicationHandler, IDeviceHandler deviceHandler, IHSApplication hs)
        {
            PageName = pageName;

            var html = new StringBuilder();

            #region BUILD CONFIG
            html.Append("<table width='1000px'>");
            html.Append("<tr><td></tr>");
            html.Append("<tr style='background-color: transparent'><td>" + BuildTabs(iniSettings, communicationHandler, deviceHandler, hs) + "</td></tr>");
            html.Append("</table>");
            #endregion

            return html.ToString();
        }

        public static string DevicesUpdateUI(IIniSettings iniSettings,ICommunicationHandler communicationHandler,IDeviceHandler deviceHandler,IHSApplication hs)
        {
            return GetDevicesTab(iniSettings,communicationHandler,deviceHandler,hs);
        }

        public static string GetImagePower(string value)
        {
            return GetImageHtml("", value, ".gif");
        }

        public static string GetImageSwing(string value)
        {
            return GetImageHtml(value, "swing");
        }

        public static string GetImageCool(string value)
        {
            return GetImageHtml(value, "Cool");
        }

        public static string GetImageHeat(string value)
        {
            return GetImageHtml(value, "Heat");
        }

        public static string GetImageThermometer(string value)
        {
            return GetImageHtml(value, "Thermometer-60");
        }

        public static string GetImageWater(string value)
        {
            return GetImageHtml(value, "water", ".gif");
        }

        private static string GetImageHtml(string value, string imageName, string format = ".png")
        {
            var html = new StringBuilder();
            html.Append("<div class='device_status_image'>");
            html.Append("<img src='/images/HomeSeer/status/" + imageName + format + "' width='32' height='32' align='absmiddle'> ");
            html.Append(value);
            html.Append("</div>");

            return html.ToString();
        }

        private static string BuildTabs(IIniSettings iniSettings,ICommunicationHandler communicationHandler,IDeviceHandler deviceHandler,IHSApplication hs,string configTab = null)
        {
            var tabs = new clsJQuery.jqTabs("oTabs", PageName);

            var tabConfig = new clsJQuery.Tab();
            tabs.postOnTabClick = true;
            tabConfig.tabTitle = "Config";
            tabConfig.tabDIVID = "oTabConfig";
            tabConfig.tabContent = "<div id='TabConfig_div'>" + GetConfigTab(iniSettings) + "</div>";
            tabs.tabs.Add(tabConfig);

            var tabDevices = new clsJQuery.Tab();
            tabs.postOnTabClick = true;
            tabDevices.tabTitle = "Devices";
            tabDevices.tabDIVID = "oTabDevices";
            tabDevices.tabContent = "<div id='TabDevices_div'>" + GetDevicesTab(iniSettings, communicationHandler, deviceHandler, hs) + "</div>";
            tabs.tabs.Add(tabDevices);

            return tabs.Build();
        }

        private static string GetConfigTab(IIniSettings iniSettings)
        {
            var resetButton = new clsJQuery.jqButton("resetKey", "RESET", PageName, true)
            {
                functionToCallOnClick = "alert(\"After perform 'RESET', please refresh the page by CTRL + F5\");",
                includeClickFunction = true,
                id = "oResetKey",
                style = "width:200px; height:40px; text-align: center;"
            };

            var inputKey = new clsJQuery.jqTextBox("apiKeyInput", "text", "", PageName, 29, true)
            {
                id = "oApiKeyInput",
                promptText = "Enter API Key",
                defaultText = !string.IsNullOrWhiteSpace(iniSettings.ApiKey) ? iniSettings.ApiKey : "Please enter the API Key",//!string.IsNullOrWhiteSpace(Storage.APIKey) ? Storage.APIKey : "Please enter the API Key",
				enabled = string.IsNullOrWhiteSpace(iniSettings.ApiKey)//!string.IsNullOrWhiteSpace(Storage.APIKey) ? false : true
			};
            var interval = iniSettings.TimerIntervalInSeconds;
			
			var inputIntervalInSeconds=
				new clsJQuery.jqTextBox(PluginConstants.TimerIntervalValueKey,"text", interval.ToString(), PageName,2,true)
				{
					id= PluginConstants.TimerIntervalInputKey,style= "text-align: right;",
					width = 1

				};

			var logLevelDropdown = new clsJQuery.jqDropList(PluginConstants.LogLevelKey, PageName, false);
			logLevelDropdown.items = new List<Pair>()
			{
				new Pair() {Name = "None", Value = "0",},
				new Pair() {Name = "Normal", Value = "1"},
				new Pair() {Name = "Debug", Value = "2"},
				new Pair() {Name = "Debug to file", Value = "3"},
				new Pair() {Name = "Debug to file and log", Value = "4"}
			};
			var iniSettingsLogLevel = iniSettings.LogLevel;
			logLevelDropdown.selectedItemIndex = iniSettingsLogLevel.ToInt();
			

			var html = new StringBuilder();

            #region BUILD CONFIG
            html.Append("<table class='tcustom' style='margin-bottom: 10px; background: #b9f6ca;'>");
            html.Append("<tr class='ttcustom'><td class='ttcustom' text-align: left;'>When you sign up for an account, you can generate API Keys <a href='https://home.sensibo.com/me/api'>here</a>, and delete API keys (as you may need to rotate your keys in the future).</td></tr>");
            html.Append("</table>");

            html.Append("<table class='tcustom'>");

            html.Append(PageBuilderAndMenu.clsPageBuilder.FormStart("configTab", "keyCreate", "Post"));
			html.Append("<tr class='ttcustom'><td text-align: center;'> After entering the API Key, the plugin will start working</td><td class='ttcustom' text-align: center;'>" + inputKey.Build() + "</td></tr>");
			html.Append("<tr class='ttcustom'><td text-align: center;'> Here you can set the value for seconds between each check of Sensibo<br/>(lower than 60 seconds might get you locked out )</td><td class='ttcustom' text-align: center;'>" + inputIntervalInSeconds.Build() + "</td></tr>");
			html.Append("<tr class='ttcustom'><td text-align: center;'> This drop down sets the level of logging.<br/></td><td class='ttcustom' text-align: center;'>" + logLevelDropdown.Build() + "</td></tr>");
			html.Append("<tr class='ttcustom'><td text-align: center;'>This action will remove the current API Key</td><td class='ttcustom' text-align: center;'>" + resetButton.Build() + "</td></tr>");
            html.Append(PageBuilderAndMenu.clsPageBuilder.FormEnd());

            html.Append("</table>");
            #endregion

            return html.ToString();
        }

        private static string GetDevicesTab(IIniSettings iniSettings,ICommunicationHandler communicationHandler, IDeviceHandler deviceHandler,IHSApplication hs)
        {
            var html = new StringBuilder();

            #region BUILD DEVICES

            if (!string.IsNullOrWhiteSpace(iniSettings.ApiKey) )
            {
	            //var devices = sensiboApi.GetAllDevices();// Storage.XMLGetAll();
	            var devices = communicationHandler.GetAllDevices();
	            var hsDevices = deviceHandler.GetAllPluginDevices();

				if (devices== null || devices.Count == 0)
		            return string.Empty;

                html.Append("<table class='tcustom' style='margin-bottom: 10px; background: #fafebf;'>");
                html.Append("<tr class='ttcustom'><td class='ttcustom' text-align: left;'>Use <b>Add</b> if you want to add a unit<br>");
				html.Append("Use <b>Remove</b> if you want to remove a unit that you is no longer in your Sensibo account or you don't want it showing in HomeSeer<br>");
                //html.Append("Use <b>Ban</b> if you want to stop displaying the device in the HomeSeer panel on this PC. Another press unlocks the device");
                html.Append("</td></tr></table>");

                html.Append("<table class='tcustom'>");

                foreach (var device in devices) //devices.ForEach(device =>
                {
	                //var root = hsDevices.FirstOrDefault(x => x.get_Address(hs).Contains(device.Id));
	                var rootId = deviceHandler.GetRootDeviceById(device.Id);
	                var addRemoveUnitButton = new clsJQuery.jqButton("addOrRemoveDevice_" + device.Id, "Remove", PageName, false)
	                {
		                id = "remove_" + device.Id,
		                style = "height:40px;"
	                };

					if (rootId == -1)
					{
						addRemoveUnitButton.id = "add_" + device.Id;
						addRemoveUnitButton.label = "Add";
					}

                    

                    //var banButton = new clsJQuery.jqButton("banDevice_" + device.Id, "BAN", PageName, false)
                    //{
                    //    id = "ban_" + device.Id,
                    //    style = "height:40px;"
                    //};

                    html.Append(string.Format("<tr class='ttcustom' {0}><td text-align: center;'>{1}</td><td text-align: center;'>{2}</td><td text-align: center;'>{3}</td><td text-align: center;'>{4}</td><td text-align: center;'>{5}</td></tr>", device.Ban ? "style='background: #fff3f3;'" : "", device.Address, device.Room, device.Mac, device.On ? GetImagePower("on") : GetImagePower("off"), addRemoveUnitButton.Build()));

                };

                html.Append("</table>");
            }
            else
            {
                html.Append("<table class='tcustom' style='margin-bottom: 10px; background: #ff9999;'>");
                html.Append("<tr class='ttcustom'><td class='ttcustom' text-align: left;'>NO DEVICES ADDED</td></tr>");
                html.Append("<tr class='ttcustom'><td class='ttcustom' text-align: left;'>When you sign up for an account, you can generate API Keys <a href='https://home.sensibo.com/me/api'>here</a>, and delete API keys (as you may need to rotate your keys in the future).</td></tr>");
                html.Append("</table>");
            }

            #endregion

            return html.ToString();
        }

    }
}
