using Transity.General;
using Transity.General.Exceptions;

namespace Transity.External
{
	//Stara se o nacitani a ukladani dat aplikace - nastaveni, uzivatelske ucty, hry apod.
	internal static class AppDataManager
	{
		//Umisteni dat
		public static string DataLocation => DirectoryManager.AppDataDirectory + "Transity\\";

		//Zkontroluje umisteni dat, pripadne vyhodi chybu
		public static string CheckLocation(string? location)
		{
			if (location is null) return "";

			location = location.Trim();
			//Umisteni nesmi obsahovat ..
			if (location.Contains(".."))
			{
				throw new TranslatableException(new(
					"invalid-application-data-location",
					"exceptions"
				));
			}
			//Pokud umisteni nekonci \ nebo /, pak ho doplnime
			if (location.Length > 0 && !location.EndsWith('\\') && !location.EndsWith('/')) location += '\\';
			return location;
		}
		//Zkontroluje nazev souboru, pripadne vyhodi chybu
		public static string CheckFileName(string fileName)
		{
			fileName = fileName.Trim();
			//Umisteni nesmi obsahovat ..
			if (fileName.Length <= 0 || fileName.Contains("..") || fileName.EndsWith('\\') || fileName.EndsWith('/'))
			{
				throw new TranslatableException(new(
					"invalid-application-data-file-name",
					"exceptions"
				));
			}
			return fileName;
		}


		//Pokusi se nacist data z pozadovaneho zdroje
		private static OfType? TryLoadData<OfType>(string? location, string fileName)
		{
			try
			{
				location = CheckLocation(location);
				fileName = CheckFileName(fileName);

				//Sestrojeni cesty
				string path = DataLocation + location + fileName + ".json";

				//Kontrola, jestli soubor existuje
				if (!FileManager.Exists(path)) return default;
				//Nacteni dat
				return FileManager.GetJsonContents<OfType>(path);
			}
			catch (Exception exception)
			{
				throw new TranslatableException(new(
					"application-data-load-error",
					"exceptions",
					new()
					{
						{ "location", location ?? "" },
						{ "file-name", fileName },
						{ "original-message", exception.Message }
					}
				));
			}
		}
		public static OfType LoadData<OfType>(string? location, string fileName, OfType defaultValue)
		{
			//Pokus o nacteni dat
			return SafeExecutor.Execute(TryLoadData<OfType>, location, fileName, defaultValue) ?? defaultValue;
		}
	
		
		//Pokusi se ulozit data na pozadovane misto
		private static void TrySaveData<OfType>(string? location, string fileName, OfType data)
		{
			try
			{
				location = CheckLocation(location);
				fileName = CheckFileName(fileName);

				//Sestrojeni cesty
				string path = DataLocation + location + fileName + ".json";

				//Ulozeni dat
				FileManager.PutJsonContents<OfType>(path, data);
			}
			catch (Exception exception)
			{
				throw new TranslatableException(new(
					"application-data-save-error",
					"exceptions",
					new()
					{
						{ "location", location ?? "" },
						{ "file-name", fileName },
						{ "original-message", exception.Message }
					}
				));
			}
		}
		public static void SaveData<OfType>(string? location, string fileName, OfType data)
		{
			//Pokus o ulozeni dat
			SafeExecutor.Execute(TrySaveData, location, fileName, data);
		}


		//Pokusi se smazat data z pozadovaneho mista
		private static void TryDeleteData(string? location, string fileName)
		{
			try
			{
				location = CheckLocation(location);
				fileName = CheckFileName(fileName);

				//Sestrojeni cesty
				string path = DataLocation + location + fileName + ".json";

				//Kontrola, jestli soubor existuje
				if (!FileManager.Exists(path)) return;
				//Smazani dat
				FileManager.Delete(path);
			}
			catch (Exception exception)
			{
				throw new TranslatableException(new(
					"application-data-delete-error",
					"exceptions",
					new()
					{
						{ "location", location ?? "" },
						{ "file-name", fileName },
						{ "original-message", exception.Message }
					}
				));
			}
		}
		public static void DeleteData(string? location, string fileName)
		{
			//Pokus o ulozeni dat
			SafeExecutor.Execute(TryDeleteData, location, fileName);
		}
		//Smaze veskera data
		public static void DeleteAllData()
		{
			//Kontrola, jestli slozka existuje
			if (!DirectoryManager.Exists(DataLocation)) return;
			try
			{
				DirectoryManager.Delete(DataLocation);
			}
			catch (Exception exception)
			{
				SafeExecutor.HandleException(new TranslatableException(new(
					"application-all-data-delete-error",
					"exceptions",
					new()
					{
						{ "original-message", exception.Message }
					}
				)));
			}
		}
	}
}