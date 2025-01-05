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

		public static string GetContentTranslationKey(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			return (string)element.GetValue(ContentTranslationKeyProperty);
		}
		public static void SetContentTranslationKey(UIElement element, string value)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ContentTranslationKeyProperty, value);
		}
		//Prelozi dany element
		public static void Translate(this UIElement element, string? targetLanguage = null)
		{
			//Kontrola elementu
			if (element is null) throw new DetailedTranslatableException(new("nothing-to-translate", "exceptions"));
			//Ziskani moznych nazvu prekladovych klicu
			string? contentTranslationKeyName = (string?)element.GetValue(ContentTranslationKeyProperty);
			//Prevod na prekladove klice
			TranslationKey? contentTranslationKey = (contentTranslationKeyName is null ? null : new(contentTranslationKeyName, TranslationSetName));

			//Preklady
			if (element is ContentControl control)
			{
				//Preklad obsahu
				if (contentTranslationKey is not null) control.Content = Translator.LoadTranslation(contentTranslationKey, targetLanguage);
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
