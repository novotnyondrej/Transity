using System.Windows;
using Transity.Content;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	//Stranka s detaily o meste
	public partial class CityDetailPage : SidePanelPageChild
	{
		//Instance teto stranky
		private static readonly Dictionary<SidePanelPage, CityDetailPage> Instances = [];
		//Mesto, pro ktere mame aktualne nacteny detail
		private City? CurrentCity;


		public CityDetailPage(SidePanelPage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentPage] = this;
			InitializeComponent();
			
			//Eventy
			parentPage.ParentWindow.OnGameChanged += OnGameChanged;
		}
		//Ziska instanci stranky
		public static CityDetailPage GetInstance(SidePanelPage parentPage)
		{
			if (Instances.TryGetValue(parentPage, out CityDetailPage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				CityDetailPage instance = value;
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentPage);
		}
		
		
		
		//Nacte detail mesta
		internal void LoadCityDetail(City? city)
		{
			if (CurrentCity == city) return;
			//Odebrani event listeneru na zmenu statusu z predchoziho mesta
			if (CurrentCity is not null) CurrentCity.Status.OnStatusChanged -= OnCurrentCityStatusChanged;
			//Prenastaveni mesta
			CurrentCity = city;
			//Pridani eventu listeneru na zmenu statusu mesta
			if (CurrentCity is not null) CurrentCity.Status.OnStatusChanged += OnCurrentCityStatusChanged;

			//Preklady
			cityNameLabel.Content = Translator.LoadTranslation(new("city", UITranslator.TranslationSetName, new() { { "city-index", city?.Index ?? 0 } }));
			buyCityButton.Content = Translator.LoadTranslation(new("buy-city", UITranslator.TranslationSetName, new() { { "city-price", City.NewCityPrice } }));
			UpdateBuyButton();
		}
		
		
		//Aktualizuje klikatelnost a viditelnost tlacitka koupit mesto
		private void UpdateBuyButton()
		{
			Game? currentGame = ParentWindow.ParentWindow.CurrentGame;

			buyCityButton.IsEnabled = (currentGame is not null && currentGame.Player.Money >= City.NewCityPrice);
			buyCityButton.Visibility = (CurrentCity is not null && !CurrentCity.Status.Bought) ? Visibility.Visible : Visibility.Hidden;
		}


		//Reakce na zmenu hry
		private void OnGameChanged(Game? previousGame, Game? currentGame)
		{
			//Odebrani eventu na zmenu penez z predchozi hry
			if (previousGame is not null) previousGame.Player.OnMoneyChanged -= OnPlayerBalanceChanged;
			//Pridani eventu na zmenu penez
			if (currentGame is not null) currentGame.Player.OnMoneyChanged += OnPlayerBalanceChanged;

			OnPlayerBalanceChanged(0, currentGame?.Player.Money ?? 0);
			UpdateBuyButton();
		}
		//Reakce na zmenu poctu penez, ktere hrac vlastni
		private void OnPlayerBalanceChanged(int previousBalance, int currentBalance)
		{
			UpdateBuyButton();
		}
		//Reakce na zmenu statusu mesta
		private void OnCurrentCityStatusChanged(City city)
		{
			UpdateBuyButton();
		}


		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
		//Uzivatel kliknul na tlacitko koupit mesto
		public void OnBuyButtonClicked(object sender, RoutedEventArgs e)
		{
			CurrentCity?.Buy();
		}
	}
}