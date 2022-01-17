using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vm
{
	using number = System.Double;

	public class MathInsider
	{
		public static object ParseNumStr(string temp)
		{
			{
				int n;
				if (int.TryParse(temp, out n))
				{
					return n;
				}
			}
			{
				long n;
				if (long.TryParse(temp, out n))
				{
					return n;
				}
			}
			{
				ulong n;
				if (ulong.TryParse(temp, out n))
				{
					return n;
				}
			}
			{
				double n;
				if (double.TryParse(temp, out n))
				{
					return n;
				}
			}
			{
				float n;
				if (float.TryParse(temp, out n))
				{
					return n;
				}
			}

			throw new NotImplementedException($"temp: {temp}");
		}
		public static object CalcUnkownNumOp(number op1, object a, object b)
		{
			object ret;
			if (a is double || b is double)
			{
				ret = MathInsider.calcNumOp(op1, Convert.ToDouble(a), Convert.ToDouble(b));
			}
			else if (a is float || b is float)
			{
				ret = MathInsider.calcNumOp(op1, Convert.ToSingle(a), Convert.ToSingle(b));
			}
			else if (a is ulong || b is ulong)
			{
				ret = MathInsider.calcNumOp(op1, Convert.ToInt64(a), Convert.ToInt64(b));
			}
			else if (a is long || b is long)
			{
				ret = MathInsider.calcNumOp(op1, Convert.ToInt64(a), Convert.ToInt64(b));
			}
			else if (a is uint || b is uint)
			{
				ret = MathInsider.calcNumOp(op1, Convert.ToUInt32(a), Convert.ToUInt32(b));
			}
			else if (a is int || b is int)
			{
				ret = MathInsider.calcNumOp(op1, Convert.ToInt32(a), Convert.ToInt32(b));
			}
			else if (a is ushort || b is ushort)
			{
				ret = MathInsider.calcNumOp(op1, Convert.ToUInt16(a), Convert.ToUInt16(b));
			}
			else if (a is short || b is short)
			{
				ret = MathInsider.calcNumOp(op1, Convert.ToInt16(a), Convert.ToInt16(b));
			}
			else
			{
				throw new NotImplementedException();
			}
			return ret;
		}
		public static double Pow(double a, double b)
		{
			return (double)Math.Pow(a, b);
		}
		public static float Pow(float a, float b)
		{
			return (float)Math.Pow(a, b);
		}
		public static int Pow(int a, int b)
		{
			return (int)Math.Pow(a, b);
		}
		public static long Pow(long a, long b)
		{
			return (long)Math.Pow(a, b);
		}
		public static short Pow(short a, short b)
		{
			return (short)Math.Pow(a, b);
		}
		public static uint Pow(uint a, uint b)
		{
			return (uint)Math.Pow(a, b);
		}
		public static ulong Pow(ulong a, ulong b)
		{
			return (ulong)Math.Pow(a, b);
		}
		public static ushort Pow(ushort a, ushort b)
		{
			return (ushort)Math.Pow(a, b);
		}
		public static object calcNumOp(number op1, double a1, double b1)
		{
			switch (op1)
			{
				case number op when op == TNodeType.Inst["**"]:
					return Pow(a1, b1);
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
					throw new Exception($"意外的二元运算符[{TNodeType.Inst[op1]}]");
			}
		}

		public static object calcNumOp(number op1, float a1, float b1)
		{
			switch (op1)
			{
				case number op when op == TNodeType.Inst["**"]:
					return Pow(a1, b1);
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
					throw new Exception($"意外的二元运算符[{TNodeType.Inst[op1]}]");
			}
		}

		public static object calcNumOp(number op1, int a1, int b1)
		{
			switch (op1)
			{
				case number op when op == TNodeType.Inst["**"]:
					return Pow(a1, b1);
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
					throw new Exception($"意外的二元运算符[{TNodeType.Inst[op1]}]");
			}
		}

		public static object calcNumOp(number op1, uint a1, uint b1)
		{
			switch (op1)
			{
				case number op when op == TNodeType.Inst["**"]:
					return Pow(a1, b1);
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
					throw new Exception($"意外的二元运算符[{TNodeType.Inst[op1]}]");
			}
		}

		public static object calcNumOp(number op1, short a1, short b1)
		{
			switch (op1)
			{
				case number op when op == TNodeType.Inst["**"]:
					return Pow(a1, b1);
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
					throw new Exception($"意外的二元运算符[{TNodeType.Inst[op1]}]");
			}
		}

		public static object calcNumOp(number op1, ushort a1, ushort b1)
		{
			switch (op1)
			{
				case number op when op == TNodeType.Inst["**"]:
					return Pow(a1, b1);
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
					throw new Exception($"意外的二元运算符[{TNodeType.Inst[op1]}]");
			}
		}

		public static object calcNumOp(number op1, long a1, long b1)
		{
			switch (op1)
			{
				case number op when op == TNodeType.Inst["**"]:
					return Pow(a1, b1);
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
					throw new Exception($"意外的二元运算符[{TNodeType.Inst[op1]}]");
			}
		}

		public static object calcNumOp(number op1, ulong a1, ulong b1)
		{
			switch (op1)
			{
				case number op when op == TNodeType.Inst["**"]:
					return Pow(a1, b1);
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
					throw new Exception($"意外的二元运算符[{TNodeType.Inst[op1]}]");
			}
		}
	}
}
