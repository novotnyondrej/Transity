using System.Windows;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages
{
	//Stranka s neulozenymi zmenami v nastaveni
	public partial class UnsavedSettingsPage : MainWindowChild
	{
		//Instance teto stranky
		private static readonly Dictionary<MainWindow, UnsavedSettingsPage> Instances = [];


		public UnsavedSettingsPage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentWindow] = this;
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static UnsavedSettingsPage GetInstance(MainWindow parentWindow)
		{
			if (Instances.TryGetValue(parentWindow, out UnsavedSettingsPage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				UnsavedSettingsPage instance = value;
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
		//Uzivatel kliknul na tlacitko zahodit zmeny
		public void OnRevertButtonClicked(object sender, RoutedEventArgs e)
		{
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
		//Uzivatel kliknul na tlacitko ulozit zmeny
		public void OnSaveButtonClicked(object sender, RoutedEventArgs e)
		{
			SettingsPage.GetInstance(ParentWindow, false).SaveSettings();
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
	}
}