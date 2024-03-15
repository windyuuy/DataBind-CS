using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTestUitls;
using DataBind.VM;

namespace TestDataBind
{
	using Node = DataBind.VM.CombineType<object, DataBind.CollectionExt.List<DataBind.VM.WordNode>, DataBind.VM.WordNode, DataBind.VM.ASTNodeBase>;
	using TObj = DataBind.VM.ProtoDict<string, object>;
	using TEnvExt = DataBind.VM.ProtoDict<string, object>;
	using number = System.Double;

	public static class InternalStringExt
	{
		/// <summary>
		/// _P_ 开头的方法可以以属性的形式在数据绑定表达式中使用
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static int _P_length(this string str)
		{
			return str.Length;
		}
	}
	
	public class SampleOBD5 : IObservable
	{
		public DataBind.VM.Observer ___Sob__;
		private int a1;

		public void Set(int a)
		{
			a1 = a;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;

		public int a
		{
			get
			{
				PropertyGot?.Invoke(this, new PropertyGetEventArgs("a", a1));
				return a1;
			}
			set
			{
				var oldValue = a1;
				a1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("a", value, oldValue));
			}
		}

		public DataBind.VM.Observer _SgetOb()
		{
			return ___Sob__;
		}

		public void _SsetOb(DataBind.VM.Observer value)
		{
			___Sob__ = value;
		}


		public int add(SampleOBD5 obj)
		{
			return this.a + obj.a;
		}
	}

