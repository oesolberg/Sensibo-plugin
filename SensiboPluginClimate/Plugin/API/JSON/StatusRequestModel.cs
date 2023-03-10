using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSPI_SensiboClimate.Plugin.API.JSON
{
    public class StatusRequestModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("acState")]
        public AcState AcState { get; set; }

        [JsonProperty("changedProperties")]
        public List<string> ChangedProperties { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("failureReason")]
        public string FailureReason { get; set; }
    }

    public partial class AcState
    {
        [JsonProperty("on")]
        public string On { get; set; }

        [JsonProperty("targetTemperature")]
        public double TargetTemperature { get; set; }

        [JsonProperty("temperatureUnit")]
        public string TemperatureUnit { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("swing")]
        public string Swing { get; set; }

        [JsonProperty("fanLevel")]
        public string FanLevel { get; set; }

        [JsonProperty("horizontalSwing")]
        public string HorizontalSwing { get; set; }

        public override string ToString()
        {
	        return $"On={On},TargetTemperature={TargetTemperature:F0}, TemperatureUnit={TemperatureUnit}, Mode={Mode}";
        }
    }
}
