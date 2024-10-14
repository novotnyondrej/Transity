using System.Diagnostics;
using Transity.Content;
using Transity.External;

namespace Transity.General.Exceptions
{
	//Vyjimka, kde zprava je prekladovy klic, a ke ktere se automaticky pripojuji informace o umisteni chyby
	internal class DetailedTranslatableException : TranslatableException
	{
		public override string Message
		{
			get
			{
				//Ziskani stack tracu
				StackTrace trace = new(this, true);
				//Ziskani podrobnosti o souboru
				StackFrame? latestFrame = trace.GetFrame(0);

				//Nacteni detailu
				string? fileName = latestFrame?.GetFileName();
				string? methodName = latestFrame?.GetMethod()?.Name;
				int? lineNumber = latestFrame?.GetFileLineNumber();

				TranslationKey detailsKey = new (
					"exception-details-text",
					"exceptions",
					new ()
					{
						{"file-name", fileName is not null ? FileManager.GetFileName(fileName) : "?"},
						{"method-name", methodName ?? "?"},
						{"line-number", lineNumber is not null ? lineNumber.ToString() ?? "?" : "?"}
					}
				);
				return base.Message + ' ' + Translator.LoadTranslation(detailsKey);
			}
		}

		//Konstruktor
		public DetailedTranslatableException(TranslationKey messageKey) : base(messageKey)
		{
		}
	}
}
