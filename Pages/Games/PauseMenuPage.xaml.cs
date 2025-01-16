using System.Windows;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	//Postranni panel hry
	public partial class PauseMenuPage : GamePageChild
	{
		//Instance teto stranky
		private static readonly Dictionary<GamePage, PauseMenuPage> Instances = [];


		public PauseMenuPage(GamePage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentPage] = this;
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static PauseMenuPage GetInstance(GamePage parentPage)
		{
			if (Instances.TryGetValue(parentPage, out PauseMenuPage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				PauseMenuPage instance = value;
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
			saveGameButton.IsEnabled = true;
		}
		//Uzivatel kliknul na tlacitko pokracovat ve hre
		public void OnResumeButtonClicked(object sender, RoutedEventArgs e)
		{
			ParentWindow.UnpauseGame();
		}
		//Uzivatel kliknul na tlacitko ulozit hru
		public void OnSaveButtonClicked(object sender, RoutedEventArgs e)
		{
			//Ulozeni hry
			ParentWindow.CurrentGame?.Save();
			saveGameButton.IsEnabled = false;
		}
		//Uzivatel kliknul na tlacitko odejit ze hry
		public void OnExitButtonClicked(object sender, RoutedEventArgs e)
		{
			//Ulozeni hry
			ParentWindow.CurrentGame?.Save();
			//Odnacteni hry
			ParentWindow.LoadGame(null);
		}
	}
}