using Transity.Pages.Games;

namespace Transity.UI
{
	//Stranka, ktera patri pouze do herniho okna
	public abstract class GamePageChild : TranslatablePage<GamePage>
	{
		public GamePageChild(GamePage parentWindow) : base(parentWindow)
		{

		}
	}
}