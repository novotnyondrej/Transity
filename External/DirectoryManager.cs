using System.IO;

namespace Transity.External
{
	//Stara se o slozky a jejich spravu
	internal static class DirectoryManager
	{
		//Vyhodnoti zda cesta existuje nebo ne
		public static bool Exists(string path)
		{
			return Directory.Exists(path);
		}


		//Ziska nazev slozky
		public static string? GetFolderName(string path)
		{
			return Path.GetFileName(path);
		}
		//Ziska cestu ke slozce
		public static string? GetDirectory(string path)
		{
			return Path.GetDirectoryName(path);
		}


		//Nacte slozky ve slozce
		public static IEnumerable<string> GetFolders(string path)
		{
			return Directory.GetDirectories(path);
		}
		//Nacte soubory ve slozce
		public static IEnumerable<string> GetFiles(string path)
		{
			return Directory.GetFiles(path);
		}
		
		
		//Vytvori slozku
		public static bool Create(string path)
		{
			Directory.CreateDirectory(path);
			return true;
		}
		//Smaze slozku
		public static bool Delete(string path)
		{
			Directory.Delete(path);
			return true;
		}
	}
}