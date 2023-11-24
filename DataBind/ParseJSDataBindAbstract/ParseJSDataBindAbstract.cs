using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using vm;

namespace ParseJSDataBindAbstract
{
	[DebuggerDisplay("{TypeLiteral} {Name}")]
	public class ClassInfo
	{
		public string Name;

		public string FullName
		{
			get
			{
				var sb = new StringBuilder();
				var curType = this;
				sb.Append(curType.Name);
				while (curType.Parent != null && curType.Parent is not EnvInfo)
				{
					curType = curType.Parent;
					sb.Insert(0, ".");
					sb.Insert(0, curType.Name);
				}

				return sb.ToString();
			}
		}

		public void ChangeName(string name)
		{
			this.Parent.InsideTypeMap[name] = this.Parent.InsideTypeMap[this.Name];
			this.Parent.InsideTypeMap.Remove(this.Name);
			this.Name = name;
		}
		public  virtual string TypeLiteral => "class";
		/// <summary>
		/// 手工书写的声明
		/// </summary>
		public string TypeDefManualCodeLine;

		public string[] AnnotationLines;
		public Dictionary<string, MemberInfo> MemberMap = new Dictionary<string, MemberInfo>();
		public MemberInfo[] Members => MemberMap.Values.ToArray();
		public int MemberCount => MemberMap.Count;

		public Dictionary<string, ClassInfo> InsideTypeMap = new Dictionary<string, ClassInfo>();
		public ClassInfo[] InsideTypes => InsideTypeMap.Values.ToArray();

		public ClassInfo AddOrGetType(string typeName, Func<ClassInfo> typeGen)
		{
			if (!InsideTypeMap.TryGetValue(typeName, out var type))
			{
				type = typeGen();
				type.Parent = this;
				InsideTypeMap.Add(typeName, type);
			}

			return type;
		}

		public ClassInfo Parent;

		public virtual MemberInfo TryAddMember(string name, (int, Func<ClassInfo>) typeGen)
		{
			if (!MemberMap.TryGetValue(name, out var memberInfo))
			{
				var typeName = "T" + ToCammelCase(name);
				var type = this.AddOrGetType(typeName, () =>
				{
					var type0 = typeGen.Item2();
					type0.Name = typeName;
					return type0;
				});
				memberInfo = new MemberInfo();
				memberInfo.Name = name;
				memberInfo.Type = type;

				MemberMap.Add(name, memberInfo);
			}

			return memberInfo;
		}

