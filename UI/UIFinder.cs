using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Transity.UI
{
	//https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type
	internal static class UIFinder
	{
		//Vrati potomky daneho typu
		public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject dependencyObject) where T : DependencyObject
		{
			if (dependencyObject is ComboBox comboBox)
			{
				foreach (ComboBoxItem comboBoxItem in comboBox.Items)
				{
					//Pokud je naseho typu, pak vratit
					if (comboBoxItem is T targetObject) yield return targetObject;
				}
			}
			else
			{
				int childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
				for (int childObjectNum = 0; childObjectNum < childrenCount; childObjectNum++)
				{
					//Ziskani potomka
					DependencyObject childObject = VisualTreeHelper.GetChild(dependencyObject, childObjectNum);
					//Kontrola potomka
					if (childObject is null) continue;
					//Pokud je naseho typu, pak vratit
					if (childObject is T targetObject) yield return targetObject;
				}
			}
		}

		//Vrati vsechny elementy daneho typu, ktere patri tomuto elementu
		public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject dependencyObject) where T : DependencyObject
		{
			//Kontrola, jestli objekt existuje
			if (dependencyObject is null) yield return (T)Enumerable.Empty<T>();
			else
			{
				//Ziskani vsech potomku
				foreach (T childObject in dependencyObject.GetChildrenOfType<T>())
				{
					yield return childObject;
					//Vyhledani dalsich potomku
					foreach (T descendantObject in childObject.GetDescendantsOfType<T>()) yield return descendantObject;
				}
			}
		}
		//Vrati vsechny potomky nehlede na typ
		public static IEnumerable<UIElement> GetChildren(this DependencyObject dependencyObject)
		{
			//Nalezeni vsech elementu
			foreach (UIElement childObject in dependencyObject.GetChildrenOfType<UIElement>()) yield return childObject;
		}
		//Vrati uplne vsechny elementy nehlede na typ
		public static IEnumerable<UIElement> GetDescendants(this DependencyObject dependencyObject)
		{
			//Nalezeni vsech elementu
			foreach (UIElement descendantObject in dependencyObject.GetDescendantsOfType<UIElement>()) yield return descendantObject;
		}
		//Vyhleda item podle nazvu
		public static ComboBoxItem? FindItemByName(this ComboBox comboBox, string name)
		{
			foreach (ComboBoxItem item in comboBox.Items)
			{
				if (item.Name == name) return item;
			}
			return null;
		}
		//Vyhleda item podle nazvu
		public static ListBoxItem? FindItemByName(this ListBox listBox, string name)
		{
			foreach (ListBoxItem item in listBox.Items)
			{
				if (item.Name == name) return item;
			}
			return null;
		}
	}
}
