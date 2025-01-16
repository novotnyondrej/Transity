using System.Windows;
using Transity.Content;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.General;
using Transity.UI;

namespace Transity.Pages
{
	//Stranka pro smazani hry
	public partial class DeleteGamePage : MainWindowChild
	{
		//Instance teto stranky
		private static readonly Dictionary<MainWindow, DeleteGamePage> Instances = [];
		//Kontext mazani hry
		private string? ContextGameKey = null;


		public DeleteGamePage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentWindow] = this;
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static DeleteGamePage GetInstance(MainWindow parentWindow)
		{
			if (Instances.TryGetValue(parentWindow, out DeleteGamePage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				DeleteGamePage instance = value;
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
		}


		//Nastavi kontext na hru, kterou chceme mazat
		public void SetContext(string? gameKey)
		{
			ContextGameKey = gameKey;
			OnContextChanged();
		}


		//Event pri zmene kontextu
		private void OnContextChanged()
		{
			deleteGameButton.IsEnabled = ContextGameKey is not null;

			if (ContextGameKey is null)
			{
				//Nelze nic smazat
				deleteGameDescriptionLabel.Content = Translator.LoadTranslation(new("no-game-to-delete", UITranslator.TranslationSetName));
			}
			else
			{
				//Ziskani hry
				GameInformation information = GamesManager.AvailableGames[ContextGameKey];
				//Nacteni nazvu hry
				deleteGameDescriptionLabel.Content = Translator.LoadTranslation(new(
					"delete-game-description",
					UITranslator.TranslationSetName,
					new() { { "game-name", information.Name } }
				));
			}
		}


		//Stranka nactena
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
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