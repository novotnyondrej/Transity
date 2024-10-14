using Transity.Content;

namespace Transity.General.Exceptions
{
	//Vyjimka, kde zpravou je klic prekladu
	internal class TranslatableException : Exception
	{
		//Prekladovy klic
		private readonly TranslationKey MessageKey;
		public override string Message => Translator.LoadTranslation(MessageKey);

		//Konstruktor
		public TranslatableException(TranslationKey messageKey) : base(messageKey.Key)
		{
			//Nastaveni klice
			MessageKey = messageKey;
		}
	}
}