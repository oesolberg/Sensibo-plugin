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


	public class SensiboFakeRestHandler:ISensiboRestHandler
	{
		private readonly IIniSettings _iniSettings;
		private readonly ILogging _log;
		//private RestClient _restClient;
		//private const string SensiboApiCloudUrl = "https://home.sensibo.com";

		public SensiboFakeRestHandler(IIniSettings iniSettings, ILogging log)
		{
			_iniSettings = iniSettings;
			_log = log;
		}

		public IRestResponse SendAcSingleStateAsync(string deviceId, string stateName,  AcSingleStateUpdate state, string apiKey)
		{
			return null;
		}

		public IRestResponse GetAllDevices(string fields="*", string apiKey="")
		{
			var response=new RestResponse();
			response.StatusCode = HttpStatusCode.OK;
			response.ResponseStatus=ResponseStatus.Completed;

			response.Content = _jsonGetAllRuneR;//_jsonGetAllDevicesMk1;
			

			return response;
		}

		public IRestResponse GetScheduledState(string deviceId, string apiKey = "TOKEN IS EMBEDDED")
		{
			var response=new RestResponse(){StatusCode = HttpStatusCode.OK,
				ResponseStatus = ResponseStatus.Completed,
			Content = "{\"status\": \"success\",\"result\": []}"};
			

			return response;
		}

		public  IRestResponse GetSmartModeState(string deviceId, string apiKey = "TOKEN IS EMBEDDED")
		{
			var response = new RestResponse()
			{
				StatusCode = HttpStatusCode.OK,
				ResponseStatus = ResponseStatus.Completed,
				Content = "{\"status\": \"success\",\"result\": {}}"
			};

			return response;
		}

		public IRestResponse SendACState(string deviceId, ACStateUpdate state, string apiKey)
		{
			
			return null;
		}


		

		private string _jsonSendACStateResponse = "{" +
												  "    \"status\": \"success\"," +
												  "    \"result\": {" +
												  "        \"status\": \"Success\"," +
												  "        \"reason\": \"UserRequest\"," +
												  "        \"acState\": {" +
												  "            \"on\": true," +
												  "            \"fanLevel\": \"medium\"," +
												  "            \"timestamp\": {" +
												  "                \"secondsAgo\": 0," +
												  "                \"time\": \"2021-02-06T12:42:41.811182Z\"" +
												  "            }," +
												  "            \"temperatureUnit\": \"C\"," +
												  "            \"targetTemperature\": 19," +
												  "            \"mode\": \"heat\"," +
												  "            \"swing\": \"stopped\"" +
												  "        }," +
												  "        \"changedProperties\": [" +
												  "            \"mode\"," +
												  "            \"targetTemperature\"," +
												  "            \"swing\"" +
												  "        ]," +
												  "        \"id\": \"uJMeTf6Hj8\"," +
												  "        \"failureReason\": null" +
												  "    }" +
												  "}";
		private string _jsonGetAllDevicesMk1= "{" +
"    \"status\": \"success\"," +
"    \"result\": [" +
"        {" +
"            \"configGroup\": \"stable\"," +
"            \"macAddress\": \"84:f3:eb:ae:b1:36\"," +
"            \"measurements\": {" +
"                \"batteryVoltage\": null," +
"                \"temperature\": 21.2," +
"                \"humidity\": 33.8," +
"                \"time\": {" +
"                    \"secondsAgo\": 30," +
"                    \"time\": \"2021-02-20T20:55:40.928758Z\"" +
"                }," +
"                \"rssi\": -53," +
"                \"piezo\": [" +
"                    null," +
"                    null" +
"                ]," +
"                \"pm25\": 0" +
"            }," +
"            \"features\": [" +
"                \"filters\"," +
"                \"softShowPlus\"" +
"            ]," +
"            \"currentlyAvailableFirmwareVersion\": \"SKY30044\"," +
"            \"cleanFiltersNotificationEnabled\": false," +
"            \"connectionStatus\": {" +
"                \"isAlive\": true," +
"                \"lastSeen\": {" +
"                    \"secondsAgo\": 30," +
"                    \"time\": \"2021-02-20T20:55:40.928758Z\"" +
"                }" +
"            }," +
"            \"filtersCleaning\": {" +
"                \"acOnSecondsSinceLastFiltersClean\": 70664," +
"                \"shouldCleanFilters\": false," +
"                \"filtersCleanSecondsThreshold\": 1080000," +
"                \"lastFiltersCleanTime\": {" +
"                    \"secondsAgo\": 100660," +
"                    \"time\": \"2021-02-19T16:58:31Z\"" +
"                }" +
"            }," +
"            \"acState\": {" +
"                \"on\": true," +
"                \"fanLevel\": \"medium_high\"," +
"                \"timestamp\": {" +
"                    \"secondsAgo\": 0," +
"                    \"time\": \"2021-02-20T20:56:11.852884Z\"" +
"                }," +
"                \"temperatureUnit\": \"C\"," +
"                \"horizontalSwing\": \"fixedCenterLeft\"," +
"                \"targetTemperature\": 22," +
"                \"mode\": \"heat\"," +
"                \"swing\": \"fixedTop\"" +
"            }," +
"            \"isOwner\": true," +
"            \"mainMeasurementsSensor\": null," +
"            \"motionSensors\": []," +
"            \"runningHealthcheck\": null," +
"            \"firmwareType\": \"esp8266ex\"," +
"            \"id\": \"MHHmhPcs\"," +
"            \"firmwareVersion\": \"SKY30044\"," +
"            \"roomIsOccupied\": null," +
"            \"warrantyEligible\": \"no\"," +
"            \"motionConfig\": null," +
"            \"schedules\": []," +
"            \"isGeofenceOnExitEnabled\": false," +
"            \"isClimateReactGeofenceOnEnterEnabledForThisUser\": false," +
"            \"homekitSupported\": false," +
"            \"pureBoostConfig\": null," +
"            \"smartMode\": {" +
"                \"deviceUid\": \"MHHmhPcs\"," +
"                \"highTemperatureWebhook\": null," +
"                \"highTemperatureThreshold\": 26.0," +
"                \"lowTemperatureWebhook\": null," +
"                \"type\": \"temperature\"," +
"                \"lowTemperatureState\": {" +
"                    \"on\": true," +
"                    \"fanLevel\": \"auto\"," +
"                    \"temperatureUnit\": \"C\"," +
"                    \"horizontalSwing\": \"stopped\"," +
"                    \"targetTemperature\": 24," +
"                    \"mode\": \"heat\"," +
"                    \"swing\": \"rangeFull\"" +
"                }," +
"                \"enabled\": false," +
"                \"highTemperatureState\": {" +
"                    \"on\": true," +
"                    \"fanLevel\": \"auto\"," +
"                    \"temperatureUnit\": \"C\"," +
"                    \"horizontalSwing\": \"stopped\"," +
"                    \"targetTemperature\": 24," +
"                    \"mode\": \"cool\"," +
"                    \"swing\": \"rangeFull\"" +
"                }," +
"                \"lowTemperatureThreshold\": 23.0" +
"            }," +
"            \"shouldShowFilterCleaningNotification\": false," +
"            \"location\": {" +
"                \"latLon\": [" +
"                    59.9437462," +
"                    11.0091908" +
"                ]," +
"                \"updateTime\": null," +
"                \"features\": []," +
"                \"country\": \"Norge\"," +
"                \"occupancy\": \"n/a\"," +
"                \"createTime\": {" +
"                    \"secondsAgo\": 68715605," +
"                    \"time\": \"2018-12-18T13:16:06Z\"" +
"                }," +
"                \"address\": [" +
"                    \"N\"," +
"                    \"y\"," +
"                    \"g\"" +
"                ]," +
"                \"geofenceTriggerRadius\": 200," +
"                \"subscription\": null," +
"                \"id\": \"qrHpRPzLS2\"," +
"                \"name\": \"Home\"" +
"            }," +
"            \"sensorsCalibration\": {" +
"                \"temperature\": 0.0," +
"                \"humidity\": 0.0" +
"            }," +
"            \"tags\": []," +
"            \"productModel\": \"skyv2\"," +
"            \"isMotionGeofenceOnEnterEnabled\": false," +
"            \"isGeofenceOnEnterEnabledForThisUser\": false," +
"            \"isClimateReactGeofenceOnExitEnabled\": false," +
"            \"remoteCapabilities\": {" +
"                \"modes\": {" +
"                    \"dry\": {" +
"                        \"horizontalSwing\": [" +
"                            \"fixedCenter\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedLeft\"," +
"                            \"fixedRight\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30," +
"                                    31" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86," +
"                                    88" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }," +
"                    \"auto\": {" +
"                        \"horizontalSwing\": [" +
"                            \"fixedCenter\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedLeft\"," +
"                            \"fixedRight\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30," +
"                                    31" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86," +
"                                    88" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }," +
"                    \"heat\": {" +
"                        \"horizontalSwing\": [" +
"                            \"fixedCenter\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedLeft\"," +
"                            \"fixedRight\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    10," +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30," +
"                                    31" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    50," +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86," +
"                                    88" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }," +
"                    \"fan\": {" +
"                        \"horizontalSwing\": [" +
"                            \"fixedCenter\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedLeft\"," +
"                            \"fixedRight\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {}," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }," +
"                    \"cool\": {" +
"                        \"horizontalSwing\": [" +
"                            \"fixedCenter\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedLeft\"," +
"                            \"fixedRight\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30," +
"                                    31" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86," +
"                                    88" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }" +
"                }" +
"            }," +
"            \"serial\": \"411800274\"," +
"            \"accessPoint\": {" +
"                \"password\": null," +
"                \"ssid\": \"SENSIBO-I-31702\"" +
"            }," +
"            \"remote\": {" +
"                \"window\": false," +
"                \"toggle\": false" +
"            }," +
"            \"room\": {" +
"                \"icon\": \"lounge\"," +
"                \"uid\": \"XFe2wSJs\"," +
"                \"name\": \"Stue\"" +
"            }," +
"            \"qrId\": \"TPJEJLXFTQ\"," +
"            \"temperatureUnit\": \"C\"," +
"            \"timer\": null," +
"            \"remoteFlavor\": \"Enthusiastic Triceratops\"," +
"            \"isMotionGeofenceOnExitEnabled\": false," +
"            \"remoteAlternatives\": [" +
"                \"_mitsubishi1fa\"," +
"                \"_mitsubishi1_for_ben_ho\"," +
"                \"_mitsubishi1_plasma_on\"," +
"                \"_mitsubishi1_plasma_on_clean_on\"," +
"                \"_mitsubishi1b\"," +
"                \"mitsubishi1f\"," +
"                \"_mitsubishi1_isee_and_absence_detection\"," +
"                \"_mitsubishi1f\"" +
"            ]" +
"        }," +
"        {" +
"            \"configGroup\": \"stable\"," +
"            \"macAddress\": \"ec:fa:bc:1d:35:95\"," +
"            \"measurements\": {" +
"                \"batteryVoltage\": null," +
"                \"temperature\": 21.7," +
"                \"humidity\": 26.7," +
"                \"time\": {" +
"                    \"secondsAgo\": 79," +
"                    \"time\": \"2021-02-20T20:54:51.752448Z\"" +
"                }," +
"                \"rssi\": -70," +
"                \"piezo\": [" +
"                    null," +
"                    null" +
"                ]," +
"                \"pm25\": 0" +
"            }," +
"            \"features\": [" +
"                \"filters\"," +
"                \"softShowPlus\"" +
"            ]," +
"            \"currentlyAvailableFirmwareVersion\": \"SKY30044\"," +
"            \"cleanFiltersNotificationEnabled\": true," +
"            \"connectionStatus\": {" +
"                \"isAlive\": true," +
"                \"lastSeen\": {" +
"                    \"secondsAgo\": 79," +
"                    \"time\": \"2021-02-20T20:54:51.752448Z\"" +
"                }" +
"            }," +
"            \"filtersCleaning\": {" +
"                \"acOnSecondsSinceLastFiltersClean\": 70912," +
"                \"shouldCleanFilters\": false," +
"                \"filtersCleanSecondsThreshold\": 1080000," +
"                \"lastFiltersCleanTime\": {" +
"                    \"secondsAgo\": 100734," +
"                    \"time\": \"2021-02-19T16:57:17Z\"" +
"                }" +
"            }," +
"            \"acState\": {" +
"                \"on\": true," +
"                \"fanLevel\": \"auto\"," +
"                \"timestamp\": {" +
"                    \"secondsAgo\": 0," +
"                    \"time\": \"2021-02-20T20:56:11.861642Z\"" +
"                }," +
"                \"temperatureUnit\": \"C\"," +
"                \"targetTemperature\": 22," +
"                \"mode\": \"heat\"," +
"                \"swing\": \"stopped\"" +
"            }," +
"            \"isOwner\": true," +
"            \"mainMeasurementsSensor\": null," +
"            \"motionSensors\": []," +
"            \"runningHealthcheck\": null," +
"            \"firmwareType\": \"esp8266ex\"," +
"            \"id\": \"B9Upa5J5\"," +
"            \"firmwareVersion\": \"SKY30044\"," +
"            \"roomIsOccupied\": null," +
"            \"warrantyEligible\": \"no\"," +
"            \"motionConfig\": null," +
"            \"schedules\": []," +
"            \"isGeofenceOnExitEnabled\": false," +
"            \"isClimateReactGeofenceOnEnterEnabledForThisUser\": false," +
"            \"homekitSupported\": false," +
"            \"pureBoostConfig\": null," +
"            \"smartMode\": null," +
"            \"shouldShowFilterCleaningNotification\": false," +
"            \"location\": {" +
"                \"latLon\": [" +
"                    59.9432019," +
"                    11.0099192" +
"                ]," +
"                \"updateTime\": null," +
"                \"features\": []," +
"                \"country\": \"Norge\"," +
"                \"occupancy\": \"n/a\"," +
"                \"createTime\": {" +
"                    \"secondsAgo\": 24481869," +
"                    \"time\": \"2020-05-13T12:25:02Z\"" +
"                }," +
"                \"address\": [" +
"                    \"G\"," +
"                    \"u\"," +
"                    \"l\"" +
"                ]," +
"                \"geofenceTriggerRadius\": 200," +
"                \"subscription\": null," +
"                \"id\": \"DGWngJmjto\"," +
"                \"name\": \"Karisveien\"" +
"            }," +
"            \"sensorsCalibration\": {" +
"                \"temperature\": 0.0," +
"                \"humidity\": 0.0" +
"            }," +
"            \"tags\": []," +
"            \"productModel\": \"skyv2\"," +
"            \"isMotionGeofenceOnEnterEnabled\": false," +
"            \"isGeofenceOnEnterEnabledForThisUser\": false," +
"            \"isClimateReactGeofenceOnExitEnabled\": false," +
"            \"remoteCapabilities\": {" +
"                \"modes\": {" +
"                    \"dry\": {" +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"both\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"low\"" +
"                        ]" +
"                    }," +
"                    \"auto\": {" +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"both\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"high\"," +
"                            \"strong\"," +
"                            \"auto\"" +
"                        ]" +
"                    }," +
"                    \"heat\": {" +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"both\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"high\"," +
"                            \"strong\"," +
"                            \"auto\"" +
"                        ]" +
"                    }," +
"                    \"fan\": {" +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"both\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"high\"," +
"                            \"strong\"," +
"                            \"auto\"" +
"                        ]" +
"                    }," +
"                    \"cool\": {" +
"                        \"swing\": [" +
"                            \"stopped\"," +
"                            \"both\"," +
"                            \"rangeFull\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium\"," +
"                            \"high\"," +
"                            \"strong\"," +
"                            \"auto\"" +
"                        ]" +
"                    }" +
"                }" +
"            }," +
"            \"serial\": \"221809474\"," +
"            \"accessPoint\": {" +
"                \"password\": null," +
"                \"ssid\": \"SENSIBO-I-07905\"" +
"            }," +
"            \"remote\": {" +
"                \"window\": false," +
"                \"toggle\": false" +
"            }," +
"            \"room\": {" +
"                \"icon\": \"Livingroom\"," +
"                \"uid\": \"rx97VmGT\"," +
"                \"name\": \"Stue\"" +
"            }," +
"            \"qrId\": \"NHGCKSELWF\"," +
"            \"temperatureUnit\": \"C\"," +
"            \"timer\": null," +
"            \"remoteFlavor\": \"Panoramic Toucan\"," +
"            \"isMotionGeofenceOnExitEnabled\": false," +
"            \"remoteAlternatives\": [" +
"                \"_mitsubishi13c\"," +
"                \"_mitsubishi13b\"" +
"            ]" +
"        }" +
"    ]" +
"}";
		private string _jsonGetAllDevicesResponse = "{" +
							  "    \"status\": \"success\"," +
							  "    \"result\": [" +
							  "        {" +
							  "            \"configGroup\": \"stable\"," +
							  "            \"macAddress\": \"34:15:13:f7:d3:a8\"," +
							  "            \"isGeofenceOnExitEnabled\": false," +
							  "            \"features\": [" +
							  "                \"showPlus\"," +
							  "                \"softShowPlus\"" +
							  "            ]," +
							  "            \"currentlyAvailableFirmwareVersion\": \"IN010056\"," +
							  "            \"cleanFiltersNotificationEnabled\": false," +
							  "            \"connectionStatus\": {" +
							  "                \"isAlive\": true," +
							  "                \"lastSeen\": {" +
							  "                    \"secondsAgo\": 34," +
							  "                    \"time\": \"2021-01-28T05:30:07.127380Z\"" +
							  "                }" +
							  "            }," +
							  "            \"filtersCleaning\": null," +
							  "            \"acState\": {" +
							  "                \"on\": true," +
							  "                \"fanLevel\": \"auto\"," +
							  "                \"timestamp\": {" +
							  "                    \"secondsAgo\": -1," +
							  "                    \"time\": \"2021-01-28T05:30:43.075389Z\"" +
							  "                }," +
							  "                \"temperatureUnit\": \"C\"," +
							  "                \"horizontalSwing\": \"stopped\"," +
							  "                \"targetTemperature\": 23," +
							  "                \"mode\": \"heat\"," +
							  "                \"swing\": \"fixedMiddleTop\"" +
							  "            }," +
							  "            \"isOwner\": true," +
							  "            \"mainMeasurementsSensor\": null," +
							  "            \"motionSensors\": []," +
							  "            \"id\": \"5AZMjeLN\"," +
							  "            \"firmwareVersion\": \"IN010056\"," +
							  "            \"roomIsOccupied\": null," +
							  "            \"warrantyEligible\": \"no\"," +
							  "            \"motionConfig\": null," +
							  "            \"measurements\": {" +
							  "                \"batteryVoltage\": null," +
							  "                \"temperature\": 20.5," +
							  "                \"humidity\": 49.4," +
							  "                \"time\": {" +
							  "                    \"secondsAgo\": 34," +
							  "                    \"time\": \"2021-01-28T05:30:07.127380Z\"" +
							  "                }," +
							  "                \"rssi\": -74," +
							  "                \"piezo\": [" +
							  "                    null," +
							  "                    null" +
							  "                ]," +
							  "                \"pm25\": 0" +
							  "            }," +
							  "            \"isClimateReactGeofenceOnEnterEnabledForThisUser\": false," +
							  "            \"homekitSupported\": false," +
							  "            \"pureBoostConfig\": null," +
							  "            \"smartMode\": null," +
							  "            \"shouldShowFilterCleaningNotification\": false," +
							  "            \"location\": {" +
							  "                \"latLon\": [" +
							  "                    58.9480419," +
							  "                    5.6744805" +
							  "                ]," +
							  "                \"updateTime\": null," +
							  "                \"features\": [" +
							  "                    \"showPlus\"," +
							  "                    \"softShowPlus\"" +
							  "                ]," +
							  "                \"country\": \"Norge\"," +
							  "                \"occupancy\": \"n/a\"," +
							  "                \"createTime\": {" +
							  "                    \"secondsAgo\": 67335563," +
							  "                    \"time\": \"2018-12-10T21:11:19Z\"" +
							  "                }," +
							  "                \"address\": [" +
							  "                    \"Gunhilds gate 22A\"," +
							  "                    \"Rogaland\"" +
							  "                ]," +
							  "                \"geofenceTriggerRadius\": 200," +
							  "                \"subscription\": null," +
							  "                \"id\": \"d3pnPcNNM6\"," +
							  "                \"name\": \"GG22A\"" +
							  "            }," +
							  "            \"sensorsCalibration\": {" +
							  "                \"temperature\": 0.0," +
							  "                \"humidity\": 0.0" +
							  "            }," +
							  "            \"tags\": []," +
							  "            \"productModel\": \"skyv2\"," +
							  "            \"isMotionGeofenceOnEnterEnabled\": false," +
							  "            \"lastHealthcheck\": null," +
							  "            \"temperatureUnit\": \"C\"," +
							  "            \"schedules\": [" +
							  "                {" +
							  "                    \"nextTime\": \"2021-02-02T12:00:00\"," +
							  "                    \"podUid\": \"5AZMjeLN\"," +
							  "                    \"recurringDays\": [" +
							  "                        \"Tuesday\"" +
							  "                    ]," +
							  "                    \"createTimeSecondsAgo\": 15886976," +
							  "                    \"isEnabled\": true," +
							  "                    \"createTime\": \"2020-07-28T08:27:47\"," +
							  "                    \"acState\": {" +
							  "                        \"on\": true," +
							  "                        \"fanLevel\": \"auto\"," +
							  "                        \"temperatureUnit\": \"C\"," +
							  "                        \"targetTemperature\": 22," +
							  "                        \"mode\": \"heat\"," +
							  "                        \"swing\": \"fixedMiddleTop\"" +
							  "                    }," +
							  "                    \"targetTimeLocal\": \"13:00\"," +
							  "                    \"timezone\": \"Europe/Oslo\"," +
							  "                    \"nextTimeSecondsFromNow\": 455356," +
							  "                    \"causedBy\": {" +
							  "                        \"username\": \"frankborlaug\"," +
							  "                        \"firstName\": \"Frank Ivar\"," +
							  "                        \"lastName\": \"Borlaug\"," +
							  "                        \"email\": \"frank.borlaug@gmail.com\"" +
							  "                    }," +
							  "                    \"id\": \"B7DdQQzCiv\"" +
							  "                }," +
							  "                {" +
							  "                    \"nextTime\": \"2021-02-02T09:30:00\"," +
							  "                    \"podUid\": \"5AZMjeLN\"," +
							  "                    \"recurringDays\": [" +
							  "                        \"Tuesday\"" +
							  "                    ]," +
							  "                    \"createTimeSecondsAgo\": 15886976," +
							  "                    \"isEnabled\": true," +
							  "                    \"createTime\": \"2020-07-28T08:27:47\"," +
							  "                    \"acState\": {" +
							  "                        \"on\": true," +
							  "                        \"fanLevel\": \"auto\"," +
							  "                        \"temperatureUnit\": \"C\"," +
							  "                        \"targetTemperature\": 20," +
							  "                        \"mode\": \"heat\"," +
							  "                        \"swing\": \"fixedMiddleTop\"" +
							  "                    }," +
							  "                    \"targetTimeLocal\": \"10:30\"," +
							  "                    \"timezone\": \"Europe/Oslo\"," +
							  "                    \"nextTimeSecondsFromNow\": 446356," +
							  "                    \"causedBy\": {" +
							  "                        \"username\": \"frankborlaug\"," +
							  "                        \"firstName\": \"Frank Ivar\"," +
							  "                        \"lastName\": \"Borlaug\"," +
							  "                        \"email\": \"frank.borlaug@gmail.com\"" +
							  "                    }," +
							  "                    \"id\": \"GQYU65FZ6D\"" +
							  "                }," +
							  "                {" +
							  "                    \"nextTime\": \"2021-02-02T13:00:00\"," +
							  "                    \"podUid\": \"5AZMjeLN\"," +
							  "                    \"recurringDays\": [" +
							  "                        \"Tuesday\"" +
							  "                    ]," +
							  "                    \"createTimeSecondsAgo\": 15886976," +
							  "                    \"isEnabled\": true," +
							  "                    \"createTime\": \"2020-07-28T08:27:47\"," +
							  "                    \"acState\": {" +
							  "                        \"on\": true," +
							  "                        \"fanLevel\": \"auto\"," +
							  "                        \"temperatureUnit\": \"C\"," +
							  "                        \"targetTemperature\": 23," +
							  "                        \"mode\": \"heat\"," +
							  "                        \"swing\": \"fixedMiddleTop\"" +
							  "                    }," +
							  "                    \"targetTimeLocal\": \"14:00\"," +
							  "                    \"timezone\": \"Europe/Oslo\"," +
							  "                    \"nextTimeSecondsFromNow\": 458956," +
							  "                    \"causedBy\": {" +
							  "                        \"username\": \"frankborlaug\"," +
							  "                        \"firstName\": \"Frank Ivar\"," +
							  "                        \"lastName\": \"Borlaug\"," +
							  "                        \"email\": \"frank.borlaug@gmail.com\"" +
							  "                    }," +
							  "                    \"id\": \"LB4anxnc7U\"" +
							  "                }," +
							  "                {" +
							  "                    \"nextTime\": \"2021-02-02T11:00:00\"," +
							  "                    \"podUid\": \"5AZMjeLN\"," +
							  "                    \"recurringDays\": [" +
							  "                        \"Tuesday\"" +
							  "                    ]," +
							  "                    \"createTimeSecondsAgo\": 15886976," +
							  "                    \"isEnabled\": true," +
							  "                    \"createTime\": \"2020-07-28T08:27:47\"," +
							  "                    \"acState\": {" +
							  "                        \"on\": true," +
							  "                        \"fanLevel\": \"auto\"," +
							  "                        \"temperatureUnit\": \"C\"," +
							  "                        \"targetTemperature\": 21," +
							  "                        \"mode\": \"heat\"," +
							  "                        \"swing\": \"fixedMiddleTop\"" +
							  "                    }," +
							  "                    \"targetTimeLocal\": \"12:00\"," +
							  "                    \"timezone\": \"Europe/Oslo\"," +
							  "                    \"nextTimeSecondsFromNow\": 451756," +
							  "                    \"causedBy\": {" +
							  "                        \"username\": \"frankborlaug\"," +
							  "                        \"firstName\": \"Frank Ivar\"," +
							  "                        \"lastName\": \"Borlaug\"," +
							  "                        \"email\": \"frank.borlaug@gmail.com\"" +
							  "                    }," +
							  "                    \"id\": \"niDZYM7zJ3\"" +
							  "                }" +
							  "            ]," +
							  "            \"isClimateReactGeofenceOnExitEnabled\": false," +
							  "            \"remoteCapabilities\": {" +
							  "                \"modes\": {" +
							  "                    \"dry\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30," +
							  "                                    31" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86," +
							  "                                    88" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"auto\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30," +
							  "                                    31" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86," +
							  "                                    88" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"heat\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    10," +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30," +
							  "                                    31" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    50," +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86," +
							  "                                    88" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"fan\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {}," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"cool\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30," +
							  "                                    31" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86," +
							  "                                    88" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }" +
							  "                }" +
							  "            }," +
							  "            \"serial\": \"43170715\"," +
							  "            \"accessPoint\": {" +
							  "                \"password\": null," +
							  "                \"ssid\": \"SENSIBO-I-38895\"" +
							  "            }," +
							  "            \"remote\": {" +
							  "                \"window\": false," +
							  "                \"toggle\": false" +
							  "            }," +
							  "            \"room\": {" +
							  "                \"icon\": \"Lounge\"," +
							  "                \"uid\": \"hVbEFwad\"," +
							  "                \"name\": \"Stue GG22A\"" +
							  "            }," +
							  "            \"qrId\": \"TWNGVJYJVD\"," +
							  "            \"firmwareType\": \"cc3100_stm32f0\"," +
							  "            \"timer\": null," +
							  "            \"remoteFlavor\": \"Enthusiastic Triceratops\"," +
							  "            \"isGeofenceOnEnterEnabledForThisUser\": false," +
							  "            \"isMotionGeofenceOnExitEnabled\": false," +
							  "            \"remoteAlternatives\": [" +
							  "                \"_mitsubishi1fa\"," +
							  "                \"_mitsubishi1_for_ben_ho\"," +
							  "                \"_mitsubishi1_plasma_on\"," +
							  "                \"_mitsubishi1_plasma_on_clean_on\"," +
							  "                \"_mitsubishi1b\"," +
							  "                \"mitsubishi1f\"," +
							  "                \"_mitsubishi1_isee_and_absence_detection\"," +
							  "                \"_mitsubishi1f\"" +
							  "            ]" +
							  "        }," +
							  "        {" +
							  "            \"configGroup\": \"stable\"," +
							  "            \"macAddress\": \"34:15:13:f7:c4:62\"," +
							  "            \"isGeofenceOnExitEnabled\": false," +
							  "            \"features\": [" +
							  "                \"showPlus\"," +
							  "                \"softShowPlus\"" +
							  "            ]," +
							  "            \"currentlyAvailableFirmwareVersion\": \"IN010056\"," +
							  "            \"cleanFiltersNotificationEnabled\": false," +
							  "            \"connectionStatus\": {" +
							  "                \"isAlive\": true," +
							  "                \"lastSeen\": {" +
							  "                    \"secondsAgo\": 76," +
							  "                    \"time\": \"2021-01-28T05:29:25.030924Z\"" +
							  "                }" +
							  "            }," +
							  "            \"filtersCleaning\": null," +
							  "            \"acState\": {" +
							  "                \"on\": true," +
							  "                \"fanLevel\": \"auto\"," +
							  "                \"light\": \"on\"," +
							  "                \"temperatureUnit\": \"C\"," +
							  "                \"horizontalSwing\": \"stopped\"," +
							  "                \"swing\": \"stopped\"," +
							  "                \"targetTemperature\": 25," +
							  "                \"mode\": \"heat\"," +
							  "                \"timestamp\": {" +
							  "                    \"secondsAgo\": -1," +
							  "                    \"time\": \"2021-01-28T05:30:43.197832Z\"" +
							  "                }" +
							  "            }," +
							  "            \"isOwner\": true," +
							  "            \"mainMeasurementsSensor\": null," +
							  "            \"motionSensors\": []," +
							  "            \"id\": \"7YNcgpbJ\"," +
							  "            \"firmwareVersion\": \"IN010056\"," +
							  "            \"roomIsOccupied\": null," +
							  "            \"warrantyEligible\": \"no\"," +
							  "            \"motionConfig\": null," +
							  "            \"measurements\": {" +
							  "                \"batteryVoltage\": null," +
							  "                \"temperature\": 28.7," +
							  "                \"humidity\": 53.8," +
							  "                \"time\": {" +
							  "                    \"secondsAgo\": 76," +
							  "                    \"time\": \"2021-01-28T05:29:25.030924Z\"" +
							  "                }," +
							  "                \"rssi\": -54," +
							  "                \"piezo\": [" +
							  "                    null," +
							  "                    null" +
							  "                ]," +
							  "                \"pm25\": 0" +
							  "            }," +
							  "            \"isClimateReactGeofenceOnEnterEnabledForThisUser\": false," +
							  "            \"homekitSupported\": false," +
							  "            \"pureBoostConfig\": null," +
							  "            \"smartMode\": null," +
							  "            \"shouldShowFilterCleaningNotification\": false," +
							  "            \"location\": {" +
							  "                \"latLon\": [" +
							  "                    58.9480419," +
							  "                    5.6744805" +
							  "                ]," +
							  "                \"updateTime\": null," +
							  "                \"features\": [" +
							  "                    \"showPlus\"," +
							  "                    \"softShowPlus\"" +
							  "                ]," +
							  "                \"country\": \"Norge\"," +
							  "                \"occupancy\": \"n/a\"," +
							  "                \"createTime\": {" +
							  "                    \"secondsAgo\": 67335563," +
							  "                    \"time\": \"2018-12-10T21:11:19Z\"" +
							  "                }," +
							  "                \"address\": [" +
							  "                    \"Gunhilds gate 22A\"," +
							  "                    \"Rogaland\"" +
							  "                ]," +
							  "                \"geofenceTriggerRadius\": 200," +
							  "                \"subscription\": null," +
							  "                \"id\": \"d3pnPcNNM6\"," +
							  "                \"name\": \"GG22A\"" +
							  "            }," +
							  "            \"sensorsCalibration\": {" +
							  "                \"temperature\": 0.0," +
							  "                \"humidity\": 0.0" +
							  "            }," +
							  "            \"tags\": []," +
							  "            \"productModel\": \"skyv2\"," +
							  "            \"isMotionGeofenceOnEnterEnabled\": false," +
							  "            \"lastHealthcheck\": null," +
							  "            \"temperatureUnit\": \"C\"," +
							  "            \"schedules\": []," +
							  "            \"isClimateReactGeofenceOnExitEnabled\": false," +
							  "            \"remoteCapabilities\": {" +
							  "                \"modes\": {" +
							  "                    \"dry\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"auto\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"heat\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    8," +
							  "                                    10," +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    46," +
							  "                                    50," +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"fan\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {}," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"cool\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }" +
							  "                }" +
							  "            }," +
							  "            \"serial\": \"43170716\"," +
							  "            \"accessPoint\": {" +
							  "                \"password\": null," +
							  "                \"ssid\": \"SENSIBO-I-16158\"" +
							  "            }," +
							  "            \"remote\": {" +
							  "                \"window\": false," +
							  "                \"toggle\": false" +
							  "            }," +
							  "            \"room\": {" +
							  "                \"icon\": \"Lounge\"," +
							  "                \"uid\": \"FhwguC77\"," +
							  "                \"name\": \"Loftstue GG22A\"" +
							  "            }," +
							  "            \"qrId\": \"OZUSLFZKAK\"," +
							  "            \"firmwareType\": \"cc3100_stm32f0\"," +
							  "            \"timer\": null," +
							  "            \"remoteFlavor\": \"Gesticulating Swan\"," +
							  "            \"isGeofenceOnEnterEnabledForThisUser\": false," +
							  "            \"isMotionGeofenceOnExitEnabled\": false," +
							  "            \"remoteAlternatives\": [" +
							  "                \"_panasonic1_swingright\"," +
							  "                \"_panasonic1_different_powerful\"," +
							  "                \"_panasonic1_left\"," +
							  "                \"_panasonic1f\"," +
							  "                \"_panasonic1_middle\"," +
							  "                \"_panasonic1_right\"," +
							  "                \"_panasonic1_33khz\"," +
							  "                \"_panasonic1_nanoex\"," +
							  "                \"_panasonic1_middle_different_quiet\"," +
							  "                \"_panasonic1d\"," +
							  "                \"_panasonic1c\"," +
							  "                \"_panasonic1b\"," +
							  "                \"_panasonic1e\"," +
							  "                \"_panasonic2d\"" +
							  "            ]" +
							  "        }," +
							  "        {" +
							  "            \"configGroup\": \"stable\"," +
							  "            \"macAddress\": \"34:15:13:f7:b4:3b\"," +
							  "            \"isGeofenceOnExitEnabled\": false," +
							  "            \"features\": [" +
							  "                \"showPlus\"," +
							  "                \"softShowPlus\"" +
							  "            ]," +
							  "            \"currentlyAvailableFirmwareVersion\": \"IN010056\"," +
							  "            \"cleanFiltersNotificationEnabled\": false," +
							  "            \"connectionStatus\": {" +
							  "                \"isAlive\": true," +
							  "                \"lastSeen\": {" +
							  "                    \"secondsAgo\": 16," +
							  "                    \"time\": \"2021-01-28T05:30:25.701110Z\"" +
							  "                }" +
							  "            }," +
							  "            \"filtersCleaning\": null," +
							  "            \"acState\": {" +
							  "                \"on\": true," +
							  "                \"fanLevel\": \"auto\"," +
							  "                \"light\": \"on\"," +
							  "                \"temperatureUnit\": \"C\"," +
							  "                \"horizontalSwing\": \"stopped\"," +
							  "                \"swing\": \"fixedBottom\"," +
							  "                \"targetTemperature\": 16," +
							  "                \"mode\": \"heat\"," +
							  "                \"timestamp\": {" +
							  "                    \"secondsAgo\": -1," +
							  "                    \"time\": \"2021-01-28T05:30:43.277205Z\"" +
							  "                }" +
							  "            }," +
							  "            \"isOwner\": true," +
							  "            \"mainMeasurementsSensor\": null," +
							  "            \"motionSensors\": []," +
							  "            \"id\": \"owWW2LBB\"," +
							  "            \"firmwareVersion\": \"IN010056\"," +
							  "            \"roomIsOccupied\": null," +
							  "            \"warrantyEligible\": \"no\"," +
							  "            \"motionConfig\": null," +
							  "            \"measurements\": {" +
							  "                \"batteryVoltage\": null," +
							  "                \"temperature\": 15.5," +
							  "                \"humidity\": 28.9," +
							  "                \"time\": {" +
							  "                    \"secondsAgo\": 16," +
							  "                    \"time\": \"2021-01-28T05:30:25.701110Z\"" +
							  "                }," +
							  "                \"rssi\": -47," +
							  "                \"piezo\": [" +
							  "                    null," +
							  "                    null" +
							  "                ]," +
							  "                \"pm25\": 0" +
							  "            }," +
							  "            \"isClimateReactGeofenceOnEnterEnabledForThisUser\": false," +
							  "            \"homekitSupported\": false," +
							  "            \"pureBoostConfig\": null," +
							  "            \"smartMode\": null," +
							  "            \"shouldShowFilterCleaningNotification\": false," +
							  "            \"location\": {" +
							  "                \"latLon\": [" +
							  "                    61.1885338," +
							  "                    6.8555206" +
							  "                ]," +
							  "                \"updateTime\": {" +
							  "                    \"secondsAgo\": 65553005," +
							  "                    \"time\": \"2018-12-31T12:20:37Z\"" +
							  "                }," +
							  "                \"features\": [" +
							  "                    \"showPlus\"," +
							  "                    \"softShowPlus\"" +
							  "                ]," +
							  "                \"country\": \"Norge\"," +
							  "                \"occupancy\": \"n/a\"," +
							  "                \"createTime\": {" +
							  "                    \"secondsAgo\": 65556149," +
							  "                    \"time\": \"2018-12-31T11:28:13Z\"" +
							  "                }," +
							  "                \"address\": [" +
							  "                    \"Røysavegen 47\"," +
							  "                    \"Sogn og Fjordane\"" +
							  "                ]," +
							  "                \"geofenceTriggerRadius\": 200," +
							  "                \"subscription\": null," +
							  "                \"id\": \"uRFs4NdYxK\"," +
							  "                \"name\": \"RV47\"" +
							  "            }," +
							  "            \"sensorsCalibration\": {" +
							  "                \"temperature\": 0.0," +
							  "                \"humidity\": 0.0" +
							  "            }," +
							  "            \"tags\": []," +
							  "            \"productModel\": \"skyv2\"," +
							  "            \"isMotionGeofenceOnEnterEnabled\": false," +
							  "            \"lastHealthcheck\": null," +
							  "            \"temperatureUnit\": \"C\"," +
							  "            \"schedules\": []," +
							  "            \"isClimateReactGeofenceOnExitEnabled\": false," +
							  "            \"remoteCapabilities\": {" +
							  "                \"modes\": {" +
							  "                    \"dry\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"auto\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"heat\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    8," +
							  "                                    10," +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    46," +
							  "                                    50," +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"fan\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {}," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"cool\": {" +
							  "                        \"light\": [" +
							  "                            \"on\"," +
							  "                            \"off\"" +
							  "                        ]," +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedRight\"," +
							  "                            \"fixedCenter\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium_low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }" +
							  "                }" +
							  "            }," +
							  "            \"serial\": \"43170817\"," +
							  "            \"accessPoint\": {" +
							  "                \"password\": null," +
							  "                \"ssid\": \"SENSIBO-I-84861\"" +
							  "            }," +
							  "            \"remote\": {" +
							  "                \"window\": false," +
							  "                \"toggle\": false" +
							  "            }," +
							  "            \"room\": {" +
							  "                \"icon\": \"Lounge\"," +
							  "                \"uid\": \"RFPkffs2\"," +
							  "                \"name\": \"Stue RV47\"" +
							  "            }," +
							  "            \"qrId\": \"SYLYLUMBRC\"," +
							  "            \"firmwareType\": \"cc3100_stm32f0\"," +
							  "            \"timer\": null," +
							  "            \"remoteFlavor\": \"Gesticulating Swan\"," +
							  "            \"isGeofenceOnEnterEnabledForThisUser\": false," +
							  "            \"isMotionGeofenceOnExitEnabled\": false," +
							  "            \"remoteAlternatives\": [" +
							  "                \"_panasonic1_swingright\"," +
							  "                \"_panasonic1_different_powerful\"," +
							  "                \"_panasonic1_left\"," +
							  "                \"_panasonic1f\"," +
							  "                \"_panasonic1_middle\"," +
							  "                \"_panasonic1_right\"," +
							  "                \"_panasonic1_33khz\"," +
							  "                \"_panasonic1_nanoex\"," +
							  "                \"_panasonic1_middle_different_quiet\"," +
							  "                \"_panasonic1d\"," +
							  "                \"_panasonic1c\"," +
							  "                \"_panasonic1b\"," +
							  "                \"_panasonic1e\"," +
							  "                \"_panasonic2d\"" +
							  "            ]" +
							  "        }," +
							  "        {" +
							  "            \"configGroup\": \"stable\"," +
							  "            \"macAddress\": \"50:02:91:B6:E2:8A\"," +
							  "            \"isGeofenceOnExitEnabled\": false," +
							  "            \"features\": [" +
							  "                \"showPlus\"," +
							  "                \"softShowPlus\"" +
							  "            ]," +
							  "            \"currentlyAvailableFirmwareVersion\": \"SKY30044\"," +
							  "            \"cleanFiltersNotificationEnabled\": false," +
							  "            \"connectionStatus\": {" +
							  "                \"isAlive\": true," +
							  "                \"lastSeen\": {" +
							  "                    \"secondsAgo\": 54," +
							  "                    \"time\": \"2021-01-28T05:29:47.309430Z\"" +
							  "                }" +
							  "            }," +
							  "            \"filtersCleaning\": null," +
							  "            \"acState\": {" +
							  "                \"on\": true," +
							  "                \"fanLevel\": \"low\"," +
							  "                \"timestamp\": {" +
							  "                    \"secondsAgo\": -1," +
							  "                    \"time\": \"2021-01-28T05:30:43.317481Z\"" +
							  "                }," +
							  "                \"temperatureUnit\": \"C\"," +
							  "                \"horizontalSwing\": \"fixedCenter\"," +
							  "                \"targetTemperature\": 17," +
							  "                \"mode\": \"heat\"," +
							  "                \"swing\": \"fixedTop\"" +
							  "            }," +
							  "            \"isOwner\": true," +
							  "            \"mainMeasurementsSensor\": null," +
							  "            \"motionSensors\": []," +
							  "            \"id\": \"afPzdVSi\"," +
							  "            \"firmwareVersion\": \"SKY30044\"," +
							  "            \"roomIsOccupied\": null," +
							  "            \"warrantyEligible\": \"no\"," +
							  "            \"motionConfig\": null," +
							  "            \"measurements\": {" +
							  "                \"batteryVoltage\": null," +
							  "                \"temperature\": 18.0," +
							  "                \"humidity\": 29.6," +
							  "                \"time\": {" +
							  "                    \"secondsAgo\": 54," +
							  "                    \"time\": \"2021-01-28T05:29:47.309430Z\"" +
							  "                }," +
							  "                \"rssi\": -63," +
							  "                \"piezo\": [" +
							  "                    null," +
							  "                    null" +
							  "                ]," +
							  "                \"pm25\": 0" +
							  "            }," +
							  "            \"isClimateReactGeofenceOnEnterEnabledForThisUser\": false," +
							  "            \"homekitSupported\": false," +
							  "            \"pureBoostConfig\": null," +
							  "            \"smartMode\": null," +
							  "            \"shouldShowFilterCleaningNotification\": false," +
							  "            \"location\": {" +
							  "                \"latLon\": [" +
							  "                    58.9111703926336," +
							  "                    5.96450595641008" +
							  "                ]," +
							  "                \"updateTime\": null," +
							  "                \"features\": [" +
							  "                    \"showPlus\"," +
							  "                    \"softShowPlus\"" +
							  "                ]," +
							  "                \"country\": null," +
							  "                \"occupancy\": \"n/a\"," +
							  "                \"createTime\": {" +
							  "                    \"secondsAgo\": 9375838," +
							  "                    \"time\": \"2020-10-11T17:06:44Z\"" +
							  "                }," +
							  "                \"address\": [" +
							  "                    \"Holavigbakken 2\"," +
							  "                    \" 4308 Sandnes\"," +
							  "                    \" Norway\"" +
							  "                ]," +
							  "                \"geofenceTriggerRadius\": 200," +
							  "                \"subscription\": null," +
							  "                \"id\": \"tF88DdZGKD\"," +
							  "                \"name\": \"HVB2\"" +
							  "            }," +
							  "            \"sensorsCalibration\": {" +
							  "                \"temperature\": 0.0," +
							  "                \"humidity\": 0.0" +
							  "            }," +
							  "            \"tags\": []," +
							  "            \"productModel\": \"skyv2\"," +
							  "            \"isMotionGeofenceOnEnterEnabled\": false," +
							  "            \"lastHealthcheck\": null," +
							  "            \"temperatureUnit\": \"C\"," +
							  "            \"schedules\": []," +
							  "            \"isClimateReactGeofenceOnExitEnabled\": false," +
							  "            \"remoteCapabilities\": {" +
							  "                \"modes\": {" +
							  "                    \"dry\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30," +
							  "                                    31" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86," +
							  "                                    88" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"auto\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30," +
							  "                                    31" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86," +
							  "                                    88" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"heat\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    10," +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30," +
							  "                                    31" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    50," +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86," +
							  "                                    88" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"fan\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {}," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }," +
							  "                    \"cool\": {" +
							  "                        \"horizontalSwing\": [" +
							  "                            \"fixedCenter\"," +
							  "                            \"fixedCenterLeft\"," +
							  "                            \"fixedCenterRight\"," +
							  "                            \"fixedLeft\"," +
							  "                            \"fixedRight\"," +
							  "                            \"stopped\"" +
							  "                        ]," +
							  "                        \"swing\": [" +
							  "                            \"stopped\"," +
							  "                            \"fixedTop\"," +
							  "                            \"fixedMiddleTop\"," +
							  "                            \"fixedMiddle\"," +
							  "                            \"fixedMiddleBottom\"," +
							  "                            \"fixedBottom\"," +
							  "                            \"rangeFull\"" +
							  "                        ]," +
							  "                        \"temperatures\": {" +
							  "                            \"C\": {" +
							  "                                \"isNative\": true," +
							  "                                \"values\": [" +
							  "                                    16," +
							  "                                    17," +
							  "                                    18," +
							  "                                    19," +
							  "                                    20," +
							  "                                    21," +
							  "                                    22," +
							  "                                    23," +
							  "                                    24," +
							  "                                    25," +
							  "                                    26," +
							  "                                    27," +
							  "                                    28," +
							  "                                    29," +
							  "                                    30," +
							  "                                    31" +
							  "                                ]" +
							  "                            }," +
							  "                            \"F\": {" +
							  "                                \"isNative\": false," +
							  "                                \"values\": [" +
							  "                                    61," +
							  "                                    63," +
							  "                                    64," +
							  "                                    66," +
							  "                                    68," +
							  "                                    70," +
							  "                                    72," +
							  "                                    73," +
							  "                                    75," +
							  "                                    77," +
							  "                                    79," +
							  "                                    81," +
							  "                                    82," +
							  "                                    84," +
							  "                                    86," +
							  "                                    88" +
							  "                                ]" +
							  "                            }" +
							  "                        }," +
							  "                        \"fanLevels\": [" +
							  "                            \"quiet\"," +
							  "                            \"low\"," +
							  "                            \"medium\"," +
							  "                            \"medium_high\"," +
							  "                            \"high\"," +
							  "                            \"auto\"," +
							  "                            \"strong\"" +
							  "                        ]" +
							  "                    }" +
							  "                }" +
							  "            }," +
							  "            \"serial\": \"511901004\"," +
							  "            \"accessPoint\": {" +
							  "                \"password\": null," +
							  "                \"ssid\": \"SENSIBO-I-38268\"" +
							  "            }," +
							  "            \"remote\": {" +
							  "                \"window\": false," +
							  "                \"toggle\": false" +
							  "            }," +
							  "            \"room\": {" +
							  "                \"icon\": \"Lounge\"," +
							  "                \"uid\": \"Ghwnosui\"," +
							  "                \"name\": \"Gang HVB2\"" +
							  "            }," +
							  "            \"qrId\": \"IGKGZCNWEB\"," +
							  "            \"firmwareType\": \"esp8266ex\"," +
							  "            \"timer\": null," +
							  "            \"remoteFlavor\": \"Enthusiastic Triceratops\"," +
							  "            \"isGeofenceOnEnterEnabledForThisUser\": false," +
							  "            \"isMotionGeofenceOnExitEnabled\": false," +
							  "            \"remoteAlternatives\": [" +
							  "                \"_mitsubishi1fa\"," +
							  "                \"_mitsubishi1_for_ben_ho\"," +
							  "                \"_mitsubishi1_plasma_on\"," +
							  "                \"_mitsubishi1_plasma_on_clean_on\"," +
							  "                \"_mitsubishi1b\"," +
							  "                \"mitsubishi1f\"," +
							  "                \"_mitsubishi1_isee_and_absence_detection\"," +
							  "                \"_mitsubishi1f\"" +
							  "            ]" +
							  "        }" +
							  "    ]" +
							  "}";

		private string _jsonGetAllRuneR= "{" +
