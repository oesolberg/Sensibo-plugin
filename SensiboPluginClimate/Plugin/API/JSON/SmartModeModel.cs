using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSPI_SensiboClimate.Plugin.API.JSON
{
    public partial class SmartModeModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("on")]
        public bool On { get; set; }
    }
}
