using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DataBind.CollectionExt;
using EngineAdapter.Diagnostics;
using EngineAdapter.StringExt;
using Console = EngineAdapter.Diagnostics.Console;

class _Env
{

	static _Env()
	{

	}

	public static readonly string[] SymbolList = new string[]{
		"(", ")", "[", "]", "{", "}", ".",
		"!",
		"**",
		"*", "/", "%",
		"+", "-",
		">", "<", ">=", "<=",
		"!=", "==",
		"&&", "||",
		",",
	};

}

namespace DataBind.VM
{
	using ASTNode = ASTNodeBase;
	using Node = CombineType<object, List<WordNode>, WordNode, ASTNodeBase>;
	using boolean = System.Boolean;
	using number = System.Double;

	using ENodeType = System.Int32;
	using TEnv = Dictionary<string, object>;

	public class TNodeType
	{
		public static readonly List<string> Symbols = new List<string>(){
		//运算符
		"P0",
		"[", "(", "{", ".", "P1",
		"!", "P2",
		"**", "P3",
		"*", "/", "%", "P4",
		"+", "-", "P5",
		">", "<", ">=", "<=", "P6",
		"!=", "==", "P7",
		"&&", "P8", "||", "P9",
		",", "P10",

		"]", ")", "}", "P11",//结束符号

        //值
        "number",
		"word",
		"string",
		"boolean",
		"null",
		"P12",
		"annotation",

        //组合，只会在AST中出现
        "call",
		"lambda"
		};

		public static readonly TNodeType Inst = new TNodeType();
		public ENodeType this[string x]
		{
			get => Symbols.IndexOf(x);
		}
		public string this[ENodeType x]
		{
			get => Symbols[(int)x];
		}

		/// <summary>
		/// [
		/// </summary>
		public static ENodeType BracketL => Symbols.IndexOf("[");
		/// <summary>
		/// ]
		/// </summary>
		public static ENodeType BracketR => Symbols.IndexOf("]");
		/// <summary>
		/// {
		/// </summary>
		public static ENodeType BraceL => Symbols.IndexOf("{");
		/// <summary>
		/// }
		/// </summary>
		public static ENodeType BraceR => Symbols.IndexOf("}");
		/// <summary>
		/// (
		/// </summary>
		public static ENodeType ParenthesesL => Symbols.IndexOf("(");
		/// <summary>
		/// )
		/// </summary>
		public static ENodeType ParenthesesR => Symbols.IndexOf(")");

		public static ENodeType P0 => Symbols.IndexOf("P0");
		public static ENodeType P1 => Symbols.IndexOf("P1");
		public static ENodeType P2 => Symbols.IndexOf("P2");
		public static ENodeType P3 => Symbols.IndexOf("P3");
		public static ENodeType P4 => Symbols.IndexOf("P4");
		public static ENodeType P5 => Symbols.IndexOf("P5");
		public static ENodeType P6 => Symbols.IndexOf("P6");
		public static ENodeType P7 => Symbols.IndexOf("P7");
		public static ENodeType P8 => Symbols.IndexOf("P8");
		public static ENodeType P9 => Symbols.IndexOf("P9");
		public static ENodeType P10 => Symbols.IndexOf("P10");
		public static ENodeType P11 => Symbols.IndexOf("P11");
		public static ENodeType P12 => Symbols.IndexOf("P12");
		public static ENodeType Number => Symbols.IndexOf("number");
		public static ENodeType Word => Symbols.IndexOf("word");
		public static ENodeType Stringx => Symbols.IndexOf("string");
		public static ENodeType Boolean => Symbols.IndexOf("boolean");
		public static ENodeType Nullx => Symbols.IndexOf("null");
		public static ENodeType Annotation => Symbols.IndexOf("annotation");
		public static ENodeType Call => Symbols.IndexOf("call");
		public static ENodeType Lambda => Symbols.IndexOf("lambda");

	}
    
	public class NodeType : TNodeType { };

	public class WordNode
	{
		public number lineEnd;
		//父节点
		public ASTNode Parent = null;
		/**
         * 相关注释
         */
		public string FrontAnnotation;
		public string BehindAnnotation;


		public ENodeType Type;

		public object Value;

		public number LineStart;

		public number ColumnStart;

