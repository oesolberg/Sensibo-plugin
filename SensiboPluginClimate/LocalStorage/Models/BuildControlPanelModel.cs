using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSPI_SensiboClimate.LocalStorage.Models
{
    public class BuildControlPanelModel
    {
        public string Location { get; set; }
        public string Id { get; set; }
        public string TemperatureUnit { get; set; }
        public List<int> TemperatureRange { get; set; }
    }
}
