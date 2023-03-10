using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using HSPI_SensiboClimate.Plugin.API.JSON;
using HSPI_SensiboClimate.Plugin.Helpers;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using ParameterType = RestSharp.ParameterType;

namespace HSPI_SensiboClimate.Plugin.API
{
	public interface ISensiboRestHandler
	{
		IRestResponse SendAcSingleStateAsync(string deviceId, string stateName,

			AcSingleStateUpdate state, string apiKey = "TOKEN IS EMBEDDED");
		IRestResponse GetAllDevices(string fields = "*", string apiKey = "TOKEN IS EMBEDDED");
		IRestResponse GetScheduledState(string deviceId, string apiKey = "TOKEN IS EMBEDDED");
		IRestResponse GetSmartModeState(string deviceId, string apiKey = "TOKEN IS EMBEDDED");

		IRestResponse SendACState(string deviceId,
			ACStateUpdate state, string apiKey = "TOKEN IS EMBEDDED");

	}

	public class SensiboRestHandler : ISensiboRestHandler
	{
		private readonly IIniSettings _iniSettings;
		private readonly ILogging _log;
		private RestClient _restClient;
		private const string SensiboApiCloudUrl = "https://home.sensibo.com";

		public SensiboRestHandler(IIniSettings iniSettings, ILogging log)
		{
			_iniSettings = iniSettings;
			_log = log;
		}

		public IRestResponse SendAcSingleStateAsync(string deviceId, string stateName, AcSingleStateUpdate state, string apiKey)
		{
			apiKey = GetApiKeyFromIni();
			if (string.IsNullOrEmpty(apiKey))
				return null;

			_restClient = new RestClient(SensiboApiCloudUrl);
			_restClient.UseNewtonsoftJson();
			var request = new RestRequest("/api/v2/pods/{deviceId}/acStates/{stateName}", Method.PATCH);
			request
				.AddUrlSegment("deviceId", deviceId)
				.AddUrlSegment("stateName", stateName)
				.AddParameter("apiKey", apiKey, ParameterType.QueryString)
				.AddJsonBody(state)
				;
			IRestResponse response = null;


			try
			{
				_log.LogDebug($"SensiboAPI-SendACState: Trying to send ac data for device {deviceId} {stateName} {state.NewValue}");
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
				//response= _restClient.Execute<StatusRequestModel>(request);
				response = _restClient.Execute(request);
				//StatusRequestModel status = SendAcSingleStateAsyncWithRestSharp(deviceId, stateName, state, apiKey);
				//_log.LogDebug($"Response: Status={status.Status},smartMode={status.Result.AcState.ToString()}");
				if (response.IsSuccessful)
					return response;
				return null;
			}
			catch (Exception exception)
			{
				_log.Log(exception.Message, Enums.LogType.Error);
				_log.Log(exception.StackTrace, Enums.LogType.Error);
				var failedStatus = new StatusRequestModel()
				{
					Status = "Failed",
					Result = null
				};

				return null;//failedStatus;
			}
			finally
			{
				if (_log.IsLoggingDebug)
					LogRequest(request, response, 0);
			}
		}

		private string GetApiKeyFromIni()
		{
			var apiKey = _iniSettings.ApiKey;
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				_log.Log("Enter a API-Key", Enums.LogType.Warning);
				return null;
			}
			return apiKey;
		}

		public IRestResponse GetAllDevices(string fields = "*", string apiKey = "")
		{

			apiKey = GetApiKeyFromIni();
			if (string.IsNullOrEmpty(apiKey))
				return null;

			_restClient = new RestClient(SensiboApiCloudUrl);
			_restClient.UseNewtonsoftJson();
			var request = new RestRequest("/api/v2/users/me/pods", Method.GET);
			request
				.AddParameter("fields", "*", ParameterType.QueryString)
				.AddParameter("apiKey", apiKey, ParameterType.QueryString);
			IRestResponse response = null;



			try
			{
				_log.LogDebug($"SensiboAPI-GetAllDevices: Trying to fetch data for device. Fields: {fields}");
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

				//var devicesInfo = await sensiboAPI.GetAllDevices(fields, apiKey);
				response = _restClient.Execute(request);
				_log.LogDebug($"Response: Content={response.Content}, statuscode={response.StatusCode}, statusDescription= {response.StatusDescription}");
				//return devicesInfo;
			}
			catch (Exception exception)
			{
				_log.LogError(exception.Message);
				_log.LogError(exception.StackTrace);

			}

			finally
			{
				if (_log.IsLoggingDebug)
					LogRequest(request, response, 0);
			}

			return response;
		}

