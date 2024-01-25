using CiLin;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Console = Game.Diagnostics.IO.Console;

namespace DataBind.Service
{
	[System.Diagnostics.DebuggerStepThrough]
	public class BindOptions
	{
		public bool WriteImmediate = true;
		public bool UseSymbols = true;
		//public Action<AssemblyDefinition> onDone;
		// public System.IO.FileStream readStream;
		public string OutputPath;
		public IAssemblyResolver Resolver;
	}

	public class BindEntry
	{
#if _DEBUG
		[System.Diagnostics.DebuggerStepThrough]
#endif
		public static void SupportU3DDataBindInLibrary()
		{
			SupportU3DDataBindInLibrary(null);
		}
#if _DEBUG
		[System.Diagnostics.DebuggerStepThrough]
#endif
		public static void SupportU3DDataBindInLibrary(IEnumerable<string> resolvePaths)
		{
			var filePaths = System.IO.Directory.GetFiles(@".\Library\ScriptAssemblies\", @"*.dll", System.IO.SearchOption.TopDirectoryOnly);
			// resolvePaths ??= new string[]
			// {
			// 	@".\Assets\Framework\Third\Demigiant\DOTween\",
			// 	@".\TestProjects\CommobLibs\Managed\UnityEngine\",
			// };
			SupportU3DDataBind(filePaths, resolvePaths);
		}
		public static void SupportU3DDataBind(IEnumerable<string> filePaths, IEnumerable<string> resolvePaths = null)
		{
			var validAssemblyPaths = filePaths
				.Where(p => IsValidDllToSupport(p))
				.ToArray();
			var buildOptions = new BindOptions();
			if (resolvePaths != null)
			{
				var resolver = new DefaultAssemblyResolver();
				//resolver.AddSearchDirectory(@".");
				//resolver.AddSearchDirectory(@"bin");
				// resolver.AddSearchDirectory(@".\Assets\Framework\Third\Demigiant\DOTween\");
				// resolver.AddSearchDirectory(@".\TestProjects\CommobLibs\Managed\UnityEngine\");
				foreach (var resolvePath in resolvePaths)
				{
					resolver.AddSearchDirectory(resolvePath);
				}

				buildOptions.Resolver = resolver;
			}
			
			SupportDataBind(validAssemblyPaths, buildOptions);
		}

		public static void SupportDataBind(string[] assemblyPaths, BindOptions buildOptions)
		{
			var postTask = new PostTask();
			postTask.Clear();
			
			SupportDataBind(assemblyPaths, buildOptions, postTask);

			postTask.Clear();
		}

		public static void SupportDataBind(string[] assemblyPaths, BindOptions buildOptions, PostTask postTask)
		{
			var assemblyList = new List<AssemblyDefinition>();
			foreach (var dllPath in assemblyPaths)
			{
				try
				{
					var assembly = LoadAssembly(dllPath, buildOptions);
					assemblyList.Add(assembly);
				}
				catch (Exception ex)
				{
					Console.Error($"LoadAssembly-Exception: {dllPath}");
					Console.Exception(ex);
				}
			}
			foreach (var assembly in assemblyList)
			{
				try
				{
					SupportDataBindInMemory(assembly, buildOptions, postTask);
				}
				catch (Exception ex)
				{
					Console.Error($"SupportDataBindInMemory-Exception: {assembly.FullName}");
					Console.Exception(ex);
				}
			}
			foreach (var assembly in assemblyList)
			{
				try
				{
					HandleDataBindPostTask(assembly, buildOptions, postTask, assemblyList);
				}
				catch (Exception ex)
				{
					Console.Error($"SupportDataBindPostTask-Exception: {assembly.FullName}");
					Console.Exception(ex);
				}
			}
			foreach (var assembly in assemblyList)
			{
				try
				{
					SaveAssembly(assembly, buildOptions);
				}
				catch (Exception ex)
				{
					Console.Error($"SaveAssembly-Exception: {assembly.FullName}");
					Console.Exception(ex);
				}
			}
			foreach (var assembly in assemblyList)
			{
				try
				{
					assembly.Dispose();
				}
				catch (Exception ex)
				{
					Console.Error($"Dispose-Exception: {assembly.FullName}");
					Console.Exception(ex);
				}
			}
		}

		public static void HandleDataBindPostTask(AssemblyDefinition assembly, BindOptions options, PostTask postTask0, List<AssemblyDefinition> assmeblyList)
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
					|| filePath == "CiLin"
					|| filePath == "EngineAdapter"
					)
			{
				return false;
			}
			return true;
		}

		//[System.Diagnostics.DebuggerStepThrough]
		public static AssemblyDefinition LoadAssembly(string inputPath, BindOptions options)
		{
			var useSymbols = options.UseSymbols;

			var resolver = options.Resolver;
			
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

		public static void SafeWriteAssembly(AssemblyDefinition assembly, string fileName, WriterParameters parameters)
		{
			try
			{
				assembly.Write(fileName, parameters);
			}
			catch (Exception ex)
			{
				try
				{
					parameters.WriteSymbols = false;
					assembly.Write(fileName, parameters);
					Console.Warn($"Load-Assembly Without-Symbol-Only: {assembly.FullName} -> {fileName}");
				}
				catch(Exception ex2)
				{
					Console.Exception(ex);
					throw;
				}
			}
		}

		public static void SafeWriteAssembly(AssemblyDefinition assembly, WriterParameters parameters)
		{
			try
			{
				assembly.Write(parameters);
			}
			catch (Exception ex)
			{
				try
				{
					parameters.WriteSymbols = false;
					assembly.Write(parameters);
					Console.Warn($"Load-Assembly Without-Symbol-Only: {assembly.FullName}");
				}
				catch(Exception ex2)
				{
					Console.Exception(ex);
					throw;
				}
			}
		}
		
		public static void SaveAssembly(AssemblyDefinition assembly, BindOptions options)
		{
			var useSymbols = options.UseSymbols;

			if (options.WriteImmediate)
			{
				if (options.OutputPath != null)
				{
					SafeWriteAssembly(assembly, options.OutputPath, new WriterParameters()
					{
						WriteSymbols = useSymbols,
					});
				}
				else
				{
					SafeWriteAssembly(assembly, new WriterParameters()
					{
						WriteSymbols = useSymbols,
					});
				}
			}
		}

		public static void SupportDataBind(string assemblyPath, BindOptions options)
		{
			var postTask = new PostTask();
			SupportDataBind(assemblyPath, options, postTask);
			postTask.Clear();
		}

		public static void SupportDataBind(string assemblyPath, BindOptions options, PostTask postTask)
		{
			using var assembly = LoadAssembly(assemblyPath, options);
			SupportDataBindInMemory(assembly, options, postTask);
			HandleDataBindPostTask(assembly, options, postTask, new List<AssemblyDefinition>() { assembly });
			SaveAssembly(assembly, options);
		}
	}
}
