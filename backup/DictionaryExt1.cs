using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using vm;

namespace System.ListExt
{
	using TConvableDict = System.Collections.Generic.IDictionary<int, object>;
	public interface IConvableDictionary
	{
		TConvableDict RawDict { get; }
	}

	public partial class Dictionary<K, V> : Dictionary, System.Collections.Generic.IDictionary<K, V>, IConvableDictionary
	{
		protected static Dictionary<int, WeakReference> keyCache = new Dictionary<int, WeakReference>();
		protected K GetKey(int hashCode)
		{
			return (K)keyCache[hashCode].Target;
		}
		protected V GetValue(K key)
		{
			return Utils.ConvItem<V>(dict[key.GetHashCode()]);
		}
		protected void SetValue(K key, V value)
		{
			SaveKey(key);
			dict[key.GetHashCode()] = value;
		}
		protected void SaveKey(K key)
		{
			keyCache[key.GetHashCode()] = new WeakReference(key);
		}
		protected TConvableDict dict;
		public override TConvableDict RawDict => dict;

		public Dictionary(TConvableDict dict)
		{
			this.dict = dict;
		}

		public Dictionary()
		{
			this.dict = new System.Collections.Generic.Dictionary<int, object>();
		}

		public Dictionary(IConvableDictionary dict)
		{
			this.dict = dict.RawDict;
		}

		public virtual V this[K key]
		{
			get
			{
				return GetValue(key);
			}
			set
			{
				SetValue(key, value);
			}
		}

		public new virtual ICollection<K> Keys => dict.Keys.Select(h => GetKey(h)).ToArray();

		public new virtual ICollection<V> Values => new System.ListExt.List<V>(dict.Values);

		public override int Count => dict.Count;

		public override bool IsReadOnly => dict.IsReadOnly;

		public virtual void Add(K key, V value)
		{
			SaveKey(key);
			dict.Add(key.GetHashCode(), value);
		}

		public virtual void Add(KeyValuePair<K, V> item)
		{
			SaveKey(item.Key);
			dict.Add(item.Key.GetHashCode(), item.Value);
		}

		public override void Clear()
		{
			dict.Clear();
		}

		public virtual bool Contains(KeyValuePair<K, V> item0)
		{
			return dict.ContainsKey(item0.Key.GetHashCode()) && Object.ReferenceEquals(item0.Value, dict[item0.Key.GetHashCode()]);
		}

		public virtual bool ContainsKey(K key)
		{
			return dict.ContainsKey(key.GetHashCode());
		}

		public virtual void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			// dict.CopyTo(array, arrayIndex);
			var i = 0;
			foreach (var kv in dict)
			{
				if (i >= array.Length)
				{
					break;
				}
				array[i] = new KeyValuePair<K, V>(GetKey(kv.Key), Utils.ConvItem<V>(kv.Value));
				i++;
			}
		}

		public new virtual IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			// TODO: 优化性能
			// return dict.GetEnumerator();
			var dict2 = new Dictionary<K, V>();
			foreach (var kv in dict)
			{
				dict2.Add(kv.Key, Utils.ConvItem<V>(kv.Value));
			}
			return dict2.GetEnumerator();
		}

		public virtual bool Remove(K key)
		{
			return dict.Remove(key.GetHashCode());
		}

		public virtual bool Remove(KeyValuePair<K, V> item)
		{
			if (this.Contains(item))
			{
				return dict.Remove(item.Key.GetHashCode());
			}
			return false;
		}

		public virtual bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
		{
			object value2;
			var ret = dict.TryGetValue(key.GetHashCode(), out value2);
			if (ret)
			{
				value = Utils.ConvItem<V>(value2);
			}
			else
			{
				value = default(V);
			}
			return ret;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return dict.GetEnumerator();
		}
	}
}
