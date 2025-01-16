using System.Windows;
using System.Windows.Controls;
using Transity.Content;
using Transity.Data.Games;
using Transity.General;
using Transity.General.Exceptions;
using Transity.Pages.Games;
using Transity.UI;

namespace Transity.Pages
{
	//Stranka pro nacteni hry
	public partial class LoadGamePage : MainWindowChild
	{
		//Instance teto stranky
		private static readonly Dictionary<MainWindow, LoadGamePage> Instances = [];

		public LoadGamePage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentWindow] = this;
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static LoadGamePage GetInstance(MainWindow parentWindow)
		{
			if (Instances.TryGetValue(parentWindow, out LoadGamePage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				LoadGamePage instance = value;
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
			//Razeni her podle casu posledniho hrani
			IEnumerable<GameInformation> informations = GamesManager.AvailableGames.Values.ToList().OrderByDescending(
				(information) => information.LastPlayedOn
			);
			//Nacteni jazyku
			int gamesCount = informations.Count();

			for (int gameNum = 0; gameNum < gamesCount; gameNum++)
			{
				//Ziskani informaci o hre
				GameInformation information = informations.ElementAt(gameNum);
				//Nacteni informaci o hre
				string gameKey = information.CodeName;
				string name = information.Name;
				string createTime = TimeConverter.ToTimestamp(information.CreatedOn);
				string lastPlayTime = (
					information.LastPlayedOn is null
					? Translator.LoadTranslation(new("never-played", UITranslator.TranslationSetName))
					: TimeConverter.ToTimestamp((int)information.LastPlayedOn)
				);
				//Nacteni prekladu
				string text = Translator.LoadTranslation(new(
					"game-item",
					UITranslator.TranslationSetName,
					new()
					{
						{ "game-name", name },
						{ "create-time", createTime },
						{ "last-play-time", lastPlayTime }
					}
				));
				//Nazev itemu
				string itemName = "game_item_" + gameKey.Replace("-", "_");
				//Ziskani elementu reprezentujici danou hru
				ListBoxItem gameItem = gamesListBox.FindItemByName(itemName) ?? gameItemTemplate.Clone(itemName);
				//Nacteni textu
				gameItem.Content = text;
				//Pridani vhodneho tagu
				gameItem.Tag = gameKey;
				//Zapnuti by default
				gameItem.IsEnabled = true;
				gameItem.IsSelected = false;
				//Odebrani elementu
				if (gamesListBox.Items.Contains(gameItem)) gamesListBox.Items.Remove(gameItem);
				//Vlozeni elementu na spravne misto
				gamesListBox.Items.Insert(gameNum, gameItem);
			}
			//Seznam neaktualnich her
			List<ListBoxItem> outdatedGameItems = new();
			//Nalezeni vsech neaktualnich her
			foreach (ListBoxItem gameItem in gamesListBox.Items)
			{
				if (!GamesManager.AvailableGames.ContainsKey((string)gameItem.Tag)) outdatedGameItems.Add(gameItem);
			}
			//Odebrani vsech neaktualnich her
			foreach (ListBoxItem outdatedGameItem in outdatedGameItems)
			{
				gamesListBox.Items.Remove(outdatedGameItem);
			}
		}
		
		
		//Stranka nactena
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Odebrani predloh
			gamesListBox.Items.Remove(gameItemTemplate);
			gamesListBox.SelectedItem = null;
			deleteGameButton.IsEnabled = false;
			playGameButton.IsEnabled = false;
			//Nacteni prekladu
			Preload();
		}
		//Zmena oznacene hry
		public void OnGamesListSelectionChanged(object sender, RoutedEventArgs e)
		{
			//Kontrola, jestli je vybrana nejaka hra, pokud neni, tak nebudou povoleny tlacitka pro zachazeni se hrou
			deleteGameButton.IsEnabled = gamesListBox.SelectedItem is not null;
			playGameButton.IsEnabled = gamesListBox.SelectedItem is not null;
		}
		//Uzivatel kliknul na tlacitko zpet
		public void OnBackButtonClicked(object sender, RoutedEventArgs e)
		{
			//Zpet na hlavni menu
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
		//Uzivatel kliknul na tlacitko smazat hru
		public void OnDeleteGameButtonClicked(object sender, RoutedEventArgs e)
		{
			//Ziskani moznosti s danou hrou
			ListBoxItem? selectedItem = (ListBoxItem?)gamesListBox.SelectedItem;
			if (selectedItem is null) return;
			//Ziskani klice hry
			string gameKey = (string)selectedItem.Tag;
			//Nastaveni kontextu pro mazani hry
			DeleteGamePage.GetInstance(ParentWindow).SetContext(gameKey);
			//Stranka pro smazani hry
			ParentWindow.ChangePage(DeleteGamePage.GetInstance(ParentWindow));
		}
		//Uzivatel kliknul na tlacitko hrat hru
		public void OnPlayGameButtonClicked(object sender, RoutedEventArgs e)
		{
			//Ziskani moznosti s danou hrou
			ListBoxItem? selectedItem = (ListBoxItem?)gamesListBox.SelectedItem;
			if (selectedItem is null) return;
			//Ziskani klice hry
			string gameKey = (string)selectedItem.Tag;
			//Nacteni oznacene hry
			Game game = GamesManager.LoadGame(gameKey);
			//Nacteni rozhrani pro hru
			GamePage.GetInstance(ParentWindow).LoadGame(game);
		}
	}
}