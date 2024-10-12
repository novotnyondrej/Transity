using System.IO;

namespace Transity.External
{
	//Stara se o soubory, jejich nacitani a zapis do nich
	internal static class FileManager
	{
		//Zda soubor existuje
		public static bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}
		//Pokusi se precist obsah souboru
		public static string GetContents(string fileName)
		{
			//Kontrola existence souboru
			if (!FileExists(fileName)) throw new Exception();
			//Pokus o cteni souboru
			string contents = File.ReadAllText(fileName);
			return contents;
		}
		//Pokusi se zapsat obsah do souboru
		public static bool PutContents(string fileName, string contents)
		{
			//Zapis do souboru
			File.WriteAllText(fileName, contents);
			
			return true;
		}
	}
}