		public IRestResponse GetScheduledState(string deviceId, string apiKey = "TOKEN IS EMBEDDED")
		{
			apiKey = GetApiKeyFromIni();
			if (string.IsNullOrEmpty(apiKey))
				return null;

			_restClient = new RestClient(SensiboApiCloudUrl);
			_restClient.UseNewtonsoftJson();
			var request = new RestRequest("/api/v1/pods/{deviceId}/schedules", Method.GET);
			request
				.AddUrlSegment("deviceId", deviceId)
				.AddParameter("apiKey", apiKey, ParameterType.QueryString);
			IRestResponse response = null;

			try
			{
				_log.LogDebug($"SensiboAPI-GetScheduleState: Trying to fetch data for device {deviceId}");
				//var scheduledStateInfo = await sensiboAPI.GetScheduledState(deviceId, apiKey);
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

				response = _restClient.Execute(request);
				//_log.LogDebug(
				//	$"Response: Status={scheduledStateInfo.Status},numberOfSchedules={(scheduledStateInfo.Result?.Count ?? 0)}");

				//return scheduledStateInfo;
			}
			catch (Exception exception)
			{
				_log.Log(exception.Message, Enums.LogType.Error);
				_log.Log(exception.StackTrace, Enums.LogType.Error);

			}

			finally
			{
				if (_log.IsLoggingDebug)
					LogRequest(request, response, 0);
			}

			return response;
		}

		public IRestResponse GetSmartModeState(string deviceId, string apiKey = "TOKEN IS EMBEDDED")
		{
			apiKey = GetApiKeyFromIni();
			if (string.IsNullOrEmpty(apiKey))
				return null;

			_restClient = new RestClient(SensiboApiCloudUrl);
			_restClient.UseNewtonsoftJson();
			var request = new RestRequest("/api/v2/pods/{deviceId}/smartmode", Method.GET);
			request
				.AddUrlSegment("deviceId", deviceId)
				.AddParameter("apiKey", apiKey, ParameterType.QueryString);
			IRestResponse response = null;

			try
			{
				_log.LogDebug($"SensiboAPI-GetSmartModeState: Trying to fetch data for device {deviceId}");
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

				response = _restClient.Execute(request);
				//_log.LogDebug($"Response: Status={smartStateInfo.Status},smartMode={smartStateInfo.Result.On}");

				//return smartStateInfo;
			}
			catch (Exception exception)
			{
				_log.Log(exception.Message, Enums.LogType.Error);
				_log.Log(exception.StackTrace, Enums.LogType.Error);

			}

			finally
			{
				if (_log.IsLoggingDebug)
					LogRequest(request, response, 0);
			}

			return response;
		}

		public IRestResponse SendACState(string deviceId, ACStateUpdate state, string apiKey)
		{
			apiKey = GetApiKeyFromIni();
			if (string.IsNullOrEmpty(apiKey))
				return null;

			_restClient = new RestClient(SensiboApiCloudUrl);
			_restClient.UseNewtonsoftJson();
			var request = new RestRequest("/api/v2/pods/{deviceId}/acStates", Method.POST);
			request
				.AddUrlSegment("deviceId", deviceId)
				.AddParameter("apiKey", apiKey, ParameterType.QueryString)
				.AddJsonBody(state);

			IRestResponse response = null;

			try
			{
				_log.LogDebug($"SensiboAPI-SendACState: Trying to send ac data for device {deviceId} {state.ToString()}");
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

				response = _restClient.Execute(request);
				//_log.LogDebug($"Response: Status={status.Status},smartMode={status.Result.AcState.ToString()}");
				//return status;
			}
			catch (Exception exception)
			{
				_log.Log(exception.Message, Enums.LogType.Error);
				_log.Log(exception.StackTrace, Enums.LogType.Error);



			}

			finally
			{
				if (_log.IsLoggingDebug)
					LogRequest(request, response, 0);
			}

			return response;
		}


		private void LogRequest(IRestRequest request, IRestResponse response, long durationMs)
		{
			//Removed this return; to view requests and responsen
			if (!_log.IsLoggingToFile)
				return;

			var requestToLog = new
			{
				resource = request.Resource,
				// Parameters are custom anonymous objects in order to have the parameter type as a nice string
				// otherwise it will just show the enum value
				parameters = request.Parameters.Select(parameter => new
				{
					name = parameter.Name,
					value = parameter.Value,
					type = parameter.Type.ToString()
				}),
				// ToString() here to have the method as a nice string otherwise it will just show the enum value
				method = request.Method.ToString(),
				// This will generate the actual Uri used in the request
				uri = _restClient.BuildUri(request),
			};

			var responseToLog = new
			{
				statusCode = response.StatusCode,
				content = response.Content,
				headers = response.Headers,
				// The Uri that actually responded (could be different from the requestUri if a redirection occurred)
				responseUri = response.ResponseUri,
				errorMessage = response.ErrorMessage,
			};

			_log.LogDebug($"Request completed in {durationMs} ms{Environment.NewLine}" +
			              $"Request: {JsonConvert.SerializeObject(requestToLog)}" +
			              $"{Environment.NewLine} Response: {JsonConvert.SerializeObject(responseToLog)}");
		}
	}
}