using System.Windows;
using System.Windows.Controls;

namespace Transity.UI
{
	public abstract class TranslatablePage<ParentWindowType> : Page where ParentWindowType : Window
	{
		//Rodicovske okno
		public readonly ParentWindowType ParentWindow;

		public TranslatablePage(ParentWindowType parentWindow)
		{
			ParentWindow = parentWindow;
		}
		//Metoda pro predbezne nacteni stranky
		public virtual void Preload()
		{
			//Prelozeni obsahu
			this.Translate();
		}
	}
}