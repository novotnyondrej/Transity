using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transity.Content;
using Transity.Data;
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages
{
	/// <summary>
	/// Interaction logic for SettingsPage.xaml
	/// </summary>
	public partial class SettingsPage : MainWindowPage
	{
		//Jiz existujici instance
		private static Dictionary<MainWindow, SettingsPage> Instances = new();

		//Konstruktor
		public SettingsPage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentWindow] = this;
			//Inicializace
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static SettingsPage GetInstance(MainWindow parentWindow)
		{
			if (Instances.ContainsKey(parentWindow))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				SettingsPage instance = Instances[parentWindow];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
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
		//Ulozi nastaveni
		public void SaveSettings()
		{
			//Ulozeni nastaveni
			AppSettings.UserSettings.TargetLanguage = (string)((ComboBoxItem)primaryLanguageComboBox.SelectedItem).Tag;
			AppSettings.UserSettings.BackupLanguage = (string)((ComboBoxItem)secondaryLanguageComboBox.SelectedItem).Tag;
			AppSettings.UserSettings.LoadTranslationsOnStartup = loadTranslationsOnStartupCheckbox.IsChecked ?? false;
			AppSettings.UserSettings.LoadPagesOnStartup = loadPagesOnStartupCheckbox.IsChecked ?? false;
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
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
		//Uzivatel klikl na tlacitko ulozit
		public void OnSaveSettingsButtonClicked(object sender, RoutedEventArgs e)
		{
			SaveSettings();
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
	}
}
