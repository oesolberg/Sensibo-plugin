using System;
using System.Reflection;
using Hspi;
using HSPI_SensiboClimate.Plugin.Helpers;

namespace HSPI_SensiboClimate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
	        Console.WriteLine($"Start {DateTime.Now.ToString(PluginConstants.IsoFormatting)} of {GetAssemblyNameAndVersion()}");
			Connector.Connect<HSPI>(args);
        }

        private static string GetAssemblyNameAndVersion()
        {
	        Assembly assembly = Assembly.GetExecutingAssembly();
	        AssemblyName assemblyName = assembly.GetName();
	        var ver = assemblyName.Version;
	        return $"{assemblyName.Name}, version {ver.ToString()}";
        }
	}
}