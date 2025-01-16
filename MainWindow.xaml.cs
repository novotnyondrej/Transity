using System.Windows;
using Transity.Data;
using Transity.Pages;
using Transity.Pages.Games;
using Transity.UI;

namespace Transity
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			//Prvotni nacteni stranek
			OnPageStrategyChanged(AppSettings.UserSettings.LoadPagesOnStartup);
			//Event listener na zmenu strategie nacitani stranek
			AppSettings.UserSettings.OnPagesLoadStrategyChanged += OnPageStrategyChanged;
			//Vychozi stranka
			ChangePage(MainMenuPage.GetInstance(this));
		}
		//Zmeni stranku
		public void ChangePage(TranslatablePage<MainWindow> page)
		{
			mainFrame.Navigate(page);
		}

		//Event listener na zmenu strategie nacitani stranek
		private void OnPageStrategyChanged(bool newValue)
		{
			//Pokud se stranky nemaji nacitat, tak neni co menit
			if (newValue == false) return;
			//Probehne nacteni vsech stranek
			MainMenuPage.GetInstance(this);
			LoadGamePage.GetInstance(this);
			DeleteGamePage.GetInstance(this);
			NewGamePage.GetInstance(this);
			SettingsPage.GetInstance(this);
			GamePage.GetInstance(this);
		}

	}
}