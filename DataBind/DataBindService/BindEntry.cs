using CiLin;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Diagnostics.IO;

namespace DataBindService
{
	[System.Diagnostics.DebuggerStepThrough]
	public class BindOptions
	{
		public bool writeImmediate = true;
		public bool useSymbols = true;
		//public Action<AssemblyDefinition> onDone;
		public System.IO.FileStream readStream;
		public string outputPath;
	}

	public class BindEntry
	{

		//[System.Diagnostics.DebuggerStepThrough]
		public static void SupportU3DDataBind()
		{
			var filePaths = System.IO.Directory.GetFiles(@".\Library\ScriptAssemblies\", @"*.dll", System.IO.SearchOption.TopDirectoryOnly);
			SupportU3DDataBind(filePaths);
		}
		public static void SupportU3DDataBind(IEnumerable<string> filePaths)
		{
			var postTask = new PostTask();

			postTask.Clear();

			var validDlls = filePaths
				.Where(p => IsValidDllToSupport(p))
				.ToArray();
			var assmeblyList = new List<AssemblyDefinition>();
			var buildOptions = new BindOptions();
			foreach (var dllPath in validDlls)
			{
				try
				{
					var assembly = LoadAssembly(dllPath, buildOptions);
					assmeblyList.Add(assembly);
				}
				catch (Exception ex)
				{
					console.error(ex.ToString());
				}
			}
			foreach (var assembly in assmeblyList)
			{
				SupportDataBindInMemory(assembly, buildOptions, postTask);
			}
			foreach (var assembly in assmeblyList)
			{
				SupportDataBindPostTask(assembly, buildOptions, postTask, assmeblyList);
			}
			foreach (var assembly in assmeblyList)
			{
				SaveAssembly(assembly, buildOptions);
				assembly.Dispose();
			}

			postTask.Clear();
		}

		public static void SupportDataBindPostTask(AssemblyDefinition assembly, BindOptions options, PostTask postTask0, List<AssemblyDefinition> assmeblyList)
		{
			foreach (var refAssembly in assmeblyList)
			{
				foreach (var task in postTask0.field2PropInfos)
				{
					CILUtils.ReplaceFieldReferWithPropertyDef(refAssembly, task.fieldDef, task.propertyDef);
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
		public static AssemblyDefinition LoadAssembly(string inputPath, BindOptions options)
		{
			var useSymbols = options.useSymbols;

			var resolver = new DefaultAssemblyResolver();
			//resolver.AddSearchDirectory(@".");
			//resolver.AddSearchDirectory(@"bin");
			resolver.AddSearchDirectory(@".\Assets\Framework\Third\Demigiant\DOTween\");
			resolver.AddSearchDirectory(@".\TestProjects\CommobLibs\Managed\UnityEngine\");

			AssemblyDefinition assembly;

			try
			{
				assembly = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters()
				{
					ReadWrite = true,
					ReadSymbols = useSymbols,
					AssemblyResolver = resolver,
				});
			}
			catch (Exception ex)
			{
				assembly = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters()
				{
					ReadWrite = true,
					ReadSymbols = false,
					AssemblyResolver = resolver,
				});
			}

			return assembly;
		}

		public static void SupportDataBindInMemory(AssemblyDefinition assembly, BindOptions options, PostTask postTask0)
		{

			{
				var postTask = new PostTask();

				using var sys = AssemblyDefinition.ReadAssembly(typeof(void).Assembly.Location);
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

					// field auto -> property
#if true
					if (type.CustomAttributes.Any(attr => CILUtils.IsSameAttr(attr, AsPropertyAttr)))
					{
						DataBindTool.HandleAutoConvFieldToProperty(type, AsPropertyAttr, postTask);
					}
					else
					{
						DataBindTool.HandleAutoConvFieldToPropertySeperately(type, AsPropertyAttr, postTask);
					}
#endif

					if (type.CustomAttributes.Any(attr => CILUtils.IsSameAttr(attr, StdHostAttr)))
					{
						if (type.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, StdHostInterface)) == false)
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
					if (attrRecursive != null)
					{
						DataBindTool.HandleObservableRecursive(type, ObservableAttrTemp);
					}

				}

				postTask0.Merge(postTask);
			}

		}

		public static void SaveAssembly(AssemblyDefinition assembly, BindOptions options)
		{
			var useSymbols = options.useSymbols;

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

		public static void SupportDataBind(string assemblyPath, BindOptions options)
		{
			var postTask = new PostTask();
			using var assembly = LoadAssembly(assemblyPath, options);
			SupportDataBindInMemory(assembly, options, postTask);
			SupportDataBindPostTask(assembly, options, postTask, new List<AssemblyDefinition>() { assembly });
			SaveAssembly(assembly, options);
		}

	}
}
