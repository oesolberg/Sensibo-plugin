using HSPI_SensiboClimate.LocalStorage.Models;
using HSPI_SensiboClimate.Plugin.API.JSON;
using HSPI_SensiboClimate.Plugin.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
    //public static class Storage
    //{
    //    public static string APIKey { get; set; }
    //    private static string Path
    //    {
    //        get
    //        {
    //            return System.IO.Path.Combine("Config", "SensiboClimateStates.xml");
    //            //return @"C:\Program Files (x86)\HomeSeer HS3\Config\SensiboClimateStates.xml";
    //        }
    //    }

    //    public static void XMLCreate()
    //    {
    //        try
    //        {
    //            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
    //            for (int i = 0; i < st.FrameCount; i++)
    //            {
    //                System.Diagnostics.StackFrame sf = st.GetFrame(i);
    //                _log.Log(string.Format("High up the call stack, Method: {0}",
    //                    sf.GetMethod()), LogType.Debug);

    //                Utils.Log(string.Format("High up the call stack, Line Number: {0}",
    //                    sf.GetFileLineNumber()), LogType.Debug);
    //            }

    //            XDocument xdoc = new XDocument(
    //                new XElement("ConfigPanel",
    //                new XElement("DevicesInfo"),
    //                new XElement("CurrentDevices"),
    //                new XElement("BlockedDevices"),
    //                new XElement("LostDevices")));

    //            Save(xdoc);
    //        }
    //        catch (Exception ex)
    //        {
    //            Utils.Log(ex.Message, Plugin.Enums.LogType.Error);
    //        }
    //    }

    //    public static void XMLUpdate(List<SensiboDeviceInfo> devicesInfo)
    //    {
    //        XDocument xdoc = GetXDocument();

    //        if (devicesInfo != null)
    //        {
    //            XElement configPanelNode = xdoc.Element("ConfigPanel").Element("DevicesInfo");
    //            XElement modesNode = configPanelNode.Element("Modes");

    //            XElement currentDevicesNode = xdoc.Element("ConfigPanel").Element("CurrentDevices");
    //            XElement lostDevicesNode = xdoc.Element("ConfigPanel").Element("LostDevices");
    //            XElement blockedDevicesNode = xdoc.Element("ConfigPanel").Element("BlockedDevices");

    //            #region DETECT AND WRITE LOST DEVICES

    //            var devicesIds = devicesInfo.Select(device => device.Id);
    //            var currentDevices = currentDevicesNode.Elements("Device").Select(xe => xe.Element("Id").Value);
    //            var lostDevices = currentDevices.Except(devicesIds).ToList();

    //            if (lostDevices.Any())
    //                lostDevices.ForEach(Id =>
    //                {
    //                    var isLost = lostDevicesNode.Elements("Device").Any(xe => xe.Element("Id").Value == Id);
    //                    if (!isLost)
    //                        lostDevicesNode.Add(
    //                            new XElement("Device",
    //                            new XElement("Id", Id)));
    //                });

    //            #endregion

    //            var areDevices = configPanelNode.Elements("Device").Any();
    //            devicesInfo.ForEach(info =>
    //            {
    //                XElement currentInfo = areDevices ?
    //                configPanelNode.Elements("Device").SingleOrDefault(xe => xe.Element("Id").Value == info.Id) :
    //                null;

    //                if (currentInfo != null)
    //                {
    //                    #region LOGIC FOR CURRENT DEVICES

    //                    currentInfo.Element("Status").Value = info.Status.ToString();
    //                    currentInfo.Element("Mac").Value = info.Mac;
    //                    currentInfo.Element("CurrentTemperature").Value = info.CurrentTemperature.ToString();
    //                    currentInfo.Element("CurrentHumidity").Value = info.CurrentHumidity.ToString();
    //                    currentInfo.Element("TemperatureUnit").Value = info.TemperatureUnit;
    //                    currentInfo.Element("On").Value = info.On.ToString();
    //                    currentInfo.Element("TargetTemperature").Value = info.TargetTemperature.ToString();
    //                    currentInfo.Element("Mode").Value = info.Mode;
    //                    currentInfo.Element("Location").Value = info.Room;
    //                    currentInfo.Element("SmartMode").Value = info.SmartMode.ToString();
    //                    currentInfo.Element("Scheduled").Value = info.Scheduled.ToString();
    //                    currentInfo.Element("Address").Value = info.Address;
    //                    currentInfo.Element("Ban").Value = info.Ban.ToString();
    //                    if (info.Swing != null)
    //                        currentInfo.Element("Swing").Value = info.Swing;
    //                    if (info.FanLevel != null)
    //                        currentInfo.Element("FanLevel").Value = info.FanLevel;

    //                    var isLost = lostDevicesNode.Elements("Device").Any(xe => xe.Element("Id").Value == info.Id);
    //                    if (isLost)
    //                        lostDevicesNode.Elements("Device").SingleOrDefault(xe => xe.Element("Id").Value == info.Id).Remove();

    //                    #endregion
    //                }
    //                else
    //                {
    //                    #region LOGIC FOR NEW DEVICES

    //                    configPanelNode.Add(
    //                        new XElement("Device",
    //                        new XElement("Id", info.Id),
    //                        new XElement("Status", info.Status),
    //                        new XElement("Mac", info.Mac),
    //                        new XElement("CurrentTemperature", info.CurrentTemperature),
    //                        new XElement("CurrentHumidity", info.CurrentHumidity),
    //                        new XElement("TemperatureUnit", info.TemperatureUnit),
    //                        new XElement("On", info.On),
    //                        new XElement("TargetTemperature", info.TargetTemperature),
    //                        new XElement("FanLevel", info.FanLevel),
    //                        new XElement("Mode", info.Mode),
    //                        new XElement("Location", info.Room),
    //                        new XElement("Swing", info.Swing),
    //                        new XElement("SmartMode", info.SmartMode),
    //                        new XElement("Scheduled", info.Scheduled),
    //                        new XElement("Address", info.Address),
    //                        new XElement("Ban", info.Ban),
    //                        new XElement("RemoteCapabilities", XMLAddAvaliableControls(configPanelNode, info))));


    //                    if (!currentDevicesNode.Elements("Device").Any(xe => xe.Element("Id").Value == info.Id))
    //                        currentDevicesNode.Add(
    //                            new XElement("Device",
    //                            new XElement("Id", info.Id)));

    //                    #endregion
    //                }
    //            });

    //            Save(xdoc);
    //        }
    //    }

    //    public static void XMLUpdate(SensiboDeviceInfo sensiboDeviceInfo)
    //    {
    //        XDocument xdoc = GetXDocument();
    //        if (xdoc == null)
    //            return;

    //        if (sensiboDeviceInfo != null)
    //        {
    //            XElement configPanelNode = xdoc.Element("ConfigPanel").Element("DevicesInfo");

    //            XElement currentInfo = configPanelNode.Elements("Device").SingleOrDefault(xe => xe.Element("Id").Value == sensiboDeviceInfo.Id);
    //            if (currentInfo != null)
    //            {
    //                currentInfo.Element("On").Value = sensiboDeviceInfo.On.ToString();
    //                currentInfo.Element("TargetTemperature").Value = sensiboDeviceInfo.TargetTemperature.ToString();
    //                currentInfo.Element("TemperatureUnit").Value = sensiboDeviceInfo.TemperatureUnit;
    //                currentInfo.Element("FanLevel").Value = sensiboDeviceInfo.FanLevel;
    //                currentInfo.Element("Mode").Value = sensiboDeviceInfo.Mode;
    //                if (sensiboDeviceInfo.Swing != null)
    //                    currentInfo.Element("Swing").Value = sensiboDeviceInfo.Swing;
    //            }

    //            Save(xdoc);
    //        }
    //    }

    //    public static List<SensiboDeviceInfo> XMLGetAll()
    //    {
    //        XDocument xdoc = GetXDocument();
    //        if (xdoc == null)
    //            return null;

    //        var devicesInfo = from xe in xdoc.Element("ConfigPanel").Element("DevicesInfo").Elements("Device")
    //                          select new SensiboDeviceInfo
    //                          {
    //                              Id = xe.Element("Id").Value,
    //                              Status = bool.Parse(xe.Element("Status").Value),
    //                              Mac = xe.Element("Mac").Value,
    //                              CurrentTemperature = double.Parse(xe.Element("CurrentTemperature").Value, CultureInfo.InvariantCulture),
    //                              CurrentHumidity = double.Parse(xe.Element("CurrentHumidity").Value, CultureInfo.InvariantCulture),
    //                              TemperatureUnit = xe.Element("TemperatureUnit").Value,
    //                              On = bool.Parse(xe.Element("On").Value),
    //                              TargetTemperature = double.Parse(xe.Element("TargetTemperature").Value, CultureInfo.InvariantCulture),
    //                              FanLevel = xe.Element("FanLevel").Value,
    //                              Mode = xe.Element("Mode").Value,
    //                              Swing = xe.Element("Swing").Value,
    //                              Room = xe.Element("Location").Value,
    //                              Address = xe.Element("Address").Value,
    //                              Ban = IsBanned(xe.Element("Id").Value),
    //                              SmartMode = bool.Parse(xe.Element("SmartMode").Value),
    //                              Scheduled = bool.Parse(xe.Element("Scheduled").Value)
    //                          };

    //        return devicesInfo.ToList();
    //    }

    //    public static void XMLRemoveDevice(string id)
    //    {
    //        XDocument xdoc = GetXDocument();

    //        var deviceInfo = from xe in xdoc.Element("ConfigPanel").Element("DevicesInfo").Elements("Device")
    //                         where xe.Element("Id").Value == id
    //                         select xe;
    //        deviceInfo.Remove();

    //        Save(xdoc);
    //    }

    //    public static SensiboDeviceInfo XMLGet(string id)
    //    {
    //        XDocument xdoc = GetXDocument();
    //        if (xdoc == null)
    //            return null;

    //        var deviceInfo = from xe in xdoc.Element("ConfigPanel").Element("DevicesInfo").Elements("Device")
    //                         where xe.Element("Id").Value == id
    //                         select new SensiboDeviceInfo
    //                         {
    //                             Id = xe.Element("Id").Value,
    //                             Status = bool.Parse(xe.Element("Status").Value),
    //                             Mac = xe.Element("Mac").Value,
    //                             CurrentTemperature = double.Parse(xe.Element("CurrentTemperature").Value, CultureInfo.InvariantCulture),
    //                             CurrentHumidity = double.Parse(xe.Element("CurrentHumidity").Value, CultureInfo.InvariantCulture),
    //                             TemperatureUnit = xe.Element("TemperatureUnit").Value,
    //                             On = bool.Parse(xe.Element("On").Value),
    //                             TargetTemperature = double.Parse(xe.Element("TargetTemperature").Value, CultureInfo.InvariantCulture),
    //                             FanLevel = xe.Element("FanLevel").Value,
    //                             Mode = xe.Element("Mode").Value,
    //                             Swing = xe.Element("Swing").Value,
    //                             Room = xe.Element("Location").Value,
    //                             Address = xe.Element("Address").Value,
    //                             Ban = bool.Parse(xe.Element("Ban").Value),
    //                             SmartMode = bool.Parse(xe.Element("SmartMode").Value),
    //                             Scheduled = bool.Parse(xe.Element("Scheduled").Value)
    //                         };

    //        return deviceInfo.Single();
    //    }

    //    public static void XMLAddBanId(string id)
    //    {
    //        XDocument xdoc = GetXDocument();
    //        XElement banNode = xdoc.Element("ConfigPanel").Element("BlockedDevices");

    //        banNode.Add(new XElement("Device", new XElement("Id", id)));

    //        Save(xdoc);
    //    }

    //    public static void XMLUnban(string id)
    //    {
    //        XDocument xdoc = GetXDocument();
    //        XElement unbanNode = xdoc.Element("ConfigPanel").Element("BlockedDevices");
    //        if (unbanNode.Elements("Device").Any(xe => xe.Element("Id").Value == id))
    //        {
    //            var unbanNodes = unbanNode.Elements("Device").Where(xe => xe.Element("Id").Value == id);
    //            unbanNodes.Remove();
    //            Save(xdoc);
    //        }
    //    }

    //    public static bool IsBanned(string id)
    //    {
    //        XDocument xdoc = GetXDocument();
    //        if (xdoc == null)
    //            return true;

    //        var xmlBlockedDevice = xdoc.Element("ConfigPanel").Element("BlockedDevices");
    //        if (xmlBlockedDevice.Elements().Any())
    //            return xmlBlockedDevice.Elements("Device").Where(el => el.Element("Id").Value == id).Any();

    //        return false;
    //    }

    //    public static List<string> XMLModes(string id)
    //    {
    //        XDocument xdoc = GetXDocument();
    //        if (xdoc == null)
    //            return null;

    //        var xmlDevice = xdoc.Element("ConfigPanel").Element("DevicesInfo").Elements("Device").Where(el => el.Element("Id").Value == id).SingleOrDefault();
    //        var modes = xmlDevice.Element("RemoteCapabilities").Elements("Modes").Elements().Select(mode => mode.Name.ToString()).ToList();

    //        return modes;
    //    }

    //    public static Mode XMLMode(string id, string modeName)
    //    {
    //        XDocument xdoc = GetXDocument();
    //        if (xdoc == null)
    //            return null;

    //        var xmlDevice = xdoc.Element("ConfigPanel").Element("DevicesInfo").Elements("Device").Where(el => el.Element("Id").Value == id).SingleOrDefault();
    //        var xmlMmodes = xmlDevice.Element("RemoteCapabilities").Elements("Modes").Elements();
    //        var xmlMode = xmlMmodes.Where(xMode => xMode.Name.ToString().Equals(modeName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

    //        var fanLevels = xmlMode.Element("FanLevels").Elements("FanLevel").Select(fan => fan.Value).ToList();
    //        var swings = xmlMode.Element("Swings").Elements("Swing").Select(swg => swg.Value).ToList();

    //        var mode = new Mode
    //        {
    //            FanLevels = fanLevels,
    //            Swing = swings
    //        };

    //        return mode;
    //    }

    //    private static XElement XMLAddAvaliableControls(XElement configPanelNode, SensiboDeviceInfo info)
    //    {
    //        var remoteCapabilitiesNode = new XElement("Modes");

    //        if (info.Modes.Auto != null)
    //        {
    //            remoteCapabilitiesNode.Add(new XElement("Auto", new XElement("Swings"), new XElement("FanLevels")));

    //            var modeNode = remoteCapabilitiesNode.Element("Auto");

    //            var swingsNode = remoteCapabilitiesNode.Element("Auto").Element("Swings");
    //            var fansNode = remoteCapabilitiesNode.Element("Auto").Element("FanLevels");

    //            if (info.Modes.Auto.Swing != null)
    //                info.Modes.Auto.Swing.ForEach(swing => swingsNode.Add(new XElement("Swing", swing)));
    //            if (info.Modes.Auto.FanLevels != null)
    //                info.Modes.Auto.FanLevels.ForEach(fan => fansNode.Add(new XElement("FanLevel", fan)));
    //        }

    //        if (info.Modes.Cool != null)
    //        {
    //            remoteCapabilitiesNode.Add(new XElement("Cool", new XElement("Swings"), new XElement("FanLevels")));

    //            var modeNode = remoteCapabilitiesNode.Element("Cool");

    //            var swingsNode = remoteCapabilitiesNode.Element("Cool").Element("Swings");
    //            var fansNode = remoteCapabilitiesNode.Element("Cool").Element("FanLevels");

    //            if (info.Modes.Cool.Swing != null)
    //                info.Modes.Cool.Swing.ForEach(swing => swingsNode.Add(new XElement("Swing", swing)));
    //            if (info.Modes.Cool.FanLevels != null)
    //                info.Modes.Cool.FanLevels.ForEach(fan => fansNode.Add(new XElement("FanLevel", fan)));
    //        }

    //        if (info.Modes.Dry != null)
    //        {
    //            remoteCapabilitiesNode.Add(new XElement("Dry", new XElement("Swings"), new XElement("FanLevels")));

    //            var modeNode = remoteCapabilitiesNode.Element("Dry");

    //            var swingsNode = remoteCapabilitiesNode.Element("Dry").Element("Swings");
    //            var fansNode = remoteCapabilitiesNode.Element("Dry").Element("FanLevels");

    //            if (info.Modes.Dry.Swing != null)
    //                info.Modes.Dry.Swing.ForEach(swing => swingsNode.Add(new XElement("Swing", swing)));
    //            if (info.Modes.Dry.FanLevels != null)
    //                info.Modes.Dry.FanLevels.ForEach(fan => fansNode.Add(new XElement("FanLevel", fan)));
    //        }

    //        if (info.Modes.Fan != null)
    //        {
    //            remoteCapabilitiesNode.Add(new XElement("Fan", new XElement("Swings"), new XElement("FanLevels")));

    //            var modeNode = remoteCapabilitiesNode.Element("Fan");

    //            var swingsNode = remoteCapabilitiesNode.Element("Fan").Element("Swings");
    //            var fansNode = remoteCapabilitiesNode.Element("Fan").Element("FanLevels");

    //            if (info.Modes.Fan.Swing != null)
    //                info.Modes.Fan.Swing.ForEach(swing => swingsNode.Add(new XElement("Swing", swing)));
    //            if (info.Modes.Fan.FanLevels != null)
    //                info.Modes.Fan.FanLevels.ForEach(fan => fansNode.Add(new XElement("FanLevel", fan)));
    //        }

    //        if (info.Modes.Heat != null)
    //        {
    //            remoteCapabilitiesNode.Add(new XElement("Heat", new XElement("Swings"), new XElement("FanLevels")));

    //            var modeNode = remoteCapabilitiesNode.Element("Heat");

    //            var swingsNode = remoteCapabilitiesNode.Element("Heat").Element("Swings");
    //            var fansNode = remoteCapabilitiesNode.Element("Heat").Element("FanLevels");

    //            if (info.Modes.Heat.Swing != null)
    //                info.Modes.Heat.Swing.ForEach(swing => swingsNode.Add(new XElement("Swing", swing)));
    //            if (info.Modes.Heat.FanLevels != null)
    //                info.Modes.Heat.FanLevels.ForEach(fan => fansNode.Add(new XElement("FanLevel", fan)));
    //        }

    //        if (info.Modes.Sleep != null)
    //        {
    //            remoteCapabilitiesNode.Add(new XElement("Sleep", new XElement("Swings"), new XElement("FanLevels")));

    //            var modeNode = remoteCapabilitiesNode.Element("Sleep");

    //            var swingsNode = remoteCapabilitiesNode.Element("Sleep").Element("Swings");
    //            var fansNode = remoteCapabilitiesNode.Element("Sleep").Element("FanLevels");

    //            if (info.Modes.Sleep.Swing != null)
    //                info.Modes.Sleep.Swing.ForEach(swing => swingsNode.Add(new XElement("Swing", swing)));
    //            if (info.Modes.Sleep.FanLevels != null)
    //                info.Modes.Sleep.FanLevels.ForEach(fan => fansNode.Add(new XElement("FanLevel", fan)));
    //        }

    //        return remoteCapabilitiesNode;
    //    }

    //    private static XDocument GetXDocument()
    //    {
    //        try
    //        {
    //            return XDocument.Load(Path);
    //        }
    //        catch
    //        {
    //            Utils.Log(string.Format("Re-created config:{0}", Path), LogType.Debug);
    //            XMLCreate();
    //            return XDocument.Load(Path);
    //        }
    //    }

    //    private static void Save(XDocument xDocumet)
    //    {
    //        xDocumet.Save(Path);
    //    }
    //}
}