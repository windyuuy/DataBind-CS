using System.Text.RegularExpressions;

namespace EngineAdapter.StringExt
{
	/// <summary>
	/// StringExt
	/// </summary>
	public static class StringExt
	{
		/// <summary>
		/// Match regex
		/// </summary>
		/// <param name="str"></param>
		/// <param name="regex"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static Match Match(this string str, string regex, RegexOptions options)
		{
			var ret = new Regex(regex, options).Match(str);
			if (ret.Success)
			{
				return ret;
			}

			return null;
		}

		/// <summary>
		/// CharAt index
		/// </summary>
		/// <param name="str"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static string CharAt(this string str, int index)
		{
			return new System.String(str[index], 1);
		}

		/// <summary>
		/// CharCodeAt index
		/// </summary>
		/// <param name="str"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static int CharCodeAt(this string str, int index)
		{
			return (int)str[index];
		}

		/// <summary>
		/// Split string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public static string[] Splits(this string str, string c)
		{
			return str.Split(new string[] { c }, System.StringSplitOptions.None);
		}

	}
}