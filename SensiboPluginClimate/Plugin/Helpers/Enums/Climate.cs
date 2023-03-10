using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSPI_SensiboClimate.Plugin.Helpers.Enums
{
	public enum ModeClimate
	{
		Off,
		Auto,
		Cool,
		Heat,
		Fan,
		Dry
	}

	public enum FanLevelsClimate
	{
		Quiet,
		Low,
		Medium_Low,
		Medium,
		Medium_High,
		High,
		Auto,
		Strong,
	}

	public enum PowerClimate
	{
		Off = 0,
		On = 1,
	}

	public enum Period
	{
		Frost,
		Cold,
		Fresh,
		Warm,
		Hot
	}

	public enum TemperatureScale
	{
		Celsius,
		Fahrenheit
	}

	public enum SwingState
	{
		Stopped=0,
		FixedTop,
		FixedMiddleTop,
		FixedMiddle,
		FixedMiddleBottom,
		FixedBottom,
		RangeFull
	}

	public enum HorizontalSwingStates
	{
		Stopped=0,
		FixedCenter,
		FixedCenterLeft,
		FixedCenterRight,
		FixedLeft,
		FixedRight
	}
}
