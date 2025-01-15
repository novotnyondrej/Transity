using System.Windows;
using Transity.UI;
using Transity.General.Exceptions;
using Transity.Data.Games;
using Transity.Pages.Games;

namespace Transity.Pages
{
	/// <summary>
	/// Interaction logic for MainMenuPage.xaml
	/// </summary>
	public partial class MainMenuPage : MainWindowPage
	{
		//Jiz existujici instance
		private static Dictionary<MainWindow, MainMenuPage> Instances = new();

		//Konstruktor
		public MainMenuPage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentWindow] = this;
			//Inicializace
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static MainMenuPage GetInstance(MainWindow parentWindow)
		{
			if (Instances.ContainsKey(parentWindow))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				MainMenuPage instance = Instances[parentWindow];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
		}
		//Preloader
		public override void Preload()
		{
			base.Preload();
			UpdateLoadGameButton();
			UpdateContinueGameButton();
		}
		//Aktualizuje klikatelnost na tlacitko nacist hru
		private void UpdateLoadGameButton()
		{
			loadGameButton.IsEnabled = GamesManager.AvailableGames.Any();
		}
		//Aktualizuje klikatelnost na tlacitko pokracovat ve hre
		private void UpdateContinueGameButton()
		{
			continueGameButton.IsEnabled = GamesManager.AvailableGames.Any((pair) => pair.Value.LastPlayedOn is not null);
		}
		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
		//Uzivatel kliknul na tlacitko pokracovat ve hre
		public void OnContinueButtonClicked(object sender, RoutedEventArgs e)
		{
			//Nalezeni posledni hrane hry
			string gameKey = GamesManager.AvailableGames.Where(
				(pair) => pair.Value.LastPlayedOn is not null
			).MaxBy(
				(pair) => pair.Value.LastPlayedOn
			).Key;
			//Nacteni hry
			Game game = GamesManager.LoadGame(gameKey);
			//Nacteni rozhrani pro hru
			GamePage.GetInstance(ParentWindow).LoadGame(game);
		}
		//Uzivatel kliknul na tlacitko nacist hru
		public void OnLoadGameButtonClicked(object sender, RoutedEventArgs e)
		{
			//Zmena stranky na nacist hru
			ParentWindow.ChangePage(LoadGamePage.GetInstance(ParentWindow));
		}
		//Uzivatel kliknul na tlacitko nova hra
		public void OnNewGameButtonClicked(object sender, RoutedEventArgs e)
		{
			//Zmena stranky na novou hru
			ParentWindow.ChangePage(NewGamePage.GetInstance(ParentWindow));
		}
		//Uzivatel kliknul na tlacitko nastaveni
		public void OnSettingsButtonClicked(object sender, RoutedEventArgs e)
		{
			//Zmena stranky na nastaveni
			ParentWindow.ChangePage(SettingsPage.GetInstance(ParentWindow));
		}
		//Uzivatel kliknul na tlacitko ukoncit
		public void OnExitButtonClicked(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
