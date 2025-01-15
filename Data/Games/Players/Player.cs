using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Transity.External;

namespace Transity.Data.Games.Players
{
	//Hrac hry
	internal class Player : IPlayer
	{
		//Nazev souboru s informacemi o hracovi
		public static readonly string SaveFileName = "player";

		//Pocet penez
		[JsonProperty("money")]
		public readonly int Money;

		//Konstruktor
		public Player(
			int? money = null
		)
		{
			Money = money ?? 1000;
		}
		//Ulozi hrace
		public void Save(GameInformation gameInformation)
		{
			AppDataManager.SaveData(GamesManager.GamesLocation + gameInformation.CodeName, SaveFileName, this);
		}
	}
}