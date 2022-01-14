
using System;

public class console
{
	public static void error(params object[] ps)
	{
		foreach (var p in ps)
		{
			Console.Error.Write(p);
		}
		Console.Error.Write("\n");
	}
	public static void log(params object[] ps)
	{
		foreach (var p in ps)
		{
			Console.Out.Write(p);
		}
		Console.Out.Write("\n");
	}
	public static void warn(params object[] ps)
	{
		foreach (var p in ps)
		{
			Console.Out.Write(p);
		}
		Console.Out.Write("\n");
	}
}
