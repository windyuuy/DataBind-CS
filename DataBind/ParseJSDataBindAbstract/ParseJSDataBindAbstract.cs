using System;
using System.Collections.Generic;
using System.Linq;
using vm;

namespace ParseJSDataBindAbstract
{
	public class ClassInfo
	{
		public string Name;
		public bool IsUsedAsValueEver = false;
		public Dictionary<string, MemberInfo> MemberMap = new Dictionary<string, MemberInfo>();
		public MemberInfo[] Members => MemberMap.Values.ToArray();

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
		public static NumberTypeInfo TNumber = new NumberTypeInfo();
		public static StringTypeInfo TString = new StringTypeInfo();
		public static BoolTypeInfo TBool = new BoolTypeInfo();

		public MemberInfo StatementReturnType;
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
		public virtual string TypeLiteral
		{
			get => "unkown_basic";
			set => throw new NotImplementedException();
		}
	}

	public class NumberTypeInfo : BasicTypeInfo
	{
		public override string TypeLiteral { get; set; }="number";
	}

	public class StringTypeInfo : BasicTypeInfo
	{
		public override string TypeLiteral { get; set; }="string";
	}

	public class BoolTypeInfo : BasicTypeInfo
	{
		public override string TypeLiteral { get; set; }="bool";
	}
	public class UnknownTypeInfo : ClassInfo
	{

	}

	public class MemberInfo
	{
		public string Name;
		public ClassInfo Type;
	}

	public class ListInfo : ClassInfo
	{
		public ClassInfo IndexerType;
	}

	public class DictInfo : ClassInfo
	{
		public ClassInfo KeyType;
		public ClassInfo ValueType;
	}

	public class FuncInfo : ClassInfo
	{
		public DataBinding.CollectionExt.List<MemberInfo> Paras = new DataBinding.CollectionExt.List<MemberInfo>();
		public MemberInfo RetType;

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
			if (astNode.OperatorX == TNodeType.Word)
			{
				return (astNode.OperatorX, () => new ClassInfo());
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
		public static MemberInfo HandleOperator(ClassInfo root, ClassInfo current, ASTNodeBase astNode)
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
				callerMember.Type = new FuncInfo
				{
					Name = callerMember.Type.Name,
					Parent = callerMember.Type.Parent,
					IsUsedAsValueEver = true,
				};
				callerMember.Type.Parent.InsideTypeMap[callerMember.Type.Name] = callerMember.Type;
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
					return HandleOperator(root, parent.Type, binaryAstNode.Right);
				}
				else
				{
					HandleOperator(root, null, binaryAstNode.Left);
					return HandleOperator(root, null, binaryAstNode.Right);
				}
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public static EnvInfo ParseTypeInfo(ASTNodeBase astNode)
		{
			var envInfo = new EnvInfo();
			var retType = HandleOperator(envInfo, null, astNode);
			envInfo.StatementReturnType = retType;
			return envInfo;
		}
	}
}
