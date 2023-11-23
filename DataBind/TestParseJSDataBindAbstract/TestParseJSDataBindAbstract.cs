using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using ParseJSDataBindAbstract;
using ParseJSDataBindAbstract.CodeWriter;
using TestingCode;
using UnitTestUitls;

namespace Tests
{
    public class TestParseJSDataBindAbstract : TestEnv
    {
        [Test]
        public void Test节点树测试2()
        {
            var interpreter = new vm.Interpreter("a.b.c+(3*4)+fe.cx.xc(n.wf,rrx.xx)+fe['ds']+few[wf.f]+few[0]+!wef");
            var envInfo = new EnvInfo();
            var retType = ParseJSDataBind.HandleOperator(envInfo, null, interpreter.Ast);
            expect(envInfo.MemberMap.Count).toBe(7);
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

        // [Test]
        // public void TestWriteCode()
        // {
        //     var interpreter = new vm.Interpreter("a.b.c+(3*4)+fe.cx.xc(n.wf,rrx.xx)+fe['ds']+few[wf.f]+few[0]+!wef");
        //     var envInfo = ParseJSDataBind.ParseTypeInfo(interpreter.Ast);
        //
        //     var codeWriter = new CodeWriter();
        //     codeWriter.UnknownTypeMark = "object";
        //     var codeText = codeWriter.WriteCode(envInfo,"TestWriteCodeCase1");
        //     // System.Console.WriteLine($"{Environment.CurrentDirectory}");
        //     File.WriteAllText("../../../DataBindGen.cs", codeText,Encoding.UTF8);
        // }

        [Test]
        public void TestWriteCodeWithCase1()
        {
            var interpreter = new vm.Interpreter("a.b.c+fe.cx.xc(n.wf,rrx.xx)");
            var data = new TestWriteCodeCase1();
            data.a.b.c = 32;
            var ret=interpreter.Run(data);
            expect(ret).toBe(data.a.b.c+564);
        }
        
        [Test]
        public void TestModifyCodeWithCase2()
        {
            var interpreter = new vm.Interpreter("a.b.c+(3*4)+fe.cx.xc(n.wf,rrx.xx)+fe['ds']+few[wf.f]+few[0]+!wef+ke.jf()+jklwe.jx.jfj(4,fd.g,fe.cx,kxx)+jklwe.jx.jfj2(4,'fdg',false,fe.cx)+kxx");
            var envInfo = ParseJSDataBind.ParseTypeInfo(interpreter.Ast, "TestWriteCodeCase2");
            
            var codeLoader = new CodeLoader();
            var content = File.ReadAllText("../../../DataBindGen2.cs");
            codeLoader.ModifyCode(envInfo,typeof(TestWriteCodeCase2),content);
            
            var codeWriter = new CodeWriter();
            codeWriter.UnknownTypeMark = "object";
            var codeText = codeWriter.WriteCode(envInfo);
            File.WriteAllText("../../../DataBindGen2.txt", codeText,Encoding.UTF8);
            expect(codeText).toBe(content);
        }
        
        [Test]
        public void TestModifyCodeWithCase3()
        {
            var interpreter = new vm.Interpreter("a.b.c+(3*4)+fe.cx.xc(n.wf,rrx.xx)+fe['ds']+few[wf.f]+few[0]+!wef+ke.jf()+jklwe.jx.jfj(4,fd.g,fe.cx,kxx)+jklwe.jx.jfj2(4,'fdg',false,fe.cx)+kxx");
            var envInfo = ParseJSDataBind.ParseTypeInfo(interpreter.Ast, "TestWriteCodeCase3");
            
            var codeLoader = new CodeLoader();
            var content = File.ReadAllText("../../../DataBindGen3.cs");
            var contentOutput = File.ReadAllText("../../../DataBindGen3.txt");
            codeLoader.ModifyCode(envInfo,typeof(TestWriteCodeCase3),content);
            
            var codeWriter = new CodeWriter();
            codeWriter.UnknownTypeMark = "object";
            var codeText = codeWriter.WriteCode(envInfo);
            expect(codeText).toBe(contentOutput);
        }
    }
}