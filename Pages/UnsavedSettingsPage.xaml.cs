﻿using System;
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
using Transity.General.Exceptions;
using Transity.UI;

namespace Transity.Pages
{
	/// <summary>
	/// Interaction logic for UnsavedSettingsPage.xaml
	/// </summary>
	public partial class UnsavedSettingsPage : MainWindowPage
	{
		private static Dictionary<MainWindow, UnsavedSettingsPage> Instances = new();

		//Konstruktor
		public UnsavedSettingsPage(MainWindow parentWindow) : base(parentWindow)
		{
			//Kontrola, jestli uz neexistuje
			if (Instances.ContainsKey(parentWindow)) throw new DetailedTranslatableException(new("page-already-exists", "exceptions"));
			//Pridani instance do seznamu
			Instances[parentWindow] = this;
			//Inicializace
			InitializeComponent();
		}
		//Ziska instanci stranky
		public static UnsavedSettingsPage GetInstance(MainWindow parentWindow)
		{
			if (Instances.ContainsKey(parentWindow))
			{
				//Instance jiz existuje, pouze ji ziskame, prelozime a vratime
				UnsavedSettingsPage instance = Instances[parentWindow];
				instance.Preload();
				return instance;
			}
			//Vytvorime novou instanci
			return new(parentWindow);
		}
		//Po nacteni elementu probehne automaticky preklad
		public void OnLoadEvent(object sender, RoutedEventArgs e)
		{
			//Nacteni prekladu
			Preload();
		}

		//Uzivatel kliknul na tlacitko zahodit zmeny
		public void OnRevertButtonClicked(object sender, RoutedEventArgs e)
		{
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
		//Uzivatel kliknul na tlacitko ulozit zmeny
		public void OnSaveButtonClicked(object sender, RoutedEventArgs e)
		{
			SettingsPage.GetInstance(ParentWindow, false).SaveSettings();
			ParentWindow.ChangePage(MainMenuPage.GetInstance(ParentWindow));
		}
	}
}
