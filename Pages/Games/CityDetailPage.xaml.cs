using System;
using System.Collections.Generic;
using System.Data;
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
using Transity.Content;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	/// <summary>
	/// Interaction logic for CityDetailPage.xaml
	/// </summary>
	public partial class CityDetailPage : SidePanelPageChild
	{
		//Jiz existujici instance
		private static readonly Dictionary<SidePanelPage, CityDetailPage> Instances = [];
		//Mesto, pro ktere mame aktualne nacteny detail
		private City? CurrentCity;


		public CityDetailPage(SidePanelPage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentPage] = this;

			InitializeComponent();
			parentPage.ParentWindow.OnGameChanged += OnGameChanged;
		}
		//Ziska instanci stranky
		public static CityDetailPage GetInstance(SidePanelPage parentPage)
		{
			if (Instances.ContainsKey(parentPage))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				CityDetailPage instance = Instances[parentPage];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentPage);
		}
		//Nacte detail mesta
		internal void LoadCityDetail(City? city)
		{
			if (CurrentCity is not null) CurrentCity.Status.OnStatusChanged -= OnCurrentCityStatusChanged;

			CurrentCity = city;
			if (CurrentCity is not null) CurrentCity.Status.OnStatusChanged += OnCurrentCityStatusChanged;

			cityNameLabel.Content = city?.Index ?? 0;
			buyCityButton.Content = Translator.LoadTranslation(new("buy-city", "ui", new() { { "city-price", City.NewCityPrice } }));
			UpdateBuyButton();
		}
		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
		private void OnGameChanged(Game? previousGame, Game? currentGame)
		{
			//Odebrani eventu na zmenu penez z predchozi hry
			if (previousGame is not null) previousGame.Player.OnMoneyChanged -= OnPlayerBalanceChanged;
			//Pridani eventu na zmenu penez
			if (currentGame is not null) currentGame.Player.OnMoneyChanged += OnPlayerBalanceChanged;

			OnPlayerBalanceChanged(0, currentGame?.Player.Money ?? 0);
			UpdateBuyButton();
		}
		//Aktualizuje klikatelnost tlacitka
		private void UpdateBuyButton()
		{
			Game? currentGame = ParentWindow.ParentWindow.CurrentGame;

			buyCityButton.IsEnabled = (currentGame is not null && currentGame.Player.Money >= City.NewCityPrice);
			buyCityButton.Visibility = (CurrentCity is not null && !CurrentCity.Status.Bought) ? Visibility.Visible : Visibility.Hidden;
		}
		private void OnPlayerBalanceChanged(int previousBalance, int currentBalance)
		{
			UpdateBuyButton();
		}
		//Reakce na zmenu statusu mesta
		private void OnCurrentCityStatusChanged(City city)
		{
			UpdateBuyButton();
		}
		//Kliknuti na tlacitko koupit mesto
		public void OnBuyButtonClicked(object sender, RoutedEventArgs e)
		{
			if (CurrentCity is not null)
			{
				CurrentCity.Buy();
			}
		}
	}
}
