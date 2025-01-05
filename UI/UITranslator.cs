using System.Windows;
using System.Windows.Controls;
using Transity.Content;
using Transity.General.Exceptions;

namespace Transity.UI
{
	//https://stackoverflow.com/questions/5782864/adding-custom-attributes-to-an-element-in-xaml
	internal static class UITranslator
	{
		//Nazev sady s preklady
		private static readonly string TranslationSetName = "ui";

		//Nazev prekladoveho klice pro obsah
		public static readonly DependencyProperty ContentTranslationKeyProperty = DependencyProperty.RegisterAttached(
			"ContentTranslationKey",
		   typeof(string),
		   typeof(UITranslator),
		   new FrameworkPropertyMetadata(null)
		);
		public static string? GetContentTranslationKey(UIElement element)
		{
			//Kontrola elementu
			if (element is null) throw new DetailedTranslatableException(new("ui-element-not-specified", "exceptions"));
			//Vraceni hodnoty
			return (string?) element.GetValue(ContentTranslationKeyProperty);
		}
		public static void SetContentTranslationKey(UIElement element, string? value)
		{
			//Kontrola elementu
			if (element is null) throw new DetailedTranslatableException(new("ui-element-not-specified", "exceptions"));
			//Nastaveni hodnoty
			element.SetValue(ContentTranslationKeyProperty, value);
		}

		//Nazev prekladoveho klice pro zaskrtnuty checkbox
		public static readonly DependencyProperty CheckboxCheckedContentTranslationKeyProperty = DependencyProperty.RegisterAttached(
			"CheckboxCheckedContentTranslationKey",
		   typeof(string),
		   typeof(UITranslator),
		   new FrameworkPropertyMetadata(null)
		);
		public static string? GetCheckboxCheckedContentTranslationKey(UIElement element)
		{
			//Kontrola elementu
			if (element is null) throw new DetailedTranslatableException(new("ui-element-not-specified", "exceptions"));
			//Vraceni hodnoty
			return (string?)element.GetValue(CheckboxCheckedContentTranslationKeyProperty);
		}
		public static void SetCheckboxCheckedContentTranslationKey(UIElement element, string? value)
		{
			//Kontrola elementu
			if (element is null) throw new DetailedTranslatableException(new("ui-element-not-specified", "exceptions"));
			//Nastaveni hodnoty
			element.SetValue(CheckboxCheckedContentTranslationKeyProperty, value);
		}

		//Nazev prekladoveho klice pro zaskrtnuty checkbox
		public static readonly DependencyProperty CheckboxUncheckedContentTranslationKeyProperty = DependencyProperty.RegisterAttached(
			"CheckboxUncheckedContentTranslationKey",
		   typeof(string),
		   typeof(UITranslator),
		   new FrameworkPropertyMetadata(null)
		);
		public static string? GetCheckboxUncheckedContentTranslationKey(UIElement element)
		{
			//Kontrola elementu
			if (element is null) throw new DetailedTranslatableException(new("ui-element-not-specified", "exceptions"));
			//Vraceni hodnoty
			return (string?)element.GetValue(CheckboxUncheckedContentTranslationKeyProperty);
		}
		public static void SetCheckboxUncheckedContentTranslationKey(UIElement element, string? value)
		{
			//Kontrola elementu
			if (element is null) throw new DetailedTranslatableException(new("ui-element-not-specified", "exceptions"));
			//Nastaveni hodnoty
			element.SetValue(CheckboxUncheckedContentTranslationKeyProperty, value);
		}



		//Prelozi dany element
		public static void Translate(this UIElement element, string? targetLanguage = null)
		{
			//Kontrola elementu
			if (element is null) throw new DetailedTranslatableException(new("nothing-to-translate", "exceptions"));
			//Ziskani moznych nazvu prekladovych klicu
			string? contentTranslationKeyName = (string?)element.GetValue(ContentTranslationKeyProperty);
			string? checkedContentTranslationKeyName = (string?)element.GetValue(CheckboxCheckedContentTranslationKeyProperty);
			string? uncheckedContentTranslationKeyName = (string?)element.GetValue(CheckboxUncheckedContentTranslationKeyProperty);
			//Prevod na prekladove klice
			TranslationKey? contentTranslationKey = (contentTranslationKeyName is null ? null : new(contentTranslationKeyName, TranslationSetName));
			TranslationKey? checkedContentTranslationKey = (checkedContentTranslationKeyName is null ? null : new(checkedContentTranslationKeyName, TranslationSetName));
			TranslationKey? uncheckedContentTranslationKey = (uncheckedContentTranslationKeyName is null ? null : new(uncheckedContentTranslationKeyName, TranslationSetName));

			//Preklady
			if (element is ContentControl control)
			{
				//Preklad obsahu
				if (contentTranslationKey is not null) control.Content = Translator.LoadTranslation(contentTranslationKey, targetLanguage);
			}
			if (element is CheckBox checkbox)
			{
				if (checkbox.IsChecked == true)
				{
					if (checkedContentTranslationKey is not null) checkbox.Content = Translator.LoadTranslation(checkedContentTranslationKey, targetLanguage);
				}
				else
				{
					if (uncheckedContentTranslationKey is not null) checkbox.Content = Translator.LoadTranslation(uncheckedContentTranslationKey, targetLanguage);
				}
			}
			//Preklad vsech potomku
			foreach (UIElement descendantElement in element.GetChildren())
			{
				//Preklad potomka
				descendantElement.Translate(targetLanguage);
			}
		}
	}
}
