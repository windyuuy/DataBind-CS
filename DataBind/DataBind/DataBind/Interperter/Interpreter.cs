using System;
using System.Linq;
using System.Text.RegularExpressions;
using DataBinding.CollectionExt;

class _Env
{

	static _Env()
	{

	}

	public static readonly string[] symbolList = new string[]{
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

namespace vm
{
	using ASTNode = ASTNodeBase;
	using Node = CombineType<object, List<WordNode>, WordNode, ASTNodeBase>;
	using boolean = System.Boolean;
	using number = System.Double;

	using ENodeType = System.Double;
	using TEnv = Dictionary<string, object>;

	public class TNodeType
	{
		public static readonly List<string> symbols = new List<string>(){
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
		public number this[string x]
		{
			get => symbols.IndexOf(x);
		}
		public string this[number x]
		{
			get => symbols[(int)x];
		}

		/// <summary>
		/// [
		/// </summary>
		public static number bracketL => symbols.IndexOf("[");
		/// <summary>
		/// ]
		/// </summary>
		public static number bracketR => symbols.IndexOf("]");
		/// <summary>
		/// {
		/// </summary>
		public static number braceL => symbols.IndexOf("{");
		/// <summary>
		/// }
		/// </summary>
		public static number braceR => symbols.IndexOf("}");
		/// <summary>
		/// (
		/// </summary>
		public static number parenthesesL => symbols.IndexOf("(");
		/// <summary>
		/// )
		/// </summary>
		public static number parenthesesR => symbols.IndexOf(")");

		public static number P0 => symbols.IndexOf("P0");
		public static number P1 => symbols.IndexOf("P1");
		public static number P2 => symbols.IndexOf("P2");
		public static number P3 => symbols.IndexOf("P3");
		public static number P4 => symbols.IndexOf("P4");
		public static number P5 => symbols.IndexOf("P5");
		public static number P6 => symbols.IndexOf("P6");
		public static number P7 => symbols.IndexOf("P7");
		public static number P8 => symbols.IndexOf("P8");
		public static number P9 => symbols.IndexOf("P9");
		public static number P10 => symbols.IndexOf("P10");
		public static number P11 => symbols.IndexOf("P11");
		public static number P12 => symbols.IndexOf("P12");
		public static number number => symbols.IndexOf("number");
		public static number word => symbols.IndexOf("word");
		public static number stringx => symbols.IndexOf("string");
		public static number boolean => symbols.IndexOf("boolean");
		public static number nullx => symbols.IndexOf("null");
		public static number annotation => symbols.IndexOf("annotation");
		public static number call => symbols.IndexOf("call");
		public static number lambda => symbols.IndexOf("lambda");

	}
	public class NodeType : TNodeType { };

	public class WordNode
	{
		public number lineEnd;
		//父节点
		public ASTNode parent = null;
		/**
         * 相关注释
         */
		public string frontAnnotation;
		public string behindAnnotation;


		public ENodeType type;

		public object value;

		public number lineStart;

		public number columnStart;

		public number columnEnd;
		public WordNode(
			ENodeType type,
			object value,
			 number lineStart,
			 number columnStart,
			 number columnEnd
		)
		{
			this.type = type;
			this.value = value;
			this.lineStart = lineStart;
			this.columnStart = columnStart;
			this.columnEnd = columnEnd;
			this.lineEnd = lineStart;
		}
	}


	public class ASTNodeBase
	{
		//父节点
		public ASTNode parent = null;
		/**
         * 相关注释
         */
		public string frontAnnotation;
		public string behindAnnotation;
		public ENodeType operatorx;

		public ASTNodeBase(
			/**
             * 操作符
             */
			ENodeType operatorx
		)
		{
			this.operatorx = operatorx;
		}

	}

	public class ValueASTNode : ASTNodeBase
	{
		public WordNode value;
		public ValueASTNode(
			 WordNode value
		) : base(value.type)
		{
			this.value = value;
		}
	}

	public class BracketASTNode : ASTNodeBase
	{
		public ASTNode node;
		public BracketASTNode(
			 ENodeType operatorx,
			 ASTNode node
		) : base(operatorx)
		{
			this.operatorx = operatorx;
			this.node = node;
		}

	}

	public class UnitaryASTNode : ASTNodeBase
	{
		/**
		 * 一元表达式的右值
		 */
		public ASTNode right;
		public UnitaryASTNode(
			 ENodeType operatorx,
			 /**
			  * 一元表达式的右值
			  */
			 ASTNode right
		) : base(operatorx)
		{
			this.operatorx = operatorx;
			this.right = right;
			this.right.parent = this;
		}
	}

	public class BinaryASTNode : ASTNodeBase
	{
		/**
		 * 二元表达式的左值
		 */
		public ASTNode left;
		/**
		 * 二元表达式的左值
		 */
		public ASTNode right;
		public BinaryASTNode(
			 /**
			  * 二元表达式的左值
			  */
			 ASTNode left,
			 /**
			  * 运算符
			  */
			 ENodeType operatorx,
			 /**
			  * 二元表达式的左值
			  */
			 ASTNode right
		) : base(operatorx)
		{
			this.left = left;
			this.operatorx = operatorx;
			this.right = right;

			this.left.parent = this;
			this.right.parent = this;
		}
	}

	public class CallASTNode : ASTNodeBase
	{
		/**
		 * 函数访问节点
		 */
		public ASTNode left;
		/**
		 * 函数参数列表
		 */
		public List<ASTNode> parameters;
		public CallASTNode(
			 /**
			  * 函数访问节点
			  */
			 ASTNode left,
			 /**
			  * 函数参数列表
			  */
			 List<ASTNode> parameters
		) : base(TNodeType.call)
		{
			this.left = left;
			this.parameters = parameters;

			this.left.parent = this;
			this.parameters.ForEach(a => a.parent = this);
		}
	}

	public class Interpreter
	{

		static Interpreter()
		{
			_Env.symbolList.ForEach(a => operatorCharMap[a.charAt(0)] = true);
			(new string[] { "\"", "'", "`" }).ForEach(a => markMap[a] = true);
			_Env.symbolList.ForEach(a =>
			{
				if (a.Length > 1)
				{
					doubleOpMap[a.charAt(0)] = true;
				}
			});
			(new string[] { " ", "\n", "\r", "\t" }).ForEach(a => spaceMap[a] = true);
		}
		static readonly int zeroCode = "0".charCodeAt(0);
		static readonly int nineCode = "9".charCodeAt(0);

		static Dictionary<string, bool> operatorCharMap = new Dictionary<string, bool>();

		static Dictionary<string, bool> markMap = new Dictionary<string, bool>();

		static Dictionary<string, bool> doubleOpMap = new Dictionary<string, bool>();


		static Dictionary<string, bool> spaceMap = new Dictionary<string, bool>();


		ASTNode ast;

		List<string> astErrorList = new List<string>();

		public string expression;
		public Interpreter(
			string expression
		)
		{
			this.expression = expression;
			this.ast = Interpreter.toAST(Interpreter.toWords(this.expression), this.expression, this.astErrorList);
		}

		public static List<WordNode> toWords(string expression)
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
					  var code = charx.charCodeAt(0);
					  if (code >= zeroCode && code <= nineCode)
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
						  else if (charx == "-" && (nodeList.Count != 0 && nodeList.TryGet(nodeList.Count - 1).type < TNodeType.P10 || nodeList.Count == 0))
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
					  var code = charx.charCodeAt(0);
					  if (code >= zeroCode && code <= nineCode || charx == ".")
					  {
						  temp += charx;
					  }
					  else
					  {
						  nodeList.Add(new WordNode(TNodeType.number, number.Parse(temp), line, column - temp.Length, column - 1));
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
							  var node = new WordNode(TNodeType.stringx, temp, line, startColum, column);
							  node.lineStart = line - (temp.match("\n", RegexOptions.Multiline)?.Length ?? 0);
							  nodeList.Add(node);
						  }
						  else
						  {
							  nodeList.Add(new WordNode(TNodeType.stringx, temp, line, startColum, column));
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
							  nodeList.Add(new WordNode(TNodeType.boolean, temp == "true", line, column - temp.Length, column - 1));

						  }
						  else if (temp == "null")
						  {
							  nodeList.Add(new WordNode(TNodeType.nullx, null, line, column - temp.Length, column - 1));

						  }
						  else
						  {
							  nodeList.Add(new WordNode(TNodeType.word, temp, line, column - temp.Length, column - 1));

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
						  nodeList.Add(new WordNode(TNodeType.annotation, temp, line, column - temp.Length, column));

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

						  var node = new WordNode(TNodeType.annotation, temp, line, startColum, column);

						  node.lineStart = line - temp.match("\n", RegexOptions.Multiline)?.Length ?? 0;
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

		protected void pushError(List<string> errorList, List<Node> nodes, string msg)
		{
			var errorPos = nodes[0];
			var errorMsg = expression + msg;
			if (errorPos != null)
			{
				errorMsg += $"，在{(errorPos.As<WordNode>()).lineEnd + 1}:{errorPos.As<WordNode>().columnEnd + 1}。";
			}
			errorList.Add(errorMsg);
		}

		public static ASTNode toAST(List<WordNode> nodeList, string expression, List<string> errorList)
		{
			void pushError(List<string> errorList, Node node, string msg)
			{
				var errorPos = node;
				var errorMsg = expression + msg;
				if (errorPos != null)
				{
					errorMsg += $"，在{(errorPos.As<WordNode>()).lineEnd + 1}:{errorPos.As<WordNode>().columnEnd + 1}。";
				}
				errorList.Add(errorMsg);
			}

			//根据运算符优先级进行分组
			List<Node> bracketList = new List<Node>();

			Dictionary<number, boolean> bracketMap = new Dictionary<number, boolean>();
			(new number[] { TNodeType.parenthesesL, TNodeType.bracketL, TNodeType.braceL }).ForEach(k => bracketMap[k] = true);

			/**
             * 将括号按层级分组成数组
             */
			number convertBracket(number start, List<Node> list, ENodeType? endType = null)
			{
				for (var i = start; i < nodeList.Count; i++)
				{
					var current = nodeList.TryGet((int)i);
					if (bracketMap.ContainsKey(current.type))
					{
						//发现括号
						ENodeType nextEndType;
						switch (current.type)
						{
							case number type when type == TNodeType.Inst["("]:
								nextEndType = TNodeType.Inst[")"];
								break;
							case number type when type == TNodeType.Inst["["]:
								nextEndType = TNodeType.Inst["]"];
								break;
							case number type when type == TNodeType.Inst["{"]:
								nextEndType = TNodeType.Inst["}"];
								break;
							default:
								throw new Exception(expression + "括号分析异常异常'" + TNodeType.Inst[current.type] + "' " + current.lineStart + ":" + current.columnStart);
						}
						List<WordNode> newList = new List<WordNode>() { current };
						i = convertBracket(i + 1, newList.AsList<Node>(), nextEndType);
						list.Add(newList.AsList<WordNode>());
					}
					else if (endType != null && endType == current.type)
					{
						list.Add(current);
						return i;
					}
					else
					{
						list.Add(current);
					}
				}
				if (endType != null && (list.TryGet(list.Count - 1).As<WordNode>()).type != endType)
				{
					pushError(errorList, list.TryGet(list.Count - 1), $"缺少闭合括号'${TNodeType.Inst[endType.Value]}'");
					//自动补充一个符号
					list.Add(new WordNode(endType.Value, null, 0, 0, 0));
				}
				return nodeList.Count;
			};

			Func<List<Node>, ASTNode> genAST = null;

			Func<List<Node>, number, number, List<Node>> unaryExp = (List<Node> list, number startPriority, number endPriority) =>
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
					 if (b.Is<WordNode>()
						&& b.As<WordNode>().type > startPriority
						&& b.As<WordNode>().type < endPriority
						)
					 {
						 if (a == null)
						 {
							 pushError(errorList, a, "一元运算符" + TNodeType.Inst[b.As<WordNode>().type] + "缺少右值");
							 a = new WordNode(TNodeType.boolean, true, 0, 0, 0);//自动补充
						 }
						 if (currentAST == null)
						 {
							 //第一次发现
							 var ls = a.IsList() ? a.AsList<Node>() : new List<Node>() { a };
							 currentAST = new UnitaryASTNode(b.As<WordNode>().type, genAST(ls));
						 }
						 else
						 {
							 //多个单目运算符连续
							 currentAST = new UnitaryASTNode(b.As<WordNode>().type, currentAST);
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
							 rlist.push(a);//上次必然已经被加入了ast中，因此不需要push
						 }
					 }
				 }
				 if (currentAST != null)
				 {
					 //边界对象不要遗留
					 rlist.push(currentAST);
				 }
				 rlist.reverse();//转为正常的顺序
				 return rlist;
			 };

			Func<List<Node>, List<ASTNode>> genParamList = null;
			Func<List<Node>, number, number, List<Node>> binaryExp = (List<Node> list, number startPriority, number endPriority) =>
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
					if (b.Is<WordNode>() && b.As<WordNode>().type > startPriority && b.As<WordNode>().type < endPriority)
					{
						if (c == null)
						{
							pushError(errorList, a, "二元运算符" + TNodeType.Inst[b.As<WordNode>().type] + "缺少右值");
							c = new WordNode(TNodeType.number, 0, 0, 0, 0);//自动补充
						}
						if (currentAST == null)
						{
							//第一次发现
							rlist.pop();//删除上次循环所插入的b
							var ls = c.IsList() ? c.AsList<WordNode>().AsList<Node>() : new List<Node>() { c };
							currentAST = new BinaryASTNode(
								genAST(a.IsList() ? a.AsList<Node>() : new List<Node>() { a }),
								b.As<WordNode>().type,
								genAST(ls));
						}
						else
						{
							//多次双目运算符连续
							currentAST = new BinaryASTNode(currentAST, b.As<WordNode>().type, genAST(c.IsList() ? c.AsList<WordNode>().AsList<Node>() : new List<Node>() { c }));
						}


						//特殊处理 . 和 [] 后续逻辑，可能会紧跟着函数调用
						var d = list.TryGet(i + 2);
						if (endPriority == TNodeType.P1 && d.IsList() && d.AsList<WordNode>().TryGet(0) is WordNode && d.AsList<WordNode>().TryGet(0).type == TNodeType.Inst["("])
						{
							currentAST = new CallASTNode(currentAST, genParamList(d.AsList<WordNode>().AsList<Node>()));

							i++;//跳过d的遍历
						}
						i++;//跳过c的遍历

					}

					//特殊处理，仅处理a['b']中括号的访问方式。
					else if (b.IsList() && b.AsList<WordNode>().TryGet(0) is WordNode && b.AsList<WordNode>().TryGet(0).type == TNodeType.Inst["["])
					{
						//中括号方式访问属性
						if (currentAST != null)
						{
							currentAST = new BinaryASTNode(currentAST, TNodeType.Inst["["], genAST(b.AsList<Node>()));
						}
						else
						{
							rlist.pop();//删除上次循环所插入的b
							currentAST = new BinaryASTNode(genAST(a.IsList() ? a.AsList<WordNode>().AsList<Node>() : new List<Node>() { a }), TNodeType.Inst["["], genAST(b.AsList<Node>()));
						}

						//特殊处理 . 和 [] 后续逻辑，可能会紧跟着函数调用
						if (endPriority == TNodeType.P1 && c.IsList() && c.AsList<WordNode>().TryGet(0) is WordNode && c.AsList<WordNode>().TryGet(0).type == TNodeType.Inst["("])
						{
							currentAST = new CallASTNode(currentAST, genParamList(c.AsList<Node>()));

							i++;//跳过c的遍历
						}

					}
					else
					{
						if (currentAST != null)
						{
							if (endPriority == TNodeType.P1 && b.IsList() && b.AsList<WordNode>().TryGet(0) is WordNode && b.AsList<WordNode>().TryGet(0).type == TNodeType.Inst["("])
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
						else if (endPriority == TNodeType.P1 && a.Is<WordNode>() && a.As<WordNode>().type == TNodeType.word && b.IsList() && b.AsList<WordNode>().TryGet(0) is WordNode && b.AsList<WordNode>().TryGet(0).type == TNodeType.Inst["("])
						{
							//特殊处理 . 和 [] 后续逻辑，可能会紧跟着函数调用
							currentAST = new CallASTNode(genAST(a.IsList() ? a.AsList<Node>() : new List<Node>() { a }), genParamList(b.AsList<Node>()));
							rlist.pop(); //删除上次循环所插入的b
							continue;//a和b都需要插入到rlist
						}
						if (i == 1)
						{//由于是从1开始遍历的，因此需要保留0的值
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
			};

			Func<List<Node>, ENodeType, List<List<Node>>> splice = (List<Node> list, ENodeType sp) =>
			{
				List<List<Node>> r = new List<List<Node>>();
				List<Node> current = new List<Node>();
				foreach (var l in list)
				{
					//这里会忽略括号
					if (l.Is<WordNode>())
					{
						if (l.As<WordNode>().type == sp)
						{
							//产生切割
							if (current.length > 0)
							{
								r.push(current);
								current = new List<Node>();
							}
						}
						else if (l.As<WordNode>().type == TNodeType.Inst["("]
							|| l.As<WordNode>().type == TNodeType.Inst[")"]
							|| l.As<WordNode>().type == TNodeType.Inst["["]
							|| l.As<WordNode>().type == TNodeType.Inst["]"]
							|| l.As<WordNode>().type == TNodeType.Inst["{"]
							|| l.As<WordNode>().type == TNodeType.Inst["}"]
							)
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
			};


			genParamList = (List<Node> list) =>
			{
				var paramList = splice(list, TNodeType.Inst[","]);
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
						a.As<WordNode>().type == TNodeType.Inst["("] && b.As<WordNode>().type == TNodeType.Inst[")"] ||
						a.As<WordNode>().type == TNodeType.Inst["["] && b.As<WordNode>().type == TNodeType.Inst["]"] ||
						a.As<WordNode>().type == TNodeType.Inst["{"] && b.As<WordNode>().type == TNodeType.Inst["}"])
				)
				{
					bracketType = a.As<WordNode>().type;
					list = list.slice(1, list.length - 1);
				}

				list = binaryExp(list, TNodeType.P0, TNodeType.P1);//分组  . 和 [] 形成访问连接，包括后面的函数
				list = unaryExp(list, TNodeType.P1, TNodeType.P2);//分组  ! ，进行一元表达式分组
				list = binaryExp(list, TNodeType.P2, TNodeType.P3);//分组  **，进行2元表达式分组
				list = binaryExp(list, TNodeType.P3, TNodeType.P4);//分组  * / %，进行2元表达式分组
				list = binaryExp(list, TNodeType.P4, TNodeType.P5);//分组  + -，进行2元表达式分组
				list = binaryExp(list, TNodeType.P5, TNodeType.P6);//分组  > < >= <=，进行2元表达式分组
				list = binaryExp(list, TNodeType.P6, TNodeType.P7);//分组  != ==，进行2元表达式分组
				list = binaryExp(list, TNodeType.P7, TNodeType.P8);//分组  && ，进行2元表达式分组
				list = binaryExp(list, TNodeType.P8, TNodeType.P9);//分组  ||，进行2元表达式分组

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

					result = new ValueASTNode(new WordNode(TNodeType.number, 0, 0, 0, 0));
				}
				else
				{
					pushError(errorList, sourcelist.TryGet(0), "无法正确解析列表");

					result = new ValueASTNode(new WordNode(TNodeType.number, 0, 0, 0, 0));
				}

				if (bracketType != null)
				{
					if (bracketType == TNodeType.Inst["{"])
					{
						return new BracketASTNode(TNodeType.lambda, result);
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
			nodeList = nodeList.FindAll(a => a.type != TNodeType.annotation);
			convertBracket(0, bracketList);//分组括号
			return genAST(bracketList);
		}


		public static string toStringAST(ASTNode ast0, boolean addBracket = false)
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
				if (ast.value.type == TNodeType.stringx)
				{
					r += $"\"{CS(ast.value.value)}\"";
				}
				else
				{
					r += $"{CS(ast.value.value)}";
				}
			}
			else if (ast0 is BracketASTNode)
			{
				var ast = ast0 as BracketASTNode;
				if (ast.operatorx == TNodeType.Inst["("])
				{
					r += $"({Interpreter.toStringAST(ast.node, addBracket)})";
				}
				else if (ast.operatorx == TNodeType.Inst["["])
				{
					r += $"[{Interpreter.toStringAST(ast.node, addBracket)}]";
				}
				else if (ast.operatorx == TNodeType.Inst["{"] || ast.operatorx == TNodeType.lambda)
				{
					r += $"{{{Interpreter.toStringAST(ast.node, addBracket)}}}";
				}
			}
			else if (ast0 is UnitaryASTNode)
			{
				var ast = ast0 as UnitaryASTNode;
				r += $"{TNodeType.Inst[ast.operatorx]}{Interpreter.toStringAST(ast.right, addBracket)}";
			}
			else if (ast0 is BinaryASTNode)
			{
				var ast = ast0 as BinaryASTNode;
				if (ast.operatorx == TNodeType.Inst["["])
				{
					r += $"{Interpreter.toStringAST(ast.left, addBracket)}{Interpreter.toStringAST(ast.right, addBracket)}";
				}
				else if (ast.operatorx == TNodeType.Inst["."])
				{
					r += $"{Interpreter.toStringAST(ast.left, addBracket)}{TNodeType.Inst[ast.operatorx]}{Interpreter.toStringAST(ast.right, addBracket)}";
				}
				else
				{
					r += $"{Interpreter.toStringAST(ast.left, addBracket)} {TNodeType.Inst[ast.operatorx]} {Interpreter.toStringAST(ast.right, addBracket)}";
				}
			}
			else if (ast0 is CallASTNode)
			{
				var ast = ast0 as CallASTNode;
				r += $"{Interpreter.toStringAST(ast.left, addBracket)}( {ast.parameters.Select(a => Interpreter.toStringAST(a, addBracket)).ToConvableList<string>().Join(", ")})";
			}
			if (addBracket && !(ast0 is ValueASTNode || ast0 is BracketASTNode))
			{
				r += ")";
			}
			return r;
		}

		public override string ToString()
		{
			return Interpreter.toStringAST(this.ast);
		}

		/**
         * 该函数所执行的表达式将自动进行容错处理
         * 1. 当访问属性产生null值时，其将不参与计算 例如：a.b+13 当a或b为空时，结果将返回13
         * 2. 当访问的表达式完全为null时，表达式将最终返回结果0，例如：a.b+c 则返回0
         * @param environment 
         * @param ast 
         */
		static object srun(IWithPrototype environment, ASTNode ast0)
		{
			object ast1 = ast0;
			if (ast1 is ValueASTNode)
			{
				var ast = ast1 as ValueASTNode;
				if (ast.operatorx == vm.TNodeType.word)
				{
					if ("this" == (Utils.ToIndexKey(ast.value.value)))
					{
						return environment;
					}
					else
					{
						var v = Utils.IndexValueRecursive(environment, Utils.ToIndexKey(ast.value.value));
						return v;
					}
				}
				else
				{
					return ast.value.value;
				}
			}
			else if (ast1 is BracketASTNode)
			{
				var ast = ast1 as BracketASTNode;
				return Interpreter.srun(environment, ast.node);//括号内必然是个整体
			}
			else if (ast1 is UnitaryASTNode)
			{
				var ast = ast1 as UnitaryASTNode;
				var b = Interpreter.srun(environment, ast.right);
				switch (ast.operatorx)
				{
					case number type when type == TNodeType.Inst["!"]:
						return Utils.IsFalse(b);
					default:
						throw new Exception($"意外的一元运算符[{TNodeType.Inst[ast.operatorx]}]");
				}
			}
			else if (ast1 is BinaryASTNode)
			{
				var ast = ast1 as BinaryASTNode;
				if (ast.operatorx == TNodeType.Inst["."] || ast.operatorx == TNodeType.Inst["["])
				{
					object a = Interpreter.srun(environment, ast.left);

					if (a == null)
					{
						console.error(Interpreter.toStringAST(ast) + "\n" + "属性访问异常" + Interpreter.toStringAST(ast.left));
						return null;//访问运算遇到null则不执行
					}
					if (ast.right is ValueASTNode)
					{
						return Utils.IndexValueRecursive(a, (ast.right as ValueASTNode).value.value);
					}
					else
					{
						return Utils.IndexValueRecursive(a, Interpreter.srun(environment, ast.right));
					}
				}

				if (ast.operatorx == TNodeType.Inst["&&"])
				{
					//先左，后右
					var a = Interpreter.srun(environment, ast.left);
					if (a == null)
					{
						return a;

					}
					else
					{
						return Interpreter.srun(environment, ast.right);
					}
				}
				else if (ast.operatorx == TNodeType.Inst["||"])
				{
					var a = Interpreter.srun(environment, ast.left);
					if (a != null)
					{
						return a;
					}
					return a ?? Interpreter.srun(environment, ast.right);
				}

				{
					var a = Interpreter.srun(environment, ast.left);
					var b = Interpreter.srun(environment, ast.right);

					if (!(ast.operatorx == TNodeType.Inst["=="] || ast.operatorx == TNodeType.Inst["!="]))
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
						if(
							(a is double)
							|| (a is int)
							|| (a is long)
							|| (a is short)
							)
						{
							var a1 = Convert.ToDouble(a);
							var b1 = Convert.ToDouble(b);

							switch (ast.operatorx)
							{
								case number op when op == TNodeType.Inst["**"]:
									return Math.Pow(a1, b1);
								case number op when op == TNodeType.Inst["*"]:
									return a1 * b1;
								case number op when op == TNodeType.Inst["/"]:
									return a1 / b1;
								case number op when op == TNodeType.Inst["%"]:
									return a1 % b1;
								case number op when op == TNodeType.Inst["+"]:
									return a1 + b1;
								case number op when op == TNodeType.Inst["-"]:
									return a1 - b1;
								case number op when op == TNodeType.Inst[">"]:
									return a1 > b1;
								case number op when op == TNodeType.Inst["<"]:
									return a1 < b1;
								case number op when op == TNodeType.Inst[">="]:
									return a1 >= b1;
								case number op when op == TNodeType.Inst["<="]:
									return a1 <= b1;
								case number op when op == TNodeType.Inst["!="]:
									return a1 != b1;
								case number op when op == TNodeType.Inst["=="]:
									return a1 == b1;
								default:
									throw new Exception($"意外的二元运算符[{TNodeType.Inst[ast.operatorx]}]");
							}
                        }
                        else
                        {
							// TODO: 支持自动定位运算符
                            switch (ast.operatorx)
							{
								case number op when op == TNodeType.Inst["**"]:
									{
										var mOp = a.GetType().GetMethod("op_Pow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case number op when op == TNodeType.Inst["*"]:
									{
										var mOp = a.GetType().GetMethod("op_Multiply",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public);
										var ret= mOp.Invoke(a, new object[] { b });
										return ret;
									}
                                case number op when op == TNodeType.Inst["/"]:
                                    {
										var mOp = a.GetType().GetMethod("op_Division", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
                                case number op when op == TNodeType.Inst["%"]:
									{
										var mOp = a.GetType().GetMethod("op_Modulus", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case number op when op == TNodeType.Inst["+"]:
									{
										var mOp = a.GetType().GetMethod("op_Addition", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case number op when op == TNodeType.Inst["-"]:
									{
										var mOp = a.GetType().GetMethod("op_Subtraction", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case number op when op == TNodeType.Inst[">"]:
									{
										var mOp = a.GetType().GetMethod("op_GreaterThan", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case number op when op == TNodeType.Inst["<"]:
									{
										var mOp = a.GetType().GetMethod("op_LessThan", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case number op when op == TNodeType.Inst[">="]:
									{
										var mOp = a.GetType().GetMethod("op_GreaterThanOrEqual", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case number op when op == TNodeType.Inst["<="]:
									{
										var mOp = a.GetType().GetMethod("op_LessThanOrEqual", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										var ret = mOp.Invoke(a, new object[] { b });
										return ret;
									}
								case number op when op == TNodeType.Inst["!="]:
									{
										//var mOp = a.GetType().GetMethod("op_Inequality", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										//var ret = mOp.Invoke(a, new object[] { b });
										//return ret;
										return a != b;
									}
								case number op when op == TNodeType.Inst["=="]:
									{
										//var mOp = a.GetType().GetMethod("op_Equality", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
										//var ret = mOp.Invoke(a, new object[] { b });
										//return ret;
										return a == b;
									}
								default:
                                    throw new Exception($"意外的二元运算符[{TNodeType.Inst[ast.operatorx]}]");
                            }
                        }
					}
				}
			}
			else if (ast1 is CallASTNode)
			{
				var ast = ast1 as CallASTNode;
				var obj = Interpreter.srun(environment, ast.left);

				object self = null;
				object func = null;

				List<object> paramList = ast.parameters.Select(p =>
				{
					if (p is BracketASTNode && p.operatorx == TNodeType.lambda)
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

							return Interpreter.srun(newEv, (p as BracketASTNode).node);
						};
						return f;
					}
					else
					{
						return Interpreter.srun(environment, p);
					}
				}).ToConvableList();

				if (ast.left is ValueASTNode)
				{
					var ret = (ast.left as ValueASTNode).value.value;
					//全局函数
					func = Utils.IndexMethodRecursive(environment, ret, Utils.ExtractValuesTypes(paramList));
				}
				else if (ast.left is BinaryASTNode)
				{
					self = Interpreter.srun(environment, (ast.left as BinaryASTNode).left);
					if (self == null)
					{
						console.error(Interpreter.toStringAST(ast) + "\n" + "函数无法访问" + Interpreter.toStringAST((ast.left as BinaryASTNode).left));
						return null;//self无法获取
					}
					if ((ast.left as BinaryASTNode).right is ValueASTNode)
					{
						var ret = ((ast.left as BinaryASTNode).right as ValueASTNode).value.value;
						func = Utils.IndexMethodRecursive(self, ret, Utils.ExtractValuesTypes(paramList));
					}
					else
					{
						var ret = Interpreter.srun(environment, (ast.left as BinaryASTNode).right);
						func = Utils.IndexMethodRecursive(self, ret, Utils.ExtractValuesTypes(paramList));
					}
				}
				if (func == null)
				{
					console.error(Interpreter.toStringAST(ast) + "\n" + "函数无法访问");
					return null;//func无法获取
				}

				if (obj == null)
				{
					//函数无法执行
					console.error(Interpreter.toStringAST(ast) + "\n" + "函数无法执行" + Interpreter.toStringAST(ast.left));
					return null;
				}
				return Utils.InvokeMethod(func, self ?? environment, paramList);
			}

			return null;
		}

		public object run(IWithPrototype environment)
		{
			return Interpreter.srun(environment, this.ast);
		}
		public object runSafe(Dictionary<string, object> environment)
		{
			try
			{
				return Interpreter.srun(environment, this.ast);
			}
			catch (Exception e)
			{
				throw new Exception(this.expression + "\n" + e.Message);
			}
		}
	}

}