using System.Collections.Generic;

namespace DataBind.VM
{
	using number = System.Double;
	using boolean = System.Boolean;

	public interface IIdMap
	{
		IIdMap Add(number value);
		void Clear();
		boolean Has(number value);
	}

	public class IdMap : IIdMap
	{
		Dictionary<number, bool> _set = new Dictionary<number, bool>();

		/// <inheritdoc />
		public bool Has(number key)
		{
            if (this._set.TryGetValue(key, out var value))
            {
				return value == true;
			}
			return false;
		}

		/// <inheritdoc />
		public IIdMap Add(number key)
		{
			this._set[key] = true;
			return this;
		}

		/// <inheritdoc />
		public void Clear()
		{
			this._set = new Dictionary<number, bool>();
		}
	}

}
