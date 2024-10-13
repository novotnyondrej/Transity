using System.Reflection.Metadata;
using System.Windows;

namespace Transity.General
{
	//Stara se o spousteni programu v bezpecnem prostredi a stara se o zachyceni vyjimek
	internal static class SafeExecutor
	{
		//Spusti program v bezpecnem rezimu
		//Action
		public static void Execute(Action call)
		{
			try
			{
				//Pokus o zvolani funkce
				call();
			}
			catch (Exception e)
			{
				//Neprobehlo uspesne, report erroru
				HandleException(e);
			}
		}
		public static void Execute<T1>(Action<T1> call, T1 arg1)
		{
			try { call(arg1); }
			catch (Exception e) { HandleException(e); }
		}
		public static void Execute<T1, T2>(Action<T1, T2> call, T1 arg1, T2 arg2)
		{
			try { call(arg1, arg2); }
			catch (Exception e) { HandleException(e); }
		}
		public static void Execute<T1, T2, T3>(Action<T1, T2, T3> call, T1 arg1, T2 arg2, T3 arg3)
		{
			try { call(arg1, arg2, arg3); }
			catch (Exception e) { HandleException(e); }
		}
		//Func
		public static TResult Execute<TResult>(Func<TResult> call, TResult defaultValue)
		{
			try { return call(); }
			catch (Exception e) { HandleException(e); }
			return defaultValue;
		}
		public static TResult Execute<T1, TResult>(Func<T1, TResult> call, T1 arg1, TResult defaultValue)
		{
			try { return call(arg1); }
			catch (Exception e) { HandleException(e); }
			return defaultValue;
		}
		public static TResult Execute<T1, T2, TResult>(Func<T1, T2, TResult> call, T1 arg1, T2 arg2, TResult defaultValue)
		{
			try { return call(arg1, arg2); }
			catch (Exception e) { HandleException(e); }
			return defaultValue;
		}
		public static TResult Execute<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> call, T1 arg1, T2 arg2, T3 arg3, TResult defaultValue)
		{
			try { return call(arg1, arg2, arg3); }
			catch (Exception e) { HandleException(e); }
			return defaultValue;
		}

		public static void HandleException(Exception e)
		{
			MessageBox.Show(e.Message);
		}
	}
}