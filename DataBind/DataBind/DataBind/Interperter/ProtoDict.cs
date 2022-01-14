
using System.ListExt;

namespace vm
{
	public partial class ProtoDict<K, V> : Dictionary<K, V>, IWithPrototype
	{
		public ProtoDict() : base()
		{
		}

		public ProtoDict(IConvableDictionary dict)
		{
			this.dict = (System.Collections.Generic.IDictionary<K, object>)dict.RawDict;
		}

		protected virtual V getProtoValue(K key, out bool exist)
		{
			return Utils.IndexValue<V>(Proto, key, out exist);
		}

		protected virtual V getProtoValue(K key)
		{
			bool exist;
			return Utils.IndexValue<V>(Proto, key, out exist);
		}

		public virtual new V this[K key]
		{
			get
			{
				if (this.ContainsKey(key))
				{
					return base[key];
				}
				else
				{
					if (this.Proto != null)
					{
						var v = getProtoValue(key);
						return v;
					}
					else
					{
						return base[key];
					}
				}
			}
			set
			{
				base[key] = value;
			}
		}

		public override V TryGet(K k)
		{
			if (dict.ContainsKey(k))
			{
				return GetValue(k);
			}
			bool exist;
			var value = getProtoValue(k, out exist);
			if (exist)
			{
				return value;
			}
			else
			{
				return default(V);
			}
		}

	}
}
