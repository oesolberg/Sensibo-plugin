using HomeSeerAPI;
using HSPI_SensiboClimate.Pages;
using HSPI_SensiboClimate.Plugin.API;
using HSPI_SensiboClimate.Plugin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices;
using HSPI_SensiboClimate.Plugin.Helpers.Requests;

namespace HSPI_SensiboClimate
{
    public class SensiboPlugin:IHandler
    {
	    private readonly IHSApplication _hs;
	    private readonly IAppCallbackAPI _appCallbackApi;

	    //private string PluginName { get => HSPI.PluginName; }
        //private IHSApplication HS { get => HSPI.HSApp; }
        private static System.Threading.Timer UpdateTimer { get; set; }

        //private IAppCallbackAPI Callback { get => HSPI.CallbackApp; }
        private string MainPageName => PluginConstants.PluginName + "MainPage";

        private ISensiboRestHandler _sensiboRestHandler;
        private MainPage _mainPage;
        private readonly IIniSettings _iniSettings;
        //private string _apiKey;
        private readonly IMediator _mediator;
        private ICommunicationHandler _communicationHandler;
        private IDeviceHandler _deviceHandler;
        private readonly ILogging _logging;
        private readonly IHomeSeerDeviceUtils _hsDeviceUtils;
        private bool _updateDone;

        public SensiboPlugin(IHSApplication hs, IAppCallbackAPI appCallbackApi)
        {
	        _hs = hs;
	        _appCallbackApi = appCallbackApi;
	        _mediator=new Mediator();
			_mediator.AddHandler(this);
           _iniSettings=new IniSettings(_hs, _mediator);
           _logging = new Logging(_iniSettings, _hs, _mediator);
           _hsDeviceUtils = new HomeSeerDeviceUtils(_hs, _logging);
           
		}


        public void InitIo()
        {
			//Utils.Log("InitIo started",LogType.Debug);
			_logging.LogDebug("InitIo started");
			InitializePlugin();
			_logging.LogDebug("InitIo finished");
		}

		private void InitializePlugin()
        {
	        //GetApiKey();
	        
			if (_iniSettings.RunFake)
			{
				_sensiboRestHandler = new SensiboFakeRestHandler(_iniSettings, _logging);
			}
			else
			{
				_sensiboRestHandler = new SensiboRestHandler(_iniSettings,_logging);
			}
			_deviceHandler = new DeviceHandler(_mediator, _hs, _logging, _hsDeviceUtils, _sensiboRestHandler);
			_communicationHandler =new CommunicationHandler(_sensiboRestHandler,_logging);
			
			_mainPage = new MainPage(MainPageName, _iniSettings, _communicationHandler, _deviceHandler, _hs, _logging);

			RunUpdateOfDevicesAndUnits();

			RegisterPages();

	        UpdateDevices();

	        CreateTimerUpdate();


        }



		public async Task SetIoMultiAsync(List<CAPI.CAPIControl> colSend)
		{
			if (!_updateDone)
			{
		
				_logging.Log("Plugin still updating to next version. Please wait.");
				return;
			}

			_deviceHandler.SetIoMulti(colSend);


		}

        public string PostBackProc(string pageName, string data, string user, int userRights)
        {
	        if (!_updateDone)
	        {

		        _logging.Log("Plugin still updating to next version. Please wait.");
		        return string.Empty;
	        }

			return _mainPage.postBackProc(pageName, data, user, userRights);
        }

        internal string GetPagePlugin(string page, string user, int userRights, string queryString)
        {
	        if (!_updateDone)
	        {

		        _logging.Log("Plugin still updating to next version. Please wait.");
		        return "Plugin still updating to next version. Please refresh in 10 seconds."; 
	        }
			switch (page)
            {
                default:
                    return _mainPage.GetPagePlugin(page, user, userRights, queryString);
            }

        }

        private void RegisterWebPage(string pageName, string linkText, string pageTitle)
        {
            try
            {
	            _hs.RegisterPage(pageName, PluginConstants.PluginName, "");

                linkText = linkText ?? pageName;
                pageTitle = pageTitle ?? linkText;

                var webPageDescription = new WebPageDesc()
                {
                    plugInName = PluginConstants.PluginName,
                    link = pageName,
                    linktext = linkText,
                    page_title = pageTitle
                };

                _appCallbackApi.RegisterLink(webPageDescription);
                _appCallbackApi.RegisterConfigLink(webPageDescription);
                //HS.RegisterHelpLink
            }
            catch (Exception ex)
            {
                Console.WriteLine("Registering Web Links (RegisterWebPage): " + ex.Message);
            }
        }

