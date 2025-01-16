namespace Transity.Data.Games
{
	//Mesto
	internal class City
	{
		//Hra, kteremu mesto patri
		private Game ParentGame;
		//Index mesta
		public readonly int Index;
		//Souradnice mesta
		public readonly Coordinate Location;
		//Status mesta
		public readonly CityStatus Status;

		//Minimalni rozestup mest
		private static readonly int MinimumCityDistance = 32;
		private static readonly int MaximumCityDistance = 48;
		//Detailnost umisteni mest na mape
		private static readonly int PlacementDetail = 8;
		//Cena koupeni noveho mesta
		public static readonly int NewCityPrice = 1000;

		private City(
			Game parentGame,
			int index,
			Coordinate location,
			CityStatus? status = null
		)
		{
			ParentGame = parentGame;
			Index = index;
			Location = location;
			Status = status ?? new();
		}
		//Hrac chce koupit mesto
		public bool Buy()
		{
			//Pokud je mesto jiz koupeno, nema duvod za nej znovu platit
			if (Status.Bought) return false;
			//Pokus o odebrani penez
			if (!ParentGame.Player.ChangeMoney(-NewCityPrice)) return false;
			//Zmena statusu
			Status.Buy(this);
			return true;
		}
		//Vygeneruje mesta
		public static IEnumerable<City> GenerateCities(Game game, IEnumerable<CityStatus>? cityStatuses = null)
		{
			//Nacteni informaci o mape
			GameInformation information = game.Information;
			int width = (int)information.MapWidth;
			int height = (int)information.MapHeight;
			int halfWidth = width / 2;
			int halfHeight = height / 2;
			//Random sekvence
			Random random = new Random(information.Seed);
			//Generace vsech moznych souradnic
			List<Coordinate> allCoordinates = [];
			for (int x = -halfWidth; x < halfWidth; x += PlacementDetail)
			{
				for (int y = -halfHeight; y < halfHeight; y += PlacementDetail)
				{
					allCoordinates.Add(new(x, y));
				}
			}
			//Kontrola, jestli vubec mame nejake souradnice
			if (allCoordinates.Count <= 0) return [];
			//Seznam souradnic, na kterych vzniknou mesta
			List<Coordinate> cityCoordinates = [];
			//Souradnice, ktere je mozne vybrat
			IEnumerable<Coordinate> availableCoordinates = allCoordinates;
			//Ziskavani souradnic
			do
			{
				//Ziskani nahodne dostupne souradnice
				cityCoordinates.Add(availableCoordinates.ElementAt(random.Next(0, availableCoordinates.Count())));
				//Aktualizace seznamu dostupnych souradnic
				availableCoordinates = allCoordinates.Where(
					(coordinate) => cityCoordinates.All(
						(cityCoordinate) => coordinate.GetDistanceTo(cityCoordinate) >= MinimumCityDistance
					) && cityCoordinates.Any(
						(cityCoordinate) => coordinate.GetDistanceTo(cityCoordinate) <= MaximumCityDistance
					)
				);
			}
			while (availableCoordinates.Any());
			cityStatuses ??= [];
			//Vytvoreni mest
			List<City> cities = [];
			for (int cityNum = 0; cityNum < cityCoordinates.Count(); cityNum++)
			{
				cities.Add(new(game, cityNum, cityCoordinates.ElementAt(cityNum), cityStatuses.ElementAtOrDefault(cityNum)));
			}
			return cities;
		}
	}
}