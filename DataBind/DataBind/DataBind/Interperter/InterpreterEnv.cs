using System;
using DataBinding.CollectionExt;
using System.Linq;

namespace vm
{
	using TEnv = Dictionary<string, object>;
	using number = System.Double;

	public class InterpreterEnv
	{
		static InterpreterEnv()
		{
			InjectStaticType(typeof(System.Math));
			InjectStaticType(typeof(MathExt.Math));
		}
		public static void InjectStaticType(Type TMath)
		{
			// Object.getOwnPropertyNames(Math).forEach(k => def(environment, k.toUpperCase(), (Math as any)[k]));
			var methods = TMath.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var fields = TMath.GetFields();
			var props = TMath.GetProperties();
			var TNumber = typeof(number);
			var TNumbers = typeof(number[]);
			methods.ForEach(m =>
			{
				var key = m.Name.ToUpper();
				if (Environment.ContainsKey(key))
				{
					var ps = m.GetParameters();
					var doubleNum = ps.Count(p => p.ParameterType == TNumber);
					doubleNum += ps.Count(p => p.ParameterType == TNumbers);
					if (doubleNum > 0)
					{
						Environment[key] = m;
					}
				}
				else
				{
					Environment[key] = m;
				}
			});
			fields.ForEach(f => Environment.Add(f.Name.ToUpper(), f.GetValue(TMath)));
			props.ForEach(p => Environment.Add(p.Name.ToUpper(), p.GetValue(TMath)));
		}
		public static readonly TEnv Environment = new TEnv();
		public static TEnv ExtendsEnvironment(TEnv ext)
		{
			if (ext is IWithPrototype ext1)
			{
				ext1.SetProto(Environment);
				//ext1._ = environment;
			}
			else
			{
				foreach (var kv in Environment)
				{
					ext.Add(kv.Key, kv.Value);
				}
			}
			return ext;
		}

		public static TEnv ImplementEnvironment(TEnv b)
		{
			foreach (var kv in Environment)
			{
				if (kv.Key == "___Sob__")
				{
					continue;
				}
				b[kv.Key] = kv.Value;
			}
			return b;
		}
		public static IWithPrototype ImplementEnvironment(IWithPrototype b)
		{
			var envDict = new TEnv();
			b.SetProto(envDict);
			foreach (var kv in Environment)
			{
				if (kv.Key == "___Sob__")
				{
					continue;
				}
				envDict[kv.Key] = kv.Value;
			}
			return b;
		}
	}

}