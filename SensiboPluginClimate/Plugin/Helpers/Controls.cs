using HomeSeerAPI;
using HSPI_SensiboClimate.Plugin.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HomeSeerAPI.Enums;
using static HomeSeerAPI.VSVGPairs;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
    public static class Controls
    {
        private static IHSApplication HS { get => HSPI.HSApp; }

        public static VSPair CreateButton(string name, int value)
        {
            var btn = new VSPair(ePairStatusControl.Both)
            {
                PairType = VSVGPairType.SingleValue,
                Render = CAPIControlType.Button,
                Status = name,
                Value = value
            };

            return btn;
        }

        public static VSPair CreateTextBox(string name, int value)
        {
            var box = new VSPair(ePairStatusControl.Both)
            {
                PairType = VSVGPairType.SingleValue,
                Render = CAPIControlType.TextBox_Number,
                Status = name,
                Value = value
            };

            return box;
        }

        public static VSPair CreateRangeControl(string name, TemperatureScale suffix)
        {
            var range = new VSPair(ePairStatusControl.Control)
            {
                PairType = VSVGPairType.Range,
                Render = CAPIControlType.ValuesRange,
                //RangeStatusSuffix = " " + VSPair.ScaleReplace,
                Status = name,
                HasScale = true,
                ControlUse = ePairControlUse._HeatSetPoint
            };

            if (suffix.Equals(TemperatureScale.Celsius))
            {
                range.RangeStart = 0;
                range.RangeEnd = 30;
            }
            else
            {
                range.RangeStart = 32;
                range.RangeEnd = 86;
            }

            return range;
        }

        public static VSPair CreateRangeStatus(string name)
        {
            var range = new VSPair(ePairStatusControl.Status)
            {
                PairType = VSVGPairType.Range,
                RangeStart = -150d,
                RangeEnd = 150d,
                Render = CAPIControlType.Values,
                RangeStatusSuffix = " " + VSPair.ScaleReplace,
                Status = name,
                HasScale = true
            };

            return range;
        }

        public static VGPair GetRangeGraphic(Period period, TemperatureScale suffix)
        {
            VGPair rangeGraphic = null;
            switch (period)
            {
                case Period.Frost:
                    var imageFrost = "/images/HomeSeer/status/Thermometer-00.png";
                    var frostPeriod = Getintervals(suffix)[0];
                    rangeGraphic = CreateRangeGraphic(frostPeriod.start, frostPeriod.end, imageFrost);
                    break;
                case Period.Cold:
                    var imageCold = "/images/HomeSeer/status/Thermometer-20.png";
                    var coldPeriod = Getintervals(suffix)[1];
                    rangeGraphic = CreateRangeGraphic(coldPeriod.start, coldPeriod.end, imageCold);
                    break;
                case Period.Fresh:
                    var imageFresh = "/images/HomeSeer/status/Thermometer-40.png";
                    var freshPeriod = Getintervals(suffix)[2];
                    rangeGraphic = CreateRangeGraphic(freshPeriod.start, freshPeriod.end, imageFresh);
                    break;
                case Period.Warm:
                    var imageWarm = "/images/HomeSeer/status/Thermometer-80.png";
                    var warmPeriod = Getintervals(suffix)[3];
                    rangeGraphic = CreateRangeGraphic(warmPeriod.start, warmPeriod.end, imageWarm);
                    break;
                case Period.Hot:
                    var imageHot = "/images/HomeSeer/status/Thermometer-100.png";
                    var hotPeriod = Getintervals(suffix)[4];
                    rangeGraphic = CreateRangeGraphic(hotPeriod.start, hotPeriod.end, imageHot);
                    break;
            }

            return rangeGraphic;
        }

        public static VGPair GetModeGraphic(int value, ModeClimate mode)
        {
            VGPair graphic = null;
            switch (mode)
            {
                case ModeClimate.Off:
                    var off = "/images/HomeSeer/status/off.gif";
                    graphic = CreateSingleGraphic(value, off);
                    break;
                case ModeClimate.Auto:
                    var auto = "/images/HomeSeer/status/mode-auto.png";
                    graphic = CreateSingleGraphic(value, auto);
                    break;
                case ModeClimate.Cool:
                    var cool = "/images/HomeSeer/status/Cool.png";
                    graphic = CreateSingleGraphic(value, cool);
                    break;
                case ModeClimate.Heat:
                    var heat = "/images/HomeSeer/status/Heat.png";
                    graphic = CreateSingleGraphic(value, heat);
                    break;
                case ModeClimate.Fan:
                    var fan = "/images/HomeSeer/status/fanonly.png";
                    graphic = CreateSingleGraphic(value, fan);
                    break;
                case ModeClimate.Dry:
                    var dry = "/images/HomeSeer/status/dry.gif";
                    graphic = CreateSingleGraphic(value, dry);
                    break;
            }

            return graphic;
        }

        public static VGPair GetFanGraphic(int value, FanLevelsClimate fan)
        {
            VGPair graphic = null;
            switch (fan)
            {
                case FanLevelsClimate.Quiet:
                    var quiet = "/images/HomeSeer/status/quiet.png";
                    graphic = CreateSingleGraphic(value, quiet);
                    break;
                case FanLevelsClimate.Low:
                    var low = "/images/HomeSeer/status/fan-state-low.png";
                    graphic = CreateSingleGraphic(value, low);
                    break;
                case FanLevelsClimate.Medium_Low:
                    var mLow = "/images/HomeSeer/status/fan.png";
                    graphic = CreateSingleGraphic(value, mLow);
                    break;
                case FanLevelsClimate.Medium:
                    var medium = "/images/HomeSeer/status/fan.png";
                    graphic = CreateSingleGraphic(value, medium);
                    break;
                case FanLevelsClimate.Medium_High:
                    var mHot = "/images/HomeSeer/status/fan-state-high.png";
                    graphic = CreateSingleGraphic(value, mHot);
                    break;
                case FanLevelsClimate.High:
                    var high = "/images/HomeSeer/status/fan-state-high.png";
                    graphic = CreateSingleGraphic(value, high);
                    break;
                case FanLevelsClimate.Auto:
                    var auto = "/images/HomeSeer/status/fan-auto.png";
                    graphic = CreateSingleGraphic(value, auto);
                    break;
            }

            return graphic;
        }

        private static List<(double start, double end)> Getintervals(TemperatureScale suffix)
        {
            var intervals = new List<(double start, double end)>();

            switch (suffix)
            {
                case TemperatureScale.Celsius:
                    intervals.Add((-150, 10));
                    intervals.Add((10.01, 15));
                    intervals.Add((15.01, 20));
                    intervals.Add((20.01, 29));
                    intervals.Add((29.01, 150));
                    break;
                case TemperatureScale.Fahrenheit:
                    intervals.Add((-150, 50));
                    intervals.Add((50.01, 59));
                    intervals.Add((59.01, 68));
                    intervals.Add((68.01, 84));
                    intervals.Add((84.01, 150));
                    break;
            }

            return intervals;
        }

        private static VGPair CreateSingleGraphic(int value, string picturePath)
        {
            var graphic = new VGPair
            {
                PairType = VSVGPairType.SingleValue,
                Graphic = picturePath,
                Set_Value = value
            };

            return graphic;
        }

        private static VGPair CreateRangeGraphic(double rangeStart, double rangeEnd, string picturePath)
        {
            var rangeGraphic = new VGPair
            {
                RangeStart = rangeStart,
                RangeEnd = rangeEnd,
                PairType = VSVGPairType.Range,
                Graphic = picturePath
            };

            return rangeGraphic;
        }
    }
}
