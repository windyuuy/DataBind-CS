using Mono.Cecil;
using NUnit.Framework;
using System;
using System.Linq;
using CiLin;

using Debug = Game.Diagnostics.Debug;
using DataBindService;

namespace TestWithInjected
{
    public class Tests
    {
#if true
        [Test]
        public void Test22()
        {
            var useSymbols = false;
            using (var assembly = AssemblyDefinition.ReadAssembly(@"E:\DATA\Codes\DataBind\DataBind\RunDataBindDemo\bin\Debug\net472\RunDataBindDemo.dll", new ReaderParameters()
            {
                ReadSymbols = useSymbols,
                //ReadingMode = ReadingMode.Immediate,
                //ReadWrite = true,
            }))
            {
                //if (false)
                {
                    var a = AssemblyDefinition.ReadAssembly(typeof(Tests).Assembly.Location);
                    var TString = typeof(string);
                    var rtstr = a.MainModule.ImportReference(TString);

                    var db = AssemblyDefinition.ReadAssembly(typeof(vm.IObservable).Assembly.Location);
                    var tIObservable = db.MainModule.ImportReference(typeof(RunDataBindDemo.ITest));
                    var types = assembly.MainModule.GetTypes();

                    var rd=AssemblyDefinition.ReadAssembly(typeof(RunDataBindDemo.Demo).Assembly.Location);
                    var tDemo = assembly.MainModule.GetType(typeof(RunDataBindDemo.Demo).FullName);

                    //var corlibReference = new AssemblyNameReference("System.Runtime", new Version(4, 2, 1, 0))
                    //{
                    //    PublicKeyToken = new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }
                    //};
                    //assembly.MainModule.AssemblyReferences.Add(corlibReference);
                    //var sys = AssemblyDefinition.ReadAssembly(typeof(System.Delegate).Assembly.Location);
                    var sys = AssemblyDefinition.ReadAssembly("E:/DATA/Projects/unity/parentclient/meta-parent-client-u3d/client/Library/Bee/Android/Prj/Mono2x/Gradle/unityLibrary/src/main/assets/bin/Data/Managed/mscorlib.dll");
                    //sys.Name = new AssemblyNameDefinition("System.Runtime", sys.Name.Version);
                    //sys.MainModule.Name = "System.Runtime.dll";
                    CILUtils.SysAssembly = sys;
                    //sys.MainModule.AssemblyReferences.Add(corlibReference);

                    var tPropertyGetEventHandler = db.MainModule.ImportReference(typeof(vm.PropertyGetEventHandler));
                    foreach (var type in types)
                    {
                        foreach (var Prop in type.Properties)
                        {
                            if (Prop.Name == "DoubleFV")
                            {
                                Debug.Log("start");

                                var inter = new InterfaceImplementation(tIObservable);
                                type.Interfaces.Add(inter);

                                CILUtils.InjectProperty(assembly, type, "CCC", typeof(double));
                                CILUtils.InjectProperty(assembly, type, "SS", typeof(string));
                                CILUtils.InjectEvent(assembly, type, "PropertyGet2", typeof(vm.PropertyGetEventHandler));
                                break;

                                //var DemoP = tDemo.Properties.First(p => p.Name == "CCC");
                                //var pp = new PropertyDefinition("CCC", DemoP.Attributes, DemoP.PropertyType);
                                //pp.GetMethod = new MethodDefinition(DemoP.GetMethod.Name, DemoP.GetMethod.Attributes | MethodAttributes.Final | MethodAttributes.Virtual, DemoP.GetMethod.ReturnType);
                                //pp.SetMethod = new MethodDefinition(DemoP.SetMethod.Name, DemoP.SetMethod.Attributes | MethodAttributes.Final | MethodAttributes.Virtual, DemoP.GetMethod.ReturnType);
                                //pp.GetMethod = DemoP.GetMethod;
                                //pp.SetMethod = DemoP.SetMethod;
                                //type.Properties.Add(pp);

                                //type.Fields.Add(new FieldDefinition("doubleFV", FieldAttributes.Public | FieldAttributes.HasDefault, rtstr));
                                //var setFuncDefineRaw = Prop.SetMethod;
                                //if (setFuncDefineRaw != null)
                                //{
                                //    var worker = setFuncDefineRaw.Body.GetILProcessor();
                                //    var lastInst = setFuncDefineRaw.Body.Instructions.Last();

                                //    var NotifyPropertyChanged = type.Methods.First(m => m.Name == "NotifyPropertyChanged");
                                //    if (NotifyPropertyChanged != null)
                                //    {
                                //        var op1 = InsertBefore(worker, lastInst, worker.Create(OpCodes.Ldarg_0));
                                //        var op2 = InsertBefore(worker, lastInst, worker.Create(OpCodes.Ldstr, "DoubleFV2"));
                                //        var op3 = InsertBefore(worker, lastInst, worker.Create(OpCodes.Call, NotifyPropertyChanged));
                                //        var op4 = InsertBefore(worker, lastInst, worker.Create(OpCodes.Nop));
                                //    }
                                //    ComputeOffsets(setFuncDefineRaw.Body);
                                //}

                            }
                        }
                    }
                }

                var writerParameters = new WriterParameters { WriteSymbols = true };
                //assembly.Write(new WriterParameters()
                //{
                //    WriteSymbols = false,
                //});
                //assembly.Name.Name = "RunDataBindDemoInjected";
                assembly.Write(@"E:\DATA\Codes\DataBind\DataBind\TestWithInjected\bin\Debug\net472\RunDataBindDemo.dll", new WriterParameters()
                {
                    WriteSymbols = useSymbols,
                });
                Debug.Log("donexx");
            }
        }
#endif

