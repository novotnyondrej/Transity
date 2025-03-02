﻿using Transity.General;
using Transity.General.Exceptions;
using Transity.External;

namespace Transity.Data.Games
{
    //Stara se o nacitani a ukladani her a aktualizaci seznamu her
    internal class GamesManager
	{
		//Nazev slozky s hrami
		public static readonly string GamesLocation = "Games\\";
		//Dostupne hry
		private static Dictionary<string, GameInformation> _AvailableGames = [];
		public static IReadOnlyDictionary<string, GameInformation> AvailableGames => _AvailableGames;

		static GamesManager()
		{
			//Nacteni dostupnych her
			LoadAvailableGames();
		}
		//Nacte dostupne hry
		private static void LoadAvailableGames()
		{
			//Slozky dostupnych her
			IEnumerable<string> availableGameFolders;
			//Ziskani her ve slozce s hrami
			if (DirectoryManager.Exists(AppDataManager.DataLocation + GamesLocation)) availableGameFolders = DirectoryManager.GetDirectories(AppDataManager.DataLocation + GamesLocation);
			else availableGameFolders = [];

			//Ziskani nazvu her
			IEnumerable<string> gameCodeNames = availableGameFolders.Select(DirectoryManager.GetDirectoryName);

			//Pokus o nacteni informaci o hrach
			IEnumerable<GameInformation> informationList = gameCodeNames.Select(
				(codeName) => AppDataManager.LoadData<GameInformation?>(GamesLocation + codeName, GameInformation.SaveFileName, null)
			).Where(
				(information) => information is not null
			);
			Dictionary<string, GameInformation> informations = informationList.ToDictionary(
				(information) => information.CodeName,
				(information) => information
			);
			_AvailableGames = informations;
		}
		//Rozhodne, zda hra existuje
		public static bool GameExists(string codeName) => _AvailableGames.ContainsKey(codeName);
		//Nacte hru
		public static Game LoadGame(string codeName)
		{
			//Pokus o nalezeni hry
			if (!GameExists(codeName))
			{
				throw new DetailedTranslatableException(
					new(
						"game-not-found",
						"exceptions",
						new() { { "code-name", codeName } }
					)
				);
			}
			//Nacteni informaci
			GameInformation information = _AvailableGames[codeName];
			//Nacteni hrace
			Player player = AppDataManager.LoadData<Player>(GamesLocation + codeName, Player.SaveFileName, new());
			//Nacteni stavu mest
			IEnumerable<CityStatus>? cityStatuses = AppDataManager.LoadData<IEnumerable<CityStatus>?>(GamesLocation + codeName, Game.CityStatusesSaveFileName, null);
			//Nacteni linek
			IEnumerable<Line>? lines = AppDataManager.LoadData<IEnumerable<Line>?>(GamesLocation + codeName, Game.LinesSaveFileName, null);
			//Vytvoreni hry
			Game game = new(
				information,
				player,
				cityStatuses,
				lines
			);
			return game;
		}
		//Smaze hru
		public static void DeleteGame(string codeName)
		{
			if (!GameExists(codeName)) return;
			//Smazani hry
			bool success = SafeExecutor.Execute(DirectoryManager.Delete, AppDataManager.DataLocation + GamesLocation + codeName, false);
			//Aktualizace seznamu dostupnych her
			LoadAvailableGames();
		}
		//Ulozi hru
		public static void SaveGame(Game game, bool updatePlayTime = true)
		{
			//Ulozeni hry
			SafeExecutor.Execute(game.Save, updatePlayTime);
			//Aktualizace seznamu dostupnych her
			LoadAvailableGames();
		}
	}
}