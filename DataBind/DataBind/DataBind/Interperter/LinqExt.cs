﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq.Ext
{
    public static class LinqExt
    {
        public static T TryGet<T>(this T[] ts, int index)
        {
            if (ts.Length > index)
            {
                return ts[index];
            }
            else
            {
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }
        public static T TryGet<K,T>(this IDictionary<K,T> dict,K key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }
        public static T[] Slice<T>(this T[] ts,int? start=null,int? ends=null)
        {
            int start1;
            if (start == null)
            {
                start1 = 0;
            }
            else
            {
                start1 = Math.Min(ts.Length, start.Value);
            }

            int ends1;
            if (ends == null)
            {
                ends1 = ts.Length;
            }
            else
            {
                ends1= Math.Min(ts.Length, ends.Value);
            }
            ends1 = Math.Max(ends1, start1);

            var count= ends1 -start1;
            var newTs=new T[count];
            for(var i=0; i<count; i++)
            {
                newTs[i]=ts[i+ start1];
            }
            return newTs;
        }
    }
}
