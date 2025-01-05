using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Transity.General;
using Transity.External;
using Transity.Content;
using Transity.General.Exceptions;
using Transity.Data;
using Transity.Pages;
using Transity.UI;

namespace Transity
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//Konstruktor
		public MainWindow()
		{
			//Inicializace
			InitializeComponent();
			//Prvotni nacteni stranek
			OnPageStrategyChanged(AppSettings.UserSettings.LoadPagesOnStartup);
			//Event listener na zmenu strategie
			AppSettings.UserSettings.OnPagesLoadStrategyChanged += OnPageStrategyChanged;
			//Vychozi stranka
			ChangePage(MainMenuPage.GetInstance(this));
		}
		//Zmeni stranku
		public void ChangePage(TranslatablePage<MainWindow> page)
		{
			MainFrame.Navigate(page);
		}

		//Event listener na zmenu strategie nacitani stranek
		private void OnPageStrategyChanged(bool newValue)
		{
			//Pokud se stranky nemaji nacitat, tak neni co menit
			if (newValue == false) return;
			//Probehne nacteni stranek
			MainMenuPage.GetInstance(this);
			NewGamePage.GetInstance(this);
			SettingsPage.GetInstance(this);
		}

	}
}