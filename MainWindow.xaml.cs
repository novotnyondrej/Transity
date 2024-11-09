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

namespace Transity
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Translator.LoadAllLanguages();
			//MessageBox.Show(Translator.LoadTranslation(new TranslationKey("exception-details-text")));
			//MessageBox.Show(Translator.LoadTranslation(new("cs"), "cs"));
			//MessageBox.Show(Translator.LoadTranslation(new("en"), "cs"));
			//MessageBox.Show(Translator.LoadTranslation(new("de"), "cs"));

			try
			{
				throw new DetailedTranslatableException(new("test-exception", "exceptions"));
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
	}
}