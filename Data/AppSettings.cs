using Newtonsoft.Json;
using System.Net.Http.Headers;
using Transity.Content;
using Transity.External;

namespace Transity.Data
{
	//Nastaveni aplikace
	internal class AppSettings
	{
		//Nazev souboru s nastavenim
		private static readonly string SaveFileName = "app-settings";


		//Vychozi nastaveni
		public static AppSettings DefaultSettings = new("cs", "en", false, false);
		//Uzivatelske nastaveni
		public static AppSettings UserSettings;


		//Event na zmenu nastaveni
		public delegate void OnSettingsChangedDelegate(string propertyName);
		public event OnSettingsChangedDelegate OnSettingsChanged = delegate { };
		//Event na zmenu ciloveho jazyka
		public delegate void OnTargetLanguageChangedDelegate(string originalLanguage, string newLanguage);
		public event OnTargetLanguageChangedDelegate OnTargetLanguageChanged = delegate { };
		//Event na zmenu zalozniho jazyka
		public delegate void OnBackupLanguageChangedDelegate(string originalLanguage, string newLanguage);
		public event OnBackupLanguageChangedDelegate OnBackupLanguageChanged = delegate { };
		//Event na zmenu strategie nacitani jazyku
		public delegate void OnTranslationLoadStrategyChangedDelegate(bool newValue);
		public event OnTranslationLoadStrategyChangedDelegate OnTranslationLoadStrategyChanged = delegate { };
		//Event na zmenu strategie nacitani stranek
		public delegate void OnPagesLoadStrategyChangedDelegate(bool newValue);
		public event OnPagesLoadStrategyChangedDelegate OnPagesLoadStrategyChanged = delegate { };

		//Jazyk zobrazeni aplikace
		private string _TargetLanguage;
		[JsonProperty("target-language")]
		public string TargetLanguage
		{
			get => _TargetLanguage;
			set
			{
				//Ziskani aktualni hodnoty
				string current = _TargetLanguage;
				//Kontrola, jestli se hodnota zmenila
				if (current == value) return;
				//Kontrola, jestli je jazyk podporovany
				if (!Translator.IsLanguageSupported(value)) return;
				//Nastaveni jazyka
				_TargetLanguage = value;
				OnSettingsChanged.Invoke(nameof(TargetLanguage));
				OnTargetLanguageChanged.Invoke(current, value);
			}
		}
		
		//Zalozni jazyk
		private string _BackupLanguage;
		[JsonProperty("backup-language")]
		public string BackupLanguage
		{
			get => _BackupLanguage;
			set
			{
				//Ziskani aktualni hodnoty
				string current = _BackupLanguage;
				//Kontrola, jestli se hodnota zmenila
				if (current == value) return;
				//Kontrola, jestli je jazyk podporovany
				if (!Translator.IsLanguageSupported(value)) return;
				//Nastaveni jazyka
				_BackupLanguage = value;
				OnSettingsChanged.Invoke(nameof(BackupLanguage));
				OnBackupLanguageChanged.Invoke(current, value);
			}
		}
		
		//Zda se maji preklady nacist hned pri spusteni aplikace
		private bool _LoadTranslationsOnStartup;
		[JsonProperty("load-translations-on-startup")]
		public bool LoadTranslationsOnStartup
		{
			get => _LoadTranslationsOnStartup;
			set
			{
				//Ziskani aktualni hodnoty
				bool current = _LoadTranslationsOnStartup;
				//Kontrola, jestli se hodnota zmenila
				if (current == value) return;
				//Nastaveni nastaveni
				_LoadTranslationsOnStartup = value;
				OnSettingsChanged.Invoke(nameof(LoadTranslationsOnStartup));
				OnTranslationLoadStrategyChanged.Invoke(value);
			}
		}

		//Zda se maji stranky nacist hned pri spusteni aplikace
		private bool _LoadPagesOnStartup;
		[JsonProperty("load-pages-on-startup")]
		public bool LoadPagesOnStartup
		{
			get => _LoadPagesOnStartup;
			set
			{
				//Ziskani aktualni hodnoty
				bool current = _LoadPagesOnStartup;
				//Kontrola, jestli se hodnota zmenila
				if (current == value) return;
				//Nastaveni nastaveni
				_LoadPagesOnStartup = value;
				OnSettingsChanged.Invoke(nameof(LoadPagesOnStartup));
				OnPagesLoadStrategyChanged.Invoke(value);
			}
		}


		//Nacteni nastaveni pri startu
		static AppSettings()
		{
			UserSettings = AppDataManager.LoadData(null, SaveFileName, DefaultSettings);
		}
		//Konstruktor
		[JsonConstructor]
		public AppSettings(
			string targetLanguage,
			string backupLanguage,
			bool loadTranslationsOnStartup,
			bool loadPagesOnStartup
		)
		{
			_TargetLanguage = targetLanguage;
			_BackupLanguage = backupLanguage;
			_LoadTranslationsOnStartup = loadTranslationsOnStartup;
			_LoadPagesOnStartup = loadPagesOnStartup;
			//Pri zmene nastaveni probehne ulozeni dat
			OnSettingsChanged += SaveSettings;
		}
		//Ulozi nastaveni
		public void SaveSettings(string propertyName)
		{
			AppDataManager.SaveData(null, SaveFileName, this);
		}
	}
}
