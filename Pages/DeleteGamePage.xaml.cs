using System.Windows;
using System.Windows.Controls;
using Transity.Content;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.General;
using Transity.UI;

namespace Transity.Pages
{
	/// <summary>
	/// Interaction logic for DeleteGamePage.xaml
	/// </summary>
	public partial class DeleteGamePage : MainWindowPage
	{
		//Jiz existujici instance
		private static Dictionary<MainWindow, DeleteGamePage> Instances = new();
		//Kontext mazani hry
		private string? ContextGameKey = null;

		public DeleteGamePage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentWindow] = this;
			//Inicializace
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static DeleteGamePage GetInstance(MainWindow parentWindow)
		{
			if (Instances.ContainsKey(parentWindow))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				DeleteGamePage instance = Instances[parentWindow];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
		}
		//Stranka nactena
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
			OnContextChanged();
		}
		//Event pri zmene kontextu
		private void OnContextChanged()
		{
			deleteGameButton.IsEnabled = ContextGameKey is not null;
			
			if (ContextGameKey is null)
			{
				deleteGameDescriptionLabel.Content = Translator.LoadTranslation(new("no-game-to-delete", "ui"));
			}
			else
			{
				//Ziskani hry
				GameInformation information = GamesManager.AvailableGames[ContextGameKey];
				//Nacteni nazvu hry
				deleteGameDescriptionLabel.Content = Translator.LoadTranslation(new(
					"delete-game-description",
					"ui",
					new() { { "game-name", information.Name } }
				));
			}
		}
		//Nastavi kontext na hru, kterou chceme mazat
		public void SetContext(string? gameKey)
		{
			ContextGameKey = gameKey;
			OnContextChanged();
		}
		//Uzivatel kliknul na tlacitko smazat hru
		public void OnDeleteGameButtonClicked(object sender, RoutedEventArgs e)
		{
			//Kontrola kontextu
			if (ContextGameKey is null) return;
			//Smazani hry
			GamesManager.DeleteGame(ContextGameKey);
			//Zmena kontextu
			SetContext(null);
			//Presmerovani uzivatele
			ParentWindow.ChangePage(
				GamesManager.AvailableGames.Any()
				? LoadGamePage.GetInstance(ParentWindow)
				: MainMenuPage.GetInstance(ParentWindow)
			);
		}
		//Uzivatel kliknul na tlacitko zrusit
		public void OnCancelButtonClicked(object sender, RoutedEventArgs e)
		{
			//Zmena kontextu
			SetContext(null);
			//Zpet na nacteni hry
			ParentWindow.ChangePage(LoadGamePage.GetInstance(ParentWindow));
		}
	}
}