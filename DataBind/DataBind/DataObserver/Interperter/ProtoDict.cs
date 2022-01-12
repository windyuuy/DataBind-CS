
using System.ListExt;

namespace vm
{
	public partial class ProtoDict<TKey, TValue> : Dictionary<TKey, TValue>
	{
		public System.Collections.Generic.IDictionary<TKey, TValue> Proto;
		public virtual object _ { get; set; }

		public ProtoDict() : base()
		{
		}

		public ProtoDict(IConvableDictionary dict)
		{
			this.dict = (System.Collections.Generic.IDictionary<TKey, object>)dict.RawDict;
		}


		public virtual new TValue this[TKey key]
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
						var v = this.Proto[key];
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

		public override TValue TryGet(TKey k)
		{
			if (dict.ContainsKey(k))
			{
				return GetValue(k);
			}
			else if (Proto.ContainsKey(k))
			{
				return Proto[k];
			}
			else
			{
				return default(TValue);
			}
		}
	}
}
