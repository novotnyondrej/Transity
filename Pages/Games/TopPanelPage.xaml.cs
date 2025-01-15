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
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	/// <summary>
	/// Interaction logic for TopPanelPage.xaml
	/// </summary>
	public partial class TopPanelPage : GamePageChild
	{
		//Jiz existujici instance
		private static readonly Dictionary<GamePage, TopPanelPage> Instances = [];


		public TopPanelPage(GamePage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentPage] = this;

			InitializeComponent();
			parentPage.OnGameChanged += OnGameChanged;
			parentPage.OnCreatingLineStatusChanged += OnCreatingLineStatusChanged;
		}
		//Ziska instanci stranky
		public static TopPanelPage GetInstance(GamePage parentPage)
		{
			if (Instances.ContainsKey(parentPage))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				TopPanelPage instance = Instances[parentPage];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentPage);
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
			if (previousGame is not null) previousGame.Player.OnMoneyChanged -= UpdatePlayerBalance;
			//Pridani eventu na zmenu penez
			if (currentGame is not null) currentGame.Player.OnMoneyChanged += UpdatePlayerBalance;
			
			UpdatePlayerBalance(0, currentGame?.Player.Money ?? 0);
			createNewLineCheckbox.IsChecked = false;
		}
		//Aktualizuje hracovy penize
		private void UpdatePlayerBalance(int previousBalance, int currentBalance)
		{
			playerBalanceLabel.Content = "$" + currentBalance;
		}
		public void OnCreateNewLineCheckboxClicked(object sender, RoutedEventArgs e)
		{
			ParentWindow.ChangeCreatingLineStatus(createNewLineCheckbox.IsChecked ?? false);
		}
		private void OnCreatingLineStatusChanged(bool status)
		{
			createNewLineCheckbox.IsChecked = status;
		}
	}
}