	public class InterpreterTest : TestEnv
	{
		[Test]
		public void Test词法分析()
		{
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("true false");

				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Boolean);
				expect(nodeList[0].Value).toEqual(true);
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Boolean);
				expect(nodeList[1].Value).toEqual(false);
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a +  b - c");
				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[0].Value).toEqual("a");
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Inst["+"]);
				expect(nodeList[1].Value).toEqual(null);
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[2].Value).toEqual("b");
				expect(nodeList[3].Type).toEqual(DataBind.VM.NodeType.Inst["-"]);
				expect(nodeList[3].Value).toEqual(null);
				expect(nodeList[4].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[4].Value).toEqual("c");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("100-1");
				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[0].Value).toEqual(100);
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Inst["-"]);
				expect(nodeList[1].Value).toEqual(null);
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[2].Value).toEqual(1);
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("100--1.5");
				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[0].Value).toEqual(100);
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Inst["-"]);
				expect(nodeList[1].Value).toEqual(null);
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[2].Value).toEqual(-1.5);
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("100+-1.11");
				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[0].Value).toEqual(100);
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Inst["+"]);
				expect(nodeList[1].Value).toEqual(null);
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[2].Value).toEqual(-1.11);
			}


			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a.b.c");
				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[0].Value).toEqual("a");
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Inst["."]);
				expect(nodeList[1].Value).toEqual(null);
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[2].Value).toEqual("b");
				expect(nodeList[3].Type).toEqual(DataBind.VM.NodeType.Inst["."]);
				expect(nodeList[3].Value).toEqual(null);
				expect(nodeList[4].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[4].Value).toEqual("c");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("aa>=b");
				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[0].Value).toEqual("aa");
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Inst[">="]);
				expect(nodeList[1].Value).toEqual(null);
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[2].Value).toEqual("b");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("(a+b-c)*d/e**4>123<312>=11.1<=222!=3==4&&'aa'||\"bb\"!`cc`,");
				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Inst["("]);
				expect(nodeList[0].Value).toEqual(null);
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[1].Value).toEqual("a");
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Inst["+"]);
				expect(nodeList[2].Value).toEqual(null);
				expect(nodeList[3].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[3].Value).toEqual("b");
				expect(nodeList[4].Type).toEqual(DataBind.VM.NodeType.Inst["-"]);
				expect(nodeList[4].Value).toEqual(null);
				expect(nodeList[5].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[5].Value).toEqual("c");
				expect(nodeList[6].Type).toEqual(DataBind.VM.NodeType.Inst[")"]);
				expect(nodeList[6].Value).toEqual(null);
				expect(nodeList[7].Type).toEqual(DataBind.VM.NodeType.Inst["*"]);
				expect(nodeList[7].Value).toEqual(null);
				expect(nodeList[8].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[8].Value).toEqual("d");
				expect(nodeList[9].Type).toEqual(DataBind.VM.NodeType.Inst["/"]);
				expect(nodeList[9].Value).toEqual(null);
				expect(nodeList[10].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[10].Value).toEqual("e");
				expect(nodeList[11].Type).toEqual(DataBind.VM.NodeType.Inst["**"]);
				expect(nodeList[11].Value).toEqual(null);
				expect(nodeList[12].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[12].Value).toEqual(4);
				expect(nodeList[13].Type).toEqual(DataBind.VM.NodeType.Inst[">"]);
				expect(nodeList[13].Value).toEqual(null);
				expect(nodeList[14].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[14].Value).toEqual(123);
				expect(nodeList[15].Type).toEqual(DataBind.VM.NodeType.Inst["<"]);
				expect(nodeList[15].Value).toEqual(null);
				expect(nodeList[16].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[16].Value).toEqual(312);
				expect(nodeList[17].Type).toEqual(DataBind.VM.NodeType.Inst[">="]);
				expect(nodeList[17].Value).toEqual(null);
				expect(nodeList[18].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[18].Value).toEqual(11.1);
				expect(nodeList[19].Type).toEqual(DataBind.VM.NodeType.Inst["<="]);
				expect(nodeList[19].Value).toEqual(null);
				expect(nodeList[20].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[20].Value).toEqual(222);
				expect(nodeList[21].Type).toEqual(DataBind.VM.NodeType.Inst["!="]);
				expect(nodeList[21].Value).toEqual(null);
				expect(nodeList[22].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[22].Value).toEqual(3);
				expect(nodeList[23].Type).toEqual(DataBind.VM.NodeType.Inst["=="]);
				expect(nodeList[23].Value).toEqual(null);
				expect(nodeList[24].Type).toEqual(DataBind.VM.NodeType.Number);
				expect(nodeList[24].Value).toEqual(4);
				expect(nodeList[25].Type).toEqual(DataBind.VM.NodeType.Inst["&&"]);
				expect(nodeList[25].Value).toEqual(null);
				expect(nodeList[26].Type).toEqual(DataBind.VM.NodeType.Stringx);
				expect(nodeList[26].Value).toEqual("aa");
				expect(nodeList[27].Type).toEqual(DataBind.VM.NodeType.Inst["||"]);
				expect(nodeList[27].Value).toEqual(null);
				expect(nodeList[28].Type).toEqual(DataBind.VM.NodeType.Stringx);
				expect(nodeList[28].Value).toEqual("bb");
				expect(nodeList[29].Type).toEqual(DataBind.VM.NodeType.Inst["!"]);
				expect(nodeList[29].Value).toEqual(null);
				expect(nodeList[30].Type).toEqual(DataBind.VM.NodeType.Stringx);
				expect(nodeList[30].Value).toEqual("cc");
				expect(nodeList[31].Type).toEqual(DataBind.VM.NodeType.Inst[","]);
				expect(nodeList[31].Value).toEqual(null);
			}
			{
				//加入{} 子表达式
				var nodeList = DataBind.VM.Interpreter.ToWords("SUM(装备列表,{装备等级*装备加成})");
				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[0].Value).toEqual("SUM");
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Inst["("]);
				expect(nodeList[1].Value).toEqual(null);
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[2].Value).toEqual("装备列表");
				expect(nodeList[3].Type).toEqual(DataBind.VM.NodeType.Inst[","]);
				expect(nodeList[3].Value).toEqual(null);
				expect(nodeList[4].Type).toEqual(DataBind.VM.NodeType.Inst["{"]);
				expect(nodeList[4].Value).toEqual(null);
				expect(nodeList[5].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[5].Value).toEqual("装备等级");
				expect(nodeList[6].Type).toEqual(DataBind.VM.NodeType.Inst["*"]);
				expect(nodeList[6].Value).toEqual(null);
				expect(nodeList[7].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[7].Value).toEqual("装备加成");
				expect(nodeList[8].Type).toEqual(DataBind.VM.NodeType.Inst["}"]);
				expect(nodeList[8].Value).toEqual(null);
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("装备.lv <= true, `装备等\n    级太高`");

				expect(nodeList[0].Type).toEqual(DataBind.VM.NodeType.Word);

				expect(nodeList[0].Value).toEqual("装备");
				expect(nodeList[0].LineStart).toEqual(0);
				expect(nodeList[0].lineEnd).toEqual(0);
				expect(nodeList[0].ColumnStart).toEqual(0);
				expect(nodeList[0].ColumnEnd).toEqual(1);
				expect(nodeList[1].Type).toEqual(DataBind.VM.NodeType.Inst["."]);
				expect(nodeList[1].Value).toEqual(null);
				expect(nodeList[2].Type).toEqual(DataBind.VM.NodeType.Word);
				expect(nodeList[2].Value).toEqual("lv");
				expect(nodeList[3].Type).toEqual(DataBind.VM.NodeType.Inst["<="]);
				expect(nodeList[3].Value).toEqual(null);
				expect(nodeList[3].LineStart).toEqual(0);
				expect(nodeList[3].lineEnd).toEqual(0);
				expect(nodeList[3].ColumnStart).toEqual(6);
				expect(nodeList[3].ColumnEnd).toEqual(7);
				expect(nodeList[4].Type).toEqual(DataBind.VM.NodeType.Boolean);
				expect(nodeList[4].Value).toEqual(true);
				expect(nodeList[4].LineStart).toEqual(0);
				expect(nodeList[4].lineEnd).toEqual(0);
				expect(nodeList[4].ColumnStart).toEqual(9);
				expect(nodeList[4].ColumnEnd).toEqual(12);
				expect(nodeList[5].Type).toEqual(DataBind.VM.NodeType.Inst[","]);
				expect(nodeList[5].Value).toEqual(null);
				expect(nodeList[6].Type).toEqual(DataBind.VM.NodeType.Stringx);
				expect(nodeList[6].Value).toEqual("装备等\n    级太高");
				expect(nodeList[6].LineStart).toEqual(0);
				expect(nodeList[6].lineEnd).toEqual(1);
				expect(nodeList[6].ColumnStart).toEqual(15);
				expect(nodeList[6].ColumnEnd).toEqual(7);
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("-1");
				expect(nodeList.length).toBe(1);
				expect(nodeList[0].Type).toBe(DataBind.VM.NodeType.Number);
				expect(nodeList[0].Value).toBe(-1);
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("CALL(-1,-1,-1)");
				expect(nodeList[0].Value).toBe("CALL");
				expect(nodeList[2].Value).toBe(-1);
				expect(nodeList[4].Value).toBe(-1);
				expect(nodeList[6].Value).toBe(-1);
			}
		}

		[Test]
		public void Test语法分析()
		{
			DataBind.CollectionExt.List<string> errorList = new DataBind.CollectionExt.List<string>();
			{
				//单个值
				var nodeList = DataBind.VM.Interpreter.ToWords("a");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a", errorList);
				expect(errorList.length).toBe(0);
				expect(tree).toBeInstanceOf(typeof(DataBind.VM.ValueASTNode));
				expect(tree.OperatorX).toBe(DataBind.VM.NodeType.Word);
				expect((tree as DataBind.VM.ValueASTNode).Value.Type).toBe(DataBind.VM.NodeType.Word);
				expect((tree as DataBind.VM.ValueASTNode).Value.Value).toBe("a");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("'a'");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a", errorList);
				expect(errorList.length).toBe(0);
				expect(tree).toBeInstanceOf(typeof(DataBind.VM.ValueASTNode));
				expect(tree.OperatorX).toBe(DataBind.VM.NodeType.Stringx);
				expect(((tree as DataBind.VM.ValueASTNode).Value).Type).toBe(DataBind.VM.NodeType.Stringx);
				expect(((tree as DataBind.VM.ValueASTNode).Value).Value).toBe("a");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("100.4");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "100.4", errorList);
				expect(errorList.length).toBe(0);
				expect(tree).toBeInstanceOf(typeof(DataBind.VM.ValueASTNode));
				expect(tree.OperatorX).toBe(DataBind.VM.NodeType.Number);
				expect(((tree as DataBind.VM.ValueASTNode).Value).Type).toBe(DataBind.VM.NodeType.Number);
				expect(((tree as DataBind.VM.ValueASTNode).Value).Value).toBe(100.4);
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("true");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "true", errorList);
				expect(errorList.length).toBe(0);
				expect(tree).toBeInstanceOf(typeof(DataBind.VM.ValueASTNode));
				expect(tree.OperatorX).toBe(DataBind.VM.NodeType.Boolean);
				expect(((tree as DataBind.VM.ValueASTNode).Value).Type).toBe(DataBind.VM.NodeType.Boolean);
				expect(((tree as DataBind.VM.ValueASTNode).Value).Value).toBe(true);
			}
			{
				//最简单的情况;
				var nodeList = DataBind.VM.Interpreter.ToWords("a +  b - c");
				var tree0 = DataBind.VM.Interpreter.ToAst(nodeList, "a +  b - c", errorList);
				expect(errorList.length).toBe(0);
				expect(tree0).toBeInstanceOf(typeof(DataBind.VM.BinaryASTNode));
				if (tree0 is DataBind.VM.BinaryASTNode tree)
				{
					expect(tree.OperatorX).toBe(DataBind.VM.TNodeType.Inst["-"]);
					expect(tree.Right.OperatorX).toBe(DataBind.VM.NodeType.Word);
					expect(tree.Right).toBeInstanceOf(typeof(DataBind.VM.ValueASTNode));
					if (tree.Right is DataBind.VM.ValueASTNode right)
					{
						expect(right.Value.Value).toBe("c");
					}

					;

					expect(tree.Left).toBeInstanceOf(typeof(DataBind.VM.BinaryASTNode));
					if (tree.Left is DataBind.VM.BinaryASTNode left)
					{
						expect(left.OperatorX).toBe(DataBind.VM.TNodeType.Inst["+"]);
						expect(left.Left).toBeInstanceOf(typeof(DataBind.VM.ValueASTNode));
						expect(left.Right).toBeInstanceOf(typeof(DataBind.VM.ValueASTNode));
						expect((left.Left as DataBind.VM.ValueASTNode).Value.Value).toBe("a");
						expect((left.Right as DataBind.VM.ValueASTNode).Value.Value).toBe("b");
					}

					;

				}

				;
			}
			{
				//包含括号;
				var nodeList = DataBind.VM.Interpreter.ToWords("(a +  b) * c");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "(a +  b) * c", errorList);
				expect(errorList.length).toBe(0);
				expect(DataBind.VM.Interpreter.ToStringAst(tree)).toBe("(a + b) * c");
			}
			{
				//先后顺序;
				var nodeList = DataBind.VM.Interpreter.ToWords("a +  b * c");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a +  b * c", errorList);
				expect(errorList.length).toBe(0);
				expect(DataBind.VM.Interpreter.ToStringAst(tree)).toBe("a + b * c");
			}
			{
				// //属性访问;
				var nodeList = DataBind.VM.Interpreter.ToWords("a.b.c +  b.b.c * c.b.c");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a.b.c +  b.b.c * c.b.c", errorList);
				expect(errorList.length).toBe(0);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(((a.b).c) + (((b.b).c) * ((c.b).c)))");
			}
			{
				//简单中括号访问;
				var nodeList = DataBind.VM.Interpreter.ToWords("a['c'] +  b");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a['c'] +  b", errorList);
				expect(errorList.length).toBe(0);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("((a[\"c\"]) + b)");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a['c']['d'] +  b");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a['c']['d'] +  b", errorList);
				expect(errorList.length).toBe(0);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(((a[\"c\"])[\"d\"]) + b)");
			}
			{
				// //中括号访问;
				var nodeList = DataBind.VM.Interpreter.ToWords("a.b['c'] +  b['b']['c'] * c.b.c");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a.b['c'] +  b['b']['c'] * c.b.c", errorList);
				expect(errorList.length).toBe(0);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true))
					.toBe("(((a.b)[\"c\"]) + (((b[\"b\"])[\"c\"]) * ((c.b).c)))");
			}
			{
				// //！运算符;
				var nodeList = DataBind.VM.Interpreter.ToWords("!a");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "!a", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(!a)");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("!(!a)");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "!(!a)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(!((!a)))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("!!a");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "!!a", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(!(!a))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("!(a >= !b)");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "!(a >= !b)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(!((a >= (!b))))");
			}
			{
				// //函数调用;
				var nodeList = DataBind.VM.Interpreter.ToWords("a()");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(a( ))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a.b()");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a.b()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("((a.b)( ))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a['b']()");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a['b']()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("((a[\"b\"])( ))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a['b']['c']()");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a['b']['c']()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(((a[\"b\"])[\"c\"])( ))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a['b'].c()");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a['b'].c()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(((a[\"b\"]).c)( ))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a['b'].c()(666)");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a['b'].c()(666)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("((((a[\"b\"]).c)( ))( 666))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a(b)");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a(b)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(a( b))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a(b,c,d)");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a(b,c,d)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(a( b, c, d))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a(b1+b2,'c',d-1)");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a(b1+b2,'c',d-1)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(a( (b1 + b2), \"c\", (d - 1)))");
			}
		}

		/// <summary>
		/// !cur.greatProperty || cur.greatProperty.length <= 0
		/// </summary>
		[Test]
		public void TestSpecial1()
		{
			DataBind.CollectionExt.List<string> errorList = new DataBind.CollectionExt.List<string>();
			var nodeList = DataBind.VM.Interpreter.ToWords("!cur.greatProperty || cur.greatProperty.length <= 0");
			var tree = DataBind.VM.Interpreter.ToAst(nodeList, "!cur.greatProperty || cur.greatProperty.length <= 0", errorList);
			expect(errorList.TryGet(0)).toBe(null);
			expect(DataBind.VM.Interpreter.ToStringAst(tree, true))
				.toBe("((!(cur.greatProperty)) || (((cur.greatProperty).length) <= 0))");
		}

		[Test]
		public void Test语法分析复杂()
		{
			DataBind.CollectionExt.List<string> errorList = new DataBind.CollectionExt.List<string>();
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("cc.lib.format(\"玩家等级 %d Lv\",100)");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "cc.lib.format(\"玩家等级 %d Lv\",100)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(((cc.lib).format)( \"玩家等级 %d Lv\", 100))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("a/(b)+c");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "a/(b)+c", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("((a / (b)) + c)");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("Min(1,暴击/(暴击+韧性)*(LvA*2/(LvA+LvB)))");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "Min(1,暴击/(暴击+韧性)*(LvA*2/(LvA+LvB)))", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true))
					.toBe("(Min( 1, ((暴击 / ((暴击 + 韧性))) * (((LvA * 2) / ((LvA + LvB)))))))");
			}

			{
				var nodeList = DataBind.VM.Interpreter.ToWords("SUM(装备列表,{等级*加成})");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "SUM(装备列表,{等级*加成})", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(SUM( 装备列表, {(等级 * 加成)}))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("SUM(装备列表,{等级*加成+100-10},{等级*加成})");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "SUM(装备列表,{等级*加成+100-10},{等级*加成})", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true))
					.toBe("(SUM( 装备列表, {(((等级 * 加成) + 100) - 10)}, {(等级 * 加成)}))");
			}

			{
				var nodeList = DataBind.VM.Interpreter.ToWords("SUM({攻击力*(1+攻击力加成*p)})//哈哈行a");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "/*略略略*/SUM(/*七七七*/{攻击力*(1+攻击力加成*p)})//嘻嘻嘻出现", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(SUM( {(攻击力 * ((1 + (攻击力加成 * p))))}))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("level<= MAX(RoleConfig,{level})*10");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "level<= MAX(RoleConfig,{level})*10", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(level <= ((MAX( RoleConfig, {level})) * 10))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("level<= MAX(RoleConfig,{level}).level");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "level<= MAX(RoleConfig,{level}).level", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(level <= ((MAX( RoleConfig, {level})).level))");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("level.MAX(RoleConfig,{level}).level");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "level.MAX(RoleConfig,{level}).level", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(((level.MAX)( RoleConfig, {level})).level)");
			}
			{
				var nodeList = DataBind.VM.Interpreter.ToWords("level == null");
				var tree = DataBind.VM.Interpreter.ToAst(nodeList, "level == null", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(DataBind.VM.Interpreter.ToStringAst(tree, true)).toBe("(level == null)");
			}

		}


		[Test]
		public void Test环境测试()
		{
			{
				var v = ((System.Reflection.MethodInfo)InterpreterEnv.Environment["MIN"])
					.Invoke(InterpreterEnv.Environment, new object[] { new double[] { 1, 2 } });
				expect(v).toBe(1);
				expect((double)DataBind.VM.InterpreterEnv.Environment["PI"]).toBe(Math.PI);
			}
			{
				var a = new TEnvExt();
				InterpreterEnv.ExtendsEnvironment(a);
				var v = ((System.Reflection.MethodInfo)a["MIN"])
					.Invoke(InterpreterEnv.Environment, new object[] { new double[] { 1, 2 } });
				expect(v).toBe(1);
				expect(a["PI"]).toBe(Math.PI);
				expect(a.Count).toBe(0);
			}
			{
				var b = new TEnvExt() { { "a", 1 } };
				var count0 = b.Count;
				InterpreterEnv.ImplementEnvironment(b);
				var v = ((System.Reflection.MethodInfo)b["MIN"])
					.Invoke(InterpreterEnv.Environment, new object[] { new double[] { 1, 2 } });
				expect(v).toBe(1);
				expect(b["PI"]).toBe(Math.PI);
				var count1 = b.Count;
				expect(count1 - count0).toBe(28);
				//expect(b.Count).toBe(43);
			}
		}

		[Test]
		public void Test表达式运行测试()
		{
			expect(new DataBind.VM.Interpreter("!false").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(true);
			expect(new DataBind.VM.Interpreter("3**2").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(9);
			expect(new DataBind.VM.Interpreter("3*2").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(6);
			expect(new DataBind.VM.Interpreter("3.0/2").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(1.5);
			expect(new DataBind.VM.Interpreter("3%2").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(1);
			expect(new DataBind.VM.Interpreter("11+12").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(23);
			expect(new DataBind.VM.Interpreter("11-12").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(-1);
			expect(new DataBind.VM.Interpreter("11>12").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(false);
			expect(new DataBind.VM.Interpreter("11<12").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(true);
			expect(new DataBind.VM.Interpreter("11>=11").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(true);
			expect(new DataBind.VM.Interpreter("11<=11").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(true);
			expect(new DataBind.VM.Interpreter("12!=11").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(true);
			expect(new DataBind.VM.Interpreter("12==11").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(false);
			expect(new DataBind.VM.Interpreter("12>11 && 11<15").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(true);
			expect(new DataBind.VM.Interpreter("12>11 || 11>15").Run(DataBind.VM.InterpreterEnv.Environment)).toBe(true);
			{
				var exp = new DataBind.VM.Interpreter("MIN(100*2,200+100,300/2)");
				expect(exp.Run(DataBind.VM.InterpreterEnv.Environment)).toBe(150);
			}
			{
				var evn = new TEnvExt(){
					{"a",100},
					{"b",200},
					{"c",300},
				};
				DataBind.VM.InterpreterEnv.ExtendsEnvironment(evn);
				var exp = new DataBind.VM.Interpreter("a+b+c");
				expect(exp.Run(evn)).toBe(600);
			}
			{
				var local = new SampleOBD2();
				local.Set(100, 200, 300);
				var evn2 = new TEnvExt(){
				{"local",local},
			};
				DataBind.VM.InterpreterEnv.ExtendsEnvironment(evn2);
				var exp = new DataBind.VM.Interpreter("local.a+local.b+local.c");
				expect(exp.Run(evn2)).toBe(600);
			}
			var haha = new SampleOBD3<SampleOBD2>();
			haha.Set(new SampleOBD2());
			haha.A.Set(100, 200, 300);
			{
				var evn3 = new TEnvExt(){
					{"haha",haha},
				};
				DataBind.VM.InterpreterEnv.ExtendsEnvironment(evn3);
				var exp = new DataBind.VM.Interpreter("haha.A.a+haha.A.b+haha.A.c");
				expect(exp.Run(evn3)).toBe(600);
			}
			{
				var evn4 = new TEnvExt(){
					{"Math",typeof(DataBind.VM.MathExt.Math)},
					{"haha",haha},
				};
				DataBind.VM.InterpreterEnv.ExtendsEnvironment(evn4);
				var exp = new DataBind.VM.Interpreter("Math.Max( haha.A.a,haha.A.b,haha.A.c)");
				expect(exp.Run(evn4)).toBe(300);
			}

			var a = new SampleOBD5();
			a.Set(100);
			var b = new SampleOBD3<SampleOBD5>();
			b.Set(new SampleOBD5());
			b.A.Set(100);
			Func<object[], Func<object, object>, number> call = (object[] list, Func<object, object> func) =>
			{
				number s = 0;
				foreach (var i in list)
				{
					s += (number)func(i);
				}
				return s;
			};
			var evn5 = new TEnvExt(){
				{"a",a },
				{"b",b},
				{"p",2},
				{"list",new DataBind.CollectionExt.Dictionary<string,number>[]{
					 new DataBind.CollectionExt.Dictionary<string, number>(){
						 {"攻击力",10.0 },
						 {"攻击力加成",0.5 },
					 },
					 new DataBind.CollectionExt.Dictionary<string, number>(){
						 {"攻击力",20.0 },
						 {"攻击力加成",0.5 },
					 },
				}},
				{ "SUM",call},
			};
			{
				DataBind.VM.InterpreterEnv.ExtendsEnvironment(evn5);
				var exp = new DataBind.VM.Interpreter("b.A.add(a)");
				expect(exp.Run(evn5)).toBe(200);
			}
			{
				var exp = new DataBind.VM.Interpreter("a.add(b.A)");
				expect(exp.Run(evn5)).toBe(200);
			}
			{
				var exp = new DataBind.VM.Interpreter("'abcd'.Length");
				expect(exp.Run(evn5)).toBe(4);
			}
			{
				var exp = new DataBind.VM.Interpreter("'ab,cd'.Split(',')");
				var result = exp.Run(evn5);
				string[] ls = (string[])result;
				expect(ls.Length).toBe(2);
			}

			{
				var exp = new DataBind.VM.Interpreter("/*嘻嘻嘻出现*/SUM(list/*哈哈哈*/,{攻击力*(1+攻击力加成 )* _self.p})//嘻嘻嘻出现");
				expect(exp.Run(evn5)).toBe(45 * 2);
			}

			{
				var exp = new DataBind.VM.Interpreter("level == null");
				expect(exp.Run(evn5)).toBe(true);
			}

			{
				var exp = new DataBind.VM.Interpreter("level != null");
				expect(exp.Run(evn5)).toBe(false);
			}

		}

		[Test]
		public void Test测试操作符执行顺序()
		{
			{
				var exp = new DataBind.VM.Interpreter("a && a.b && a.b.c || 6");
				expect(exp.Run(new TEnvExt())).toBe(6);
			}
			{
				var exp = new DataBind.VM.Interpreter("a && a.b && a.b.c || 6");
				expect(exp.Run(new TEnvExt() { { "a", new TObj() { } } })).toBe(6);
			}
			{
				var exp = new DataBind.VM.Interpreter("a && a.b && a.b.c || 6");
				expect(exp.Run(new TEnvExt() { { "a", new TObj() { { "b", new TObj() { } } } } })).toBe(6);
			}
			{
				var exp = new DataBind.VM.Interpreter("a && a.b && a.b.c || 6");
				expect(exp.Run(new TEnvExt() { { "a", new TObj() { { "b", new TObj() { { "c", 7 } } } } } })).toBe(7);
			}
			{
				var exp = new DataBind.VM.Interpreter("a && a.b && a.b.c || d");
				expect(exp.Run(new TEnvExt() { { "a", new TObj() { { "b", new TObj() { { "c", 7 } } } } } })).toBe(7);
			}
			{
				var exp = new DataBind.VM.Interpreter("a && a.b && a.b.c || d.e.f.x.s");
				expect(exp.Run(new TEnvExt() { { "a", new TObj() { { "b", new TObj() { { "c", 7 } } } } } })).toBe(7);
			}
		}
	}
}
