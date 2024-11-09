using System.IO;
using System.Security;
using Transity.General.Exceptions;

namespace Transity.External
{
	//Stara se o soubory, jejich nacitani a zapis do nich
	internal static class FileManager
	{
		//Zda soubor existuje
		public static bool Exists(string path)
		{
			return File.Exists(path);
		}


		//Ziska nazev souboru podle cesty
		public static string GetFileName(string path)
		{
			try
			{
				return Path.GetFileNameWithoutExtension(path);
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-file", "exceptions"));
			}
		}
		//Ziska priponu souboru podle cesty
		public static string GetFileExtension(string path)
		{
			try
			{
				//Ziskani pripony
				string extension = Path.GetExtension(path);
				//Pokud existuje, tak odebereme .
				if (extension.Length > 0) extension = extension[1..];
				return extension;
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-file", "exceptions"));
			}
		}

		
		//Pokusi se nacist obsah souboru
		public static string GetContents(string path)
		{
			try
			{
				//Pokus o cteni souboru
				string contents = File.ReadAllText(path);
				return contents;
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-file", "exceptions"));
			}
			catch (UnauthorizedAccessException)
			{
				throw new TranslatableException(new("unauthorized-access", "exceptions"));
			}
			catch (PathTooLongException)
			{
				throw new TranslatableException(new("path-too-long", "exceptions"));
			}
			catch (DirectoryNotFoundException)
			{
				throw new TranslatableException(new("directory-not-found", "exceptions"));
			}
			catch (FileNotFoundException)
			{
				throw new TranslatableException(new("file-not-found", "exceptions"));
			}
			catch (NotSupportedException)
			{
				throw new TranslatableException(new("path-not-supported", "exceptions"));
			}
			catch (SecurityException)
			{
				throw new TranslatableException(new("file-read-insufficient-permission", "exceptions"));
			}
			catch (IOException)
			{
				throw new TranslatableException(new("path-inaccessible", "exceptions"));
			}
		}
		//Pokusi se nacist objekt ze souboru
		public static OfType? GetJsonContents<OfType>(string path)
		{
			//Nacteni obsahu souboru
			string contents = GetContents(path);
			//Prevod na objekt
			return JsonConverter.ConvertFromJson<OfType>(contents);
		}


		//Pokusi se zapsat obsah do souboru
		public static bool PutContents(string path, string contents)
		{
			//Pokud soubor neexistuje, musime si overit, ze alespon existuje cesta k souboru
			if (!Exists(path))
			{
				//Ziskani cesty ke slozce
				string directory = DirectoryManager.GetParentDirectory(path);
				//Pokud slozka neexistuje, je treba ji vytvorit
				if (!DirectoryManager.Exists(directory))
				{
					DirectoryManager.Create(directory);
				}
			}
			try
			{
				//Zapis do souboru
				File.WriteAllText(path, contents);
				return true;
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-file", "exceptions"));
			}
			catch (UnauthorizedAccessException)
			{
				throw new TranslatableException(new("unauthorized-access", "exceptions"));
			}
			catch (PathTooLongException)
			{
				throw new TranslatableException(new("path-too-long", "exceptions"));
			}
			catch (DirectoryNotFoundException)
			{
				throw new TranslatableException(new("directory-not-found", "exceptions"));
			}
			catch (NotSupportedException)
			{
				throw new TranslatableException(new("path-not-supported", "exceptions"));
			}
			catch (SecurityException)
			{
				throw new TranslatableException(new("file-read-insufficient-permission", "exceptions"));
			}
			catch (IOException)
			{
				throw new TranslatableException(new("path-inaccessible", "exceptions"));
			}
		}
		//Pokusi se zapsat objekt do souboru
		public static bool PutJsonContents<OfType>(string path, OfType obj)
		{
			//Prevod na json
			string contents = JsonConverter.ConvertToJson(obj);
			//Zapis do souboru
			return PutContents(path, contents);
		}
	}
}