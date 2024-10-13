using Newtonsoft.Json;

namespace Transity.External
{
	//Trida pro prevadeni objektu na json a naopak
	internal static class JsonConverter
	{
		//Prevede na json
		public static string ConvertToJson<OfType>(OfType obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
		//Prevede z jsonu na objekt
		public static OfType? ConvertFromJson<OfType>(string json)
		{
			return JsonConvert.DeserializeObject<OfType>(json);
		}
	}
}