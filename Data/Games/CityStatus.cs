using Newtonsoft.Json;

namespace Transity.Data.Games
{
	internal class CityStatus
	{
		//Zda je hracem koupeno
		[JsonProperty("bought")]
		public bool Bought { get; private set; }

		//Konstruktor
		[JsonConstructor]
		public CityStatus(bool bought = false)
		{
			Bought = bought;
		}
	}
}