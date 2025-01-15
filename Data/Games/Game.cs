using Transity.Data.Games.Players;
using Transity.External;

namespace Transity.Data.Games
{
	//Trida pro hru
	internal class Game
	{
		//Nazev souboru se stavy mest
		public static readonly string CityStatusesSaveFileName = "cities";

		//Informace o hre
		public readonly GameInformation Information;
		//Informace o hracovi
		public readonly Player Player;
		//Mesta
		public readonly IEnumerable<City> Cities;
		//Stavy mest
		public IEnumerable<CityStatus> CityStatuses { get => Cities.Select((city) => city.Status); }

		//Konstruktor
		public Game(
			GameInformation information,
			Player? player = null,
			IEnumerable<CityStatus>? cityStatuses = null
		)
		{
			Information = information;
			Player = player ?? new();
			//Generace mest
			Cities = City.GenerateCities(this, cityStatuses);
		}
		//Ulozi hru
		public void Save(bool updatePlayTime = true)
		{
			//Ulozeni informaci o hre
			Information.Save(updatePlayTime);
			//Ulozeni informaci o hracovi
			Player.Save(Information);
			//Ulozeni stavu mest
			AppDataManager.SaveData(GamesManager.GamesLocation + Information.CodeName, CityStatusesSaveFileName, CityStatuses);
		}
	}
}