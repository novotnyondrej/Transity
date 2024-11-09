using System.Text.RegularExpressions;
using Transity.Data;
using Transity.External;
using Transity.General;
using Transity.General.Exceptions;

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
		public static string TranslationsLocation { get => DirectoryManager.ProjectDirectory + "Resources\\Translations\\"; }


		//Seznam dostupnych jazyku
		//To ze je seznam vyplneny, neznamena ze vsechny jazyky jsou nactene, pouze to poukazuje na dostupnost daneho jazyka
		private static List<string> _AvailableLanguages { get; set; }
		public static IEnumerable<string> AvailableLanguages => _AvailableLanguages;
		//Seznam dostupnych sad prekladu pro jednotlive jazyky
		//Klicem je oznaceni jazyka, hodnotou je seznam nazvu jednotlivych sad
		//Pokud dany jazyk nema definovane dostupne sady prekladu, pak sady existovat mohou, ale nejsou jenom nactene
		//Nektere prekladove sady se nemusi podarit nacist, pokud se tomu tak stane, sada bude odebrana ze seznamu a program se ji tak nadale nebude snazit nacist
		//Poukazuje pouze na dostupnost prekladovych sad - nenaznacuje, ze je sada nactena
		private static Dictionary<string, List<string>> AvailableLanguageSets { get; } = [];
		//Nactene preklady
		//Klicem 1 je oznaceni jazyka
		//Klicem 2 je nazev prekladove sady
		//Klicem 3 je prekladovy klic
		//Hodnotou je vysledny preklad
		private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> LoadedTranslations { get; } = [];


		//Prvotni nacteni
		static Translator()
		{
			AppSettings userSettings = AppSettings.UserSettings;
			bool loadOnStartup = userSettings.LoadTranslationsOnStartup;

			//Nacteni dostupnych jazyku
			_AvailableLanguages = [];
			LoadAvailableLanguages();
			//Prvotni nacteni prekladu
			OnTranslationStrategyChanged(loadOnStartup);

			//Event listenery
			//Pri zmene jazyka dojde k odnacteni daneho jazyka a pripadne nacteni toho noveho, zalezi na nastaveni
			userSettings.OnTargetLanguageChanged += OnLanguagePreferencesChanged;
			userSettings.OnBackupLanguageChanged += OnLanguagePreferencesChanged;
			userSettings.OnTranslationLoadStrategyChanged += OnTranslationStrategyChanged;
		}
		//Event listener na zmenu jazyka
		private static void OnLanguagePreferencesChanged(string? originalLanguage, string? newLanguage)
		{
			AppSettings userSettings = AppSettings.UserSettings;
			string targetLanguage = userSettings.TargetLanguage;
			string backupLanguage = userSettings.BackupLanguage;
			bool loadOnStartup = userSettings.LoadTranslationsOnStartup;

			//Kontrola, jestli je puvodni jazyk stale aktivni
			if (originalLanguage is not null && originalLanguage != targetLanguage && originalLanguage != backupLanguage)
			{
				//Jazyk se jiz nepouziva, pokud existuje, tak ho odnacteme - az na prekladovou sadu "languages"
				if (LoadedTranslations.TryGetValue(originalLanguage, out Dictionary<string, Dictionary<string, string>>? translationSets))
				{
					LoadedTranslations[originalLanguage] = translationSets.Where(
						(pair) => pair.Key == "languages"
					).ToDictionary();
				}
			}
			//Nacteni noveho jazyka, pokud ho mame nacist pri startu
			if (newLanguage is not null && loadOnStartup)
			{
				LoadAllTranslationSets(newLanguage);
			}
		}
		//Event listener na zmenu strategie nacitani jazyku
		private static void OnTranslationStrategyChanged(bool newValue)
		{
			//Pokud se jazyky nemaji nacitat, tak neni co menit
			if (newValue == false) return;
			//Probehne nacteni jazyku

			//Nacteni nastaveni
			AppSettings userSettings = AppSettings.UserSettings;
			string targetLanguage = userSettings.TargetLanguage;
			string backupLanguage = userSettings.BackupLanguage;

			//Nacteni jazyku
			LoadAllTranslationSets(targetLanguage);
			LoadAllTranslationSets(backupLanguage);
			//Nacteni sady s preklady jazyku pro vsechny ostatni jazyky
			foreach (string language in _AvailableLanguages)
			{
				LoadTranslationSet(language, "languages");
			}
		}


		//Nacte dostupne jazyky
		private static void LoadAvailableLanguages()
		{
			//Ziskani jazykovych sad ve slozce s preklady
			IEnumerable<string> availableLanguagesPaths = DirectoryManager.GetDirectories(TranslationsLocation);
			//Ziskani nazvu jazyku
			IEnumerable<string> availableLanguages = availableLanguagesPaths.Select(DirectoryManager.GetDirectoryName);
			//Filtrace neplatnych nazvu
			availableLanguages = availableLanguages.Where((language) => language == ToValidKey(language));
			_AvailableLanguages = availableLanguages.ToList();
		}
		//Nacte dostupne prekladove sady pro dany jazyk
		private static IEnumerable<string> GetAvailableTranslationSetNames(string language)
		{
			//Kontrola, jestli je jazyk podporovany
			if (!IsLanguageSupported(language)) return [];
			//Pokud jiz byly prekladove sady nacteny drive, vratime je
			if (AvailableLanguageSets.TryGetValue(language, out List<string>? translationSetNames)) return translationSetNames;

			//Pokud prekladove sady doposud nebyly nacteny, napravime to
			IEnumerable<string> availableTranslationSetPaths = DirectoryManager.GetFiles(TranslationsLocation + "\\" + language);
			//Filtrace souboru s priponou json
			availableTranslationSetPaths = availableTranslationSetPaths.Where((setPath) => FileManager.GetFileExtension(setPath) == "json");

			//Ziskani nazvu prekladovych sad
			IEnumerable<string> availableTranslationSetNames = availableTranslationSetPaths.Select(FileManager.GetFileName);
			//Filtrace neplatnych nazvu sad
			availableTranslationSetNames = availableTranslationSetNames.Where((setName) => setName == ToValidKey(setName));

			//Ulozeni dostupnych sad pro pristi nacteni
			AvailableLanguageSets[language] = availableTranslationSetNames.ToList();
			return availableTranslationSetNames;
		}
		//Nacte prekladovou sadu
		private static Dictionary<string, string> LoadTranslationSet(string language, string setName)
		{
			//Kontrola, jestli je prekladova sada podporovana
			if (!IsTranslationSetSupported(language, setName)) return [];

			//Vysledna prekladova sada
			Dictionary<string, string>? translationSet;
			//Pokud jiz byla prekladove sada nactena drive, vratime ji
			if (LoadedTranslations.TryGetValue(language, out Dictionary<string, Dictionary<string, string>>? translationSets))
			{
				if (translationSets.TryGetValue(setName, out translationSet)) return translationSet;
			}
			
			//Pokud prekladova sada doposud nebyla nactena, napravime to
			try
			{
				translationSet = FileManager.GetJsonContents<Dictionary<string, string>>(TranslationsLocation + "\\" + language + "\\" + setName + ".json");
			}
			catch (Exception exception)
			{
				//Prekladovou sadu se nepodarilo nacist, pravdepodobne je v danem souboru chyba
				//Odebrani sady ze seznamu dostupnych sad, protoze ji nelze nacist, abychom se neobtezovali ji priste nacist znovu
				RemoveAvailableTranslationSet(language, setName);
				//Vyhozeni vyjimky
				throw new TranslatableException(new(
					"load-translation-set-failed-due-to-error",
					"exceptions",
					new() {
						{ "language", language },
						{ "set-name", setName },
						{ "original-message", exception.Message }
					}
				));
			}
			//Pokud sada nebyla nactena, pravdepodobne nastala nejaka chyba
			if (translationSet is null)
			{
				RemoveAvailableTranslationSet(language, setName);
				//Vyhozeni vyjimky
				throw new TranslatableException(new(
					"load-translation-set-failed",
					"exceptions",
					new() {
						{ "language", language },
						{ "set-name", setName }
					}
				));
			}
			//Pokud je sada prazdna, tak neni treba si o ni udrzovat info
			if (translationSet.Count <= 0)
			{
				//Odebrani sady, protoze je zbytecna
				RemoveAvailableTranslationSet(language, setName);
				return translationSet;
			}

			//Sada byla uspesne nactena, pridame ji do seznamu nactenych prekladu pro pristi pouziti
			if (!LoadedTranslations.ContainsKey(language)) LoadedTranslations[language] = [];
			LoadedTranslations[language][setName] = translationSet;
			//OK
			return translationSet;
		}
		//Nacte vsechny prekladove sady pro dany jazyk
		private static Dictionary<string, Dictionary<string, string>> LoadAllTranslationSets(string language)
		{
			//Kontrola, ze je jazyk podporovan
			if (!IsLanguageSupported(language)) return [];
			//Ziskani vsech dostupnych sad
			IEnumerable<string> availableTranslationSets = GetAvailableTranslationSetNames(language);
			//Vysledne prekladove sady
			Dictionary<string, Dictionary<string, string>> translationSets = [];

			//Nacteni prekladovych sad
			foreach (string setName in availableTranslationSets)
			{
				//Pokus o nacteni sady
				Dictionary<string, string> translationSet = SafeExecutor.Execute(LoadTranslationSet, language, setName, []);
				//Prazdne sady jsou nam k nicemu
				if (translationSet.Count <= 0) continue;
				//Pridani sady
				translationSets[setName] = translationSet;
			}
			return translationSets;
		}
		//Nacte vsechny preklady
		private static void LoadAllTranslations()
		{
			foreach (string language in _AvailableLanguages) LoadAllTranslationSets(language);
		}


		//Odebere dostupnou prekladovou sadu (napriklad proto, ze je v ni chyba)
		private static void RemoveAvailableTranslationSet(string language, string setName)
		{
			//Kontrola, jestli je jazyk podporovan
			if (!IsLanguageSupported(language)) return;
			//Odebrani sady
			AvailableLanguageSets[language].Remove(setName);

			//Pokud jiz nejsou zadne sady v danem jazyce, neni treba si o nem udrzovat zaznam
			if (AvailableLanguageSets[language].Count <= 0)
			{
				_AvailableLanguages.Remove(language);
				AvailableLanguageSets.Remove(language);
			}
		}


		//Zda je jazyk podporovan
		public static bool IsLanguageSupported(string language) => _AvailableLanguages.Contains(language);
		//Zda je prekladova sada podporovana
		private static bool IsTranslationSetSupported(string language, string setName) => GetAvailableTranslationSetNames(language).Contains(setName);
		//Prevede string na platny klic
		private static string ToValidKey(string key)
		{
			key = key.Trim();

			//Nahrazeni pripadneho podtrzitka pomlckou
			key = key.Replace('_', '-');
			//SnakeCase to kebab-case
			key = Regex.Replace(key, @"([a-z])([A-Z]+)", "$1-$2").ToLower();
			//Filtr klice
			key = Regex.Match(key, @"[a-z0-9-]{1,64}").Value;
			//Odebrani opakujicich se polmcek
			key = Regex.Replace(key, @"(-+)", "-");
			return key;
		}


		//Nacte preklad
		public static string LoadTranslation(TranslationKey key, string? requestedLanguage = null)
		{
			AppSettings userSettings = AppSettings.UserSettings;
			string targetLanguage = userSettings.TargetLanguage;
			string backupLanguage = userSettings.BackupLanguage;


			string language = (ToValidKey(requestedLanguage ?? targetLanguage));
			string? setName = (key.Set is not null ? ToValidKey(key.Set) : null);
			string translationKey = ToValidKey(key.Key);

			//Prekladova sada, ktera splnuje pozadavek
			Dictionary<string, string> targetTranslationSet;
			//Pokud zname prekladovou sadu, muzeme ji rovnou nacist
			if (setName is not null)
			{
				//Nacteni konkretni prekladove sady
				targetTranslationSet = SafeExecutor.Execute(LoadTranslationSet, language, setName, []);
			}
			else
			{
				//Nacteni vsech prekladovych sad v jazyce
				Dictionary<string, Dictionary<string, string>> allTranslationSets = LoadAllTranslationSets(language);
				//Vybrani vyhovujici sady
				targetTranslationSet = allTranslationSets.FirstOrDefault(
					(pair) => pair.Value.ContainsKey(translationKey)
				).Value;
			}
			//Kontrola, jestli preklad existuje
			if (!targetTranslationSet.ContainsKey(translationKey))
			{
				//Preklad v tomto jazyce neexistuje.
				//Muzeme se ho pokusit v nasem pozadovanem jazyce
				if (language != targetLanguage && language != backupLanguage) return LoadTranslation(key, targetLanguage);
				//Nebo take v zaloznim jazyce
				if (language == targetLanguage) return LoadTranslation(key, backupLanguage);
				//Tak toto je konecna, uz nemame dalsi tipy, musime proste vratit nazev klice
				return translationKey;
			}
			//Vraceni prekladu
			return key.FillVariables(targetTranslationSet[translationKey], language);
		}
	}
}