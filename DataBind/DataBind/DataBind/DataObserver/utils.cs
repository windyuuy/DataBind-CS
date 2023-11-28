using DataBinding.CollectionExt;
using System;
using System.Linq;
using System.Reflection;
using Game.Diagnostics.IO;
using Console = Game.Diagnostics.IO.Console;

namespace VM
{
	using boolean = System.Boolean;
	using number = System.Double;

	public struct MemberMethod
	{
		public object Self;
		public MethodInfo MethodInfo;

		public MemberMethod(object self, MethodInfo methodInfo)
		{
			this.Self = self;
			this.MethodInfo = methodInfo;
		}

		public object Invoke(object[] paras)
		{
			return this.MethodInfo.Invoke(Self, paras);
		}

		public ParameterInfo[] GetParameters()
		{
			return this.MethodInfo.GetParameters();
		}
		public int GetParametersCount()
		{
			return this.MethodInfo.GetParameters().Length;
		}
	}

	// TODO: 避免原型链死循环
	public partial class Utils
	{
		/**
		* 讲使用.分隔的路径访问转换为函数。
		* @param path 
		*/

		public static bool IsTrue(object obj)
		{
			if (obj is bool objBool)
			{
				return objBool;
			}
			else if (obj is number objNum)
			{
				return objNum != 0;
			}
			else if (obj is string objStr)
			{
				return !string.IsNullOrEmpty(objStr);
			}
			else
			{
				return obj != null;
			}
		}
		public static bool IsFalse(object obj)
		{
			if (obj is bool objBool)
			{
				return !objBool;
			}
			else if (obj is number objNum)
			{
				return objNum == 0;
			}
			else if (obj is string objStr)
			{
				return string.IsNullOrEmpty(objStr);
			}
			else
			{
				return obj == null;
			}
		}


		public static Dictionary<string, Func<object, object, object>> pathCacheMap = new Dictionary<string, Func<object, object, object>>();
		public static Func<object, object, object> parsePath(string path)
		{
			Func<object, object, object> func;
			pathCacheMap.TryGetValue(path, out func);


			if (func != null)
			{
				return func;
			}
#if true
			{
				//复杂表达式
				var i = new Interpreter(path);

				func = (object self, object env) =>
				{
					var env1 = (IWithPrototype)env;
					return i.Run(env1);
				};
			}
#else
			{
				var segments = path.Split('.');
				func = (object self, object obj) =>
				{
					for (int i = 0; i < segments.Length; i++)
					{
						if (obj == null)
						{
							throw new Exception($"Uncaught TypeError: Cannot read property '{segments[i]}' of null");
						}
						else
						{
							obj = IndexValue(obj, segments[i]);
						}
					}

					return obj;
				};
			}
#endif
			pathCacheMap[path] = func;
			return func;
		}

