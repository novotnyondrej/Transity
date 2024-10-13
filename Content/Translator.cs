using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Transity.External;

namespace Transity.Content
{
	/*
	Hierarchie pro preklady obsahu:
		- V hlavni slozce s preklady bude slozka se zkratkou jazyka podle ISO-639-1, v pripade potreby podle ISO-639-2
			- V teto slozce budou brany v potaz pouze souboru, ktere budou ve formatu json.
			- Nazev souboru bude naznacovat set prekladu.
			- Obsah souboru bude ve formatu json a bude obsahovat jednu Dictionary<string, string>
			- Klice prekladu budou psany v kebab case - malymi pismeny, kde budou jednotliva slova oddelena "-"
	Pouzivane sety prekladu:
		- Languages
			- Preklady nazvu cizich jazyku

	Pokud v pozadovanem jazyce preklad neexistuje, Translator se ho alespon pokusi nacist v zaloznim jazyce (jak definovano ve tride)
	Pokud nevyjde nacteni prekladu, bude navracen klic prekladu
	*/
	//Trida pro prekladani obsahu
	internal static class Translator
	{
		//Umisteni slozky s preklady
		public static string TranslationsLocation { get; } = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Resources/Translations/";
		//Zalozni jazyk pri neuspechu nacteni prekladu
		public static string BackupLanguage { get; } = "en";
		//Pozadovany jazyk
		public static string DesiredLanguage { get; } = "cs";


		//Nactene preklady, kde klicem je oznaceni jazyka, hodnotou je Dictionary<string, Dictionary<string, string>>, ve ktere je klicem nazev setu a hodnotou jednotlive preklady
		private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> LoadedTranslations { get; } = [];
		//Seznam kompletne nactenych jazyku
		private static List<string> LoadedLanguages { get; } = [];


		//Zda je jazyk plne nacteny
		private static bool IsLanguageFullyLoaded(string languageKey) => LoadedLanguages.Contains(languageKey);
		//Zda jazyk existuje
		private static bool IsLanguageLoaded(string languageKey) => LoadedTranslations.ContainsKey(languageKey);
		//Zda set existuje
		private static bool IsSetLoaded(string languageKey, string setKey) => IsLanguageLoaded(languageKey) && LoadedTranslations[languageKey].ContainsKey(setKey);


		//Vyhodnoti, zda je nazev jazyka/setu platny
		private static bool ValidName(string language, string? setName = null)
		{
			language = language.Trim();
			setName = (setName == null ? null : setName.Trim());

			//Nesmime povolit nacitani jazyka mimo slozku jazyku
			if (language.StartsWith("..")) return false;
			if (setName != null && (setName.StartsWith("..") || setName.Contains("/..") || setName.Contains("\\.."))) return false;
			return true;
		}
		//Nacte set ze souboru
		private static Dictionary<string, string> LoadSetFromFile(string language, string setName)
		{
			//Validace umisteni
			if (!ValidName(language, setName)) throw new Exception();

			//Cesta k setu
			string setLocation = $"{TranslationsLocation}{language}/{setName}.json";
			//Nacteni setu
			Dictionary<string, string> set = FileManager.GetJsonContents<Dictionary<string, string>>(setLocation) ?? [];
			//Zajisteni spravnych klicu
			set = set.Select(
				(pair) => new KeyValuePair<string, string>(ToValidKey(pair.Key), pair.Value)
			).ToDictionary(
				(pair) => pair.Key,
				(pair) => pair.Value
			);
			return set;
		}
		//Nacte nazvy dostupnych setu v jazyce
		private static IEnumerable<string> LoadLanguageSetNames(string language)
		{
			//Validace umisteni 
			if (!ValidName(language)) throw new Exception();

			//Cesta k jazyku
			string languageLocation = $"{TranslationsLocation}{language}/";
			//Ziskani vsech setu daneho jazyka
			IEnumerable<string> sets = DirectoryManager.GetFiles(languageLocation);
			//Prevod pouze na nazvy setu
			sets = sets.Where(
				(setLocation) => FileManager.GetFileExtension(setLocation) == "json"
			).Select(
				(setLocation) => FileManager.GetFileName(setLocation)
			);
			return sets;
		}
		//Nacte dostupne jazyky
		private static IEnumerable<string> LoadAvailableLanguageNames()
		{
			//Ziskani vsech slozek
			IEnumerable<string> languages = DirectoryManager.GetFolders(TranslationsLocation);
			//Prevod na nazvy jazyku
			languages = languages.Select(
				(languageLocation) => DirectoryManager.GetFolderName(languageLocation) ?? ""
			).Where(
				(language) => language.Length > 0
			);
			return languages;
		}


