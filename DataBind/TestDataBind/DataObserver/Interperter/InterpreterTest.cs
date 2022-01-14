using NUnit.Framework;
using System.ListExt;
using System;
using vm;

namespace TestDataBind
{
	using Node = vm.CombineType<object, List<vm.WordNode>, vm.WordNode, vm.ASTNodeBase>;
	using TObj = vm.ProtoDict<string, object>;
	using TEnvExt = vm.ProtoDict<string, object>;
	using number = System.Double;

	public class SampleOBD5 : IObservable
	{
		public vm.Observer ___Sob__;
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

		public vm.Observer _SgetOb()
		{
			return ___Sob__;
		}

		public void _SsetOb(vm.Observer value)
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
				var nodeList = vm.Interpreter.toWords("true false");

				expect(nodeList[0].type).toEqual(vm.NodeType.boolean);
				expect(nodeList[0].value).toEqual(true);
				expect(nodeList[1].type).toEqual(vm.NodeType.boolean);
				expect(nodeList[1].value).toEqual(false);
			}
			{
				var nodeList = vm.Interpreter.toWords("a +  b - c");
				expect(nodeList[0].type).toEqual(vm.NodeType.word);
				expect(nodeList[0].value).toEqual("a");
				expect(nodeList[1].type).toEqual(vm.NodeType.Inst["+"]);
				expect(nodeList[1].value).toEqual(null);
				expect(nodeList[2].type).toEqual(vm.NodeType.word);
				expect(nodeList[2].value).toEqual("b");
				expect(nodeList[3].type).toEqual(vm.NodeType.Inst["-"]);
				expect(nodeList[3].value).toEqual(null);
				expect(nodeList[4].type).toEqual(vm.NodeType.word);
				expect(nodeList[4].value).toEqual("c");
			}
			{
				var nodeList = vm.Interpreter.toWords("100-1");
				expect(nodeList[0].type).toEqual(vm.NodeType.number);
				expect(nodeList[0].value).toEqual(100);
				expect(nodeList[1].type).toEqual(vm.NodeType.Inst["-"]);
				expect(nodeList[1].value).toEqual(null);
				expect(nodeList[2].type).toEqual(vm.NodeType.number);
				expect(nodeList[2].value).toEqual(1);
			}
			{
				var nodeList = vm.Interpreter.toWords("100--1.5");
				expect(nodeList[0].type).toEqual(vm.NodeType.number);
				expect(nodeList[0].value).toEqual(100);
				expect(nodeList[1].type).toEqual(vm.NodeType.Inst["-"]);
				expect(nodeList[1].value).toEqual(null);
				expect(nodeList[2].type).toEqual(vm.NodeType.number);
				expect(nodeList[2].value).toEqual(-1.5);
			}
			{
				var nodeList = vm.Interpreter.toWords("100+-1.11");
				expect(nodeList[0].type).toEqual(vm.NodeType.number);
				expect(nodeList[0].value).toEqual(100);
				expect(nodeList[1].type).toEqual(vm.NodeType.Inst["+"]);
				expect(nodeList[1].value).toEqual(null);
				expect(nodeList[2].type).toEqual(vm.NodeType.number);
				expect(nodeList[2].value).toEqual(-1.11);
			}


