using DataBinding;
using Game.Diagnostics;
using Mono.Cecil;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using Mono.Cecil.Cil;
using RunDataBindDemo;

namespace TestDataBind
{

    public class Tests
    {
        [Test]
        [Order(0)]
        public void Test22()
        {
            using (var assembly = AssemblyDefinition.ReadAssembly(@"E:\DATA\Projects\unity\parentclient\meta-parent-client-u3d\client\TestProjects\CSNetClient\RunDataBindDemo\bin\Debug\netcoreapp3.1\RunDataBindDemo.dll", new ReaderParameters()
            {
                //ReadSymbols = true,
                ReadingMode = ReadingMode.Immediate,
                //ReadWrite = true,
            }))
            {
                //if (false)
                {
                    var a = AssemblyDefinition.ReadAssembly(typeof(Tests).Assembly.Location);
                    var TString = typeof(string);
                    var rtstr = a.MainModule.ImportReference(TString);

                    var types = assembly.MainModule.GetTypes();
                    foreach (var type in types)
                    {
                        foreach (var Prop in type.Properties)
                        {
                            if (Prop.Name == "DoubleFV")
                            {
                                Debug.Log("start");
                                type.Fields.Add(new FieldDefinition("doubleFV", FieldAttributes.Public | FieldAttributes.HasDefault, rtstr));
                                var setFuncDefineRaw = Prop.SetMethod;
                                if (setFuncDefineRaw != null)
                                {
                                    var worker = setFuncDefineRaw.Body.GetILProcessor();
                                    var lastInst = setFuncDefineRaw.Body.Instructions.Last();

                                    var NotifyPropertyChanged = type.Methods.First(m => m.Name == "NotifyPropertyChanged");
                                    if (NotifyPropertyChanged != null)
                                    {
                                        var op1 = InsertBefore(worker, lastInst, worker.Create(OpCodes.Ldarg_0));
                                        var op2 = InsertBefore(worker, lastInst, worker.Create(OpCodes.Ldstr, "DoubleFV2"));
                                        var op3 = InsertBefore(worker, lastInst, worker.Create(OpCodes.Call, NotifyPropertyChanged));
                                        var op4 = InsertBefore(worker, lastInst, worker.Create(OpCodes.Nop));
                                    }
                                    ComputeOffsets(setFuncDefineRaw.Body);
                                }

                            }
                        }
                    }
                }

                var writerParameters = new WriterParameters { WriteSymbols = true };
                //assembly.Write(new WriterParameters()
                //{
                //    WriteSymbols = false,
                //});
                assembly.Name.Name = "RunDataBindDemoInjected";
                assembly.Write(@"E:\DATA\Projects\unity\parentclient\meta-parent-client-u3d\client\TestProjects\CSNetClient\RunDataBindDemo\bin\Debug\netcoreapp3.1\RunDataBindDemoInjected.dll", new WriterParameters()
                {
                    //WriteSymbols = true,
                });
                Debug.Log("donexx");
            }
        }

        private static Instruction InsertBefore(ILProcessor worker, Instruction target, Instruction instruction)
        {
            worker.InsertBefore(target, instruction);
            return instruction;
        }
        private static Instruction InsertAfter(ILProcessor worker, Instruction target, Instruction instruction)
        {
            worker.InsertAfter(target, instruction);
            return instruction;
        }

        private static void ComputeOffsets(MethodBody body)
        {
            var offset = 0;
            foreach (var instruction in body.Instructions)
            {
                instruction.Offset = offset;
                offset += instruction.GetSize();
            }
        }

        [Test]
        public void TestInjected()
        {
            var target = new TSampleTarget();
            var notifyTimes = 0;
            target.PropertyChanged += (s, e) =>
            {
                notifyTimes++;
            };
            target.DoubleFV = 234;
            Assert.AreEqual(1, notifyTimes);
        }
    }
}