		public number ColumnEnd;
		public WordNode(
			ENodeType type,
			object value,
			 number lineStart,
			 number columnStart,
			 number columnEnd
		)
		{
			this.Type = type;
			this.Value = value;
			this.LineStart = lineStart;
			this.ColumnStart = columnStart;
			this.ColumnEnd = columnEnd;
			this.lineEnd = lineStart;
		}
	}


	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class ASTNodeBase
	{
		//父节点
		public ASTNode Parent = null;
		/**
         * 相关注释
         */
		// public virtual string FrontAnnotation=>"";
		// public virtual string BehindAnnotation=>"";
		/// <summary>
		/// @type: ENodeType
		/// </summary>
		public ENodeType OperatorX;

		public string OperatorName => TNodeType.Inst[OperatorX];

		public ASTNodeBase(
			/**
             * 操作符
             */
			ENodeType operatorX
		)
		{
			this.OperatorX = operatorX;
		}

		internal virtual string DebuggerDisplay => $"{OperatorName}";
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class ValueASTNode : ASTNodeBase
	{
		public WordNode Value;
		public ValueASTNode(
			 WordNode value
		) : base(value.Type)
		{
			this.Value = value;
		}
		
		internal override string DebuggerDisplay => $"{Value.Value}";
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class BracketASTNode : ASTNodeBase
	{
		public ASTNode Node;
		public BracketASTNode(
			 ENodeType operatorX,
			 ASTNode node
		) : base(operatorX)
		{
			this.OperatorX = operatorX;
			this.Node = node;
		}

		internal override string DebuggerDisplay
		{
			get
			{
				if (OperatorX == TNodeType.Inst["["])
				{
					return $"[{Node.DebuggerDisplay}]";
				}
				else if (OperatorX == TNodeType.Inst["{"])
				{
					return $"{{{Node.DebuggerDisplay}}}";
				}
				else if (OperatorX == TNodeType.Inst["<"])
				{
					return $"<{Node.DebuggerDisplay}>";
				}
				else
				{
					return $"({Node.DebuggerDisplay})";
				}
			}
		}
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class UnitaryASTNode : ASTNodeBase
	{
		/**
		 * 一元表达式的右值
		 */
		public ASTNode Right;
		public UnitaryASTNode(
			 ENodeType operatorX,
			 /**
			  * 一元表达式的右值
			  */
			 ASTNode right
		) : base(operatorX)
		{
			this.OperatorX = operatorX;
			this.Right = right;
			this.Right.Parent = this;
		}
		
		internal override string DebuggerDisplay => $"{OperatorName}{Right.DebuggerDisplay}";
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class BinaryASTNode : ASTNodeBase
	{
		/**
		 * 二元表达式的左值
		 */
		public ASTNode Left;
		/**
		 * 二元表达式的左值
		 */
		public ASTNode Right;
		public BinaryASTNode(
			 /**
			  * 二元表达式的左值
			  */
			 ASTNode left,
			 /**
			  * 运算符
			  */
			 ENodeType operatorX,
			 /**
			  * 二元表达式的左值
			  */
			 ASTNode right
		) : base(operatorX)
		{
			this.Left = left;
			this.OperatorX = operatorX;
			this.Right = right;

			this.Left.Parent = this;
			this.Right.Parent = this;
		}

		internal override string DebuggerDisplay
		{
			get
			{
				if (OperatorX == TNodeType.Inst["["] && Right is BracketASTNode && Right.OperatorX == TNodeType.Inst["["])
				{
					return $"{Left.DebuggerDisplay}{Right.DebuggerDisplay}";
				}
				else
				{
					return $"{Left.DebuggerDisplay}{OperatorName}{Right.DebuggerDisplay}";
				}
			}
		}
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class CallASTNode : ASTNodeBase
	{
		/**
		 * 函数访问节点
		 */
		public ASTNode Left;
		/**
		 * 函数参数列表
		 */
		public List<ASTNode> Parameters;
		public CallASTNode(
			 /**
			  * 函数访问节点
			  */
			 ASTNode left,
			 /**
			  * 函数参数列表
			  */
			 List<ASTNode> parameters
		) : base(TNodeType.Call)
		{
			this.Left = left;
			this.Parameters = parameters;

			this.Left.Parent = this;
			this.Parameters.ForEach(a => a.Parent = this);
		}
		
		internal override string DebuggerDisplay => $"{Left.DebuggerDisplay}()";
	}

	public class Interpreter
	{

		static Interpreter()
		{
			_Env.SymbolList.ForEach(a => operatorCharMap[a.CharAt(0)] = true);
			(new string[] { "\"", "'", "`" }).ForEach(a => markMap[a] = true);
			_Env.SymbolList.ForEach(a =>
			{
				if (a.Length > 1)
				{
					doubleOpMap[a.CharAt(0)] = true;
				}
			});
			(new string[] { " ", "\n", "\r", "\t" }).ForEach(a => spaceMap[a] = true);
		}
		static readonly int ZeroCode = "0".CharCodeAt(0);
		static readonly int NineCode = "9".CharCodeAt(0);

		static readonly Dictionary<string, bool> operatorCharMap = new Dictionary<string, bool>();

		static readonly Dictionary<string, bool> markMap = new Dictionary<string, bool>();

		static readonly Dictionary<string, bool> doubleOpMap = new Dictionary<string, bool>();


		static readonly Dictionary<string, bool> spaceMap = new Dictionary<string, bool>();


		public readonly ASTNode Ast;

		public readonly List<string> AstErrorList = new List<string>();

		public string Expression;
		public Interpreter(
			string expression
		)
		{
			this.Expression = expression;
			this.Ast = Interpreter.ToAst(Interpreter.ToWords(this.Expression), this.Expression, this.AstErrorList);
		}

		public static List<WordNode> ToWords(string expression)
		{
			number line = 0;
			number column = 0;
			number startColum = -1;//仅仅在多行的处理中使用
			var temp = "";
			var lastChar = "";
			number state = 0;//0初始状态；1数字；2运算符；3引号字符串；4单词；5行注释；6块注释
			string markType = null;

			List<WordNode> nodeList = new List<WordNode>();

			Action reset = () =>
			{
				state = 0;
				temp = "";
			};
			Action<string> run = null;
			run = (string charx) =>
			  {
				  if (state == 0)
				  {
					  if (spaceMap.ContainsKey(charx))
					  {
						  return;
					  }
					  var code = charx.CharCodeAt(0);
					  if (code >= ZeroCode && code <= NineCode)
					  {
						  //数字
						  state = 1;
						  temp += charx;
					  }
					  else if (operatorCharMap.ContainsKey(charx))
					  {
						  //运算符
						  temp += charx;
						  if (doubleOpMap.ContainsKey(charx) || charx == "/")
						  {//有// 和 /* 等两种注释的情况
						   //可能是多运算符
							  state = 2;
						  }
						  else if (charx == "-" && (nodeList.Count != 0 && nodeList.TryGet(nodeList.Count - 1).Type < TNodeType.P10 || nodeList.Count == 0))
						  {
							  //负数数字
							  state = 1;
						  }
						  else
						  {
							  if (TNodeType.Inst[temp] < 0)
							  {
								  throw new Exception("表达式编译失败" + expression + " 不支持的运算符: " + temp);
							  }
							  nodeList.Add(new WordNode(TNodeType.Inst[temp], null, line, column - temp.Length + 1, column));

							  reset();
						  }

					  }
					  else if (markMap.ContainsKey(charx))
					  {
						  //引号
						  markType = charx;
						  startColum = column;
						  state = 3;
					  }
					  else
					  {
						  //单词
						  temp += charx;
						  state = 4;
					  }
				  }
				  else if (state == 1)
				  {
					  //数字
					  var code = charx.CharCodeAt(0);
					  if (code >= ZeroCode && code <= NineCode || charx == ".")
					  {
						  temp += charx;
					  }
					  else
					  {
						  object eNumValue = MathInsider.ParseNumStr(temp);
						  nodeList.Add(new WordNode(TNodeType.Number, eNumValue, line, column - temp.Length, column - 1));
						  reset();
						  run(charx);//重新执行
					  }
				  }
				  else if (state == 2)
				  {
					  //运算符
					  var mg = temp + charx;
					  if (mg == "//")
					  {
						  //行注释
						  temp += charx;
						  state = 5;
					  }
					  else if (mg == "/*")
					  {
						  //块注释
						  temp += charx;
						  startColum = column - 1;
						  state = 6;
					  }
					  else if (TNodeType.Inst[mg] >= 0)
					  {
						  //识别到运算符
						  temp += charx;
						  nodeList.Add(new WordNode(TNodeType.Inst[temp], null, line, column - temp.Length + 1, column));

						  reset();
					  }
					  else
					  {
						  nodeList.Add(new WordNode(TNodeType.Inst[temp], null, line, column - temp.Length, column - 1));
						  reset();
						  run(charx);//重新执行
					  }

				  }
				  else if (state == 3)
				  {
					  //引号
					  if (charx == markType && lastChar != "\\")
					  {
						  if (markType == "`")
						  {
							  var node = new WordNode(TNodeType.Stringx, temp, line, startColum, column);
							  node.LineStart = line - (temp.Match("\n", RegexOptions.Multiline)?.Length ?? 0);
							  nodeList.Add(node);
						  }
						  else
						  {
							  nodeList.Add(new WordNode(TNodeType.Stringx, temp, line, startColum, column));
						  }
						  reset();
					  }
					  else
					  {
						  temp += charx;
					  }
				  }
				  else if (state == 4)
				  {
					  //单词
					  if (spaceMap.ContainsKey(charx) || operatorCharMap.ContainsKey(charx) || markMap.ContainsKey(charx))
					  {
						  if (temp == "true" || temp == "false")
						  {
							  nodeList.Add(new WordNode(TNodeType.Boolean, temp == "true", line, column - temp.Length, column - 1));

						  }
						  else if (temp == "null")
						  {
							  nodeList.Add(new WordNode(TNodeType.Nullx, null, line, column - temp.Length, column - 1));

						  }
						  else
						  {
							  nodeList.Add(new WordNode(TNodeType.Word, temp, line, column - temp.Length, column - 1));

						  }
						  reset();
						  run(charx);//重新执行
					  }
					  else
					  {
						  temp += charx;
					  }
				  }
				  else if (state == 5)
				  {
					  //行注释
					  if (charx == "\n" || charx == "\r")
					  {
						  nodeList.Add(new WordNode(TNodeType.Annotation, temp, line, column - temp.Length, column));

						  reset();
						  //不需要重新执行，换行可以丢弃
					  }
					  else
					  {
						  temp += charx;
					  }
				  }
				  else if (state == 6)
				  {
					  //块注释
					  if (lastChar + charx == "*/")
					  {
						  temp += charx;

						  var node = new WordNode(TNodeType.Annotation, temp, line, startColum, column);

						  node.LineStart = line - temp.Match("\n", RegexOptions.Multiline)?.Length ?? 0;
						  nodeList.Add(node);
						  reset();
					  }
					  else
					  {
						  temp += charx;
					  }

				  }

			  };

			foreach (var charx0 in expression)
			{
				var charx = "" + charx0;
				run(charx);
				lastChar = charx;
				if (charx == "\n")
				{
					line++;
					column = 0;
				}
				else
				{
					column++;
				}
			}
			run(" ");//传入空格，使其收集最后的结束点

			return nodeList;
		}

		protected void PushError(List<string> errorList, List<Node> nodes, string msg)
		{
			var errorPos = nodes[0];
			var errorMsg = Expression + msg;
			if (errorPos != null)
			{
				errorMsg += $"，在{(errorPos.As<WordNode>()).lineEnd + 1}:{errorPos.As<WordNode>().ColumnEnd + 1}。";
			}
			errorList.Add(errorMsg);
		}

		public static ASTNode ToAst(List<WordNode> nodeList, string expression, List<string> errorList)
		{
			void pushError(List<string> errorList, Node node, string msg)
			{
				var errorPos = node;
				var errorMsg = expression + msg;
				if (errorPos != null)
				{
					errorMsg += $"，在{(errorPos.As<WordNode>()).lineEnd + 1}:{errorPos.As<WordNode>().ColumnEnd + 1}。";
				}
				errorList.Add(errorMsg);
			}

			//根据运算符优先级进行分组
			List<Node> bracketList = new List<Node>();

			Dictionary<number, boolean> bracketMap = new Dictionary<number, boolean>();
			(new number[] { TNodeType.ParenthesesL, TNodeType.BracketL, TNodeType.BraceL }).ForEach(k => bracketMap[k] = true);

			/**
             * 将括号按层级分组成数组
             */
			number ConvertBracket(number start, List<Node> list, ENodeType? endType = null)
			{
				for (var i = start; i < nodeList.Count; i++)
				{
					var current = nodeList.TryGet((int)i);
					if (bracketMap.ContainsKey(current.Type))
					{
						//发现括号
						ENodeType nextEndType;
						switch (current.Type)
						{
							case ENodeType type when type == TNodeType.Inst["("]:
								nextEndType = TNodeType.Inst[")"];
								break;
							case ENodeType type when type == TNodeType.Inst["["]:
								nextEndType = TNodeType.Inst["]"];
								break;
							case ENodeType type when type == TNodeType.Inst["{"]:
								nextEndType = TNodeType.Inst["}"];
								break;
							default:
								throw new Exception(expression + "括号分析异常异常'" + TNodeType.Inst[current.Type] + "' " + current.LineStart + ":" + current.ColumnStart);
						}
						List<WordNode> newList = new List<WordNode>() { current };
						i = ConvertBracket(i + 1, newList.AsList<Node>(), nextEndType);
						list.Add(newList.AsList<WordNode>());
					}
					else if (endType != null && endType == current.Type)
					{
						list.Add(current);
						return i;
					}
					else
					{
						list.Add(current);
					}
				}
				if (endType != null && (list.TryGet(list.Count - 1).As<WordNode>()).Type != endType)
				{
					pushError(errorList, list.TryGet(list.Count - 1), $"缺少闭合括号'${TNodeType.Inst[endType.Value]}'");
					//自动补充一个符号
					list.Add(new WordNode(endType.Value, null, 0, 0, 0));
				}
				return nodeList.Count;
			};

			Func<List<Node>, ASTNode> genAST = null;

			List<Node> UnaryExp(List<Node> list, double startPriority, double endPriority)
			{
				if (list.Count <= 1)
				{
					return list;
				}

				//当前环境下单目运算符只会在值的左边
				//连续多个单目运算符从右往左组合运算
				List<Node> rlist = new List<Node>();
				ASTNode currentAST = null;
				for (var i = list.Count - 1; i >= 0; i--)
				{
					var a = list.TryGet(i);
					var b = list.TryGet(i - 1);
					if (b.Is<WordNode>() && b.As<WordNode>().Type > startPriority && b.As<WordNode>().Type < endPriority)
					{
						if (a == null)
						{
							pushError(errorList, a, "一元运算符" + TNodeType.Inst[b.As<WordNode>().Type] + "缺少右值");
							a = new WordNode(TNodeType.Boolean, true, 0, 0, 0); //自动补充
						}

						if (currentAST == null)
						{
							//第一次发现
							var ls = a.IsList() ? a.AsList<Node>() : new List<Node>() { a };
							currentAST = new UnitaryASTNode(b.As<WordNode>().Type, genAST(ls));
						}
						else
						{
							//多个单目运算符连续
							currentAST = new UnitaryASTNode(b.As<WordNode>().Type, currentAST);
						}
					}
					else
					{
						if (currentAST != null)
						{
							//一轮连续的单目运算符组合完毕
							rlist.push(currentAST);
							currentAST = null;
						}
						else
						{
							rlist.push(a); //上次必然已经被加入了ast中，因此不需要push
						}
					}
				}

				if (currentAST != null)
				{
					//边界对象不要遗留
					rlist.push(currentAST);
				}

				rlist.reverse(); //转为正常的顺序
				return rlist;
			}

			Func<List<Node>, List<ASTNode>> genParamList = null;

			List<Node> BinaryExp(List<Node> list, double startPriority, double endPriority)
			{
				if (list.length <= 1)
				{
					return list;
				}

				List<Node> rlist = new List<Node>();
				ASTNode currentAST = null;
				for (int i = 1, l = list.length; i < l; i++)
				{
					var a = list.TryGet(i - 1);
					var b = list.TryGet(i);
					var c = list.TryGet(i + 1);
					if (b.Is<WordNode>() && startPriority < b.As<WordNode>().Type && b.As<WordNode>().Type < endPriority)
					{
						if (c == null)
						{
							pushError(errorList, a, "二元运算符" + TNodeType.Inst[b.As<WordNode>().Type] + "缺少右值");
							c = new WordNode(TNodeType.Number, 0, 0, 0, 0); //自动补充
						}

						if (currentAST == null)
						{
							//第一次发现
							rlist.pop(); //删除上次循环所插入的b
							var ls = c.IsList() ? c.AsList<WordNode>().AsList<Node>() : new List<Node>() { c };
							currentAST = new BinaryASTNode(genAST(a.IsList() ? a.AsList<Node>() : new List<Node>() { a }), b.As<WordNode>().Type, genAST(ls));
						}
						else
						{
							//多次双目运算符连续
							currentAST = new BinaryASTNode(currentAST, b.As<WordNode>().Type, genAST(c.IsList() ? c.AsList<WordNode>().AsList<Node>() : new List<Node>() { c }));
						}


						//特殊处理 . 和 [] 后续逻辑，可能会紧跟着函数调用
						var d = list.TryGet(i + 2);
						if (endPriority == TNodeType.P1 && d.IsList() && d.AsList<WordNode>().TryGet(0) is WordNode && d.AsList<WordNode>().TryGet(0).Type == TNodeType.Inst["("])
						{
							currentAST = new CallASTNode(currentAST, genParamList(d.AsList<WordNode>().AsList<Node>()));

							i++; //跳过d的遍历
						}

						i++; //跳过c的遍历
					}

					//特殊处理，仅处理a['b']中括号的访问方式。
					else if (b.IsList() && b.AsList<WordNode>().TryGet(0) is WordNode && b.AsList<WordNode>().TryGet(0).Type == TNodeType.Inst["["])
					{
						//中括号方式访问属性
						if (currentAST != null)
						{
							currentAST = new BinaryASTNode(currentAST, TNodeType.Inst["["], genAST(b.AsList<Node>()));
						}
						else
						{
							rlist.pop(); //删除上次循环所插入的b
							currentAST = new BinaryASTNode(genAST(a.IsList() ? a.AsList<WordNode>().AsList<Node>() : new List<Node>() { a }), TNodeType.Inst["["], genAST(b.AsList<Node>()));
						}

						//特殊处理 . 和 [] 后续逻辑，可能会紧跟着函数调用
						if (endPriority == TNodeType.P1 && c.IsList() && c.AsList<WordNode>().TryGet(0) is WordNode && c.AsList<WordNode>().TryGet(0).Type == TNodeType.Inst["("])
						{
							currentAST = new CallASTNode(currentAST, genParamList(c.AsList<Node>()));

							i++; //跳过c的遍历
						}
					}
					else
					{
						if (currentAST != null)
						{
							if (endPriority == TNodeType.P1 && b.IsList() && b.AsList<WordNode>().TryGet(0) is WordNode && b.AsList<WordNode>().TryGet(0).Type == TNodeType.Inst["("])
							{
								currentAST = new CallASTNode(currentAST, genParamList(b.AsList<Node>()));
								continue;
							}
							else
							{
								//一轮连续的双目运算符组合完毕
								rlist.push(currentAST);
								currentAST = null;
							}
						}
						else if (endPriority == TNodeType.P1 && a.Is<WordNode>() && a.As<WordNode>().Type == TNodeType.Word && b.IsList() && b.AsList<WordNode>().TryGet(0) is WordNode && b.AsList<WordNode>().TryGet(0).Type == TNodeType.Inst["("])
						{
							//特殊处理 . 和 [] 后续逻辑，可能会紧跟着函数调用
							currentAST = new CallASTNode(genAST(a.IsList() ? a.AsList<Node>() : new List<Node>() { a }), genParamList(b.AsList<Node>()));
							rlist.pop(); //删除上次循环所插入的b
							continue; //a和b都需要插入到rlist
						}

						if (i == 1)
						{
							//由于是从1开始遍历的，因此需要保留0的值
							rlist.push(a);
						}

						rlist.push(b);
					}
				}

				if (currentAST != null)
				{
					//边界对象不要遗留
					rlist.push(currentAST);
				}

				return rlist;
			}

			List<List<Node>> Splice(List<Node> list, double sp)
			{
				List<List<Node>> r = new List<List<Node>>();
				List<Node> current = new List<Node>();
				foreach (var l in list)
				{
					//这里会忽略括号
					if (l.Is<WordNode>())
					{
						if (l.As<WordNode>().Type == sp)
						{
							//产生切割
							if (current.length > 0)
							{
								r.push(current);
								current = new List<Node>();
							}
						}
						else if (l.As<WordNode>().Type == TNodeType.Inst["("] || l.As<WordNode>().Type == TNodeType.Inst[")"] || l.As<WordNode>().Type == TNodeType.Inst["["] || l.As<WordNode>().Type == TNodeType.Inst["]"] || l.As<WordNode>().Type == TNodeType.Inst["{"] || l.As<WordNode>().Type == TNodeType.Inst["}"])
						{
							//跳过该字符
						}
						else
						{
							current.push(l);
						}
					}
					else
					{
						current.push(l);
					}
				}

				if (current.length > 0)
				{
					r.push(current);
				}

				return r;
			}


			genParamList = (List<Node> list) =>
			{
				var paramList = Splice(list, TNodeType.Inst[","]);
				List<ASTNode> rlist = new List<ASTNode>();
				foreach (var p in paramList)
				{
					rlist.push(genAST(p));
				}
				return rlist;
			};

			genAST = (List<Node> sourcelist) =>
			{
				if (sourcelist.Count == 1 && sourcelist[0].Is<ASTNodeBase>())
				{
					return sourcelist[0].As<ASTNodeBase>();
				}
				if (sourcelist.length == 1 && sourcelist[0].IsList())
				{
					return genAST(sourcelist[0].AsList<Node>());
				}

				var list = sourcelist;

				//进行括号处理
				ENodeType? bracketType = null;
				var a = list.TryGet(0);
				var b = list.TryGet(list.length - 1);
				if (a.Is<WordNode>() && b.Is<WordNode>() &&
					(
						a.As<WordNode>().Type == TNodeType.Inst["("] && b.As<WordNode>().Type == TNodeType.Inst[")"] ||
						a.As<WordNode>().Type == TNodeType.Inst["["] && b.As<WordNode>().Type == TNodeType.Inst["]"] ||
						a.As<WordNode>().Type == TNodeType.Inst["{"] && b.As<WordNode>().Type == TNodeType.Inst["}"])
				)
				{
					bracketType = a.As<WordNode>().Type;
					list = list.slice(1, list.length - 1);
				}

				list = BinaryExp(list, TNodeType.P0, TNodeType.P1);//分组  . 和 [] 形成访问连接，包括后面的函数
				list = UnaryExp(list, TNodeType.P1, TNodeType.P2);//分组  ! ，进行一元表达式分组
				list = BinaryExp(list, TNodeType.P2, TNodeType.P3);//分组  **，进行2元表达式分组
				list = BinaryExp(list, TNodeType.P3, TNodeType.P4);//分组  * / %，进行2元表达式分组
				list = BinaryExp(list, TNodeType.P4, TNodeType.P5);//分组  + -，进行2元表达式分组
				list = BinaryExp(list, TNodeType.P5, TNodeType.P6);//分组  > < >= <=，进行2元表达式分组
				list = BinaryExp(list, TNodeType.P6, TNodeType.P7);//分组  != ==，进行2元表达式分组
				list = BinaryExp(list, TNodeType.P7, TNodeType.P8);//分组  && ，进行2元表达式分组
				list = BinaryExp(list, TNodeType.P8, TNodeType.P9);//分组  ||，进行2元表达式分组

				ASTNode result;
				if (list.length == 1 && list[0].Is<ASTNodeBase>())
				{
					//正常返回
					result = list[0].As<ASTNodeBase>();
				}
				else if (list.length == 1 && list[0].Is<WordNode>())
				{
					//单纯的数值
					result = new ValueASTNode(list[0].As<WordNode>());
				}
				else if (list.length > 1)
				{
					pushError(errorList, sourcelist[0], "解析后节点列表无法归一");

					result = new ValueASTNode(new WordNode(TNodeType.Number, 0, 0, 0, 0));
				}
				else
				{
					pushError(errorList, sourcelist.TryGet(0), "无法正确解析列表");

					result = new ValueASTNode(new WordNode(TNodeType.Number, 0, 0, 0, 0));
				}

				if (bracketType != null)
				{
					if (bracketType == TNodeType.Inst["{"])
					{
						return new BracketASTNode(TNodeType.Lambda, result);
					}
					else
					{
						return new BracketASTNode(bracketType.Value, result);
					}
				}
				else
				{
					return result;
				}
			};
			nodeList = nodeList.FindAll(a => a.Type != TNodeType.Annotation);
			ConvertBracket(0, bracketList);//分组括号
			return genAST(bracketList);
		}


		public static string ToStringAst(ASTNode ast0, boolean addBracket = false)
		{
			object CS(object v)
			{
				if (v == null)
				{
					return "null";
				}
				else
				{
					return v;
				}
			}

			var r = "";
			if (addBracket && !(ast0 is ValueASTNode || ast0 is BracketASTNode))
			{
				r += "(";
			}
			if (ast0 is ValueASTNode)
			{
				var ast = ast0 as ValueASTNode;
				if (ast.Value.Type == TNodeType.Stringx)
				{
					r += $"\"{CS(ast.Value.Value)}\"";
				}
				else
				{
					r += $"{CS(ast.Value.Value)}";
				}
			}
			else if (ast0 is BracketASTNode)
			{
				var ast = ast0 as BracketASTNode;
				if (ast.OperatorX == TNodeType.Inst["("])
				{
					r += $"({Interpreter.ToStringAst(ast.Node, addBracket)})";
				}
				else if (ast.OperatorX == TNodeType.Inst["["])
				{
					r += $"[{Interpreter.ToStringAst(ast.Node, addBracket)}]";
				}
				else if (ast.OperatorX == TNodeType.Inst["{"] || ast.OperatorX == TNodeType.Lambda)
				{
					r += $"{{{Interpreter.ToStringAst(ast.Node, addBracket)}}}";
				}
			}
			else if (ast0 is UnitaryASTNode)
			{
				var ast = ast0 as UnitaryASTNode;
				r += $"{TNodeType.Inst[ast.OperatorX]}{Interpreter.ToStringAst(ast.Right, addBracket)}";
			}
			else if (ast0 is BinaryASTNode)
			{
				var ast = ast0 as BinaryASTNode;
				if (ast.OperatorX == TNodeType.Inst["["])
				{
					r += $"{Interpreter.ToStringAst(ast.Left, addBracket)}{Interpreter.ToStringAst(ast.Right, addBracket)}";
				}
				else if (ast.OperatorX == TNodeType.Inst["."])
				{
					r += $"{Interpreter.ToStringAst(ast.Left, addBracket)}{TNodeType.Inst[ast.OperatorX]}{Interpreter.ToStringAst(ast.Right, addBracket)}";
				}
				else
				{
					r += $"{Interpreter.ToStringAst(ast.Left, addBracket)} {TNodeType.Inst[ast.OperatorX]} {Interpreter.ToStringAst(ast.Right, addBracket)}";
				}
			}
			else if (ast0 is CallASTNode)
			{
				var ast = ast0 as CallASTNode;
				r += $"{Interpreter.ToStringAst(ast.Left, addBracket)}( {ast.Parameters.Select(a => Interpreter.ToStringAst(a, addBracket)).ToConvableList<string>().Join(", ")})";
			}
			if (addBracket && !(ast0 is ValueASTNode || ast0 is BracketASTNode))
			{
				r += ")";
			}
			return r;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return Interpreter.ToStringAst(this.Ast);
		}

		/**
         * 该函数所执行的表达式将自动进行容错处理
         * 1. 当访问属性产生null值时，其将不参与计算 例如：a.b+13 当a或b为空时，结果将返回13
         * 2. 当访问的表达式完全为null时，表达式将最终返回结果0，例如：a.b+c 则返回0
         * @param environment 
         * @param ast 
         */
		static object SRun(IWithPrototype environment, ASTNode ast0)
		{
			object ast1 = ast0;
			if (ast1 is ValueASTNode)
			{
				var ast = ast1 as ValueASTNode;
				if (ast.OperatorX == TNodeType.Word)
				{
					if ("this" == (Utils.ToIndexKey(ast.Value.Value)))
					{
						return environment;
					}
					else
					{
						var v = Utils.IndexValueRecursive(environment, Utils.ToIndexKey(ast.Value.Value));
						return v;
					}
				}
				else
				{
					return ast.Value.Value;
				}
			}
			else if (ast1 is BracketASTNode)
			{
				var ast = ast1 as BracketASTNode;
				return Interpreter.SRun(environment, ast.Node);//括号内必然是个整体
			}
			else if (ast1 is UnitaryASTNode)
			{
				var ast = ast1 as UnitaryASTNode;
				var b = Interpreter.SRun(environment, ast.Right);
				switch (ast.OperatorX)
				{
					case ENodeType type when type == TNodeType.Inst["!"]:
						return Utils.IsFalse(b);
					default:
						throw new Exception($"意外的一元运算符[{TNodeType.Inst[ast.OperatorX]}]");
				}
			}
			else if (ast1 is BinaryASTNode)
			{
				var ast = ast1 as BinaryASTNode;
				if (ast.OperatorX == TNodeType.Inst["."] || ast.OperatorX == TNodeType.Inst["["])
				{
					object a = Interpreter.SRun(environment, ast.Left);

					if (a == null)
					{
						Console.Error(Interpreter.ToStringAst(ast) + "\n" + "属性访问异常" + Interpreter.ToStringAst(ast.Left));
						return null;//访问运算遇到null则不执行
					}
					if (ast.Right is ValueASTNode)
					{
						bool exist;
						var ret1=Utils.IndexValueRecursive(a, (ast.Right as ValueASTNode).Value.Value, out exist);
						if(exist == false)
                        {
							//console.warn(Interpreter.toStringAST(ast.left) + "\n" + "无法获取属性" + Interpreter.toStringAST(ast.right));
						}
						return ret1;
					}
					else
					{
						bool exist;
						var ret2=Utils.IndexValueRecursive(a, Interpreter.SRun(environment, ast.Right), out exist);
                        if (exist == false)
                        {
							//console.warn(Interpreter.toStringAST(ast.left) + "\n" + "无法获取属性" + Interpreter.toStringAST(ast.right));
						}
						return ret2;
					}
				}

				if (ast.OperatorX == TNodeType.Inst["&&"])
				{
					//先左，后右
					var a = Interpreter.SRun(environment, ast.Left);
					if (a == null)
					{
						return a;

					}
					else
					{
						return Interpreter.SRun(environment, ast.Right);
					}
				}
				else if (ast.OperatorX == TNodeType.Inst["||"])
				{
					var a = Interpreter.SRun(environment, ast.Left);
					if (a != null)
					{
						return a;
					}
					return a ?? Interpreter.SRun(environment, ast.Right);
				}

				{
					var a = Interpreter.SRun(environment, ast.Left);
					var b = Interpreter.SRun(environment, ast.Right);

					if (!(ast.OperatorX == TNodeType.Inst["=="] || ast.OperatorX == TNodeType.Inst["!="]))
					{
						if (a == null && b == null)
						{
							return null;
						}
						else if (a == null && b != null)
						{
							return b;
						}
						else if (a != null && b == null)
						{
							return a;
						}
					}

					{
						if (
							a != null && b != null &&
							(a.GetType().IsPrimitive && b.GetType().IsPrimitive)
							&& (
								(a is double || a is double)
								|| (a is float || b is float)
								|| (a is ulong || b is ulong)
								|| (a is long || b is long)
								|| (a is uint || b is uint)
								|| (a is int || b is int)
								|| (a is ushort || b is ushort)
								|| (a is short || b is short)
								)
							)
						{
							var ret = MathInsider.CalcUnkownNumOp(ast.OperatorX, a, b);
							return ret;
						}
						else
						{
							// TODO: 支持自动定位运算符
							switch (ast.OperatorX)
							{
								case ENodeType op when op == TNodeType.Inst["**"]:
									{
										var mOp = a.GetType().GetMethod("op_Pow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst["*"]:
									{
										var mOp = a.GetType().GetMethod("op_Multiply", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst["/"]:
									{
										var mOp = a.GetType().GetMethod("op_Division", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst["%"]:
									{
										var mOp = a.GetType().GetMethod("op_Modulus", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst["+"]:
									{
										var mOp = a.GetType().GetMethod("op_Addition", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst["-"]:
									{
										var mOp = a.GetType().GetMethod("op_Subtraction", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst[">"]:
									{
										var mOp = a.GetType().GetMethod("op_GreaterThan", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst["<"]:
									{
										var mOp = a.GetType().GetMethod("op_LessThan", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst[">="]:
									{
										var mOp = a.GetType().GetMethod("op_GreaterThanOrEqual", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst["<="]:
									{
										var mOp = a.GetType().GetMethod("op_LessThanOrEqual", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case ENodeType op when op == TNodeType.Inst["!="]:
									{
										//var mOp = a.GetType().GetMethod("op_Inequality", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										//var ret = mOp.Invoke(a, new object[] { b });
										//return ret;
										return a != b;
									}
								case ENodeType op when op == TNodeType.Inst["=="]:
									{
										//var mOp = a.GetType().GetMethod("op_Equality", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										//var ret = mOp.Invoke(a, new object[] { b });
										//return ret;
										return a == b;
									}
								default:
									throw new Exception($"意外的二元运算符[{TNodeType.Inst[ast.OperatorX]}]");
							}
						}
					}
				}
			}
			else if (ast1 is CallASTNode)
			{
				var ast = ast1 as CallASTNode;
				var obj = Interpreter.SRun(environment, ast.Left);

				object self = null;
				object func = null;

				List<object> paramList = ast.Parameters.Select(p =>
				{
					if (p is BracketASTNode && p.OperatorX == TNodeType.Lambda)
					{
						Func<object, object> f = (object a) =>
						{
							IWithPrototype newEv;
							if (a.IsWithProto())
							{
								newEv = (IWithPrototype)Convert.ChangeType(a, typeof(IWithPrototype));
							}
							else
							{
								newEv = new TEnv() { { "value", a } };
							}
							newEv.SetProto(environment);

							var ret = Interpreter.SRun(newEv, (p as BracketASTNode).Node);
							return ret;
						};
						return f;
					}
					else
					{
						return Interpreter.SRun(environment, p);
					}
				}).ToConvableList();

				if (ast.Left is ValueASTNode valueL)
				{
					var ret = valueL.Value.Value;
					//全局函数
					func = Utils.IndexMethodRecursive(environment, ret, Utils.ExtractValuesTypes(paramList));
				}
				else if (ast.Left is BinaryASTNode bL)
				{
					self = Interpreter.SRun(environment, bL.Left);
					if (self == null)
					{
						Console.Error(Interpreter.ToStringAst(ast) + "\n" + "函数无法访问" + Interpreter.ToStringAst((ast.Left as BinaryASTNode).Left));
						return null;//self无法获取
					}
					if ((ast.Left as BinaryASTNode).Right is ValueASTNode nodeLR)
					{
						var ret = nodeLR.Value.Value;
						func = Utils.IndexMethodRecursive(self, ret, Utils.ExtractValuesTypes(paramList));
					}
					else
					{
						var ret = Interpreter.SRun(environment, (ast.Left as BinaryASTNode).Right);
						func = Utils.IndexMethodRecursive(self, ret, Utils.ExtractValuesTypes(paramList));
					}
				}
				if (func == null)
				{
					Console.Error(Interpreter.ToStringAst(ast) + "\n" + "函数无法访问");
					return null;//func无法获取
				}

				if (obj == null)
				{
					//函数无法执行
					Console.Error(Interpreter.ToStringAst(ast) + "\n" + "函数无法执行" + Interpreter.ToStringAst(ast.Left));
					return null;
				}
				return Utils.InvokeMethod(func, self ?? environment, paramList);
			}

			return null;
		}

		public object Run(IWithPrototype environment)
		{
			return Interpreter.SRun(environment, this.Ast);
		}
		public object RunSafe(Dictionary<string, object> environment)
		{
			try
			{
				return Interpreter.SRun(environment, this.Ast);
			}
			catch (Exception e)
			{
				throw new Exception(this.Expression + "\n" + e.Message);
			}
		}
	}

}