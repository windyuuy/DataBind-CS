using CiLin;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBindService
{
    public class InjectOptions
    {
        public bool writeImmediate = true;
        public bool useSymbols = true;
        public Action<AssemblyDefinition> onDone;
        public System.IO.FileStream readStream;
        public string outputPath;
    }

    public class BindEntry
    {
        public static void InjectDataBind(string inputPath,InjectOptions options)
        {
            var useSymbols = options.useSymbols;

            using (var assembly = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters()
            {
                ReadWrite = true,
                ReadSymbols = useSymbols,
            }))
            {
                {
                    var sys = AssemblyDefinition.ReadAssembly(typeof(void).Assembly.Location);
                    CILUtils.SysAssembly = sys;
                    DataBindTool.MainAssembly = assembly;
                    DataBindTool.SysAssembly = sys;

                    var types = assembly.MainModule.GetTypes();
                    var MarkAttr = assembly.MainModule.ImportReference(typeof(DataBinding.ObservableAttribute));
                    var MarkAttrCtor = assembly.MainModule.ImportReference(typeof(DataBinding.ObservableAttribute).GetConstructor(new Type[] { typeof(int) }));

                    var MarkInterface = assembly.MainModule.ImportReference(typeof(DataBinding.IHostStand));
                    var IntRef = assembly.MainModule.ImportReference(typeof(int));

                    foreach (var type in types)
                    {

                        if (type.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, MarkInterface)))
                        {
                            DataBindTool.HandleHost(type);

                            var attr2 = new CustomAttribute(MarkAttrCtor);
                            attr2.ConstructorArguments.Add(new CustomAttributeArgument(IntRef, 0));
                            DataBindTool.HandleObservable(type, attr2);
                            continue;
                        }

                        var attr = type.CustomAttributes.FirstOrDefault(c => CILUtils.IsSameAttr(c, MarkAttr));
                        if (attr != null)
                        {
                            DataBindTool.HandleObservable(type, attr);
                            continue;
                        }

                    }
                }

                if (options.onDone != null)
               {
                    options.onDone(assembly);
                }

                if (options.writeImmediate)
                {
                    if (options.outputPath != null)
                    {
                        assembly.Write(options.outputPath, new WriterParameters()
                        {
                            WriteSymbols = useSymbols,
                        });
                    }
                    else
                    {
                        assembly.Write(new WriterParameters()
                        {
                            WriteSymbols = useSymbols,
                        });
                    }
                }
            }
        }

    }
}
