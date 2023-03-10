using HSPI_SensiboClimate.Plugin.API;
using HSPI_SensiboClimate.Plugin.API.JSON;
using HSPI_SensiboClimate.Plugin.Helpers;
using NSubstitute;
using NUnit.Framework;
using Should;

namespace SensiboPluginClimate.IntegrationTests
{
	[TestFixture]
	public class SensiboRestHandlerTests
	{
		private IIniSettings _iniSettings;
		private ILogging _logging;
		[SetUp]
		public void SetUp()
		{
			_iniSettings = Substitute.For<IIniSettings>();
			_logging = Substitute.For<ILogging>();
		}

		[Test,Explicit]
		public void SendSingleState_SendChangeOfOn_ShouldChangeOn()
		{
			
			var sut=new SensiboRestHandler(_iniSettings,_logging);
			var res = sut.SendAcSingleStateAsync("7YNcgpbJ", "on", new AcSingleStateUpdate() {NewValue = false},
				"M93Rtn72nnCVjsRKpi9HjqzCkkYwZC");
		}

		[Test, Explicit]
		public void GetAllDevices_GetInfo_ShouldGetInfo()
		{
			var sut = new SensiboRestHandler(_iniSettings, _logging);

			var response = sut.GetAllDevices("*", "M93Rtn72nnCVjsRKpi9HjqzCkkYwZC");

			response.ShouldNotBeNull();
			response.Content.ShouldNotBeEmpty();
		}

		[Test, Explicit]
		public void SendACState_SendChangeOfOn_ShouldChangeOn()
		{
			var sut = new SensiboRestHandler(_iniSettings, _logging);

			var response = sut.SendACState ("7YNcgpbJ", new ACStateUpdate()
				{
					ACState = new ACState()
					{
						On=true,
						FanLevel = "medium",
						TemperatureUnit = "C",
						Swing = "stopped",
						TargetTemperature = 19,
						Mode = "heat"
					}
				}, 
				"M93Rtn72nnCVjsRKpi9HjqzCkkYwZC");

			response.ShouldNotBeNull();
			response.Content.ShouldNotBeEmpty();
		}


		[Test, Explicit]
		public void GetScheduledState_ShouldGetScheduledState()
		{
			var sut = new SensiboRestHandler(_iniSettings, _logging);

			var response = sut.GetScheduledState("7YNcgpbJ","M93Rtn72nnCVjsRKpi9HjqzCkkYwZC");

			response.ShouldNotBeNull();
			response.Content.ShouldNotBeEmpty();
		}

		[Test, Explicit]
		public void GetSmartModeState_ShouldGetScheduledState()
		{
			var sut = new SensiboRestHandler(_iniSettings, _logging);

			var response = sut.GetSmartModeState("7YNcgpbJ", "M93Rtn72nnCVjsRKpi9HjqzCkkYwZC");

			response.ShouldNotBeNull();
			response.Content.ShouldNotBeEmpty();
		}

	}
}