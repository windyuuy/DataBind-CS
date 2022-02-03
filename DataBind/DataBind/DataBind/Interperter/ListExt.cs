
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataBinding.CollectionExt
{

	public static class ListExt
	{
		public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> selector)
		{
			var i = 0;
			foreach (var item in source)
			{
				selector(item, i);
				i++;
			}
		}
		public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> selector)
		{
			foreach (var item in source)
			{
				selector(item);
			}
		}

		public static List<TSource> ToConvableList<TSource>(this IEnumerable<TSource> source)
		{
			return new List<TSource>(source);
		}

	}

}
