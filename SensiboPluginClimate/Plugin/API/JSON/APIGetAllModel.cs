using System;
using System.Collections.Generic;
using HSPI_SensiboClimate.Plugin.Helpers.Enums;
using Newtonsoft.Json;

namespace HSPI_SensiboClimate.Plugin.API.JSON
{
    public partial class APIGetAllModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public List<ACInfo> ACInfoList { get; set; }
    }

    public partial class ACInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("configGroup")]
        public string ConfigGroup { get; set; }

        [JsonProperty("macAddress")]
        public string MacAddress { get; set; }

        [JsonProperty("cleanFiltersNotificationEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool CleanFiltersNotificationEnabled { get; set; }

        [JsonProperty("measurements", NullValueHandling = NullValueHandling.Ignore)]
        public Measurements Measurements { get; set; }

        [JsonProperty("isGeofenceOnEnterEnabledForThisUser", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsGeofenceOnEnterEnabledForThisUser { get; set; }

        [JsonProperty("temperatureUnit")]
        public string TemperatureUnit { get; set; }

        [JsonProperty("connectionStatus")]
        public ConnectionStatus ConnectionStatus { get; set; }

        [JsonProperty("acState")]
        public ACState AcState { get; set; }

        [JsonProperty("shouldShowFilterCleaningNotification")]
        public bool ShouldShowFilterCleaningNotification { get; set; }

        [JsonProperty("remoteCapabilities")]
        public RemoteCapabilities RemoteCapabilities { get; set; }

        [JsonProperty("room")]
        public Room Room { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("address")]
        public List<string> Address { get; set; }
    }

    public partial class RemoteCapabilities
    {
        [JsonProperty("modes")]
        public Modes Modes { get; set; }
    }

    public partial class Modes
    {
        [JsonProperty("dry", NullValueHandling = NullValueHandling.Ignore)]
        public Mode Dry { get; set; }

        [JsonProperty("auto", NullValueHandling = NullValueHandling.Ignore)]
        public Mode Auto { get; set; }

        [JsonProperty("heat", NullValueHandling = NullValueHandling.Ignore)]
        public Mode Heat { get; set; }

        [JsonProperty("fan", NullValueHandling = NullValueHandling.Ignore)]
        public Mode Fan { get; set; }

        [JsonProperty("cool", NullValueHandling = NullValueHandling.Ignore)]
        public Mode Cool { get; set; }

        [JsonProperty("sleep", NullValueHandling = NullValueHandling.Ignore)]
        public Mode Sleep { get; set; }
    }

    public partial class Mode
    {
        [JsonProperty("swing", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Swing { get; set; }

        [JsonProperty("fanLevels")]
        public List<string> FanLevels { get; set; }

        [JsonProperty("temperatures")]
        public Temperatures Temperatures { get; set; }
    }

    public partial class Temperatures
    {
        [JsonProperty("C", NullValueHandling = NullValueHandling.Ignore)]
        public Temperature C { get; set; }

        [JsonProperty("F", NullValueHandling = NullValueHandling.Ignore)]
        public Temperature F { get; set; }
    }

    public partial class Temperature
    {
        [JsonProperty("isNative", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsNative { get; set; }

        [JsonProperty("values")]
        public List<int> Values { get; set; }
    }

    public partial class ACState
    {
        [JsonProperty("on", NullValueHandling = NullValueHandling.Ignore)]
        public bool On { get; set; }

        [JsonProperty("fanLevel", NullValueHandling = NullValueHandling.Ignore)]
        public string FanLevel { get; set; }

        [JsonProperty("temperatureUnit", NullValueHandling = NullValueHandling.Ignore)]
        public string TemperatureUnit { get; set; }

        [JsonProperty("targetTemperature", NullValueHandling = NullValueHandling.Ignore)]
        public int TargetTemperature { get; set; }

        [JsonProperty("mode", NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }

        [JsonProperty("swing", NullValueHandling = NullValueHandling.Ignore)]
        public string Swing { get; set; }

        public override string ToString()
        {
	        return $"AcState: On={On.ToString()}, FanLevelsClimate={FanLevel}, TemperatureUnit={TemperatureUnit}, Mode={Mode}, Swing={Swing}";
        }
    }

    public partial class ConnectionStatus
    {
        [JsonProperty("isAlive", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsAlive { get; set; }

        [JsonProperty("lastSeen")]
        public LastSeen LastSeen { get; set; }
    }

    public partial class LastSeen
    {
        [JsonProperty("secondsAgo", NullValueHandling = NullValueHandling.Ignore)]
        public long SecondsAgo { get; set; }

        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset Time { get; set; }
    }

    public partial class Measurements
    {
        [JsonProperty("batteryVoltage")]
        public object BatteryVoltage { get; set; }

        [JsonProperty("temperature", NullValueHandling = NullValueHandling.Ignore)]
        public double Temperature { get; set; }

        [JsonProperty("humidity", NullValueHandling = NullValueHandling.Ignore)]
        public double Humidity { get; set; }

        [JsonProperty("time")]
        public LastSeen Time { get; set; }
    }

    public partial class Room
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

