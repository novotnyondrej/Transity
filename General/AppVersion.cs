using System.Collections;

namespace Transity.General
{
	/*
	Verzovani projektu:
		 a.b.cd?
		 | | ||
	i.e. 0.3.4x

	a	- Cislo vydani - pri zmene cisla vydani dochazi ke zmene obsahu, pocita se s tim, ze vsechno je otestovano a pripraveno k vydani
	b	- Cislo obsahu - jednotlive obsahy se skladaji do jednoho vydani
	c	- Cislo dodatku - dodatky jeden po druhem tvori obsah
	d	- Doplnujici informace k verzi - nepovinny.
		- Platne hodnoty:
			x - eXperiment - Experimentalni verze, ktera nebyla otestovana
			p - Patch - Oprava existujici chybi ve verzi
		- Je mozne, ze k verzi bude patrit x i p, v tomto pripade budou v nasledujicim poradi: xp

	Cislovani verzi:
		v-1.3.4 - vydani 1 s obsahem 3. Vydani 1 i obsah 3 jsou momentalne ve vyvoji. K obsahu doposud patri 4 dodatky
		v-1.3 - vydani 1, ktere je stale ve vyvoji, obsahujici obsah 3, ktery je nyni dokoncen a pripraven k pridani do vydani
		v-1 - vydani 1, ktere je pripravene ke spusteni
	*/
	//Trida pro poskytovani informaci o jedne konkretni verzi (budto te aktualni ci kompletne jine)
	internal class AppVersion
	{
		//Pripadna predpona verze
		public static string OptionalVersionPrefix { get; } = "v-";
		//Oddelovac jednotlivych cisel
		public static char NumberSeparator { get; } = '.';
		//Oznaceni doplnkovych informaci
		public static char ExperimentalVersionTag { get; } = 'x';
		public static char PatchVersionTag { get; } = 'p';

		//ID nejnovejsi verze (optimalne aktualizovat)
		public static string CurrentVersionID { get; } = "1.3.3";
		public static AppVersion Current { get; }


		//Cislo vydani
		public int ReleaseNumber { get; }
		//Cislo obsahu
		public int? FeatureNumber { get; }
		//Cislo dodatku k dosazeni daneho obsahu
		public int? StepNumber { get; }

		//Zda je verze experimentalni
		public bool Experimental { get; }
		//Zda verze obsahuje patch
		public bool Patched { get; }

		//Kompletni id verze
		public string VersionID
		{
			get
			{
				string version = $"{OptionalVersionPrefix}{ReleaseNumber}";
				//Obsah
				if (FeatureNumber != null) version += $".{FeatureNumber}";
				//Dodatek
				if (FeatureNumber != null && StepNumber != null) version += $".{StepNumber}";
				//Doplnujici informace
				if (Experimental) version += ExperimentalVersionTag;
				if (Patched) version += PatchVersionTag;
				return version;
			}
		}


		//Konstruktor
		AppVersion(string versionID)
		{
			//Prevedeni vsech charakteru na mala pismena v pripade pouziti velkych pismen
			versionID = versionID.Trim().ToLower();

			//Odebrani pripadneho "v-" na zacatku
			if (versionID.StartsWith(OptionalVersionPrefix))
			{
				versionID = versionID.Substring(OptionalVersionPrefix.Length);
			}
			//Nacteni doplnkovych informaci
			(Patched, versionID) = DetermineTag(versionID, PatchVersionTag);
			(Experimental, versionID) = DetermineTag(versionID, ExperimentalVersionTag);

			//Rozdeleni na jednotlive casti verze
			IEnumerator numbers = versionID.Split(NumberSeparator).GetEnumerator();
			//Nacteni cisel
			ReleaseNumber = LoadNextNumber(numbers, true) ?? 0;
			FeatureNumber = LoadNextNumber(numbers);
			StepNumber = LoadNextNumber(numbers);
		}
		//Nacteni aktualni verze pri spusteni
		static AppVersion()
		{
			Current = new(CurrentVersionID);
		}

		//Rozhodne, zda verze obsahuje urcite oznaceni na konci a vrati jeji podobu bez tohoto oznaceni
		private static (bool hasTag, string tagFree) DetermineTag(string version, char tag)
		{
			//Zda obsahuje oznaceni
			bool hasTag = version.EndsWith(tag);
			//Odebrani pripadneho tagu
			if (hasTag) version = version[..version.IndexOf(tag)];
			//Vysledek
			return (hasTag, version);
		}
		//Nacte cislo verze
		private static int? LoadNextNumber(IEnumerator numbers, bool required = false)
		{
			//Kontrola existence cisla
			if (!numbers.MoveNext())
			{
				//Jestli je cislo vyzadovano, tak je tady neco spatne
				if (required) throw new Exception();
				//OK, ale nespecifikovano
				return null;
			}
			//OK
			return ToInteger(numbers.Current?.ToString() ?? "");
		}
		//Prevede cislo verze ze stringu na int
		private static int ToInteger(string value)
		{
			return Convert.ToInt32(value);
		}
	}
}