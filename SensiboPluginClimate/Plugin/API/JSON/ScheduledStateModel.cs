using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSPI_SensiboClimate.Plugin.API.JSON
{
    public partial class ScheduledStateModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public List<Result> Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
