using System.Windows;
using System.Windows.Threading;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	//Stranka s hrou
	public partial class GamePage : MainWindowChild
	{
		//Instance teto stranky
		private static readonly Dictionary<MainWindow, GamePage> Instances = [];
		//Autosave interval
		private static readonly int AutosaveInterval = 300;
		
		//Aktualne nactena hra
		internal Game? CurrentGame { get; private set; }
		//Za jak dlouho probehne dalsi autosave
		private int? NextAutosaveIn;
		//Zda je hra pauznuta
		private bool Paused;
		//Prave oznacene mesto
		internal int? SelectedCityIndex { get; private set; }
		internal City? SelectedCity { get => (CurrentGame is null || SelectedCityIndex is null ? null : CurrentGame.Cities.ElementAtOrDefault((int)SelectedCityIndex)); }
		//Aktualne oznacena linka
		internal string? SelectedLineId { get; private set; }
		internal Line? SelectedLine { get => (CurrentGame is null || SelectedLineId is null ? null : CurrentGame.Lines.FirstOrDefault((line) => line.Id == SelectedLineId)); }
		
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
		//Event na zmenu oznacene linky
		internal delegate void OnSelectedLineChangedDelegate(Line? previousLine, Line? currentLine);
		internal event OnSelectedLineChangedDelegate OnSelectedLineChanged = delegate { };
		//Event na vytvareni linky
		internal delegate void OnCreatingLineStatusChangedDelegate(bool status);
		internal event OnCreatingLineStatusChangedDelegate OnCreatingLineStatusChanged = delegate { };


		public GamePage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentWindow] = this;
			InitializeComponent();
			
			//Vytvoreni stranek
			topPanelFrame.Navigate(TopPanelPage.GetInstance(this));
			mainContentFrame.Navigate(MainPanelPage.GetInstance(this));
			sidePanelFrame.Navigate(SidePanelPage.GetInstance(this));

			//Vytvoreni displatcheru pro gameTick
			DispatcherTimer dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Tick += new EventHandler(OnGameTickTriggered);
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();
		}
		//Ziska instanci stranky
		public static GamePage GetInstance(MainWindow parentWindow)
		{
			if (Instances.TryGetValue(parentWindow, out GamePage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				GamePage instance = value;
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
		}


		//Nacte hru
		internal void LoadGame(Game? game)
		{
			//Predchozi hra
			Game? previousGame = CurrentGame;
			//Odebrani event listeneru z predchozi hry
			if (previousGame is not null) previousGame.OnLineRemoved -= OnLineRemoved;
			//Nacteni hry
			CurrentGame = game;
			SelectedCityIndex = null;
			CreatingLine = false;
			NextAutosaveIn = (game is not null ? AutosaveInterval : null);
			UnpauseGame();
			//Pridani event listeneru na novou hru
			if (game is not null) game.OnLineRemoved += OnLineRemoved;
			//Event
			OnGameChanged.Invoke(previousGame, CurrentGame);

			//Hlavni menu
			if (game is null) ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
			//Hra
			else ParentWindow.ChangePage(this);
		}
		//Odpauzuje hru
		internal void UnpauseGame()
		{
			Paused = false;
			overlayFrame.Navigate(null);
			overlayFrame.Visibility = Visibility.Hidden;
		}
		//Pauzuje hru
		internal void PauseGame()
		{
			Paused = true;
			overlayFrame.Navigate(PauseMenuPage.GetInstance(this));
			overlayFrame.Visibility = Visibility.Visible;
		}


		//Zmeni index oznaceneho mesta
		internal void ChangeSelectedCityIndex(int? cityIndex)
		{
			if (SelectedCityIndex is null && cityIndex is null) return;
			//V moment, kdy oznacujeme ten samy element, ho odznacime
			if (SelectedCityIndex == cityIndex) cityIndex = null;

			//Momentalne oznacene mesto
			City? previousCity = SelectedCity;
			//Nove oznacene mesto
			City? newCity = (CurrentGame is null || cityIndex is null ? null : CurrentGame.Cities.ElementAt((int)cityIndex));

			//Pokud vytvarime linku
			if (CreatingLine)
			{
				if (newCity is null || !newCity.Status.Bought)
				{
					//Nove mesto bud neexistuje nebo ho hrac nevlastni
					ChangeCreatingLineStatus(false);
					cityIndex = null;
				}
				else if (LineCityIndexFrom is null)
				{
					//Zacatek linky
					LineCityIndexFrom = cityIndex;
				}
				else if (CurrentGame is not null)
				{
					//Konec linky
					CurrentGame.AddLine(LineCityIndexFrom ?? -1, cityIndex ?? -1);
					cityIndex = null;
					ChangeCreatingLineStatus(false);
				}
			}
			SelectedCityIndex = cityIndex;
			ChangeSelectedLineIndex(null);
			OnSelectedCityChanged.Invoke(previousCity, SelectedCity);
		}
		//Zmeni index oznacene linky
		internal void ChangeSelectedLineIndex(string? lineId)
		{
			if (SelectedLineId is null && lineId is null) return;
			//V moment, kdy oznacujeme ten samy element, ho odznacime
			if (SelectedLineId == lineId) lineId = null;
			//
			Line? previousLine = SelectedLine;
			SelectedLineId = lineId;
			ChangeSelectedCityIndex(null);
			OnSelectedLineChanged.Invoke(previousLine, SelectedLine);
		}
		//Zmeni status vytvareni linky
		internal void ChangeCreatingLineStatus(bool status)
		{
			//Kontrola, jestli se vubec meni status
			if (CreatingLine == status) return;
			//Pokud jsme zacali vytvaret novou linku, odznacime aktualni mesto a linku
			if (status)
			{
				ChangeSelectedCityIndex(null);
				ChangeSelectedLineIndex(null);
			}
			//Zmena statusu
			CreatingLine = status;
			LineCityIndexFrom = null;
			OnCreatingLineStatusChanged.Invoke(status);
		}
		
		
		//Reaguje na odebranou linku
		internal void OnLineRemoved(Line line)
		{
			if (SelectedLineId == line.Id) ChangeSelectedLineIndex(null);
		}


		//Game tick
		private void OnGameTickTriggered(object? sender, EventArgs e)
		{
			if (Paused) return;
			//Game tick
			CurrentGame?.GameTick();
			//Autosave se priblizi o 1 vterinu
			if (NextAutosaveIn is not null)
			{
				NextAutosaveIn -= 1;
				//Zda nastal cas na autosave
				if (NextAutosaveIn <= 0)
				{
					//Reset
					NextAutosaveIn = (CurrentGame is not null ? AutosaveInterval : null);
					//Autosave
					CurrentGame?.Save();
				}
			}
		}
		//Stranka nactena
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
			overlayFrame.Visibility = Visibility.Hidden;
		}
	}
}