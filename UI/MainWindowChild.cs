namespace Transity.UI
{
	//Stranka, ktera patri do hlavniho okna
	public abstract class MainWindowChild : TranslatablePage<MainWindow>
	{
		public MainWindowChild(MainWindow parentWindow) : base(parentWindow)
		{

		}
	}
}