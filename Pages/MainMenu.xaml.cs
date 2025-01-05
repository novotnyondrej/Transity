using System.Windows;
using Transity.UI;
using Transity.General.Exceptions;

namespace Transity.Pages
{
	/// <summary>
	/// Interaction logic for MainMenu.xaml
	/// </summary>
	public partial class MainMenu : MainWindowPage
	{
		//Jiz existujici instance
		private static Dictionary<MainWindow, MainMenu> Instances = new();

		//Konstruktor
		public MainMenu(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentWindow] = this;
			//Inicializace
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static MainMenu GetInstance(MainWindow parentWindow)
		{
			if (Instances.ContainsKey(parentWindow))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				MainMenu instance = Instances[parentWindow];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
		}
		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
		//Uzivatel kliknul na tlacitko nova hra
		public void OnNewGameButtonClicked(object sender, RoutedEventArgs e)
		{
			//Zmena stranky na novou hru
			ParentWindow.ChangePage(NewGame.GetInstance(ParentWindow));
		}
		//Uzivatel kliknul na tlacitko ukoncit
		public void OnExitButtonClicked(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
