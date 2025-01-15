using Transity.Data.Games.Players;

namespace Transity.Data.Games
{
	//Trida pro hru
	internal class Game
	{
		//Informace o hre
		public readonly GameInformation Information;
		//Informace o hracovi
		public readonly Player Player;

		//Konstruktor
		public Game(
			GameInformation information,
			Player? player = null
		)
		{
			Information = information;
			Player = player ?? new();
		}
		//Ulozi hru
		public void Save()
		{
			//Ulozeni informaci o hre
			Information.Save();
			//Ulozeni informaci o hracovi
			Player.Save(Information);
		}
	}
}