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