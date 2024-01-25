using System;
using System.Text;

namespace Game.Diagnostics.IO
{
	public class Console
	{
		public static void Error(params object[] ps)
		{
			var sbd = new StringBuilder();
			foreach (var p in ps)
			{
				sbd.Append(p.ToString());
			}
			var ret = sbd.ToString();
			Diagnostics.Console.LogError(ret);
		}
		public static void Log(params object[] ps)
		{
			var sbd = new StringBuilder();
			foreach (var p in ps)
			{
				sbd.Append(p.ToString());
			}
			var ret = sbd.ToString();
			Diagnostics.Console.Log(ret);
		}
		public static void Warn(params object[] ps)
		{
			var sbd = new StringBuilder();
			foreach (var p in ps)
			{
				sbd.Append(p.ToString());
			}
			var ret = sbd.ToString();
			Diagnostics.Console.LogWarning(ret);
		}
		public static void Exception(Exception exception)
		{
			Diagnostics.Console.LogException(exception);
		}
	}
}
