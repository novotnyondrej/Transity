using Transity.Pages.Games;

namespace Transity.UI
{
	public abstract class GamePageChild : TranslatablePage<GamePage>
	{
		public GamePageChild(GamePage parentWindow) : base(parentWindow)
		{

		}
	}
}