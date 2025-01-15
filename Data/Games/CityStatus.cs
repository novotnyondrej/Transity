using Newtonsoft.Json;

namespace Transity.Data.Games
{
	internal class CityStatus
	{
		//Zda je hracem koupeno
		[JsonProperty("bought")]
		public bool Bought { get; private set; }

		//Event na zmenu statusu
		internal delegate void OnStatusChangedDelegate(City city);
		internal event OnStatusChangedDelegate OnStatusChanged = delegate { };

		//Konstruktor
		[JsonConstructor]
		public CityStatus(bool bought = false)
		{
			Bought = bought;
		}
		//Zmeni status na koupeno
		public void Buy(City city)
		{
			Bought = true;
			OnStatusChanged.Invoke(city);
		}
	}
}