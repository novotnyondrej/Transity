using System.Windows;
using Transity.UI;
using Transity.General.Exceptions;
using Transity.General;
using Transity.Data.Games;
using System.Windows.Controls;

namespace Transity.Pages
{
	/// <summary>
	/// Interaction logic for NewGamePage.xaml
	/// </summary>
	public partial class NewGamePage : MainWindowPage
	{
		//Jiz existujici instance
		private static Dictionary<MainWindow, NewGamePage> Instances = new();

		//Konstruktor
		public NewGamePage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentWindow] = this;
			//Inicializace
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static NewGamePage GetInstance(MainWindow parentWindow)
		{
			if (Instances.ContainsKey(parentWindow))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				NewGamePage instance = Instances[parentWindow];
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
			//Nacteni vychozich hodnot
			gameNameTextbox.Text = "";
			gameSeedTextbox.Text = "";
			mapWidthComboBox.SelectedIndex = 1;
			mapHeightComboBox.SelectedIndex = 1;
			createGameButton.IsEnabled = false;
		}
		//Zmena textu v policku pro nazev hry
		public void OnGameNameChanged(object sender, RoutedEventArgs e)
		{
			//Tlacitko vytvorit hru je mozne zakliknout pouze pokud je vyplnen nazev hry
			createGameButton.IsEnabled = gameNameTextbox.Text.Trim().Length >= 3;
		}
		//Stranka nactena
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
		//Uzivatel klikl na tlacitko zrusit
		public void OnCancelButtonClicked(object sender, RoutedEventArgs e)
		{
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
		//Vytvori novou hru
		private Game CreateNewGame()
		{
			//Ziskani zvolene velikosti mapy
			string widthName = ((ComboBoxItem)mapWidthComboBox.SelectedItem).Tag.ToString() ?? "";
			string heightName = ((ComboBoxItem)mapHeightComboBox.SelectedItem).Tag.ToString() ?? "";
			//Prevod na enum
			if (!MapSize.TryParse(widthName, out MapSize width)) throw new DetailedTranslatableException(new("invalid-map-width", "exceptions", new() { { "map-width", widthName } }));
			if (!MapSize.TryParse(heightName, out MapSize height)) throw new DetailedTranslatableException(new("invalid-map-height", "exceptions", new() { { "map-height", heightName } }));
			//Vytvoreni nove hry
			return new Game(
				new(
					gameNameTextbox.Text,
					gameSeedTextbox.Text,
					width,
					height
				)
			);
		}
		//Uzivatel klikl na tlacitko vytvorit hru
		public void OnCreateGameButtonClicked(object sender, RoutedEventArgs e)
		{
			//Vytvoreni nove hry
			Game? newGame = SafeExecutor.Execute(CreateNewGame, null);
			//Kontrola vysledku
			if (newGame is null)
			{
				//Nepovedlo se zalozit novou hru, probehla zpetna vazba
				return;
			}
			//Ulozeni nove hry
			GamesManager.SaveGame(newGame);
			//Spusteni samotne hry
		}
	}
}