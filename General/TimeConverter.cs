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
	}
}