using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSPI_SensiboClimate.Plugin.API.JSON
{
    public class ACStateUpdate
    {
        [JsonProperty("acState")]
        public ACState ACState { get; set; }

        public override string ToString()
        {
	        if (ACState != null)
	        {
		        return ACState.ToString();
	        }

	        return "AcState=null";
        }
    }

    public class AcSingleStateUpdate
    {
	    [JsonProperty("newValue")]
		public object NewValue { get; set; }
    }
}
