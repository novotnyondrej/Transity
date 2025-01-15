using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transity.UI;
using Transity.Data.Games;
using Transity.General.Exceptions;

namespace Transity.Pages.Games
{
	/// <summary>
	/// Interaction logic for MainPanelPage.xaml
	/// </summary>
	public partial class MainPanelPage : GamePagePage
	{
		//Jiz existujici instance
		private static readonly Dictionary<GamePage, MainPanelPage> Instances = [];
		
		
		public MainPanelPage(GamePage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentPage] = this;
			
			InitializeComponent();
			parentPage.OnGameChanged += OnGameChanged;
			parentPage.OnSelectedCityChanged += OnSelectedCityChanged;
		}
		//Ziska instanci stranky
		public static MainPanelPage GetInstance(GamePage parentPage)
		{
			if (Instances.ContainsKey(parentPage))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				MainPanelPage instance = Instances[parentPage];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentPage);
		}
		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Odebrani predlohy
			if (mapElement.Children.Contains(cityTemplate)) mapElement.Children.Remove(cityTemplate);
			//Nacteni prekladu
			Preload();
		}
		private void OnGameChanged(Game game)
		{
			//Ziskani maximalnich rozmeru mapy
			int width = (int)game.Information.MapWidth;
			int height = (int)game.Information.MapHeight;
			//Ziskani extremni hodnoty
			int size = (width > height ? width : height);
			//Nasobek odstupu mest
			double multiplier = size / 32;
			//Smazani elementu na mape
			mapElement.Children.Clear();
			//Ziskani mest
			IEnumerable<City> cities = game.Cities;
			//Vytvoreni mest a umisteni na mapu
			foreach (City city in cities)
			{
				//Souradnice
				int x = city.Location.X;
				int y = city.Location.Y;
				//Klic
				string cityKey = "city_" + city.Index;
				//Mesto
				Button cityElement = cityTemplate.Clone(cityKey);
				//Tag
				cityElement.Tag = city.Index;
				//Umisteni mesta na spravnou lokaci
				cityElement.Margin = new Thickness(x * multiplier, y * multiplier, 0, 0);
				//
				cityElement.Click += OnCityElementClicked;
				//Pridani mesta na mapu
				mapElement.Children.Add(cityElement);
			}
		}
		//Aktualizuje barvu mesta
		private void UpdateCityElementColor(City city)
		{
			string cityKey = "city_" + city.Index;
			Button? cityElement = mapElement.FindChildByName<Button>(cityKey);

			if (cityElement is null) return;
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
		private void OnCityElementClicked(object sender, RoutedEventArgs e)
		{
			//Prevod na tlacitko
			Button button = (Button)sender;
			//Ziskani indexu
			int cityIndex = (int)button.Tag;
			//Zmena oznaceneho mesta
			ParentWindow.ChangeSelectedCityIndex(cityIndex);
		}
		//Reaguje na zmenu oznaceneho mesta
		private void OnSelectedCityChanged(City? previousCity, City? currentCity)
		{
			if (previousCity is not null) UpdateCityElementColor(previousCity);
			if (currentCity is not null) UpdateCityElementColor(currentCity);
		}
	}
}