		public static string ToCammelCase(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}
			else if (name.Length <= 1)
			{
				return name.ToUpper();
			}
			else
			{
				return name.Substring(0, 1).ToUpper() + name.Substring(1);
			}
		}
	}

	public class EnvInfo : ClassInfo
	{
		public override string TypeLiteral => "env";
		public EnvInfo(){}

		public EnvInfo(string name)
		{
			Name = name;
		}
		public static NumberTypeInfo TNumber = new NumberTypeInfo();
		public static StringTypeInfo TString = new StringTypeInfo();
		public static BoolTypeInfo TBool = new BoolTypeInfo();

		public MemberInfo StatementReturnType;
		public List<string> FileHeaders = new();
		protected Dictionary<string, string> Namespaces = new();
		public void AddNamespace(string ns)
		{
			this.Namespaces[ns] = null;
		}

		public void AddTypeAlias(string ns, string alias)
		{
			this.Namespaces[ns] = alias;
		}

		public IEnumerable<string> UsingNamespaces =>
			Namespaces.Where(item => item.Value == null).Select(item => item.Key);
		
		public IEnumerable<KeyValuePair<string,string>> UsingAlias =>
			Namespaces.Where(item => item.Value != null);
		public override MemberInfo TryAddMember(string name, (int, Func<ClassInfo>) typeGen)
		{
			if (!MemberMap.TryGetValue(name, out var memberInfo))
			{
				var typeName = "T" + ToCammelCase(name);
				memberInfo = new MemberInfo();
				memberInfo.Name = name;

				if (typeGen.Item1 == TNodeType.Number || typeGen.Item1 == TNodeType.Stringx ||
				    typeGen.Item1 == TNodeType.Boolean)
				{
					ClassInfo type;
					if (typeGen.Item1 == TNodeType.Number)
					{
						type = TNumber;
					}
					else if (typeGen.Item1 == TNodeType.Stringx)
					{
						type = TString;
					}
					else if (typeGen.Item1 == TNodeType.Boolean)
					{
						type = TBool;
					}
					else
					{
						throw new NotImplementedException();
					}
					type.Name = typeName;
					type.Parent = this;
					memberInfo.Type = type;
				}
				else
				{
					var type = this.AddOrGetType(typeName, () =>
					{
						var type0 = typeGen.Item2();
						type0.Name = typeName;
						return type0;
					});
					memberInfo.Type = type;

					MemberMap.Add(name, memberInfo);
				}

			}

			return memberInfo;
		}

	}

	public class BasicTypeInfo : ClassInfo
	{
		public override string TypeLiteral
		{
			get => "unkown_basic";
		}
	}

	public class NumberTypeInfo : BasicTypeInfo
	{
		public override string TypeLiteral { get; } = "number";
	}

	public class StringTypeInfo : BasicTypeInfo
	{
		public override string TypeLiteral { get; } = "string";
	}

	public class BoolTypeInfo : BasicTypeInfo
	{
		public override string TypeLiteral { get; } = "bool";
	}
	public class UnknownTypeInfo : ClassInfo
	{

	}

	[DebuggerDisplay("public {Type.TypeLiteral}<{Type.Name}> {Name};")]
	public class MemberInfo
	{
		public string Name;
		public ClassInfo Type;
		public List<string> UsedCases = new List<string>();
		/// <summary>
		/// 手工书写的声明
		/// </summary>
		public string MemberManualCodeLine;

		public string[] AnnotationLines;

		public static readonly Regex IsBoolMatcher = new(@"^(?:is|be|are|was|were|Is|Be|Are|Was|Were)[A-Z_]");
		public static readonly Regex IsBoolMatcher3 = new(@"[a-z]able$");
		public static readonly Regex IsBoolMatcher2 = new(@"^(?:enabled|visible|enable|active|valid)$");
		public static readonly Regex IsStringMatcher = new(@"(?:Label|Text|Txt|Name|Title|Tag|Content|Url|Uri)$");
		public static readonly Regex IsStringMatcher2 = new(@"^(?:label|text|txt|name|title|tag|content|url|uri)$");
		public static readonly Regex IsNumberMatcher = new(@"(?:Count|Length|Len|Num|Progress)$");
		public static readonly Regex IsNumberMatcher2 = new(@"^(?:n)[A-Z_]");
		public static readonly Regex IsNumberMatcher3 = new(@"^(?:progress)$");
		public static readonly Regex IsActionMatcher1 = new(@"(?:Click)$");
		public static readonly Regex IsActionMatcher2 = new(@"^(?:click)$");
		public string InferType(string defaultValue)
		{
			if (this.Type.MemberCount == 0)
			{
				if (IsBoolMatcher.IsMatch(Name) 
				    || IsBoolMatcher2.IsMatch(Name) 
				    || IsBoolMatcher3.IsMatch(Name)
				    )
				{
					return "bool";
				}
				else if (IsStringMatcher.IsMatch(Name)
				         || IsStringMatcher2.IsMatch(Name))
				{
					return "string";
				}
				else if (IsNumberMatcher.IsMatch(Name)
				         || IsNumberMatcher2.IsMatch(Name)
				         || IsNumberMatcher3.IsMatch(Name)
				         )
				{
					return "number";
				}
				else if (IsActionMatcher1.IsMatch(Name)
				         || IsActionMatcher2.IsMatch(Name)
				        )
				{
					return "Action";
				}
			}
			
			return defaultValue;
		}
		
		public FuncInfo CastToFunc()
		{
			var funcInfo = new FuncInfo
			{
				Name = this.Type.Name,
				Parent = this.Type.Parent,
			};
			this.Type = funcInfo;
			this.Type.Parent.InsideTypeMap[funcInfo.Name] = funcInfo;
			return funcInfo;
		}
		
		public ArrayTypeInfo CastToArray(ClassInfo keyType, ClassInfo eleType)
		{
			var arrayInfo = new ArrayTypeInfo()
			{
				Name = this.Type.Name,
				Parent = this.Type.Parent,
				KeyType = keyType,
				ElementType = eleType,
			};
			this.Type = arrayInfo;
			this.Type.Parent.InsideTypeMap[arrayInfo.Name] = arrayInfo;
			return arrayInfo;
		}

		public DictionaryTypeInfo CastToDict(ClassInfo keyType, ClassInfo eleType)
		{
			var dictInfo = new DictionaryTypeInfo()
			{
				Name = this.Type.Name,
				Parent = this.Type.Parent,
				KeyType = keyType,
				ElementType = eleType,
			};
			this.Type = dictInfo;
			this.Type.Parent.InsideTypeMap[dictInfo.Name] = dictInfo;
			return dictInfo;
		}

	}

	public class ArrayTypeInfo : ClassInfo
	{
		public ClassInfo KeyType;
		public ClassInfo ElementType;

		public override string TypeLiteral
		{
			get
			{
				if (ElementType == null)
				{
					return $"object[]";
				}
				else
				{
					return $"{ElementType.TypeLiteral}[]";
				}
			}
		}
	}

	public class DictionaryTypeInfo : ClassInfo
	{
		public ClassInfo KeyType;
		public ClassInfo ElementType;
		
		public override string TypeLiteral
		{
			get
			{
				if (ElementType == null)
				{
					return $"Dictionary<{KeyType.TypeLiteral}, object>";
				}
				else
				{
					return $"Dictionary<{KeyType.TypeLiteral}, {ElementType.TypeLiteral}>";
				}
			}
		}
	}

	public class FuncInfo : ClassInfo
	{
		public override string TypeLiteral => "func";
		public DataBinding.CollectionExt.List<MemberInfo> Paras = new DataBinding.CollectionExt.List<MemberInfo>();
		public MemberInfo RetType;
		public string[] FuncBodyManualCodeLines;

		public void SetParas(IEnumerable<MemberInfo> members)
		{
			Paras.Clear();
			foreach (var member in members)
			{
				Paras.Add(member);
			}
		}
	}

	public class ParseJSDataBind
	{
		internal static (int, Func<ClassInfo>) SelectTypeGen(ASTNodeBase astNode)
		{
			if (astNode is ValueASTNode valueAstNode && astNode.OperatorX == TNodeType.Word)
			{
				if (astNode.Parent?.OperatorX == TNodeType.Inst["!"])
				{
					return (astNode.OperatorX, () => new BoolTypeInfo());
				}
				else
				{
					return (astNode.OperatorX, () => new ClassInfo());
				}
			}
			else if (astNode.OperatorX == TNodeType.Number)
			{
				return (astNode.OperatorX, () => new NumberTypeInfo());
			}
			else if (astNode.OperatorX == TNodeType.Stringx)
			{
				return (astNode.OperatorX, () => new StringTypeInfo());
			}
			else if (astNode.OperatorX == TNodeType.Boolean)
			{
				return (astNode.OperatorX, () => new BoolTypeInfo());
			}
			// else if (astNode.Parent is CallASTNode)
			// {
			// 	return () => new FuncInfo();
			// }
			else
			{
				throw new InvalidCastException();
			}
		}
		public static string GetNodeIndexPath(ValueASTNode targetNode)
		{
			var sb = new StringBuilder();
			var parentNode = targetNode.Parent as BinaryASTNode;
			while (parentNode!=null && parentNode.OperatorX == TNodeType.Inst["."])
			{
				if (parentNode.Right is ValueASTNode rightNode)
				{
					sb.Insert(0, rightNode.Value.Value.ToString());
				}
				else
				{
					sb.Insert(0, "Unkown");
				}
				var leftNode = parentNode.Left;
				if (leftNode is BinaryASTNode binaryAstNode1 && leftNode.OperatorX==TNodeType.Inst["."])
				{
					sb.Insert(0, ".");
					parentNode = binaryAstNode1;
				}
				else if (leftNode is ValueASTNode valueNode)
				{
					sb.Insert(0, ".");
					sb.Insert(0, valueNode.Value.Value.ToString());
					break;
				}
				else
				{
					break;
				}
			}

			return sb.ToString();
		}
		public static MemberInfo HandleOperator(EnvInfo root, ClassInfo current, ASTNodeBase astNode)
		{
			if (astNode is ValueASTNode valueAstNode)
			{
				if (current != null)
				{
					return current.TryAddMember(valueAstNode.Value.Value.ToString(), SelectTypeGen(astNode));
				}
				else
				{
					return root.TryAddMember(valueAstNode.Value.Value.ToString(), SelectTypeGen(astNode));
				}
			}
			else if (astNode is CallASTNode callAstNode)
			{
				var callerMember = HandleOperator(root, null, callAstNode.Left);
				callerMember.CastToFunc();
				var callerType = callerMember.Type as FuncInfo;
				var paras = callAstNode.Parameters.Select(para => HandleOperator(root, null, para)).ToArray();
				callerType.SetParas(paras);
				return callerMember;
			}
			else if (astNode is BracketASTNode bracketAstNode)
			{
				return HandleOperator(root, null, bracketAstNode.Node);
			}
			else if (astNode is UnitaryASTNode unitaryAstNode)
			{
				return HandleOperator(root, null, unitaryAstNode.Right);
			}
			else if (astNode is BinaryASTNode binaryAstNode)
			{
				if (astNode.OperatorX == TNodeType.Inst["."])
				{
					var parent = HandleOperator(root, null, binaryAstNode.Left);
					var right = HandleOperator(root, parent.Type, binaryAstNode.Right);
					if (astNode.Parent==null ||
					    (astNode.Parent != null && astNode.Parent.OperatorX != TNodeType.Inst["."]))
					{
						right.UsedCases.Add(GetNodeIndexPath(binaryAstNode.Right as ValueASTNode));
					}
					return right;
				}
				else
				{
					var left = HandleOperator(root, null, binaryAstNode.Left);
					var right = HandleOperator(root, null, binaryAstNode.Right);
					if (binaryAstNode.OperatorX == TNodeType.Inst["["])
					{
						if (right.Type is NumberTypeInfo)
						{
							left.CastToArray(right.Type, null);
						}
						else
						{
							left.CastToDict(right.Type, null);
						}
						root.AddNamespace("DataBinding.CollectionExt");
					}
					return right;
				}
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public static EnvInfo ParseTypeInfo(ASTNodeBase astNode, string name)
		{
			var envInfo = new EnvInfo(name);
			return ParseTypeInfo(astNode, envInfo);
		}
		public static EnvInfo ParseTypeInfo(ASTNodeBase astNode, EnvInfo envInfo)
		{
			envInfo.AddTypeAlias("number", "System.Single");
			envInfo.AddTypeAlias("Action", "System.Action");
			var retType = HandleOperator(envInfo, null, astNode);
			envInfo.StatementReturnType = retType;
			return envInfo;
		}
	}
}
