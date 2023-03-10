using HomeSeerAPI;
using Hspi;
using System.Collections.Generic;
using HSPI_SensiboClimate.Plugin.Helpers;

namespace HSPI_SensiboClimate
{
    public class HSPI : HspiBase2
    {
        public static IHSApplication HSApp { get; private set; }
        public static IAppCallbackAPI CallbackApp { get; private set; }
        public static string PluginName { get => PluginConstants.PluginName; }

        private SensiboPlugin Plugin { get; set; }

        protected override bool GetHscomPort()
        {
            return false;
        }

        public override string InitIO(string port)
        {
			HSApp = HS;
			CallbackApp = Callback;

			Plugin = new SensiboPlugin(HS,CallbackApp);
			Plugin.InitIo();

			return base.InitIO(port);
        }

        public override string PostBackProc(string page, string data, string user, int userRights)
        {
            return Plugin.PostBackProc(page, data, user, userRights);
        }

        public override async void SetIOMulti(List<CAPI.CAPIControl> colSend)
        {
            await Plugin.SetIoMultiAsync(colSend);
        }

        protected override string GetName()
        {
            return PluginName;
        }

        public override string GetPagePlugin(string page, string user, int userRights, string queryString)
        {
            return Plugin.GetPagePlugin(page, user, userRights, queryString);
        }

        public override void ShutdownIO()
        {
	        //// shut everything down here
	        //// debug
	        //HS.WriteLog(DaikinSeerConstants.PluginName, "Entering ShutdownIO");

	        //// shut everything down here
	        //_daikinSeerPlugin.ShutdownIO();
	        //_iniSettings.Dispose();
	        //_log.Dispose();
	        //_daikinSeerPlugin.Dispose();

	        //// let our console wrapper know we are finished
	        //Shutdown = true;

	        //// debug
	        //HS.WriteLog(DaikinSeerConstants.PluginName, "Completed ShutdownIO");
        }

	}
}