"    \"status\": \"success\"," +
"    \"result\": [" +
"        {" +
"            \"configGroup\": \"stable\"," +
"            \"macAddress\": \"F4:CF:A2:50:2A:7C\"," +
"            \"measurements\": {" +
"                \"batteryVoltage\": null," +
"                \"temperature\": 20.0," +
"                \"humidity\": 32.2," +
"                \"time\": {" +
"                    \"secondsAgo\": 80," +
"                    \"time\": \"2021-02-21T05:59:48.368603Z\"" +
"                }," +
"                \"rssi\": -61," +
"                \"piezo\": [" +
"                    null," +
"                    null" +
"                ]," +
"                \"pm25\": 0" +
"            }," +
"            \"features\": [" +
"                \"showPlus\"" +
"            ]," +
"            \"currentlyAvailableFirmwareVersion\": \"SKY30044\"," +
"            \"cleanFiltersNotificationEnabled\": false," +
"            \"connectionStatus\": {" +
"                \"isAlive\": true," +
"                \"lastSeen\": {" +
"                    \"secondsAgo\": 80," +
"                    \"time\": \"2021-02-21T05:59:48.368603Z\"" +
"                }" +
"            }," +
"            \"filtersCleaning\": null," +
"            \"acState\": {" +
"                \"on\": true," +
"                \"fanLevel\": \"auto\"," +
"                \"light\": \"on\"," +
"                \"temperatureUnit\": \"C\"," +
"                \"horizontalSwing\": \"stopped\"," +
"                \"swing\": \"stopped\"," +
"                \"targetTemperature\": 16," +
"                \"mode\": \"heat\"," +
"                \"timestamp\": {" +
"                    \"secondsAgo\": 0," +
"                    \"time\": \"2021-02-21T06:01:09.699305Z\"" +
"                }" +
"            }," +
"            \"isOwner\": true," +
"            \"mainMeasurementsSensor\": null," +
"            \"motionSensors\": []," +
"            \"runningHealthcheck\": null," +
"            \"firmwareType\": \"esp8266ex\"," +
"            \"id\": \"W4YJk5dx\"," +
"            \"firmwareVersion\": \"SKY30044\"," +
"            \"roomIsOccupied\": null," +
"            \"warrantyEligible\": \"no\"," +
"            \"motionConfig\": null," +
"            \"schedules\": []," +
"            \"isGeofenceOnExitEnabled\": false," +
"            \"isClimateReactGeofenceOnEnterEnabledForThisUser\": false," +
"            \"homekitSupported\": false," +
"            \"pureBoostConfig\": null," +
"            \"smartMode\": null," +
"            \"shouldShowFilterCleaningNotification\": false," +
"            \"location\": {" +
"                \"latLon\": [" +
"                    0.1," +
"                    0.1" +
"                ]," +
"                \"updateTime\": null," +
"                \"features\": []," +
"                \"country\": null," +
"                \"occupancy\": \"n/a\"," +
"                \"createTime\": {" +
"                    \"secondsAgo\": 4120514," +
"                    \"time\": \"2021-01-04T13:25:55Z\"" +
"                }," +
"                \"address\": []," +
"                \"geofenceTriggerRadius\": 200," +
"                \"subscription\": null," +
"                \"id\": \"cxx8W4JD8T\"," +
"                \"name\": \"Default Home\"" +
"            }," +
"            \"sensorsCalibration\": {" +
"                \"temperature\": 0.0," +
"                \"humidity\": 0.0" +
"            }," +
"            \"tags\": []," +
"            \"productModel\": \"skyv2\"," +
"            \"isMotionGeofenceOnEnterEnabled\": false," +
"            \"isGeofenceOnEnterEnabledForThisUser\": false," +
"            \"isClimateReactGeofenceOnExitEnabled\": false," +
"            \"remoteCapabilities\": {" +
"                \"modes\": {" +
"                    \"dry\": {" +
"                        \"light\": [" +
"                            \"on\"," +
"                            \"off\"" +
"                        ]," +
"                        \"horizontalSwing\": [" +
"                            \"fixedLeft\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedRight\"," +
"                            \"fixedCenter\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium_low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }," +
"                    \"auto\": {" +
"                        \"light\": [" +
"                            \"on\"," +
"                            \"off\"" +
"                        ]," +
"                        \"horizontalSwing\": [" +
"                            \"fixedLeft\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedRight\"," +
"                            \"fixedCenter\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium_low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }," +
"                    \"heat\": {" +
"                        \"light\": [" +
"                            \"on\"," +
"                            \"off\"" +
"                        ]," +
"                        \"horizontalSwing\": [" +
"                            \"fixedLeft\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedRight\"," +
"                            \"fixedCenter\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    8," +
"                                    10," +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    46," +
"                                    50," +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium_low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }," +
"                    \"fan\": {" +
"                        \"light\": [" +
"                            \"on\"," +
"                            \"off\"" +
"                        ]," +
"                        \"horizontalSwing\": [" +
"                            \"fixedLeft\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedRight\"," +
"                            \"fixedCenter\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"temperatures\": {}," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium_low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }," +
"                    \"cool\": {" +
"                        \"light\": [" +
"                            \"on\"," +
"                            \"off\"" +
"                        ]," +
"                        \"horizontalSwing\": [" +
"                            \"fixedLeft\"," +
"                            \"fixedCenterLeft\"," +
"                            \"fixedCenterRight\"," +
"                            \"fixedRight\"," +
"                            \"fixedCenter\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"swing\": [" +
"                            \"fixedTop\"," +
"                            \"fixedMiddleTop\"," +
"                            \"fixedMiddle\"," +
"                            \"fixedMiddleBottom\"," +
"                            \"fixedBottom\"," +
"                            \"stopped\"" +
"                        ]," +
"                        \"temperatures\": {" +
"                            \"C\": {" +
"                                \"isNative\": true," +
"                                \"values\": [" +
"                                    16," +
"                                    17," +
"                                    18," +
"                                    19," +
"                                    20," +
"                                    21," +
"                                    22," +
"                                    23," +
"                                    24," +
"                                    25," +
"                                    26," +
"                                    27," +
"                                    28," +
"                                    29," +
"                                    30" +
"                                ]" +
"                            }," +
"                            \"F\": {" +
"                                \"isNative\": false," +
"                                \"values\": [" +
"                                    61," +
"                                    63," +
"                                    64," +
"                                    66," +
"                                    68," +
"                                    70," +
"                                    72," +
"                                    73," +
"                                    75," +
"                                    77," +
"                                    79," +
"                                    81," +
"                                    82," +
"                                    84," +
"                                    86" +
"                                ]" +
"                            }" +
"                        }," +
"                        \"fanLevels\": [" +
"                            \"quiet\"," +
"                            \"low\"," +
"                            \"medium_low\"," +
"                            \"medium\"," +
"                            \"medium_high\"," +
"                            \"high\"," +
"                            \"auto\"," +
"                            \"strong\"" +
"                        ]" +
"                    }" +
"                }" +
"            }," +
"            \"serial\": \"452005396\"," +
"            \"accessPoint\": {" +
"                \"password\": null," +
"                \"ssid\": \"SENSIBO-I-66915\"" +
"            }," +
"            \"remote\": {" +
"                \"window\": false," +
"                \"toggle\": false" +
"            }," +
"            \"room\": {" +
"                \"icon\": \"Livingroom\"," +
"                \"uid\": \"B7P6rkzq\"," +
"                \"name\": \"Stue\"" +
"            }," +
"            \"qrId\": \"HFXHAZTDAD\"," +
"            \"temperatureUnit\": \"C\"," +
"            \"timer\": null," +
"            \"remoteFlavor\": \"Gesticulating Swan\"," +
"            \"isMotionGeofenceOnExitEnabled\": false," +
"            \"remoteAlternatives\": [" +
"                \"_panasonic1_swingright\"," +
"                \"_panasonic1_different_powerful\"," +
"                \"_panasonic1_left\"," +
"                \"_panasonic1f\"," +
"                \"_panasonic1_middle\"," +
"                \"_panasonic1_right\"," +
"                \"_panasonic1_33khz\"," +
"                \"_panasonic1_nanoex\"," +
"                \"_panasonic1_middle_different_quiet\"," +
"                \"_panasonic1d\"," +
"                \"_panasonic1c\"," +
"                \"_panasonic1b\"," +
"                \"_panasonic1e\"," +
"                \"_panasonic2d\"" +
"            ]" +
"        }" +
"    ]" +
"}";
	}
}