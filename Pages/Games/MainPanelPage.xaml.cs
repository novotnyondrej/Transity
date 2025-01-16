using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Transity.UI;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Microsoft.VisualBasic;

namespace Transity.Pages.Games
{
	/// <summary>
	/// Interaction logic for MainPanelPage.xaml
	/// </summary>
	public partial class MainPanelPage : GamePageChild
	{
		//Instance teto stranky
		private static readonly Dictionary<GamePage, MainPanelPage> Instances = [];
		
		
		public MainPanelPage(GamePage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentPage] = this;
			InitializeComponent();
			
			//Eventy
			parentPage.OnGameChanged += OnGameChanged;
			parentPage.OnSelectedCityChanged += OnSelectedCityChanged;
			parentPage.OnSelectedLineChanged += OnSelectedLineChanged;
		}
		//Ziska instanci stranky
		public static MainPanelPage GetInstance(GamePage parentPage)
		{
			if (Instances.TryGetValue(parentPage, out MainPanelPage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				MainPanelPage instance = value;
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentPage);
		}
		
		
		//Ziska scale factor podle nastaveni
		private double GetScaleFactor()
		{
			GameInformation? information = ParentWindow.CurrentGame?.Information;
			if (information is null) return 1;
			//Ziskani maximalnich rozmeru mapy
			int width = (int)information.MapWidth;
			int height = (int)information.MapHeight;
			//Ziskani extremni hodnoty
			int size = (width > height ? width : height);
			//Nasobek odstupu mest
			double scaleFactor = 32 / (size / 64);
			return scaleFactor;
		}
		//Vytvori element pro mesto
		private Button CreateCity(City city)
		{
			double scaleFactor = GetScaleFactor();
			//Souradnice
			int x = (int)city.Location.X;
			int y = (int)city.Location.Y;
			//Nazev
			string cityKey = "city_" + city.Index;
			//Element reprezentujici mesto
			Button cityElement = cityTemplate.Clone(cityKey);
			//Identifikator
			cityElement.Tag = city.Index;
			//Umisteni mesta na spravnou lokaci
			cityElement.Margin = new Thickness(x * scaleFactor, y * scaleFactor, 0, 0);
			//Event listener na kliknuti
			cityElement.Click += OnCityElementClicked;
			//Pridani mesta na mapu
			mapElement.Children.Add(cityElement);

			UpdateCityElementColor(city);
			return cityElement;
		}
		//Vytvori element pro linku
		private Button? CreateLine(Line line)
		{
			double scaleFactor = GetScaleFactor();
			//Nazev
			string lineKey = "line_" + line.FromIndex + "_" + line.ToIndex;
			//Pokus o nalezeni jiz existujici instance nebo vytvoreni nove
			Button lineElement = mapElement.FindChildByName<Button>(lineKey) ?? lineTemplate.Clone(lineKey);

			//Nacteni hry
			Game? currentGame = ParentWindow.CurrentGame;
			if (currentGame is null) return null;
			
			//Nacteni mest
			City? fromCity = currentGame.Cities.ElementAtOrDefault(line.FromIndex);
			City? toCity = currentGame.Cities.ElementAtOrDefault(line.ToIndex);
			if (fromCity == default || toCity == default) return null;

			//Nacteni informaci o umisteni linky
			Coordinate fromLocation = fromCity.Location;
			Coordinate toLocation = toCity.Location;

			//Umisteni linky mezi mesta
			lineElement.Margin = new Thickness((fromLocation.X + toLocation.X) / 2d * scaleFactor, (fromLocation.Y + toLocation.Y) / 2d * scaleFactor, 0, 0);
			//Velikost linky na vzdalenost mezi mesty
			lineElement.Width = 40 + (fromLocation * scaleFactor).GetDistanceTo((toLocation * scaleFactor)) / 2;
			//Otoceni linky tak, aby se dotykala obou mest
			lineElement.RenderTransform = new RotateTransform((fromLocation - toLocation).Angle());
			//Identifikator
			lineElement.Tag = line.Id;
			//Event listener na kliknuti
			lineElement.Click += OnLineElementClicked;
			//Pridani elementu na mapu
			if (!mapElement.Children.Contains(lineElement)) mapElement.Children.Add(lineElement);
			
			UpdateLineElementColor(line);
			return lineElement;
		}
		//Reaguje na novou linku
		private void OnLineAdded(Line line)
		{
			CreateLine(line);
		}
		//Reaguje na smazanou linku
		private void OnLineRemoved(Line line)
		{
			//Pokus o nalezeni tlacitka
			string lineKey = "line_" + line.FromIndex + "_" + line.ToIndex;
			Button? lineElement = mapElement.FindChildByName<Button>(lineKey);

			//Smazani tlacitka
			if (lineElement is not null) mapElement.Children.Remove(lineElement);
		}


		//Aktualizuje barvu mesta
		private void UpdateCityElementColor(City city)
		{
			//Pokus o nalezeni tlacitka
			string cityKey = "city_" + city.Index;
			Button? cityElement = mapElement.FindChildByName<Button>(cityKey);

			if (cityElement is null) return;
			//Aktualizace stylu
			if (city.Status.Bought)
			{
				if (ParentWindow.SelectedCityIndex == city.Index) cityElement.Style = FindResource("SelectedOwnedCity") as Style;
				else cityElement.Style = FindResource("OwnedCity") as Style;
			}
			else
			{
				if (ParentWindow.SelectedCityIndex == city.Index) cityElement.Style = FindResource("SelectedCity") as Style;
				else cityElement.Style = FindResource("City") as Style;
			}
		}
		//Aktualizuje barvu mesta
		private void UpdateLineElementColor(Line line)
		{
			//Pokus o nalezeni tlacitka
			string lineKey = "line_" + line.FromIndex + "_" + line.ToIndex;
			Button? lineElement = mapElement.FindChildByName<Button>(lineKey);

			if (lineElement is null) return;
			//Aktualizace stylu
			if (ParentWindow.SelectedLine == line) lineElement.Style = FindResource("SelectedLine") as Style;
			else lineElement.Style = FindResource("Line") as Style;
		}


		//Reaguje na zmenu hry
		private void OnGameChanged(Game? previousGame, Game? currentGame)
		{
			if (previousGame is not null)
			{
				//Odebrani event listeneru z predchozi hry
				foreach (City city in previousGame.Cities)
				{
					city.Status.OnStatusChanged -= OnCityStatusChanged;
				}
				previousGame.OnLineAdded -= OnLineAdded;
				previousGame.OnLineRemoved -= OnLineRemoved;
			}
			//Smazani elementu na mape
			mapElement.Children.Clear();

			if (currentGame is null) return;
			//Nacteni nove mapy
			//Ziskani mest
			IEnumerable<City> cities = currentGame.Cities;
			IEnumerable<Line> lines = currentGame.Lines;

			//Umisteni mest na mapu
			foreach (City city in cities)
			{
				CreateCity(city);
				//Event listener na zmenu statusu
				city.Status.OnStatusChanged += OnCityStatusChanged;
			}
			//Umisteni linek na mapu
			foreach (Line line in lines)
			{
				CreateLine(line);
			}
			//Event listenery na pridani/odebrani linky
			currentGame.OnLineAdded += OnLineAdded;
			currentGame.OnLineRemoved += OnLineRemoved;
		}
		//Reaguje na zmenu statusu mesta
		private void OnCityStatusChanged(City city)
		{
			UpdateCityElementColor(city);
		}
		//Reaguje na zmenu oznaceneho mesta
		private void OnSelectedCityChanged(City? previousCity, City? currentCity)
		{
			if (previousCity is not null) UpdateCityElementColor(previousCity);
			if (currentCity is not null) UpdateCityElementColor(currentCity);
		}
		//Reaguje na zmenu oznacene linky
		private void OnSelectedLineChanged(Line? previousLine, Line? currentLine)
		{
			if (previousLine is not null) UpdateLineElementColor(previousLine);
			if (currentLine is not null) UpdateLineElementColor(currentLine);
		}


		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Odebrani predlohy
			mapElement.Children.Remove(cityTemplate);
			mapElement.Children.Remove(lineTemplate);
			//Nacteni prekladu
			Preload();
		}
		//Uzivatel kliknul na mesto
		private void OnCityElementClicked(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				//Ziskani indexu
				int cityIndex = (int)button.Tag;
				//Zmena oznaceneho mesta
				ParentWindow.ChangeSelectedCityIndex(cityIndex);
			}
		}
		//Uzivatel kliknul na linku
		private void OnLineElementClicked(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				//Ziskani id
				string lineId = (string)button.Tag;
				//Zmena oznaceneho mesta
				ParentWindow.ChangeSelectedLineIndex(lineId);
			}
		}
	}
}