using System.Windows;
using Transity.Content;
using Transity.Data.Games;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages.Games
{
	//Stranka s detaily o meste
	public partial class LineDetailPage : SidePanelPageChild
	{
		//Instance teto stranky
		private static readonly Dictionary<SidePanelPage, LineDetailPage> Instances = [];
		//Mesto, pro ktere mame aktualne nacteny detail
		private Line? CurrentLine;


		public LineDetailPage(SidePanelPage parentPage) : base(parentPage)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentPage)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentPage] = this;
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static LineDetailPage GetInstance(SidePanelPage parentPage)
		{
			if (Instances.TryGetValue(parentPage, out LineDetailPage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				LineDetailPage instance = value;
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentPage);
		}



		//Nacte detail linky
		internal void LoadLineDetail(Line? line)
		{
			if (CurrentLine == line) return;
			//Prenastaveni linky
			CurrentLine = line;
		}


		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}
		//Uzivatel kliknul na tlacitko smazat linku
		public void OnDeleteLineButtonClicked(object sender, RoutedEventArgs e)
		{
			if (CurrentLine is not null) ParentWindow.ParentWindow.CurrentGame?.RemoveLine(CurrentLine);
		}
	}
}