		//Nacte set
		private static void LoadSet(string language, string setName)
		{
			language = language.Trim();
			setName = setName.Trim();
			//Prevod na klice
			string languageKey = ToValidKey(language);
			string setKey = ToValidKey(setName);

			//Jestli je set nacteny, neni duvod to delat znovu
			if (IsSetLoaded(languageKey, setKey)) return;

			//Nacteni setu
			Dictionary<string, string> set = LoadSetFromFile(language, setName);
			//Kontrola setu
			if (set.Count <= 0) return;

			//Pridani setu k jazyku
			if (!LoadedTranslations.ContainsKey(languageKey)) LoadedTranslations[languageKey] = [];
			LoadedTranslations[languageKey][setKey] = set;
		}
		//Nacte jazyk
		public static void LoadLanguage(string language)
		{
			//Klic jazyka
			string languageKey = ToValidKey(language);
			//Kontrola, jestli jazyk uz neni nacteny - abychom nemuseli nacitat vsechno od zacatku
			if (LoadedLanguages.Contains(languageKey)) return;

			//Ziskani vsech setu daneho jazyka
			IEnumerable<string> sets = LoadLanguageSetNames(language);
			//Nacteni prekladu jednotlivych setu
			foreach (string setName in sets)
			{
				LoadSet(language, setName);
			}
			//Pridani jazyku do seznamu nactenych jazyku
			LoadedLanguages.Add(languageKey);
        }
		//Nacte vsechny jazyky
		public static void LoadAllLanguages()
		{
			IEnumerable<string> languages = LoadAvailableLanguageNames();
			//Nacteni jednotlivych jazyku
			foreach (string language in languages)
			{
				LoadLanguage(language);
			}
		}


		//Nacte preklad
		public static string LoadTranslation(TranslationKey key, string? targetLanguage = null)
		{
			string keyStr = ToValidKey(key.Key);
			string? targetSet = key.Set;
			string? setKey = (targetSet != null ? ToValidKey(targetSet) : null);
			string language = targetLanguage ?? DesiredLanguage;
			string languageKey = ToValidKey(language);
			//Zda set je definovan
			bool hasSet = (setKey != null);

			//Pokud je specifikovan set, nacteme ho
			if (hasSet) LoadSet(language, targetSet ?? "");
			//Jinak musime bohuzel nacist cely jazyk
			else LoadLanguage(language);

			//Kontrola dostupnosti
			if (!IsLanguageLoaded(languageKey) || (hasSet && !IsSetLoaded(languageKey, setKey ?? "")))
			{
				//Jazyk/set neexistuje/nelze nacist
				//Pokud jsme prave nenacitali zalozni jazyk, pokusime se najit preklad pro nej
				if (language != BackupLanguage) return LoadTranslation(key, BackupLanguage);
				//Pokud jsme prave nacitali zalozni jazyk, a pro nej to taky nevyslo, nezbyva nam nic jineho nez vratit klic ve sve plne krase
				return keyStr;
			}
			//Jazyk/set existuje, ale neni zarucene, ze existuje preklad
			//Set, ktery splnuje pozadavky
			Dictionary<string, string>? matchingSet;
			//Pokus o nalezeni prekladu
			if (hasSet)
			{
				matchingSet = LoadedTranslations[languageKey][setKey ?? ""];
			}
			else
			{
				matchingSet = LoadedTranslations[languageKey].Values.FirstOrDefault(
					(set) => set.ContainsKey(keyStr)
				);
			}
			//Nacteni prekladu
			string? translation = (matchingSet == null ? null : matchingSet.GetValueOrDefault(keyStr));

			//Kontrola prekladu
			if (translation == null || translation.Length <= 0)
			{
				//Preklad neexistuje
				//Pokud jsme prave nenacitali zalozni jazyk, pokusime se najit preklad pro nej
				if (language != BackupLanguage) return LoadTranslation(key, BackupLanguage);
				//Pokud jsme prave nacitali zalozni jazyk, a pro nej to taky nevyslo, nezbyva nam nic jineho nez vratit klic ve sve plne krase
				return keyStr;
			}
			//Nacteni promennych do prekladu
			return key.FillVariables(translation ?? "", language);
		}


		//Prevede string na platny klic
		private static string ToValidKey(string key)
		{
			key = key.Trim().ToLower();

			//Regex
			Regex regex = new(@"[a-z0-9-_]{1,64}");
			//Filtr klice
			key = regex.Match(key).Value;

			//Nahrazeni pripadneho podtrzitka pomlckou
			key = key.Replace('_', '-');
			return key;
		}
	}
}