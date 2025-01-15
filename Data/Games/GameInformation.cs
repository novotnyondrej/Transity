using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json;
using Transity.External;
using Transity.General;
using Transity.General.Exceptions;

namespace Transity.Data.Games
{
	//Informace o hre, nazev, seed, velikost mapy
	internal class GameInformation
	{
		//Nazev souboru s nastavenim
		public static readonly string SaveFileName = "information";

		//Nazev hry
		[JsonProperty("name")]
		public readonly string Name;
		//Nazev hry pri ukladani
		[JsonProperty("code-name")]
		public readonly string CodeName;
		//Seed pro danou hru
		[JsonProperty("seed")]
		public readonly int Seed;
		//Velikost mapy
		[JsonProperty("map-width")]
		public readonly MapSize MapWidth;
		[JsonProperty("map-height")]
		public readonly MapSize MapHeight;
		[JsonProperty("created-on")]
		public readonly int CreatedOn;
		[JsonProperty("last-played-on")]
		public int LastPlayedOn { get; private set; }

		[JsonConstructor]
		public GameInformation(
			string name,
			string codeName,
			int seed,
			MapSize mapWidth,
			MapSize mapHeight
		)
		{
			Name = name;
			CodeName = codeName;
			Seed = seed;
			MapWidth = mapWidth;
			MapHeight = mapHeight;
			CreatedOn = LastPlayedOn = TimeConverter.GetTime();
		}
		public GameInformation(
			string name,
			string? seed,
			MapSize mapWidth,
			MapSize mapHeight
		)
		{
			Name = name;
			CodeName = GetAvailableCodeName(name);
			Seed = TranslateSeed(seed);
			MapWidth = mapWidth;
			MapHeight = mapHeight;
			CreatedOn = LastPlayedOn = TimeConverter.GetTime();
		}
		//Prevede seed ze stringu na integer
		private int TranslateSeed(string? seed)
		{
			if (seed is not null) seed = seed.Trim();
			//Pokud nebyl seed uveden, vratime nahodny seed
			if (seed is null || seed.Length <= 0) return new Random().Next();
			//Pokud je seed numericky, provedeme pouze prevod na integer
			if (Int32.TryParse(seed, out int n)) return n;
			//Hash
			return seed.GetHashCode();
		}
		//Nalezne vhodne kodove jmeno pro hru
		private string GetAvailableCodeName(string name)
		{
			string codeName = name.Trim();
			//Nahrazeni pripadneho podtrzitka pomlckou
			codeName = codeName.Replace('_', '-');
			//SnakeCase to kebab-case
			codeName = Regex.Replace(codeName, @"([a-z])([A-Z]+)", "$1-$2").ToLower();
			//Filtr klice
			codeName = Regex.Match(codeName, @"[a-z0-9-]{1,64}").Value;
			//Odebrani opakujicich se polmcek
			codeName = Regex.Replace(codeName, @"(-+)", "-");

			//Kontrola dostupnosti jmena
			int index = 0;
			bool available;
			string proposedCodeName;
			do
			{
				proposedCodeName = index == 0 ? codeName : codeName + "-" + index;
				available = !GamesManager.GameExists(proposedCodeName);
				index++;
			}
			while (!available && index < 100);
			//Kontrola uspesnosti
			if (!available) throw new DetailedTranslatableException(new("game-name-not-available", "exceptions", new() { { "game-name", name } }));
			//Vysledny kodovy nazev
			return proposedCodeName;
		}
		//Ulozi nastaveni
		public void Save()
		{
			//Aktualizace posledniho hrani hry
			LastPlayedOn = TimeConverter.GetTime();
			//Ulozeni dat
			AppDataManager.SaveData(GamesManager.GamesLocation + CodeName, SaveFileName, this);
		}
	}
}