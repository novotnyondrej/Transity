using System.Windows;
using Transity.UI;
using Transity.General.Exceptions;

namespace Transity.Pages
{
	/// <summary>
	/// Interaction logic for NewGame.xaml
	/// </summary>
	public partial class NewGame : MainWindowPage
	{
		//Jiz existujici instance
		private static Dictionary<MainWindow, NewGame> Instances = new();

		//Konstruktor
		public NewGame(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentWindow] = this;
			//Inicializace
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static NewGame GetInstance(MainWindow parentWindow)
		{
			if (Instances.ContainsKey(parentWindow))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				NewGame instance = Instances[parentWindow];
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
			ParentWindow.ChangePage(MainMenu.GetInstance(ParentWindow));
		}
	}
}