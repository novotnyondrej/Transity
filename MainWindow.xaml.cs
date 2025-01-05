using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Transity.General;
using Transity.External;
using Transity.Content;
using Transity.General.Exceptions;
using Transity.Data;
using Transity.Pages;
using Transity.UI;

namespace Transity
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//Konstruktor
		public MainWindow()
		{
			//Inicializace
			InitializeComponent();
			//Vychozi stranka
			ChangePage(MainMenu.GetInstance(this));
		}
		//Zmeni stranku
		public void ChangePage(TranslatablePage<MainWindow> page)
		{
			MainFrame.Navigate(page);
		}
	}
}