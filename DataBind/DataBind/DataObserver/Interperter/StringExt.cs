
using System.Text.RegularExpressions;

public static class StringExt
{
	public static Match match(this string str, string regex, RegexOptions options)
	{
		var ret = new Regex(regex, options).Match(str);
		if (ret.Success)
		{
			return ret;
		}
		return null;
	}

	public static string charAt(this string str, int index)
	{
		return new System.String(str[index], 1);
	}
	public static int charCodeAt(this string str, int index)
	{
		return (int)str[index];
	}
	public static string[] split(this string str,string c)
	{
		return str.Split(c);
	}
	public static int length(this string str)
    {
		return str.Length;
	}
	public static int _P_length(this string str)
	{
		return str.Length;
	}

}
