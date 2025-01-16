using Transity.Pages.Games;

namespace Transity.UI
{
	//Stranka, ktera patri do postranniho panelu
	public abstract class SidePanelPageChild : TranslatablePage<SidePanelPage>
	{
		public SidePanelPageChild(SidePanelPage parentPage) : base(parentPage)
		{

		}
	}
}