		public static T IndexValue<T>(object a, object key)
		{
			var ret = IndexValue(a, key);
			return (T)ret;
		}
		public static T IndexValue<T>(object a, object key, out bool exist)
		{
			var ret = IndexValue(a, key, out exist);
			return (T)ret;
		}
		public static object IndexValue(object a, object key)
		{
			bool exist;
			return IndexValue(a, key, out exist);
		}
		public static object IndexValueRecursive(object a, object key)
		{
			bool exist;
			return IndexValueRecursive(a, key, out exist);
		}
		public static object IndexValueRecursive(object a, object key, out bool exist)
		{
			var value = IndexValue(a, key, out exist);
			if (exist)
			{
				return value;
			}
			if (a is IWithPrototype pa)
			{
				var proto = pa.GetProto();
				if (proto != null)
				{
					var valueP = IndexValueRecursive(proto, key, out exist);
					if (exist)
					{
						return valueP;
					}
				}
				return null;
			}
			else
			{
				return null;
			}
		}
		public static object IndexValue(object a, object key, out bool exist)
		{
			if (a == null)
			{
				throw new Exception($"Uncaught TypeError: Cannot read property '{key.ToString()}' of null");
			}
			else
			{
				exist = true;

				string skey;
				if (key is string)
				{
					skey = (string)key;
				}
				else
				{
					skey = key.ToString();
				}
				var isClass = a is Type;
				Type type;
				if (isClass)
				{
					type = (Type)a;
				}
				else
				{
					type = a.GetType();
				}
				System.Reflection.PropertyInfo p;
				if (isClass)
				{
					p = type.GetProperty(skey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
				}
				else
				{
					p = type.GetProperty(skey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
				}
				if (p == null)
				{
					foreach (var prop in GetExtensionProperties(Assembly.GetExecutingAssembly(), type))
					{
						if (prop.Name == skey)
						{
							p = prop;
							break;
						}
					}
				}
				if (p == null)
				{
					var pkey = "_P_" + skey;
					foreach (var method in GetExtensionMethods(Assembly.GetExecutingAssembly(), type))
					{
						if (method.Name == pkey && method.GetParameters().Length == 1)
						{
							var v = method.Invoke(null, new object[] { a });
							return v;
						}
					}
				}
				if (p == null)
				{
					MethodInfo m;
					if (isClass)
					{
						m = type.GetMethod(skey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
					}
					else
					{
						m = type.GetMethod(skey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					}
					if (m == null)
					{
						// 尝试从索引中获取
						var tkey = key.GetType();
						var mget = type.GetMethod("get_Item", new Type[] { tkey });
						if (mget != null)
						{
							var hasKey = true;
							var mhas = type.GetMethod("ContainsKey", new Type[] { key.GetType() });
							if (mhas != null)
							{
								hasKey = (bool)mhas.Invoke(a, new object[] { key });
							}else if(key is int index && a is System.Collections.ICollection coll)
                            {
								hasKey = coll.Count > index;
                            }
							if (hasKey)
                            {
                                try
                                {
									var v = mget.Invoke(a, new object[] { key });
									return v;
								}catch (Exception e)
                                {
									Console.Warn(e);
                                }
							}
						}

						// try extend method
						{
							foreach (var method in GetExtensionMethods(Assembly.GetExecutingAssembly(), type))
							{
								if (method.Name == skey)
								{
									m = method;
									break;
								}
							}
							if (m != null)
							{
								return m;
							}

						}

						var field=type.GetField(skey);
						if(field != null)
                        {
							Console.Error($"不可观测的对象字段: {type.Name}.{skey}");
                        }
						exist = false;
						return null;
					}
					else
					{
						return new MemberMethod(a, m);
					}
				}
				else
				{
					var v = p.GetValue(a);
					return v;
				}
			}
		}

		public static System.Collections.Generic.IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
		{
			var query = from type in assembly.GetTypes()
						where !type.IsGenericType && !type.IsNested
						from method in type.GetMethods(BindingFlags.Static
							| BindingFlags.Public | BindingFlags.NonPublic)
						where method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
						where method.GetParameters()[0].ParameterType == extendedType
						select method;
			return query;
		}
		public static System.Collections.Generic.IEnumerable<PropertyInfo> GetExtensionProperties(Assembly assembly, Type extendedType)
		{
			var query = from type in assembly.GetTypes()
						where !type.IsGenericType && !type.IsNested
						from prop in type.GetProperties(BindingFlags.Static
							| BindingFlags.Public | BindingFlags.NonPublic)
						where prop.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
						where prop.PropertyType == extendedType
						select prop;
			return query;
		}
		public static bool IsMatchedMethod(MethodInfo subM, Type[] types, string skey)
		{
			if (subM.Name != skey)
			{
				return false;
			}

			var paras = subM.GetParameters();
			if (paras.Length > 0)
			{
				var lastPara = paras.Last();
				var isParas = false;
				foreach (var p in lastPara.CustomAttributes)
				{
					if (p.AttributeType == typeof(ParamArrayAttribute))
					{
						isParas = true;
						break;
					}
				}
				var isMatchedParams = true;
				if (isParas)
				{
					var lastType = types.Last();
					if (lastType.IsAssignableFrom(lastPara.ParameterType.GetElementType()))
					{
						for (var i = 0; i < paras.Length - 1; i++)
						{
							if (types[i].IsAssignableFrom(paras[i].ParameterType))
							{
								continue;
							}
							isMatchedParams = false;
							break;
						}
					}
				}
				if (isMatchedParams)
				{
					return true;
				}
			}

			return false;
		}
		public static object IndexMethodRecursive(object a, object key, Type[] types)
		{
			bool exist;
			return IndexMethodRecursive(a, key, types, out exist);
		}
		public static object IndexMethodRecursive(object a, object key, Type[] types, out bool exist)
		{
			object v;
			if (types.Contains(null))
			{
				// 如果类型中包含null，那么无法推断类型信息，尝试只以参数数量推断
				// Console.Warn($"call func {key} para-types contains null, may infer incorrect");
				v = IndexMethodByParasCount(a, key, types.Length, out exist);
			}
			else
			{
				v = IndexMethod(a, key, types, out exist);
			}
			if (exist)
			{
				return v;
			}
			else
			{
				if (a is IWithPrototype pa)
				{
					var proto = pa.GetProto();
					if (proto != null)
					{
						var v2 = IndexMethodRecursive(proto, key, types, out exist);
						if (exist)
						{
							return v2;
						}
					}
					return null;
				}
				else
				{
					return null;
				}
			}
		}

		public static bool IsMethodCountMatched(MethodInfo m, int parasCount)
		{
			var noDefaultCount = m.GetParameters().Select(p => !p.HasDefaultValue).Count();
			if (m.GetParameters().Length >= parasCount && parasCount >= noDefaultCount)
			{
				return true;
			}
			return false;
		}
		
		public static object IndexMethodByParasCount(object a, object key, int parasCount, out bool exist)
		{
			if (a == null)
			{
				throw new Exception($"Uncaught TypeError: Cannot read property '{key.ToString()}' of null");
			}
			else
			{
				exist = true;

				string skey;
				if (key is string)
				{
					skey = (string)key;
				}
				else
				{
					skey = key.ToString();
				}
				var isClass = a is Type;
				Type type;
				if (isClass)
				{
					type = (Type)a;
				}
				else
				{
					type = a.GetType();
				}
				var m = type.GetMethod(key.ToString());

				if (m != null)
				{
					if (IsMethodCountMatched(m, parasCount))
					{
						return m;
					}
					else
					{
						m = null;
					}
				}
				if (m == null)
				{
					m = type.GetMethods().FirstOrDefault(m1 => IsMethodCountMatched(m1, parasCount));
				}
				if (m == null)
				{
					System.Reflection.MethodInfo[] methods;
					if (type.IsClass)
					{
						methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
					}
					else
					{
						methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					}
					// 尝试获取近似项
					MethodInfo matchedMethodInfo = methods.FirstOrDefault(m1 => IsMethodCountMatched(m1, parasCount));
					if (matchedMethodInfo != null)
					{
						return matchedMethodInfo;
					}
					else
					{
						// 尝试从索引中获取
						var mget = type.GetMethod("get_Item", new Type[] { key.GetType() });
						if (mget != null)
						{
							var hasKey = true;
							var mhas = type.GetMethod("ContainsKey", new Type[] { key.GetType() });
							if (mhas != null)
							{
								hasKey = (bool)mhas.Invoke(a, new object[] { key });
							}
							if (hasKey)
							{
								var v = mget.Invoke(a, new object[] { key });
								return v;
							}
						}

						// try extend method
						{
							if (matchedMethodInfo == null)
							{
								matchedMethodInfo = GetExtensionMethods(Assembly.GetExecutingAssembly(), type)
									.FirstOrDefault(m1 => IsMethodCountMatched(m1, parasCount));
							}
							if (matchedMethodInfo == null)
							{
								var pkey = "_P_" + skey;
								foreach (var method in GetExtensionMethods(Assembly.GetExecutingAssembly(), type))
								{
									if (method.Name == pkey && method.GetParameters().Length == 1 && type.IsAssignableFrom(method.ReturnType))
									{
										var v = method.Invoke(null, new object[] { a });
										return v;
									}
								}
							}
							if (matchedMethodInfo != null)
							{
								return matchedMethodInfo;
							}
							else
							{

								var field = type.GetField(skey);
								if (field != null)
								{
									Console.Error($"不可观测的对象字段: {type.Name}.{skey}");
								}

								exist = false;
								return null;
							}
						}
					}
				}
				else
				{
					return m;
				}
			}
		}
		public static object IndexMethod(object a, object key, Type[] types, out bool exist)
		{
			if (a == null)
			{
				throw new Exception($"Uncaught TypeError: Cannot read property '{key.ToString()}' of null");
			}
			else
			{
				exist = true;

				string skey;
				if (key is string)
				{
					skey = (string)key;
				}
				else
				{
					skey = key.ToString();
				}
				var isClass = a is Type;
				Type type;
				if (isClass)
				{
					type = (Type)a;
				}
				else
				{
					type = a.GetType();
				}
				var m = type.GetMethod(key.ToString(), types);
				if (m == null)
				{
					System.Reflection.MethodInfo[] methods;
					if (type.IsClass)
					{
						methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
					}
					else
					{
						methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					}
					// 尝试获取近似项
					MethodInfo matchedMethodInfo = null;
					foreach (var subM in methods)
					{
						if (IsMatchedMethod(subM, types, skey))
						{
							matchedMethodInfo = subM;
							break;
						}
					}
					if (matchedMethodInfo != null)
					{
						return matchedMethodInfo;
					}
					else
					{
						// 尝试从索引中获取
						var mget = type.GetMethod("get_Item", new Type[] { key.GetType() });
						if (mget != null)
						{
							var hasKey = true;
							var mhas = type.GetMethod("ContainsKey", new Type[] { key.GetType() });
							if (mhas != null)
							{
								hasKey = (bool)mhas.Invoke(a, new object[] { key });
							}
							if (hasKey)
							{
								var v = mget.Invoke(a, new object[] { key });
								return v;
							}
						}

						// try extend method
						{
							if (matchedMethodInfo == null)
							{
								foreach (var subM in GetExtensionMethods(Assembly.GetExecutingAssembly(), type))
								{
									if (IsMatchedMethod(subM, types, skey))
									{
										matchedMethodInfo = subM;
										break;
									}
								}
							}
							if (matchedMethodInfo == null)
							{
								var pkey = "_P_" + skey;
								foreach (var method in GetExtensionMethods(Assembly.GetExecutingAssembly(), type))
								{
									if (method.Name == pkey && method.GetParameters().Length == 1 && type.IsAssignableFrom(method.ReturnType))
									{
										var v = method.Invoke(null, new object[] { a });
										return v;
									}
								}
							}
							if (matchedMethodInfo != null)
							{
								return matchedMethodInfo;
							}
							else
							{

								var field = type.GetField(skey);
								if (field != null)
								{
									Console.Error($"不可观测的对象字段: {type.Name}.{skey}");
								}

								exist = false;
								return null;
							}
						}
					}
				}
				else
				{
					return m;
				}
			}
		}
		public static object InvokeMethodConsiderExtend(System.Reflection.MethodInfo func, object obj, List<object> paramList)
		{
			if (func.IsStatic && func.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
			{
				var paramList2 = paramList.Clone();
				paramList2.Insert(0, obj);
				return func.Invoke(null, paramList2.ToArray());
			}
			else
			{
				return func.Invoke(obj, paramList.ToArray());
			}
		}
		public static object InvokeMethod(object func, object obj, List<object> paramList0)
		{
			if (func is MethodInfo func1)
			{
				return InvokeMethodRaw(func1, obj, paramList0);
			}
			else
			{
				if (func is Delegate de)
				{
					var ps = paramList0.ToArray();
					var v = de.DynamicInvoke(ps);
					return v;
				}
				else
				{
					throw new Exception($"unsupport type of value to invoke: {func.GetType().Name}");
				}
			}
		}
		public static object InvokeMethodRaw(System.Reflection.MethodInfo func, object obj, List<object> paramList0)
		{
			if (paramList0 == null)
			{
				return func.Invoke(obj, null);
			}
			else
			{
				var paramList = (List<object>)paramList0;

				var paras = func.GetParameters();
				if (paras.Length >= 1)
				{
					var isParas = false;
					var lastPara = paras.Last();
					var attrs = lastPara.CustomAttributes;
					foreach (var attr in attrs)
					{
						if (attr.AttributeType == typeof(ParamArrayAttribute))
						{
							isParas = true;
							break;
						}
					}
					if (isParas)
					{
						var paraType = lastPara.ParameterType.GetElementType();
						var paramList2 = paramList.slice(0, paras.Length - 1);
						var paramList3 = paramList.slice(paras.Length - 1);
						var arr3 = paramList3.ToArray(paraType);
						paramList2.Add(arr3);
						return InvokeMethodConsiderExtend(func, obj, paramList2);
					}
					else
					{
						return InvokeMethodConsiderExtend(func, obj, paramList);
					}
				}
				else
				{
					return InvokeMethodConsiderExtend(func, obj, paramList);
				}
			}

		}

		// public static Type[] ExtractValuesTypes(object[] values)
		// {
		// 	return values.Select(x => x?.GetType()).ToArray();
		// }
		public static Type[] ExtractValuesTypes(List<object> values)
		{
			return values.Select(x => x?.GetType()).ToArray();
		}

		public static F ConvItem<F>(object value)
		{
			if (value == null)
			{
				return default(F);
			}

			var v = value;
			if (v is F vF)
			{
				return vF;
			}
			else if (typeof(F).IsAssignableFrom(v.GetType()))
			{
				return (F)v;
			}
			else
			{
				var m = typeof(F).GetMethod("op_Implicit", new Type[] { v.GetType() });
				if (m != null)
				{
					var v2 = m.Invoke(null, new object[] { v });
					return (F)v2;
				}
				else
				{
					// return (F)v;
					return (F)Convert.ChangeType(v, typeof(F));
				}
			}
		}

		public static string ToIndexKey(object key)
		{
			if (key is string skey)
			{
				return skey;
			}
			else
			{
				return key.ToString();
			}
		}
	}
}