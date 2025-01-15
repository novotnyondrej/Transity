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
		//Zda aktualne vytvari linku
		internal bool CreatingLine { get; private set; }
		//Index mesta, odkud ma linka vest
		private int? LineCityIndexFrom { get; set; }

		//Event na zmenu hry
		internal delegate void OnGameChangedDelegate(Game? previousGame, Game? currentGame);
		internal event OnGameChangedDelegate OnGameChanged = delegate { };
		//Event na zmenu oznaceneho mesta
		internal delegate void OnSelectedCityChangedDelegate(City? previousCity, City? currentCity);
		internal event OnSelectedCityChangedDelegate OnSelectedCityChanged = delegate { };
		//Event na vytvareni linky
		internal delegate void OnCreatingLineStatusChangedDelegate(bool status);
		internal event OnCreatingLineStatusChangedDelegate OnCreatingLineStatusChanged = delegate { };

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
			CreatingLine = false;
			OnGameChanged.Invoke(previousGame, CurrentGame);
			ParentWindow.ChangePage(this);
		}
		//Zmeni index oznaceneho mesta
		internal void ChangeSelectedCityIndex(int? cityIndex)
		{
			//Momentalne oznacene mesto
			City? currentCity = SelectedCity;
			City? newCity = (CurrentGame is null || cityIndex is null ? null : CurrentGame.Cities.ElementAt((int)cityIndex));

			if (CreatingLine)
			{
				if (newCity is null || !newCity.Status.Bought)
				{
					ChangeCreatingLineStatus(false);
					cityIndex = null;
				}
				else if(LineCityIndexFrom is null)
				{
					LineCityIndexFrom = cityIndex;
				}
				else if (CurrentGame is not null)
				{
					CurrentGame.AddLine(LineCityIndexFrom ?? -1, cityIndex ?? -1);
					SelectedCityIndex = null;
					ChangeCreatingLineStatus(false);
				}
			}
			SelectedCityIndex = cityIndex;
			OnSelectedCityChanged.Invoke(currentCity, SelectedCity);
		}
		//Zmeni status vytvareni linky
		internal void ChangeCreatingLineStatus(bool status)
		{
			if (status == CreatingLine) return;
			if (status) ChangeSelectedCityIndex(null);

			CreatingLine = status;
			LineCityIndexFrom = null;
			OnCreatingLineStatusChanged.Invoke(status);
		}
		//Stranka nactena
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
	}
}
