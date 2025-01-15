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
		//Vychozi pocet penez
		private static readonly int InitialMoney = 10000;

		//Pocet penez
		[JsonProperty("money")]
		public readonly int Money;

		//Konstruktor
		[JsonConstructor]
		public Player(
			int? money = null
		)
		{
			Money = money ?? InitialMoney;
		}
		//Ulozi hrace
		public void Save(GameInformation gameInformation)
		{
			AppDataManager.SaveData(GamesManager.GamesLocation + gameInformation.CodeName, SaveFileName, this);
		}
	}
}