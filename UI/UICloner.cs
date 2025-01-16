using System.Windows;
using System.IO;
using System.Windows.Markup;
using System.Xml;

namespace Transity.UI
{
	//https://shrinandvyas.blogspot.com/2011/08/wpf-how-to-deep-copy-wpf-object-eg.html
	//Klonovani elementu
	internal static class UICloner
	{
		//Klonuje objekt a prideli mu jmeno
		public static T Clone<T>(this T element, string name) where T : FrameworkElement
		{
			//Klonovani objektu
			string xaml = XamlWriter.Save(element);
			StringReader xamlString = new(xaml);
			XmlTextReader xmlTextReader = new(xamlString);
			T clonedObject = (T)XamlReader.Load(xmlTextReader);
			//Nastaveni jmena
			clonedObject.Name = name;
			//Vysledek
			return clonedObject;
		}
	}
}