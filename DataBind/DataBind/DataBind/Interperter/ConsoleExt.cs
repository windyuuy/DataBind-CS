using System.Text;
using Game.Diagnostics;

public class console
{
	public static void error(params object[] ps)
	{
		var sbd=new StringBuilder();
		foreach (var p in ps)
		{
			sbd.Append(p.ToString());
		}
		var ret = sbd.ToString();
		Console.LogError(ret);
	}
	public static void log(params object[] ps)
	{
		var sbd = new StringBuilder();
		foreach (var p in ps)
		{
			sbd.Append(p.ToString());
		}
		var ret = sbd.ToString();
		Console.Log(ret);
	}
	public static void warn(params object[] ps)
	{
		var sbd = new StringBuilder();
		foreach (var p in ps)
		{
			sbd.Append(p.ToString());
		}
		var ret = sbd.ToString();
		Console.LogWarning(ret);
	}
}
