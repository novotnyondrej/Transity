using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	/// <summary>
	/// Interaction logic for SidePanelPage.xaml
	/// </summary>
	public partial class SidePanelPage : GamePageChild
	{
		//Jiz existujici instance
		private static readonly Dictionary<GamePage, SidePanelPage> Instances = [];


		public SidePanelPage(GamePage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentPage] = this;

			InitializeComponent();
			parentPage.OnGameChanged += OnGameChanged;
			parentPage.OnSelectedCityChanged += OnSelectedCityChanged;
		}
		//Ziska instanci stranky
		public static SidePanelPage GetInstance(GamePage parentPage)
		{
			if (Instances.ContainsKey(parentPage))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				SidePanelPage instance = Instances[parentPage];
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
		}
		private void OnGameChanged(Game? previousGame, Game? currentGame)
		{
			contentFrame.Navigate(null);
		}
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
	}
}
