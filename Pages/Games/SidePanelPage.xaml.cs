using System.Windows;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	//Postranni panel hry
	public partial class SidePanelPage : GamePageChild
	{
		//Instance teto stranky
		private static readonly Dictionary<GamePage, SidePanelPage> Instances = [];


		public SidePanelPage(GamePage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentPage] = this;
			InitializeComponent();
			
			//Eventy
			parentPage.OnGameChanged += OnGameChanged;
			parentPage.OnSelectedCityChanged += OnSelectedCityChanged;
			parentPage.OnSelectedLineChanged += OnSelectedLineChanged;
		}
		//Ziska instanci stranky
		public static SidePanelPage GetInstance(GamePage parentPage)
		{
			if (Instances.TryGetValue(parentPage, out SidePanelPage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				SidePanelPage instance = value;
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentPage);
		}


		//Reakce na zmenu hry
		private void OnGameChanged(Game? previousGame, Game? currentGame)
		{
			contentFrame.Navigate(null);
		}
		//Reakce na zmenu oznaceneho mesta
		private void OnSelectedCityChanged(City? previousCity, City? currentCity)
		{
			if (currentCity is not null)
			{
				//Ziskani stranky
				CityDetailPage page = CityDetailPage.GetInstance(this);
				//Nacteni informaci o meste
				page.LoadCityDetail(currentCity);
				contentFrame.Navigate(page);
			}
			else
			{
				contentFrame.Navigate(null);
			}
		}
		//Reakce na zmenu oznacene linky
		private void OnSelectedLineChanged(Line? previousLine, Line? currentLine)
		{
			if (currentLine is not null)
			{
				//Ziskani stranky
				LineDetailPage page = LineDetailPage.GetInstance(this);
				//Nacteni informaci o meste
				page.LoadLineDetail(currentLine);
				contentFrame.Navigate(page);
			}
			else
			{
				contentFrame.Navigate(null);
			}
		}


		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
	}
}