			{
				var nodeList = vm.Interpreter.toWords("a.b.c");
				expect(nodeList[0].type).toEqual(vm.NodeType.word);
				expect(nodeList[0].value).toEqual("a");
				expect(nodeList[1].type).toEqual(vm.NodeType.Inst["."]);
				expect(nodeList[1].value).toEqual(null);
				expect(nodeList[2].type).toEqual(vm.NodeType.word);
				expect(nodeList[2].value).toEqual("b");
				expect(nodeList[3].type).toEqual(vm.NodeType.Inst["."]);
				expect(nodeList[3].value).toEqual(null);
				expect(nodeList[4].type).toEqual(vm.NodeType.word);
				expect(nodeList[4].value).toEqual("c");
			}
			{
				var nodeList = vm.Interpreter.toWords("aa>=b");
				expect(nodeList[0].type).toEqual(vm.NodeType.word);
				expect(nodeList[0].value).toEqual("aa");
				expect(nodeList[1].type).toEqual(vm.NodeType.Inst[">="]);
				expect(nodeList[1].value).toEqual(null);
				expect(nodeList[2].type).toEqual(vm.NodeType.word);
				expect(nodeList[2].value).toEqual("b");
			}
			{
				var nodeList = vm.Interpreter.toWords("(a+b-c)*d/e**4>123<312>=11.1<=222!=3==4&&'aa'||\"bb\"!`cc`,");
				expect(nodeList[0].type).toEqual(vm.NodeType.Inst["("]);
				expect(nodeList[0].value).toEqual(null);
				expect(nodeList[1].type).toEqual(vm.NodeType.word);
				expect(nodeList[1].value).toEqual("a");
				expect(nodeList[2].type).toEqual(vm.NodeType.Inst["+"]);
				expect(nodeList[2].value).toEqual(null);
				expect(nodeList[3].type).toEqual(vm.NodeType.word);
				expect(nodeList[3].value).toEqual("b");
				expect(nodeList[4].type).toEqual(vm.NodeType.Inst["-"]);
				expect(nodeList[4].value).toEqual(null);
				expect(nodeList[5].type).toEqual(vm.NodeType.word);
				expect(nodeList[5].value).toEqual("c");
				expect(nodeList[6].type).toEqual(vm.NodeType.Inst[")"]);
				expect(nodeList[6].value).toEqual(null);
				expect(nodeList[7].type).toEqual(vm.NodeType.Inst["*"]);
				expect(nodeList[7].value).toEqual(null);
				expect(nodeList[8].type).toEqual(vm.NodeType.word);
				expect(nodeList[8].value).toEqual("d");
				expect(nodeList[9].type).toEqual(vm.NodeType.Inst["/"]);
				expect(nodeList[9].value).toEqual(null);
				expect(nodeList[10].type).toEqual(vm.NodeType.word);
				expect(nodeList[10].value).toEqual("e");
				expect(nodeList[11].type).toEqual(vm.NodeType.Inst["**"]);
				expect(nodeList[11].value).toEqual(null);
				expect(nodeList[12].type).toEqual(vm.NodeType.number);
				expect(nodeList[12].value).toEqual(4);
				expect(nodeList[13].type).toEqual(vm.NodeType.Inst[">"]);
				expect(nodeList[13].value).toEqual(null);
				expect(nodeList[14].type).toEqual(vm.NodeType.number);
				expect(nodeList[14].value).toEqual(123);
				expect(nodeList[15].type).toEqual(vm.NodeType.Inst["<"]);
				expect(nodeList[15].value).toEqual(null);
				expect(nodeList[16].type).toEqual(vm.NodeType.number);
				expect(nodeList[16].value).toEqual(312);
				expect(nodeList[17].type).toEqual(vm.NodeType.Inst[">="]);
				expect(nodeList[17].value).toEqual(null);
				expect(nodeList[18].type).toEqual(vm.NodeType.number);
				expect(nodeList[18].value).toEqual(11.1);
				expect(nodeList[19].type).toEqual(vm.NodeType.Inst["<="]);
				expect(nodeList[19].value).toEqual(null);
				expect(nodeList[20].type).toEqual(vm.NodeType.number);
				expect(nodeList[20].value).toEqual(222);
				expect(nodeList[21].type).toEqual(vm.NodeType.Inst["!="]);
				expect(nodeList[21].value).toEqual(null);
				expect(nodeList[22].type).toEqual(vm.NodeType.number);
				expect(nodeList[22].value).toEqual(3);
				expect(nodeList[23].type).toEqual(vm.NodeType.Inst["=="]);
				expect(nodeList[23].value).toEqual(null);
				expect(nodeList[24].type).toEqual(vm.NodeType.number);
				expect(nodeList[24].value).toEqual(4);
				expect(nodeList[25].type).toEqual(vm.NodeType.Inst["&&"]);
				expect(nodeList[25].value).toEqual(null);
				expect(nodeList[26].type).toEqual(vm.NodeType.stringx);
				expect(nodeList[26].value).toEqual("aa");
				expect(nodeList[27].type).toEqual(vm.NodeType.Inst["||"]);
				expect(nodeList[27].value).toEqual(null);
				expect(nodeList[28].type).toEqual(vm.NodeType.stringx);
				expect(nodeList[28].value).toEqual("bb");
				expect(nodeList[29].type).toEqual(vm.NodeType.Inst["!"]);
				expect(nodeList[29].value).toEqual(null);
				expect(nodeList[30].type).toEqual(vm.NodeType.stringx);
				expect(nodeList[30].value).toEqual("cc");
				expect(nodeList[31].type).toEqual(vm.NodeType.Inst[","]);
				expect(nodeList[31].value).toEqual(null);
			}
			{
				//加入{} 子表达式
				var nodeList = vm.Interpreter.toWords("SUM(装备列表,{装备等级*装备加成})");
				expect(nodeList[0].type).toEqual(vm.NodeType.word);
				expect(nodeList[0].value).toEqual("SUM");
				expect(nodeList[1].type).toEqual(vm.NodeType.Inst["("]);
				expect(nodeList[1].value).toEqual(null);
				expect(nodeList[2].type).toEqual(vm.NodeType.word);
				expect(nodeList[2].value).toEqual("装备列表");
				expect(nodeList[3].type).toEqual(vm.NodeType.Inst[","]);
				expect(nodeList[3].value).toEqual(null);
				expect(nodeList[4].type).toEqual(vm.NodeType.Inst["{"]);
				expect(nodeList[4].value).toEqual(null);
				expect(nodeList[5].type).toEqual(vm.NodeType.word);
				expect(nodeList[5].value).toEqual("装备等级");
				expect(nodeList[6].type).toEqual(vm.NodeType.Inst["*"]);
				expect(nodeList[6].value).toEqual(null);
				expect(nodeList[7].type).toEqual(vm.NodeType.word);
				expect(nodeList[7].value).toEqual("装备加成");
				expect(nodeList[8].type).toEqual(vm.NodeType.Inst["}"]);
				expect(nodeList[8].value).toEqual(null);
			}
			{
				var nodeList = vm.Interpreter.toWords("装备.lv <= true, `装备等\n    级太高`");

				expect(nodeList[0].type).toEqual(vm.NodeType.word);

				expect(nodeList[0].value).toEqual("装备");
				expect(nodeList[0].lineStart).toEqual(0);
				expect(nodeList[0].lineEnd).toEqual(0);
				expect(nodeList[0].columnStart).toEqual(0);
				expect(nodeList[0].columnEnd).toEqual(1);
				expect(nodeList[1].type).toEqual(vm.NodeType.Inst["."]);
				expect(nodeList[1].value).toEqual(null);
				expect(nodeList[2].type).toEqual(vm.NodeType.word);
				expect(nodeList[2].value).toEqual("lv");
				expect(nodeList[3].type).toEqual(vm.NodeType.Inst["<="]);
				expect(nodeList[3].value).toEqual(null);
				expect(nodeList[3].lineStart).toEqual(0);
				expect(nodeList[3].lineEnd).toEqual(0);
				expect(nodeList[3].columnStart).toEqual(6);
				expect(nodeList[3].columnEnd).toEqual(7);
				expect(nodeList[4].type).toEqual(vm.NodeType.boolean);
				expect(nodeList[4].value).toEqual(true);
				expect(nodeList[4].lineStart).toEqual(0);
				expect(nodeList[4].lineEnd).toEqual(0);
				expect(nodeList[4].columnStart).toEqual(9);
				expect(nodeList[4].columnEnd).toEqual(12);
				expect(nodeList[5].type).toEqual(vm.NodeType.Inst[","]);
				expect(nodeList[5].value).toEqual(null);
				expect(nodeList[6].type).toEqual(vm.NodeType.stringx);
				expect(nodeList[6].value).toEqual("装备等\n    级太高");
				expect(nodeList[6].lineStart).toEqual(0);
				expect(nodeList[6].lineEnd).toEqual(1);
				expect(nodeList[6].columnStart).toEqual(15);
				expect(nodeList[6].columnEnd).toEqual(7);
			}
			{
				var nodeList = vm.Interpreter.toWords("-1");
				expect(nodeList.length).toBe(1);
				expect(nodeList[0].type).toBe(vm.NodeType.number);
				expect(nodeList[0].value).toBe(-1);
			}
			{
				var nodeList = vm.Interpreter.toWords("CALL(-1,-1,-1)");
				expect(nodeList[0].value).toBe("CALL");
				expect(nodeList[2].value).toBe(-1);
				expect(nodeList[4].value).toBe(-1);
				expect(nodeList[6].value).toBe(-1);
			}
		}

		[Test]
		public void test语法分析()
		{
			List<string> errorList = new List<string>();
			{
				//单个值
				var nodeList = vm.Interpreter.toWords("a");
				var tree = vm.Interpreter.toAST(nodeList, "a", errorList);
				expect(errorList.length).toBe(0);
				expect(tree).toBeInstanceOf(typeof(vm.ValueASTNode));
				expect(tree.operatorx).toBe(vm.NodeType.word);
				expect((tree as vm.ValueASTNode).value.type).toBe(vm.NodeType.word);
				expect((tree as vm.ValueASTNode).value.value).toBe("a");
			}
			{
				var nodeList = vm.Interpreter.toWords("'a'");
				var tree = vm.Interpreter.toAST(nodeList, "a", errorList);
				expect(errorList.length).toBe(0);
				expect(tree).toBeInstanceOf(typeof(vm.ValueASTNode));
				expect(tree.operatorx).toBe(vm.NodeType.stringx);
				expect(((tree as vm.ValueASTNode).value).type).toBe(vm.NodeType.stringx);
				expect(((tree as vm.ValueASTNode).value).value).toBe("a");
			}
			{
				var nodeList = vm.Interpreter.toWords("100.4");
				var tree = vm.Interpreter.toAST(nodeList, "100.4", errorList);
				expect(errorList.length).toBe(0);
				expect(tree).toBeInstanceOf(typeof(vm.ValueASTNode));
				expect(tree.operatorx).toBe(vm.NodeType.number);
				expect(((tree as vm.ValueASTNode).value).type).toBe(vm.NodeType.number);
				expect(((tree as vm.ValueASTNode).value).value).toBe(100.4);
			}
			{
				var nodeList = vm.Interpreter.toWords("true");
				var tree = vm.Interpreter.toAST(nodeList, "true", errorList);
				expect(errorList.length).toBe(0);
				expect(tree).toBeInstanceOf(typeof(vm.ValueASTNode));
				expect(tree.operatorx).toBe(vm.NodeType.boolean);
				expect(((tree as vm.ValueASTNode).value).type).toBe(vm.NodeType.boolean);
				expect(((tree as vm.ValueASTNode).value).value).toBe(true);
			}
			{
				//最简单的情况;
				var nodeList = vm.Interpreter.toWords("a +  b - c");
				var tree0 = vm.Interpreter.toAST(nodeList, "a +  b - c", errorList);
				expect(errorList.length).toBe(0);
				expect(tree0).toBeInstanceOf(typeof(vm.BinaryASTNode));
				if (tree0 is vm.BinaryASTNode)
				{
					var tree = tree0 as vm.BinaryASTNode;
					expect(tree.operatorx).toBe(vm.TNodeType.Inst["-"]);
					expect(tree.right.operatorx).toBe(vm.NodeType.word);
					expect(tree.right).toBeInstanceOf(typeof(vm.ValueASTNode));
					if (tree.right is vm.ValueASTNode)
					{
						expect((tree.right as vm.ValueASTNode).value.value).toBe("c");
					};

					expect(tree.left).toBeInstanceOf(typeof(vm.BinaryASTNode));
					if (tree.left is vm.BinaryASTNode)
					{
						var left = tree.left as vm.BinaryASTNode;
						expect(left.operatorx).toBe(vm.TNodeType.Inst["+"]);
						expect(left.left).toBeInstanceOf(typeof(vm.ValueASTNode));
						expect(left.right).toBeInstanceOf(typeof(vm.ValueASTNode));
						expect((left.left as vm.ValueASTNode).value.value).toBe("a");
						expect((left.right as vm.ValueASTNode).value.value).toBe("b");
					};

				};
			}
			{
				//包含括号;
				var nodeList = vm.Interpreter.toWords("(a +  b) * c");
				var tree = vm.Interpreter.toAST(nodeList, "(a +  b) * c", errorList);
				expect(errorList.length).toBe(0);
				expect(vm.Interpreter.toStringAST(tree)).toBe("(a + b) * c");
			}
			{
				//先后顺序;
				var nodeList = vm.Interpreter.toWords("a +  b * c");
				var tree = vm.Interpreter.toAST(nodeList, "a +  b * c", errorList);
				expect(errorList.length).toBe(0);
				expect(vm.Interpreter.toStringAST(tree)).toBe("a + b * c");
			}
			{
				// //属性访问;
				var nodeList = vm.Interpreter.toWords("a.b.c +  b.b.c * c.b.c");
				var tree = vm.Interpreter.toAST(nodeList, "a.b.c +  b.b.c * c.b.c", errorList);
				expect(errorList.length).toBe(0);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(((a.b).c) + (((b.b).c) * ((c.b).c)))");
			}
			{
				//简单中括号访问;
				var nodeList = vm.Interpreter.toWords("a['c'] +  b");
				var tree = vm.Interpreter.toAST(nodeList, "a['c'] +  b", errorList);
				expect(errorList.length).toBe(0);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("((a[\"c\"]) + b)");
			}
			{
				var nodeList = vm.Interpreter.toWords("a['c']['d'] +  b");
				var tree = vm.Interpreter.toAST(nodeList, "a['c']['d'] +  b", errorList);
				expect(errorList.length).toBe(0);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(((a[\"c\"])[\"d\"]) + b)");
			}
			{
				// //中括号访问;
				var nodeList = vm.Interpreter.toWords("a.b['c'] +  b['b']['c'] * c.b.c");
				var tree = vm.Interpreter.toAST(nodeList, "a.b['c'] +  b['b']['c'] * c.b.c", errorList);
				expect(errorList.length).toBe(0);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(((a.b)[\"c\"]) + (((b[\"b\"])[\"c\"]) * ((c.b).c)))");
			}
			{
				// //！运算符;
				var nodeList = vm.Interpreter.toWords("!a");
				var tree = vm.Interpreter.toAST(nodeList, "!a", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(!a)");
			}
			{
				var nodeList = vm.Interpreter.toWords("!(!a)");
				var tree = vm.Interpreter.toAST(nodeList, "!(!a)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(!((!a)))");
			}
			{
				var nodeList = vm.Interpreter.toWords("!!a");
				var tree = vm.Interpreter.toAST(nodeList, "!!a", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(!(!a))");
			}
			{
				var nodeList = vm.Interpreter.toWords("!(a >= !b)");
				var tree = vm.Interpreter.toAST(nodeList, "!(a >= !b)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(!((a >= (!b))))");
			}
			{
				// //函数调用;
				var nodeList = vm.Interpreter.toWords("a()");
				var tree = vm.Interpreter.toAST(nodeList, "a()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(a( ))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a.b()");
				var tree = vm.Interpreter.toAST(nodeList, "a.b()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("((a.b)( ))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a['b']()");
				var tree = vm.Interpreter.toAST(nodeList, "a['b']()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("((a[\"b\"])( ))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a['b']['c']()");
				var tree = vm.Interpreter.toAST(nodeList, "a['b']['c']()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(((a[\"b\"])[\"c\"])( ))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a['b'].c()");
				var tree = vm.Interpreter.toAST(nodeList, "a['b'].c()", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(((a[\"b\"]).c)( ))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a['b'].c()(666)");
				var tree = vm.Interpreter.toAST(nodeList, "a['b'].c()(666)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("((((a[\"b\"]).c)( ))( 666))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a(b)");
				var tree = vm.Interpreter.toAST(nodeList, "a(b)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(a( b))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a(b,c,d)");
				var tree = vm.Interpreter.toAST(nodeList, "a(b,c,d)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(a( b, c, d))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a(b1+b2,'c',d-1)");
				var tree = vm.Interpreter.toAST(nodeList, "a(b1+b2,'c',d-1)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(a( (b1 + b2), \"c\", (d - 1)))");
			}
		}

		/// <summary>
		/// !cur.greatProperty || cur.greatProperty.length <= 0
		/// </summary>
		[Test]
		public void testSpecial1()
		{
			List<string> errorList = new List<string>();
			var nodeList = vm.Interpreter.toWords("!cur.greatProperty || cur.greatProperty.length <= 0");
			var tree = vm.Interpreter.toAST(nodeList, "!cur.greatProperty || cur.greatProperty.length <= 0", errorList);
			expect(errorList.TryGet(0)).toBe(null);
			expect(vm.Interpreter.toStringAST(tree, true)).toBe("((!(cur.greatProperty)) || (((cur.greatProperty).length) <= 0))");
		}

		[Test]
		public void test语法分析复杂()
		{
			List<string> errorList = new List<string>();
			{
				var nodeList = vm.Interpreter.toWords("cc.lib.format(\"玩家等级 %d Lv\",100)");
				var tree = vm.Interpreter.toAST(nodeList, "cc.lib.format(\"玩家等级 %d Lv\",100)", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(((cc.lib).format)( \"玩家等级 %d Lv\", 100))");
			}
			{
				var nodeList = vm.Interpreter.toWords("a/(b)+c");
				var tree = vm.Interpreter.toAST(nodeList, "a/(b)+c", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("((a / (b)) + c)");
			}
			{
				var nodeList = vm.Interpreter.toWords("Min(1,暴击/(暴击+韧性)*(LvA*2/(LvA+LvB)))");
				var tree = vm.Interpreter.toAST(nodeList, "Min(1,暴击/(暴击+韧性)*(LvA*2/(LvA+LvB)))", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(Min( 1, ((暴击 / ((暴击 + 韧性))) * (((LvA * 2) / ((LvA + LvB)))))))");
			}

			{
				var nodeList = vm.Interpreter.toWords("SUM(装备列表,{等级*加成})");
				var tree = vm.Interpreter.toAST(nodeList, "SUM(装备列表,{等级*加成})", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(SUM( 装备列表, {(等级 * 加成)}))");
			}
			{
				var nodeList = vm.Interpreter.toWords("SUM(装备列表,{等级*加成+100-10},{等级*加成})");
				var tree = vm.Interpreter.toAST(nodeList, "SUM(装备列表,{等级*加成+100-10},{等级*加成})", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(SUM( 装备列表, {(((等级 * 加成) + 100) - 10)}, {(等级 * 加成)}))");
			}

			{
				var nodeList = vm.Interpreter.toWords("SUM({攻击力*(1+攻击力加成*p)})//哈哈行a");
				var tree = vm.Interpreter.toAST(nodeList, "/*略略略*/SUM(/*七七七*/{攻击力*(1+攻击力加成*p)})//嘻嘻嘻出现", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(SUM( {(攻击力 * ((1 + (攻击力加成 * p))))}))");
			}
			{
				var nodeList = vm.Interpreter.toWords("level<= MAX(RoleConfig,{level})*10");
				var tree = vm.Interpreter.toAST(nodeList, "level<= MAX(RoleConfig,{level})*10", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(level <= ((MAX( RoleConfig, {level})) * 10))");
			}
			{
				var nodeList = vm.Interpreter.toWords("level<= MAX(RoleConfig,{level}).level");
				var tree = vm.Interpreter.toAST(nodeList, "level<= MAX(RoleConfig,{level}).level", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(level <= ((MAX( RoleConfig, {level})).level))");
			}
			{
				var nodeList = vm.Interpreter.toWords("level.MAX(RoleConfig,{level}).level");
				var tree = vm.Interpreter.toAST(nodeList, "level.MAX(RoleConfig,{level}).level", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(((level.MAX)( RoleConfig, {level})).level)");
			}
			{
				var nodeList = vm.Interpreter.toWords("level == null");
				var tree = vm.Interpreter.toAST(nodeList, "level == null", errorList);
				expect(errorList.TryGet(0)).toBe(null);
				expect(vm.Interpreter.toStringAST(tree, true)).toBe("(level == null)");
			}

		}


		[Test]
		public void test环境测试()
		{
            {
				var v = ((System.Reflection.MethodInfo)InterpreterEnv.environment["MIN"])
					.Invoke(InterpreterEnv.environment, new object[] { new double[] { 1, 2 } });
				expect(v).toBe(1);
                expect((double)vm.InterpreterEnv.environment["PI"]).toBe(Math.PI);
            }
            {
				var a = new TEnvExt();
                InterpreterEnv.extendsEnvironment(a);
				var v = ((System.Reflection.MethodInfo)a["MIN"])
					.Invoke(InterpreterEnv.environment, new object[] { new double[] { 1, 2 } });
				expect(v).toBe(1);
                expect(a["PI"]).toBe(Math.PI);
                expect(a.Count).toBe(0);
            }
            {
                var b = new TEnvExt(){ { "a", 1 } };
				var count0 = b.Count;
                InterpreterEnv.implementEnvironment(b);
				var v = ((System.Reflection.MethodInfo)b["MIN"])
					.Invoke(InterpreterEnv.environment, new object[] { new double[] { 1, 2 } });
				expect(v).toBe(1);
                expect(b["PI"]).toBe(Math.PI);
				var count1 = b.Count;
                expect(count1-count0).toBe(28);
				//expect(b.Count).toBe(43);
			}
		}

		[Test]
		public void Test表达式运行测试()
		{
			expect(new vm.Interpreter("!false").run(vm.InterpreterEnv.environment)).toBe(true);
			expect(new vm.Interpreter("3**2").run(vm.InterpreterEnv.environment)).toBe(9);
			expect(new vm.Interpreter("3*2").run(vm.InterpreterEnv.environment)).toBe(6);
			expect(new vm.Interpreter("3/2").run(vm.InterpreterEnv.environment)).toBe(1.5);
			expect(new vm.Interpreter("3%2").run(vm.InterpreterEnv.environment)).toBe(1);
			expect(new vm.Interpreter("11+12").run(vm.InterpreterEnv.environment)).toBe(23);
			expect(new vm.Interpreter("11-12").run(vm.InterpreterEnv.environment)).toBe(-1);
			expect(new vm.Interpreter("11>12").run(vm.InterpreterEnv.environment)).toBe(false);
			expect(new vm.Interpreter("11<12").run(vm.InterpreterEnv.environment)).toBe(true);
			expect(new vm.Interpreter("11>=11").run(vm.InterpreterEnv.environment)).toBe(true);
			expect(new vm.Interpreter("11<=11").run(vm.InterpreterEnv.environment)).toBe(true);
			expect(new vm.Interpreter("12!=11").run(vm.InterpreterEnv.environment)).toBe(true);
			expect(new vm.Interpreter("12==11").run(vm.InterpreterEnv.environment)).toBe(false);
			expect(new vm.Interpreter("12>11 && 11<15").run(vm.InterpreterEnv.environment)).toBe(true);
			expect(new vm.Interpreter("12>11 || 11>15").run(vm.InterpreterEnv.environment)).toBe(true);
			{
				var exp = new vm.Interpreter("MIN(100*2,200+100,300/2)");
				expect(exp.run(vm.InterpreterEnv.environment)).toBe(150);
			}
			{
				var evn = new TEnvExt(){
					{"a",100},
					{"b",200},
					{"c",300},
				};
				vm.InterpreterEnv.extendsEnvironment(evn);
				var exp = new vm.Interpreter("a+b+c");
				expect(exp.run(evn)).toBe(600);
			}
			{
				var local = new SampleOBD2();
				local.Set(100, 200, 300);
				var evn2 = new TEnvExt(){
				{"local",local},
			};
				vm.InterpreterEnv.extendsEnvironment(evn2);
				var exp = new vm.Interpreter("local.a+local.b+local.c");
				expect(exp.run(evn2)).toBe(600);
			}
			var haha = new SampleOBD3<SampleOBD2>();
			haha.Set(new SampleOBD2());
			haha.A.Set(100, 200, 300);
			{
				var evn3 = new TEnvExt(){
					{"haha",haha},
				};
				vm.InterpreterEnv.extendsEnvironment(evn3);
				var exp = new vm.Interpreter("haha.A.a+haha.A.b+haha.A.c");
				expect(exp.run(evn3)).toBe(600);
			}
			{
				var evn4 = new TEnvExt(){
					{"Math",typeof(vm.MathExt.Math)},
					{"haha",haha},
				};
				vm.InterpreterEnv.extendsEnvironment(evn4);
				var exp = new vm.Interpreter("Math.Max( haha.A.a,haha.A.b,haha.A.c)");
				expect(exp.run(evn4)).toBe(300);
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
				{"list",new Dictionary<string,number>[]{
					 new Dictionary<string, number>(){
						 {"攻击力",10.0 },
						 {"攻击力加成",0.5 },
					 },
					 new Dictionary<string, number>(){
						 {"攻击力",20.0 },
						 {"攻击力加成",0.5 },
					 },
				}},
				{ "SUM",call},
			};
			{
				vm.InterpreterEnv.extendsEnvironment(evn5);
				var exp = new vm.Interpreter("b.A.add(a)");
				expect(exp.run(evn5)).toBe(200);
			}
			{
				var exp = new vm.Interpreter("a.add(b.A)");
				expect(exp.run(evn5)).toBe(200);
			}
			{
				var exp = new vm.Interpreter("'abcd'.length()");
				expect(exp.run(evn5)).toBe(4);
			}
			{
				var exp = new vm.Interpreter("'ab,cd'.split(',')");
				var result = exp.run(evn5);
				string[] ls = (string[])result;
				expect(ls.Length).toBe(2);
			}

			{
				var exp = new vm.Interpreter("/*嘻嘻嘻出现*/SUM(list/*哈哈哈*/,{攻击力*(1+攻击力加成 )* _.p})//嘻嘻嘻出现");
				expect(exp.run(evn5)).toBe(45 * 2);
			}

			{
				var exp = new vm.Interpreter("level == null");
				expect(exp.run(evn5)).toBe(true);
			}

			{
				var exp = new vm.Interpreter("level != null");
				expect(exp.run(evn5)).toBe(false);
			}

		}

		[Test]
		public void test测试操作符执行顺序()
		{
			{
				var exp = new vm.Interpreter("a && a.b && a.b.c || 6");
				expect(exp.run(new TEnvExt())).toBe(6);
			}
			{
				var exp = new vm.Interpreter("a && a.b && a.b.c || 6");
				expect(exp.run(new TEnvExt() { { "a", new TObj() { } } })).toBe(6);
			}
			{
				var exp = new vm.Interpreter("a && a.b && a.b.c || 6");
				expect(exp.run(new TEnvExt() { { "a", new TObj() { { "b", new TObj() { } } } } })).toBe(6);
			}
			{
				var exp = new vm.Interpreter("a && a.b && a.b.c || 6");
				expect(exp.run(new TEnvExt() { { "a", new TObj() { { "b", new TObj() { { "c", 7 } } } } } })).toBe(7);
			}
			{
				var exp = new vm.Interpreter("a && a.b && a.b.c || d");
				expect(exp.run(new TEnvExt() { { "a", new TObj() { { "b", new TObj() { { "c", 7 } } } } } })).toBe(7);
			}
			{
				var exp = new vm.Interpreter("a && a.b && a.b.c || d.e.f.x.s");
				expect(exp.run(new TEnvExt() { { "a", new TObj() { { "b", new TObj() { { "c", 7 } } } } } })).toBe(7);
			}
		}
	}
}