        [Test]
        public void Test23()
        {
            var useSymbols = false;
            using (var assembly = AssemblyDefinition.ReadAssembly(@"E:\DATA\Codes\DataBind\DataBind\RunDataBindDemo\bin\Debug\net472\RunDataBindDemo.dll", new ReaderParameters()
            {
                ReadSymbols = useSymbols,
                //ReadingMode = ReadingMode.Immediate,
                //ReadWrite = true,
            }))
            {
                //if (false)
                {
                    var sys = AssemblyDefinition.ReadAssembly(typeof(void).Assembly.Location);
                    CILUtils.SysAssembly = sys;

                    var types = assembly.MainModule.GetTypes();

                    foreach (var type in types)
                    {
                        foreach (var Prop in type.Properties)
                        {
                            if (Prop.Name == "DoubleFV")
                            {
                                Console.Out.WriteLine("start");

                                CILUtils.InjectProperty(assembly, type, "CCC", typeof(double));
                                break;

                            }
                        }
                    }
                }

                assembly.Write(@"E:\DATA\Codes\DataBind\DataBind\TestWithInjected\bin\Debug\net472\RunDataBindDemo.dll", new WriterParameters()
                {
                    WriteSymbols = useSymbols,
                });
                Console.Out.WriteLine("donex22x");
            }
        }

        [Test]
        public void TestInjectDataBind()
        {
            var useSymbols = false;
            using (var assembly = AssemblyDefinition.ReadAssembly(@"E:\DATA\Codes\DataBind\DataBind\RunDataBindDemo\bin\Debug\net472\RunDataBindDemo.dll", new ReaderParameters()
            {
                ReadSymbols = useSymbols,
                //ReadingMode = ReadingMode.Immediate,
                //ReadWrite = true,
            }))
            {
                //if (false)
                {
                    var sys = AssemblyDefinition.ReadAssembly(typeof(void).Assembly.Location);
                    CILUtils.SysAssembly = sys;
                    DataBindTool.MainAssembly = assembly;
                    DataBindTool.SysAssembly = sys;

                    var types = assembly.MainModule.GetTypes();
                    var MarkAttr=assembly.MainModule.ImportReference(typeof(DataBinding.ObservableAttribute));

                    var MarkInterface = assembly.MainModule.ImportReference(typeof(vm.IHostStand));

                    foreach (var type in types)
                    {

                        if (type.Interfaces.Any(inter => CILUtils.IsSameInterface(inter,MarkInterface)))
                        {
                            DataBindTool.HandleHost(type);
                            continue;
                        } 

                        var attr=type.CustomAttributes.FirstOrDefault(c=>CILUtils.IsSameAttr(c,MarkAttr));
                        if(attr != null)
                        {
                            DataBindTool.HandleObservable(type,attr);
                            continue;
                        }

                    }
                }

                assembly.Write(@"E:\DATA\Codes\DataBind\DataBind\TestWithInjected\bin\Debug\net472\RunDataBindDemo.dll", new WriterParameters()
                {
                    WriteSymbols = useSymbols,
                });
                Console.Out.WriteLine("donex22x");
            }
        }

    }
}