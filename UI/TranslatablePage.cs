using System.Windows;
using System.Windows.Controls;

namespace Transity.UI
{
	public abstract class TranslatablePage<ParentWindowType> : Page
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