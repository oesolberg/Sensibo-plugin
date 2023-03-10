using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HSPI_SensiboClimate.Plugin.Helpers.HomeSeerDevices
{
	public interface IGetVersionIntFromVersionString
	{
		int GetVersionFromString(string versionString);
		//int GetVersionFromAssembly();
		string GetVersionStringFromAssembly();
	}
	public class GetVersionIntFromVersionString : IGetVersionIntFromVersionString
	{
		//public int GetVersionFromAssembly()
		//{
		//	var versionString = GetAssemblyVersion();
		//	return GetVersionFromString(versionString);
		//}

		public string GetVersionStringFromAssembly()
		{
			return GetAssemblyVersion();
		}

		public int GetVersionFromString(string versionString)
		{
			if (string.IsNullOrEmpty(versionString)) return 0;
			int versionNumber = 0;
			var versionRegExp = new Regex(@"\d{1,4}\.\d{1,4}\.\d{1,4}\.\d{1,4}");
			if (versionRegExp.IsMatch(versionString))
			{
				var splitResult = versionString.Split('.');
				versionNumber = (int.Parse(splitResult[0]) * 1000000) + (int.Parse(splitResult[1]) * 10000) +
				                (int.Parse(splitResult[2]) * 100) + (int.Parse(splitResult[3]));
			}
			return versionNumber;
		}

		private string GetAssemblyVersion()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			string version = fileVersionInfo.ProductVersion;
			return version;
		}
	}
}