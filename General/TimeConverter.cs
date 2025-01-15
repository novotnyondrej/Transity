﻿using Transity.General.Exceptions;

namespace Transity.General
{
	//Trida pro prevody casu
	internal static class TimeConverter
	{
		//Vrati aktualni cas v sekundach
		//https://stackoverflow.com/questions/5918832/does-the-net-framework-support-getting-the-current-time-in-seconds-based-on-uni
		public static int GetTime()
		{
			TimeSpan timespan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
			return (int)timespan.TotalSeconds;
		}
		//Vynuti platny cas
		public static void ForceValidTime(int time)
		{
			//Kontrola casu
			if (time < 0) throw new DetailedTranslatableException(new("invalid-time", "exceptions", new() { { "time", time.ToString() } }));
			//Ziskani aktualniho casu
			int currentTime = GetTime();
			//Kontrola, ze cas neni v budoucnosti
			if (time > currentTime) throw new DetailedTranslatableException(new("invalid-time", "exceptions", new() { { "time", time.ToString() } }));
		}
	}
}