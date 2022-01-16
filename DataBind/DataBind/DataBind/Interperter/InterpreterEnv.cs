using System;
using System.ListExt;
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
				if (environment.ContainsKey(key))
				{
					var ps = m.GetParameters();
					var doubleNum = ps.Count(p => p.ParameterType == TNumber);
					doubleNum += ps.Count(p => p.ParameterType == TNumbers);
					if (doubleNum > 0)
					{
						environment[key] = m;
					}
				}
				else
				{
					environment[key] = m;
				}
			});
			fields.ForEach(f => environment.Add(f.Name.ToUpper(), f.GetValue(TMath)));
			props.ForEach(p => environment.Add(p.Name.ToUpper(), p.GetValue(TMath)));
		}
		public static TEnv environment = new TEnv();
		public static TEnv extendsEnvironment(TEnv ext)
		{
			if (ext is IWithPrototype)
			{
				var ext1 = ext as IWithPrototype;
				ext1.SetProto(environment);
				//ext1._ = environment;
			}
			else
			{
				foreach (var kv in environment)
				{
					ext.Add(kv.Key, kv.Value);
				}
			}
			return ext;
		}

		public static TEnv implementEnvironment(TEnv b)
		{
			foreach (var kv in environment)
			{
				if (kv.Key == "___Sob__")
				{
					continue;
				}
				b[kv.Key] = kv.Value;
			}
			return b;
		}
	}

}