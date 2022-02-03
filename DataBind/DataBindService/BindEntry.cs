using CiLin;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBindService
{
    [System.Diagnostics.DebuggerStepThrough]
    public class BindOptions
	{
		public bool writeImmediate = true;
		public bool useSymbols = true;
		public Action<AssemblyDefinition> onDone;
		public System.IO.FileStream readStream;
		public string outputPath;
	}

	public class BindEntry
	{
        //[System.Diagnostics.DebuggerStepThrough]
        public static void SupportU3DDataBind()
		{
			var filePaths = System.IO.Directory.GetFiles(@".\Library\ScriptAssemblies\", @"*.dll", System.IO.SearchOption.TopDirectoryOnly);
			var validDlls=filePaths
				.Where(p=>IsValidDllToSupport(p))
				.ToArray();
			foreach (var dllPath in validDlls)
			{
				SupportDataBind(dllPath, new BindOptions());
			}
		}
		public static void SupportU3DDataBind(IEnumerable<string> filePaths)
		{
			var validDlls=filePaths
				.Where(p=>IsValidDllToSupport(p))
				.ToArray();
			foreach (var dllPath in validDlls)
			{
                try
                {
					SupportDataBind(dllPath, new BindOptions());
				}
				catch(Exception ex)
                {
					console.error(ex.ToString());
                }
			}
		}

		public static bool IsValidDllToSupport(string filePath)
        {
			if (
					filePath.Contains("Unity.")
					|| filePath.Contains("UnityEngine.")
					|| filePath.Contains("UnityEditor.")
                    || filePath.StartsWith("DataBind.")
                    || filePath.Contains(".Editor.")
					|| filePath.Contains(".Cecil.")
					)
			{
				return false;
			}
			return true;
		}

        //[System.Diagnostics.DebuggerStepThrough]
        public static void SupportDataBind(string inputPath, BindOptions options)
		{
			var useSymbols = options.useSymbols;

            try
			{
				using var assembly = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters()
				{
					ReadWrite = true,
					ReadSymbols = useSymbols,
				});
				SupportDataBind(assembly, options);
			}
			catch (Exception ex)
            {
				using var assembly = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters()
				{
					ReadWrite = true,
					ReadSymbols = false,
				});
				options.useSymbols = false;
				SupportDataBind(assembly, options);
				options.useSymbols = useSymbols;
			}

		}

		public static void SupportDataBind(AssemblyDefinition assembly, BindOptions options)
		{
			var useSymbols = options.useSymbols;

			{
				var sys = AssemblyDefinition.ReadAssembly(typeof(void).Assembly.Location);
				CILUtils.SysAssembly = sys;
				DataBindTool.MainAssembly = assembly;
				DataBindTool.SysAssembly = sys;

				var types = assembly.MainModule.GetTypes();
				var MarkAttr = assembly.MainModule.ImportReference(typeof(DataBinding.ObservableAttribute));
				var MarkAttrCtor = assembly.MainModule.ImportReference(typeof(DataBinding.ObservableAttribute).GetConstructor(new Type[] { typeof(int) }));
				var MarkRecursiveAttr = assembly.MainModule.ImportReference(typeof(DataBinding.ObservableRecursiveAttribute));

				var StdHostInterface = assembly.MainModule.ImportReference(typeof(DataBinding.IStdHost));
				var IntRef = assembly.MainModule.ImportReference(typeof(int));
				var StdHostAttr = assembly.MainModule.ImportReference(typeof(DataBinding.StdHostAttribute));

				// 模板
				var ObservableAttrTemp = new CustomAttribute(MarkAttrCtor);
				ObservableAttrTemp.ConstructorArguments.Add(new CustomAttributeArgument(IntRef, 1));

				var AsPropertyAttr = assembly.MainModule.ImportReference(typeof(DataBinding.AutoFieldPropertyAttribute));

				foreach (var type in types)
				{

					if(type.CustomAttributes.Any(attr=>CILUtils.IsSameAttr(attr, AsPropertyAttr)))
                    {
						DataBindTool.HandleAutoConvFieldToProperty(type,AsPropertyAttr);
                    }
                    else
                    {
						DataBindTool.HandleAutoConvFieldToPropertySeperately(type,AsPropertyAttr);
					}

					if(type.CustomAttributes.Any(attr=>CILUtils.IsSameAttr(attr, StdHostAttr)))
                    {
						if(type.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, StdHostInterface)) == false)
                        {
							CILUtils.InjectInteface(assembly, type, StdHostInterface);
						}
					}

					if (type.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, StdHostInterface)))
					{
						DataBindTool.HandleHost(type);

						DataBindTool.HandleObservable(type, ObservableAttrTemp);
                    }
                    else
                    {
						var attr = type.CustomAttributes.FirstOrDefault(c => CILUtils.IsSameAttr(c, MarkAttr));
						if (attr != null)
						{
							DataBindTool.HandleObservable(type, ObservableAttrTemp);
						}
					}

					var attrRecursive = type.CustomAttributes.FirstOrDefault(c => CILUtils.IsSameAttr(c, MarkRecursiveAttr));
					if(attrRecursive != null)
                    {
						DataBindTool.HandleObservableRecursive(type, ObservableAttrTemp);
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
