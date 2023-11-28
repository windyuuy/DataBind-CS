using System;
using System.Collections;
using VM;

namespace DataBinding.CollectionExt
{
	public interface IConvableDictionary
	{
		object RawDict { get; }
	}

	public partial class Dictionary<K, V> : Dictionary, System.Collections.Generic.IDictionary<K, V>, IConvableDictionary
	{
		protected System.Collections.Generic.IDictionary<K, object> dict;

		public override object RawDict => dict;

		public Dictionary(System.Collections.Generic.IDictionary<K, object> dict)
		{
			this.dict = dict;
		}

		public Dictionary()
		{
			this.dict = new System.Collections.Generic.Dictionary<K, object>();
		}

		public Dictionary(IConvableDictionary dict)
		{
			this.dict = (System.Collections.Generic.IDictionary<K, object>)dict.RawDict;
		}

		public virtual V GetValue(K key)
		{
			return Utils.ConvItem<V>(dict[key]);
		}

		public override object GetValueByKey(object key)
		{
			var key2 = Utils.ConvItem<K>(key);
			return this[key2];
		}
		public override void SetValueByKey(object key, object value)
		{
			var key2 = Utils.ConvItem<K>(key);
			this[key2] = Utils.ConvItem<V>(value);
		}

        public virtual V this[K key]
		{
			get
			{
				var value = Utils.ConvItem<V>(dict[key]);
				this.NotifyPropertyGot(value, Utils.ToIndexKey(key));
				return value;
			}
			set
			{
				object v0;
				dict.TryGetValue(key, out v0);
				var keyExist = dict.ContainsKey(key);
				dict[key] = value;
                if (keyExist)
                {
                    this.NotifyPropertyChanged(value, v0, Utils.ToIndexKey(key));
                }
                else
                {
					this.NotifyAddRelation(value, Utils.ToIndexKey(key));
				}
			}
		}

		public new virtual System.Collections.Generic.ICollection<K> Keys => dict.Keys;

		public new virtual System.Collections.Generic.ICollection<V> Values => new DataBinding.CollectionExt.List<V>(dict.Values);

		public override int Count => dict.Count;

		public override bool IsReadOnly => dict.IsReadOnly;

		public virtual void Add(K key, V value)
		{
			dict.Add(key, value);
			NotifyAddRelation(value,Utils.ToIndexKey(key));
		}

		public virtual void Add(System.Collections.Generic.KeyValuePair<K, V> item)
		{
			dict.Add(item.Key, item.Value);
			NotifyAddRelation(item.Value,Utils.ToIndexKey(item.Key));
		}

		public override void Clear()
		{
			dict.Clear();
			NotifyChangedRelation();
		}

		public virtual bool Contains(System.Collections.Generic.KeyValuePair<K, V> item0)
		{
			return dict.ContainsKey(item0.Key) && Object.ReferenceEquals(item0.Value, dict[item0.Key]);
		}

		public virtual bool ContainsKey(K key)
		{
			return dict.ContainsKey(key);
		}

		public virtual void CopyTo(System.Collections.Generic.KeyValuePair<K, V>[] array, int arrayIndex)
		{
			// dict.CopyTo(array, arrayIndex);
			var i = 0;
			foreach (var kv in dict)
			{
				if (i >= array.Length)
				{
					break;
				}
				array[i] = new System.Collections.Generic.KeyValuePair<K, V>(kv.Key, Utils.ConvItem<V>(kv.Value));
				i++;
			}
		}

		public new virtual System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<K, V>> GetEnumerator()
		{
			// TODO: 优化性能
			// return dict.GetEnumerator();
			var dict2 = new System.Collections.Generic.Dictionary<K, V>();
			foreach (var kv in dict)
			{
				dict2.Add(kv.Key, Utils.ConvItem<V>(kv.Value));
			}
			return dict2.GetEnumerator();
		}

		public virtual bool Remove(K key)
		{
			var ret = dict.Remove(key);
			this.NotifyChangedRelation();
			return ret;
		}

		public virtual bool Remove(System.Collections.Generic.KeyValuePair<K, V> item)
		{
			if (this.Contains(item))
			{
				return dict.Remove(item.Key);
			}
			this.NotifyChangedRelation();
			return false;
		}

		public virtual bool TryGetValue(K key, out V value)
		{
			object value2;
			var ret = dict.TryGetValue(key, out value2);
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
