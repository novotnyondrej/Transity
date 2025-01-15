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
		public int Money { get; private set; }

		//Event na zmenu poctu penez
		internal delegate void OnMoneyChangedDelegate(int previousValue, int currentValue);
		internal event OnMoneyChangedDelegate OnMoneyChanged = delegate { };

		//Konstruktor
		[JsonConstructor]
		public Player(
			int? money = null
		)
		{
			Money = money ?? InitialMoney;
		}
		//Zmeni penize o nejakou castku
		public bool ChangeMoney(int change)
		{
			if (Money + change < 0) return false;

			int previousValue = Money;
			Money += change;
			OnMoneyChanged(previousValue, Money);
			return true;
		}
		//Ulozi hrace
		public void Save(GameInformation gameInformation)
		{
			AppDataManager.SaveData(GamesManager.GamesLocation + gameInformation.CodeName, SaveFileName, this);
		}
	}
}