        private void RegisterPages()
        {
            RegisterWebPage(MainPageName, "Admin Panel", "Sensibo");
        }


        private void RunUpdateOfDevicesAndUnits()
        {
	        var updaterThread = new Thread(StartDeviceUpdate);
	        updaterThread.Start();
        }

        private void StartDeviceUpdate()
        {
	        var sensiboUnits = _communicationHandler.GetAllDevices().ToList();
	        _deviceHandler.DoUpdates(sensiboUnits);
        }

		


		private bool UpdateDevices()
		{
			if(!_updateDone)
			{
				_logging.Log("Plugin still updating to next version. Please wait.");
				return false;
			}
			//var hsDeviceList = Utils.DevicesOnlyForPlugin();
			var hsDeviceList = _deviceHandler.GetLocalDevices();

			
			//Setting everything to missing api key if we dont have api key, but have devices
            if (string.IsNullOrWhiteSpace(_iniSettings.ApiKey))
            {
				_logging.LogWarning($"No api key found in settings for Sensibo plugin");
                if (hsDeviceList.Any())
                {
					_deviceHandler.UpdateControlPanelOffline(hsDeviceList);
				}
                return false;
            }

            //var buildList = new List<BuildControlPanelModel>();

            var devicesInfo =  _communicationHandler.GetAllDevices();
            if (_logging.IsLoggingDebug)
            {
	            if (devicesInfo != null)
	            {
		            _logging.LogDebug($"Got {devicesInfo.Count} devices from Sensibo Api");
	            }
				else
				{
					_logging.LogDebug($"Got null response from Sensibo Api");
				}
			}
            if (devicesInfo != null && devicesInfo.Count>0)
            {
	            
				//List<SensiboDeviceInfo> devicesInfoXML = new List<SensiboDeviceInfo>();
				for (int i = devicesInfo.Count-1; i > -1; i--)
                {
	                var device = devicesInfo[i];
	                if (!hsDeviceList.Any(x => x.Address.Contains(device.Id) && x.Address.EndsWith(PluginConstants.HsRootMarker)))
					{
						//Remove data for any devices not found
						_logging.LogDebug($"Removing device with id {device.Id} since it is not found in current devices");
						devicesInfo.RemoveAt(i);
					}
				}

           
                //UI Update
                _deviceHandler.UpdateControlPanel(devicesInfo);

				//UpdateControlPanel(devicesInfo);
				_logging.LogDebug("Done checking of states");
				return true;
            }
            else
            {
	            _deviceHandler.UpdateControlPanelOffline(hsDeviceList, "No status");
	            return false;
            }
        }

        private void CreateTimerUpdate()
        {

	        UpdateTimer = new System.Threading.Timer(new TimerCallback(UpdateAirConStates), null,
		        Timeout.Infinite, _iniSettings.TimerIntervalInSeconds * 1000);
	        UpdateTimer.Change(new TimeSpan(0, 0, 10), new TimeSpan(0, 0, _iniSettings.TimerIntervalInSeconds));
        }

        private void UpdateAirConStates(object obj=null)
        {
			//Start thread for checking data
			_logging.LogDebug("Starting check of states");
			var thread = new Thread(() => UpdateDevices());
			thread.Start();
        }


        public void Handle(IRequest request)
        {
	        if (request is TimerIntervalUpdatedRequest)
	        {
		        RestartTimer();
	        }

	        if (request is ApiUpdatedRequest && !string.IsNullOrEmpty(_iniSettings.ApiKey))
	        {
				//Handle change of api key?? Do we need to do anything?

	        }

	        if (request is HsDevicesUpdatedEvent)
	        {
		        _hsDeviceUtils.UpdateCachedDevices();
	        }

	        if (request is UpdateDoneEvent)
	        {
		        _iniSettings.ReloadIni();
		        _updateDone = true;
	        }
        }

        private void RestartTimer()
        {
	        if (UpdateTimer == null)
	        {
		        CreateTimerUpdate();
	        }
	        else
	        {
		        UpdateTimer.Change((long)DateTime.Now.TimeOfDay.TotalMilliseconds,_iniSettings.TimerIntervalInSeconds* 1000);
	        }
        }

		//For mediator
        public string Name => this.GetType().FullName;
    }
}
