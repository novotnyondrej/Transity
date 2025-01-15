using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xaml.Schema;
using System.Windows;

namespace Transity.Data.Games
{
	internal class City
	{
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


		private City(
			int index,
			Coordinate location,
			CityStatus? status = null
		)
		{
			Index = index;
			Location = location;
			Status = status ?? new();
		}
		//Vygeneruje mesta
		public static IEnumerable<City> GenerateCities(GameInformation information, IEnumerable<CityStatus>? cityStatuses = null)
		{
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
			//Zvoleni nahodne pocatecni souradnice
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
				cities.Add(new(cityNum, cityCoordinates.ElementAt(cityNum), cityStatuses.ElementAtOrDefault(cityNum)));
			}
			return cities;
		}
	}
}