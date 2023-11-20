using NUnit.Framework;
using ParseJSDataBindAbstract;
using UnitTestUitls;

namespace Tests
{
	public class TestParseJSDataBindAbstract: TestEnv
	{
		[Test]
		public void Test节点树测试2()
		{
			var interpreter = new vm.Interpreter("a.b.c+(3*4)+fe.cx.xc(n.wf,rrx.xx)+fe['ds']+few[wf.f]+few[0]");
			var envInfo = new EnvInfo();
			var retType = ParseJSDataBind.HandleOperator(envInfo, null, interpreter.Ast);
			expect(envInfo.MemberMap.Count).toBe(6);
			expect(envInfo.MemberMap["a"].Type.GetType()).toBe(typeof(ClassInfo));
			expect(envInfo.MemberMap["a"].Type.Name).toBe("TA");

			var funcType = envInfo.MemberMap["fe"].Type.MemberMap["cx"].Type.MemberMap["xc"].Type as FuncInfo;
			expect(funcType != null).toBe(true);
			expect(funcType).toBe(envInfo.InsideTypeMap["TFe"].InsideTypeMap["TCx"].InsideTypeMap["TXc"]);
			expect(funcType.Paras[0].Name).toBe("wf");
			expect(funcType.Paras[0].Type.Parent.Name).toBe("TN");
			expect(funcType.Paras[1].Name).toBe("xx");
			expect(funcType.Paras[1].Type.Parent.Name).toBe("TRrx");
		}

	}
}