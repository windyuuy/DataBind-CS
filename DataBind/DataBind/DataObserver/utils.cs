using System.ListExt;
using System;
using System.Linq;
using System.Reflection;

namespace vm
{
	using boolean = System.Boolean;
	using number = System.Double;

	public partial class Utils
	{
		/**
		* 讲使用.分隔的路径访问转换为函数。
		* @param path 
		*/

		/**
		 * 向目标对象实现所有基础属性
		 */
		public object implementEnvironment(object obj)
		{
			return obj;
		}

		public static bool IsTrue(object obj)
		{
			if (obj is bool)
			{
				return (bool)obj;
			}
			else if (obj is number)
			{
				return (number)obj != 0;
			}
			else if (obj is string)
			{
				return (string)obj != "";
			}
			else
			{
				return obj != null;
			}
		}
		public static bool IsFalse(object obj)
		{
			if (obj is bool)
			{
				return !(bool)obj;
			}
			else if (obj is number)
			{
				return (number)obj == 0;
			}
			else if (obj is string)
			{
				return (string)obj == "";
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
#if false
			{
				//复杂表达式
				var i = new Interpreter(path);

				func = (object self, object env) =>
				{
					var env1 = (Dictionary<string, object>)env;
					if (env != null && env1 == null)
					{
						throw new Exception("env conversion failed");
					}
					return i.run(env1);
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
		public static object IndexValue(object a, object key)
		{
			if (a == null)
			{
				throw new Exception($"Uncaught TypeError: Cannot read property '{key.ToString()}' of null");
			}
			else
			{
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
						var mget = type.GetMethod("get_Item", new Type[] { typeof(string) });
						if (mget == null)
						{
							foreach (var method in GetExtensionMethods(Assembly.GetExecutingAssembly(), type))
							{
								if (method.Name == skey)
								{
									m = method;
									break;
								}
							}
							if (m == null)
							{
								return null;
							}
							else
							{
								return m;
							}
						}
						else
						{
							var hasKey = true;
							var mhas = type.GetMethod("ContainsKey", new Type[] { typeof(string) });
							if (mhas != null)
							{
								hasKey = (bool)mhas.Invoke(a, new object[] { skey });
							}
                            if (hasKey)
                            {
								var v = mget.Invoke(a, new object[] { skey });
								return v;
                            }
                            else
                            {
								return null;
                            }
						}
					}
					else
					{
						return m;
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
		public static object IndexMethod(object a, object key, Type[] types)
		{
			if (a == null)
			{
				throw new Exception($"Uncaught TypeError: Cannot read property '{key.ToString()}' of null");
			}
			else
			{
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
						var mget = type.GetMethod("get_Item", new Type[] { typeof(string) });
						if (mget == null)
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
								return null;
							}
						}
						else
						{
							var v = mget.Invoke(a, new object[] { skey });
							var mv = v;
							return mv;
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
			if (func is MethodInfo)
			{
				return InvokeMethod((MethodInfo)func, obj, paramList0);
			}
			else
			{
				if (func is Delegate)
				{
					var de = func as Delegate;
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
		public static object InvokeMethod(System.Reflection.MethodInfo func, object obj, List<object> paramList0)
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

		public static Type[] ExtractValuesTypes(object[] values)
		{
			return values.Select(x => x.GetType()).ToArray();
		}
		public static Type[] ExtractValuesTypes(List<object> values)
		{
			return values.Select(x => x.GetType()).ToArray();
		}

		public static F ConvItem<F>(object value)
		{
			var v = value;
			if (v is F)
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
					return (F)v;
				}
			}
		}

		public static string ToIndexKey(object key)
		{
			if (key is string)
			{
				return (string)key;
			}
			else
			{
				return key.ToString();
			}
		}
	}
}