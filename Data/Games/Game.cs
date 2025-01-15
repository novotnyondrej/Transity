using System.Windows;
using System.Windows.Media.Media3D;
using Transity.Data.Games.Players;
using Transity.External;

namespace Transity.Data.Games
{
	//Trida pro hru
	internal class Game
	{
		//Nazev souboru se stavy mest
		public static readonly string CityStatusesSaveFileName = "cities";
		public static readonly string LinesSaveFileName = "lines";

		//Informace o hre
		public readonly GameInformation Information;
		//Informace o hracovi
		public readonly Player Player;
		//Mesta
		public readonly IEnumerable<City> Cities;
		//Stavy mest
		public IEnumerable<CityStatus> CityStatuses { get => Cities.Select((city) => city.Status); }
		//Linky
		private readonly List<Line> _Lines;
		public IEnumerable<Line> Lines => _Lines;

		//Event na pridani nove linky
		internal delegate void OnLineAddedDelegate(Line line);
		internal event OnLineAddedDelegate OnLineAdded = delegate { };
		//Event na odebrani linky
		internal delegate void OnLineRemovedDelegate(Line line);
		internal event OnLineRemovedDelegate OnLineRemoved = delegate { };

		//Konstruktor
		public Game(
			GameInformation information,
			Player? player = null,
			IEnumerable<CityStatus>? cityStatuses = null,
			IEnumerable<Line>? lines = null
		)
		{
			Information = information;
			Player = player ?? new();
			//Generace mest
			Cities = City.GenerateCities(this, cityStatuses);
			_Lines = lines?.ToList() ?? [];
		}
		//Prida novou linku
		public void AddLine(int cityIndexFrom, int cityIndexTo)
		{
			int citiesCount = Cities.Count();
			if (cityIndexFrom < 0 || citiesCount <= cityIndexFrom) return;
			if (cityIndexTo < 0 || citiesCount <= cityIndexTo) return;
			if (cityIndexFrom == cityIndexTo) return;
			if (_Lines.Any(
				(line) => (
					(line.FromIndex == cityIndexFrom && line.ToIndex == cityIndexTo)
					|| (line.FromIndex == cityIndexTo && line.ToIndex == cityIndexFrom))
				)
			) return;

			City cityA = Cities.ElementAt(cityIndexFrom);
			City cityB = Cities.ElementAt(cityIndexTo);

			if (!cityA.Status.Bought || !cityB.Status.Bought) return;

			Line line = new(cityIndexFrom, cityIndexTo);
			_Lines.Add(line);
			OnLineAdded.Invoke(line);
		}
		//Odebere linku
		public void RemoveLine(Line line)
		{
			_Lines.Remove(line);
			OnLineRemoved(line);
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
			//Ulozeni linek
			AppDataManager.SaveData(GamesManager.GamesLocation + Information.CodeName, LinesSaveFileName, Lines);
		}
	}
}