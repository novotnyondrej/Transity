using System.Windows;
using System.Windows.Controls;
using Transity.Content;
using Transity.Data;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages
{
	//Stranka s nastavenim aplikace
	public partial class SettingsPage : MainWindowChild
	{
		//Instance teto stranky
		private static readonly Dictionary<MainWindow, SettingsPage> Instances = [];
		

		public SettingsPage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje instance pro tohoto rodice
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani sama sebe do seznamu instanci
			Instances[parentWindow] = this;
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static SettingsPage GetInstance(MainWindow parentWindow, bool forceUpdate = true)
		{
			if (Instances.TryGetValue(parentWindow, out SettingsPage? value))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				SettingsPage instance = value;
				if (forceUpdate) instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
		}


		//Preloader
		public override void Preload()
		{
			base.Preload();
			//Nacteni jazyku
			int languagesCount = Translator.AvailableLanguages.Count();
			for (int languageNum = 0; languageNum < languagesCount; languageNum++)
			{
				//Ziskani jazyka
				string language = Translator.AvailableLanguages.ElementAt(languageNum);
				string primaryName = "primary_" + language;
				string secondaryName = "secondary_" + language;
				//Nacteni prekladu jazyku
				string targetTranslation = Translator.LoadTranslation(new(language, "languages"));
				string homeTranslation = Translator.LoadTranslation(new(language, "languages"), language);
				string translation = (targetTranslation == homeTranslation ? targetTranslation : targetTranslation + " (" + homeTranslation + ")");
				//Ziskani elementu s danym jazykem
				ComboBoxItem primaryElement = primaryLanguageComboBox.FindItemByName(primaryName) ?? primaryLanguageItemTemplate.Clone(primaryName);
				ComboBoxItem secondaryElement = secondaryLanguageComboBox.FindItemByName(secondaryName) ?? secondaryLanguageItemTemplate.Clone(secondaryName);
				//Nacteni textu
				primaryElement.Content = translation;
				secondaryElement.Content = translation;
				//Pridani vhodneho tagu
				primaryElement.Tag = language;
				secondaryElement.Tag = language;
				//Zapnuti by default
				primaryElement.IsEnabled = true;
				secondaryElement.IsEnabled = true;
				//Odebrani elementu
				if (primaryLanguageComboBox.Items.Contains(primaryElement)) primaryLanguageComboBox.Items.Remove(primaryElement);
				if (secondaryLanguageComboBox.Items.Contains(secondaryElement)) secondaryLanguageComboBox.Items.Remove(secondaryElement);
				//Vlozeni elementu na spravne misto
				primaryLanguageComboBox.Items.Insert(languageNum, primaryElement);
				secondaryLanguageComboBox.Items.Insert(languageNum, secondaryElement);
			}
			//Nacteni nastaveni
			string targetLanguage = AppSettings.UserSettings.TargetLanguage;
			string backupLanguage = AppSettings.UserSettings.BackupLanguage;
			ComboBoxItem? primaryItem = primaryLanguageComboBox.FindItemByName("primary_" + targetLanguage);
			ComboBoxItem? secondaryItem = secondaryLanguageComboBox.FindItemByName("secondary_" + backupLanguage);
			//Pokus o nalezeni elementu
			if (primaryItem is not null) primaryLanguageComboBox.SelectedItem = primaryItem;
			if (secondaryItem is not null) secondaryLanguageComboBox.SelectedItem = secondaryItem;
			//Checkboxy
			loadTranslationsOnStartupCheckbox.IsChecked = AppSettings.UserSettings.LoadTranslationsOnStartup;
			loadPagesOnStartupCheckbox.IsChecked = AppSettings.UserSettings.LoadPagesOnStartup;
		}


		//Vyhodnoti, zda bylo nastaveni upraveno
		private bool SettingsModified()
		{
			AppSettings userSettings = AppSettings.UserSettings;
			//Kontrola nastaveni
			if (userSettings.TargetLanguage != (string)((ComboBoxItem)primaryLanguageComboBox.SelectedItem).Tag) return true;
			if (userSettings.BackupLanguage != (string)((ComboBoxItem)secondaryLanguageComboBox.SelectedItem).Tag) return true;
			if (userSettings.LoadTranslationsOnStartup != (loadTranslationsOnStartupCheckbox.IsChecked ?? false)) return true;
			if (userSettings.LoadPagesOnStartup != (loadPagesOnStartupCheckbox.IsChecked ?? false)) return true;
			return false;
		}
		//Ulozi nastaveni
		public void SaveSettings()
		{
			AppSettings userSettings = AppSettings.UserSettings;
			//Ulozeni nastaveni
			userSettings.TargetLanguage = (string)((ComboBoxItem)primaryLanguageComboBox.SelectedItem).Tag;
			userSettings.BackupLanguage = (string)((ComboBoxItem)secondaryLanguageComboBox.SelectedItem).Tag;
			userSettings.LoadTranslationsOnStartup = loadTranslationsOnStartupCheckbox.IsChecked ?? false;
			userSettings.LoadPagesOnStartup = loadPagesOnStartupCheckbox.IsChecked ?? false;
		}


		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Odebrani predloh
			primaryLanguageComboBox.Items.Remove(primaryLanguageItemTemplate);
			secondaryLanguageComboBox.Items.Remove(secondaryLanguageItemTemplate);
			//Nacteni prekladu
			Preload();
		}
		//Zaskrtnute policko s nacitanim jazyka pri spusteni
		public void OnTranslationsCheckboxChecked(object sender, RoutedEventArgs e)
		{
			UITranslator.Translate(loadTranslationsOnStartupCheckbox);
		}
		public void OnTranslationsCheckboxUnchecked(object sender, RoutedEventArgs e)
		{
			UITranslator.Translate(loadTranslationsOnStartupCheckbox);
		}
		//Zaskrtnute policko s nacitanim stranek pri spusteni
		public void OnPagesCheckboxChecked(object sender, RoutedEventArgs e)
		{
			UITranslator.Translate(loadPagesOnStartupCheckbox);
		}
		public void OnPagesCheckboxUnchecked(object sender, RoutedEventArgs e)
		{
			UITranslator.Translate(loadPagesOnStartupCheckbox);
		}

		//Uzivatel klikl na tlacitko vratit se zpet
		public void OnBackButtonClicked(object sender, RoutedEventArgs e)
		{
			if (!SettingsModified()) ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
			else ParentWindow.ChangePage(UnsavedSettingsPage.GetInstance(ParentWindow));
		}
		//Uzivatel klikl na tlacitko ulozit
		public void OnSaveSettingsButtonClicked(object sender, RoutedEventArgs e)
		{
			SaveSettings();
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
	}
}