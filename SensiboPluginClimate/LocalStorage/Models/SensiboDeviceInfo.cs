using HSPI_SensiboClimate.Plugin.API.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSPI_SensiboClimate.LocalStorage.Models
{
    public class SensiboDeviceInfo
    {
        public string Id { get; set; }
        public bool Status { get; set; }
        public bool On { get; set; }
        public bool PowerChanged { get; set; }
        public bool Ban { get; set; }
        public bool SmartMode { get; set; }
        public bool Scheduled { get; set; }
        public string Mac { get; set; }
        public double CurrentTemperature { get; set; }
        public double CurrentHumidity { get; set; }
        public double TargetTemperature { get; set; }
        public string TemperatureUnit { get; set; }
        public string FanLevel { get; set; }
        public string Mode { get; set; }
        public string Swing { get; set; }
        public string Room { get; set; }
        public string Address { get; set; }
        public Modes Modes { get; set; }
    }
}
