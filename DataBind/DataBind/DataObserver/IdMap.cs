using System.Collections.Generic;

namespace vm
{
	using number = System.Double;
	using boolean = System.Boolean;

	public interface IIdMap
	{
		IIdMap add(number value);
		void clear();
		boolean has(number value);
	}

	public class IdMap : IIdMap
	{
		Dictionary<number, bool> set = new Dictionary<number, bool>();
		public bool has(number key)
		{
            if (this.set.ContainsKey(key))
            {
				return this.set[key] == true;
			}
			return false;
		}
		public IIdMap add(number key)
		{
			this.set[key] = true;
			return this;
		}
		public void clear()
		{
			this.set = new Dictionary<number, bool>();
		}
	}

}
