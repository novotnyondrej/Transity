namespace Transity.Content
{
	internal class TranslationKey
	{
		//Klic prekladu
		public string Key;
		public string? Set;
		//Promenne do prekladu
		private Dictionary<string, string> Variables;
		//Specialni promenne zavisle na jazyce
		private Dictionary<string, TranslationKey> TranslatableVariables;


		/*public TranslationKey(
			string key,
			string? set = null,
			Dictionary<string, string>? variables = null,
			Dictionary<string, TranslationKey>? translatableVariables = null
		)
		{
			Key = key;
			Set = set;
			Variables = variables ?? [];
			TranslatableVariables = translatableVariables ?? [];
		}*/
		//Konstruktor s objekty
		public TranslationKey(
			string key,
			string? set = null,
			Dictionary<string, object>? variables = null
		)
		{
			Key = key;
			Set = set;
			Variables = (variables ?? []).Where(
				(pair) => pair.Value is not TranslationKey
			).ToDictionary(
				(pair) => pair.Key,
				(pair) => pair.Value.ToString() ?? ""
			);
			TranslatableVariables = (variables ?? []).Where(
				(pair) => pair.Value is TranslationKey
			).ToDictionary(
				(pair) => pair.Key,
				(pair) => (TranslationKey)pair.Value
			);
		}

		//Vlozi promenne do prelozeneho textu
		public string FillVariables(string content, string? language)
		{
			//Vlozeni znamych promennych
			foreach (KeyValuePair<string, string> variable in Variables)
			{
				content = content.Replace("{" + variable.Key + "}", variable.Value);
			}
			//Vlozeni dalsich prekladu
			foreach (KeyValuePair<string, TranslationKey> variable in TranslatableVariables)
			{
				content = content.Replace("{" + variable.Key + "}", Translator.LoadTranslation(variable.Value, language));
			}
			return content;
		}
	}
}
