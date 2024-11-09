using Newtonsoft.Json;
using System.Windows;
using Transity.General.Exceptions;

namespace Transity.External
{
	//Trida pro prevadeni objektu na json a naopak
	internal static class JsonConverter
	{
		//Prevede na json nebo vyhodi error, pokud se prevod nezdaril
		public static string ConvertToJson<OfType>(OfType obj)
		{
			try
			{
				return JsonConvert.SerializeObject(obj);
			}
			catch (Exception exception)
			{
				throw new TranslatableException(
					new(
						"json-serialization-error",
						"exceptions",
						new()
						{
							{ "original-message", exception.Message }
						}
					)
				);
			}
		}
		//Prevede z jsonu na objekt nebo vyhodi error, pokud se prevod nezdaril
		public static OfType? ConvertFromJson<OfType>(string json)
		{
			try
			{
				return JsonConvert.DeserializeObject<OfType>(json);
			}
			catch (Exception exception)
			{
				throw new TranslatableException(
					new(
						"json-deserialization-error",
						"exceptions",
						new()
						{
							{ "original-message", exception.Message }
						}
					)	
				);
			}
		}
	}
}