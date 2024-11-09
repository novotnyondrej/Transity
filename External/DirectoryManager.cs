using System.IO;
using System.Security;
using Transity.General.Exceptions;

namespace Transity.External
{
	//Stara se o slozky a jejich spravu
	internal static class DirectoryManager
	{
		//Ziska slozku tohoto projektu
		public static string ProjectDirectory
		{
			get
			{
				DirectoryInfo? info = null;

				try
				{
					info = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent;
				}
				catch (ArgumentException)
				{
					throw new TranslatableException(new("invalid-path-of-directory", "exceptions"));
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
					throw new TranslatableException(new("directory-access-insufficient-permission", "exceptions"));
				}
				catch (IOException)
				{
					throw new TranslatableException(new("path-inaccessible", "exceptions"));
				}
				//Kontrola, jestli se nam podarilo najit slozku
				if (info is null) throw new TranslatableException(new("project-directory-not-found", "exceptions"));
				//OK
				return info.FullName;
			}
		}


		//Vyhodnoti zda cesta dana slozka existuje nebo ne
		public static bool Exists(string path)
		{
			return Directory.Exists(path);
		}
		//Vytvori slozku s danou cestou
		public static bool Create(string path)
		{
			try
			{
				Directory.CreateDirectory(path);
				return true;
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-directory", "exceptions"));
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
			catch (IOException)
			{
				throw new TranslatableException(new("path-inaccessible", "exceptions"));
			}
		}
		//Smaze slozku
		public static bool Delete(string path)
		{
			try
			{
				Directory.Delete(path);
				return true;
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-directory", "exceptions"));
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
			catch (IOException)
			{
				throw new TranslatableException(new("path-inaccessible", "exceptions"));
			}
		}


		//Ziska nazev slozky z dane cesty
		public static string GetDirectoryName(string path)
		{
			try
			{
				return Path.GetFileName(path) ?? "";
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-directory", "exceptions"));
			}
		}
		

		//Ziska cestu ke slozce/souboru
		public static string GetParentDirectory(string path)
		{
			try
			{
				return Path.GetDirectoryName(path) ?? "";
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-directory", "exceptions"));
			}
			catch (PathTooLongException)
			{
				throw new TranslatableException(new("path-too-long", "exceptions"));
			}
		}


		//Nacte slozky na dane ceste
		public static IEnumerable<string> GetDirectories(string path)
		{
			try
			{
				return Directory.GetDirectories(path);
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-directory", "exceptions"));
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
			catch (IOException)
			{
				throw new TranslatableException(new("path-inaccessible", "exceptions"));
			}
		}
		//Nacte soubory ve slozce
		public static IEnumerable<string> GetFiles(string path)
		{
			try
			{
				return Directory.GetFiles(path);
			}
			catch (ArgumentException)
			{
				throw new TranslatableException(new("invalid-path-of-directory", "exceptions"));
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
			catch (IOException)
			{
				throw new TranslatableException(new("path-inaccessible", "exceptions"));
			}
		}
	}
}