using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using HomeSeerAPI;

namespace HSPI_SensiboClimate.Plugin.Helpers
{

	public interface IHsCollectionFactory
	{
		Classes.action GetActionsIfPossible(IPlugInAPI.strTrigActInfo trigActInfo);
	}

	public class HsCollectionFactory : IHsCollectionFactory
	{

		public HsCollectionFactory()
		{

		}

		public Classes.action GetActionsIfPossible(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			object objAction = new Classes.action();
			if (trigActInfo.DataIn != null && trigActInfo.DataIn.Length > 0)
			{
				DeSerializeObject(ref trigActInfo.DataIn,
					ref objAction);
				Classes.action formattedAction = (Classes.action)objAction;
				if (formattedAction != null)
				{
					return formattedAction;
				}
			}
			return (Classes.action)objAction;
		}


		public static bool DeSerializeObject(ref byte[] bteIn, ref object objOut, IHSApplication hs = null)
		{
			bool result;
			if (bteIn == null)
			{
				result = false;
			}
			else if (bteIn.Length < 1)
			{
				result = false;
			}
			else if (objOut == null)
			{
				result = false;
			}
			else
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				try
				{
					objOut.GetType();
					objOut = null;
					MemoryStream serializationStream = new MemoryStream(bteIn);
					object objectValue = RuntimeHelpers.GetObjectValue(binaryFormatter.Deserialize(serializationStream));
					if (objectValue == null)
					{
						result = false;
					}
					else
					{
						objectValue.GetType();
						objOut = RuntimeHelpers.GetObjectValue(objectValue);
						if (objOut == null)
						{
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
				catch (InvalidCastException invalidCastException)
				{
					//ProjectData.SetProjectError(expr_6C);
					result = false;
					Console.WriteLine(invalidCastException.Message);
					//ProjectData.ClearProjectError();
				}
				catch (Exception exception)
				{
					//ProjectData.SetProjectError(expr_7D);
					//Exception ex = expr_7D;
					//Utils._log.Error(ex,  + " Error: DeSerializing object: " + ex.Message, new object[0]);
					result = false;
					Console.WriteLine(exception.Message);
					if (hs != null)
						hs.WriteLog(PluginConstants.PluginName, "General exception error when casting in deserializer: " + exception.Message + Environment.NewLine + exception.StackTrace);
					//ProjectData.ClearProjectError();
				}
			}
			return result;
		}

		public static bool SerializeObject(ref object objIn, ref byte[] bteOut, IHSApplication hs = null)
		{
			bool result;
			if (objIn == null)
			{
				result = false;
			}
			else
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				try
				{
					binaryFormatter.Serialize(memoryStream, RuntimeHelpers.GetObjectValue(objIn));
					bteOut = new byte[checked((int)(memoryStream.Length - 1L) + 1)];
					bteOut = memoryStream.ToArray();
					result = true;
				}
				catch (Exception exception)
				{
					//Hs.WriteLog(Utils.IfaceName, "Error when serializing object");
					//Utils._log.Error(ex, string.Concat(new string[]
					//{
					//    Utils.IfaceName,
					//    " Error: Serializing object ",
					//    objIn.ToString(),
					//    " :",
					//    ex.Message
					//}), new object[0]);
					result = false;
					if (hs != null)
						hs.WriteLog(PluginConstants.PluginName, "General exception error when casting in serializer: " + exception.Message + Environment.NewLine + exception.StackTrace);
				}
			}
			return result;
		}

	}
}