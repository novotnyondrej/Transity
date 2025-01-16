using System.Windows;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	//Horni panel hry
	public partial class TopPanelPage : GamePageChild
	{
		//Instance teto stranky
		private static readonly Dictionary<GamePage, TopPanelPage> Instances = [];


		public TopPanelPage(GamePage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentPage] = this;
			InitializeComponent();
			
			//Eventy
			parentPage.OnGameChanged += OnGameChanged;
			parentPage.OnCreatingLineStatusChanged += OnCreatingLineStatusChanged;
		}
		//Ziska instanci stranky
		public static TopPanelPage GetInstance(GamePage parentPage)
		{
			if (Instances.TryGetValue(parentPage, out TopPanelPage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				TopPanelPage instance = value;
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentPage);
		}


		//Aktualizuje hracovy penize
		private void UpdatePlayerBalance(int previousBalance, int currentBalance)
		{
			playerBalanceLabel.Content = "$" + currentBalance;
		}


		//Reaguje na zmenu nactene hry
		private void OnGameChanged(Game? previousGame, Game? currentGame)
		{
			//Odebrani eventu na zmenu penez z predchozi hry
			if (previousGame is not null) previousGame.Player.OnMoneyChanged -= UpdatePlayerBalance;
			//Pridani eventu na zmenu penez
			if (currentGame is not null) currentGame.Player.OnMoneyChanged += UpdatePlayerBalance;

			UpdatePlayerBalance(0, currentGame?.Player.Money ?? 0);
			createNewLineCheckbox.IsChecked = false;
		}
		//Reakce na zmenu statusu vytvareni linky
		private void OnCreatingLineStatusChanged(bool status)
		{
			if (createNewLineCheckbox.IsChecked == status) return;
			createNewLineCheckbox.IsChecked = status;
		}


		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
		//Uzivatel klikl na checkbox s vytvorenim nove linky
		public void OnCreateNewLineCheckboxClicked(object sender, RoutedEventArgs e)
		{
			ParentWindow.ChangeCreatingLineStatus(createNewLineCheckbox.IsChecked ?? false);
		}
		//Uzivatel klikl na pozastaveni hry
		public void OnPauseButtonClicked(object sender, RoutedEventArgs e)
		{
			ParentWindow.PauseGame();
		}
	}
}