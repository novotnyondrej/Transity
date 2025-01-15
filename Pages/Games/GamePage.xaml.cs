using System.Windows;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	/// <summary>
	/// Interaction logic for GamePage.xaml
	/// </summary>
	public partial class GamePage : MainWindowChild
	{
		//Jiz existujici instance
		private static readonly Dictionary<MainWindow, GamePage> Instances = [];
		
		//Nactena hra
		internal Game? CurrentGame { get; private set; }
		//Index prave oznaceneho mesta
		internal int? SelectedCityIndex { get; private set; }
		//Aktualne oznacene mesto
		internal City? SelectedCity { get => (CurrentGame is null || SelectedCityIndex is null ? null : CurrentGame.Cities.ElementAt((int)SelectedCityIndex)); }

		//Event na zmenu hry
		internal delegate void OnGameChangedDelegate(Game? previousGame, Game? currentGame);
		internal event OnGameChangedDelegate OnGameChanged = delegate { };
		//Event na zmenu oznaceneho mesta
		internal delegate void OnSelectedCityChangedDelegate(City? previousCity, City? currentCity);
		internal event OnSelectedCityChangedDelegate OnSelectedCityChanged = delegate { };
		
		public GamePage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentWindow] = this;
			//Inicializace
			InitializeComponent();
			//Vytvoreni stranek
			topPanelFrame.Navigate(TopPanelPage.GetInstance(this));
			mainContentFrame.Navigate(MainPanelPage.GetInstance(this));
			sidePanelFrame.Navigate(SidePanelPage.GetInstance(this));
		}
		//Ziska instanci stranky
		public static GamePage GetInstance(MainWindow parentWindow)
		{
			if (Instances.ContainsKey(parentWindow))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				GamePage instance = Instances[parentWindow];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
		}
		//Nacte hru
		internal void LoadGame(Game? game)
		{
			Game? previousGame = CurrentGame;
			CurrentGame = game;
			SelectedCityIndex = null;
			OnGameChanged.Invoke(previousGame, CurrentGame);
			ParentWindow.ChangePage(this);
		}
		//Zmeni index oznaceneho mesta
		internal void ChangeSelectedCityIndex(int? cityIndex)
		{
			//Momentalne oznacene mesto
			City? currentCity = SelectedCity;
			SelectedCityIndex = cityIndex;
			OnSelectedCityChanged.Invoke(currentCity, SelectedCity);
		}
		//Stranka nactena
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
	}
}
