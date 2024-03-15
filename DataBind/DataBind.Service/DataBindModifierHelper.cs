using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CiLin;
using Mono.Cecil;
using Console = EngineAdapter.Diagnostics.Console;

namespace DataBind.Service
{
	public static class DataBindModifierHelper
	{
		public static void SupportDataBind(string[] assemblyPaths, BindOptions buildOptions)
		{
			var postTask = new PostTask();
			postTask.Clear();

			SupportDataBind(assemblyPaths, buildOptions, postTask);

			postTask.Clear();
		}


		public static void HandleDataBindPostTask(AssemblyDefinition assembly, BindOptions options,
			PostTask postTask0, ref bool isAnyChanged)
		{
			var anyReferExist = false;
			if (postTask0.field2PropInfos.Count > 0)
			{
				var field2PropInfos = postTask0.field2PropInfos.Select(info => (info.fieldDef, info.propertyDef)).ToArray();
				anyReferExist |= CILUtils.ReplaceFieldReferWithPropertyDef(assembly, field2PropInfos);
			}

			if (anyReferExist)
			{
				isAnyChanged = true;
			}
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

		public static void SupportDataBindInMemory(AssemblyDefinition assembly, BindOptions options, PostTask postTask0, ref bool isAnyChanged)
		{
			{
				var postTask = new PostTask();

				using var sys = AssemblyDefinition.ReadAssembly(typeof(void).Assembly.Location);
				CILUtils.SysAssembly = sys;
				DataBindTool.MainAssembly = assembly;
				DataBindTool.SysAssembly = sys;

				var types = assembly.MainModule.GetTypes();
				var MarkAttr = assembly.MainModule.ImportReference(typeof(DataBind.ObservableAttribute));
				var MarkAttrCtor = assembly.MainModule.ImportReference(
					typeof(DataBind.ObservableAttribute).GetConstructor(new Type[] { typeof(int) }));
				var MarkRecursiveAttr =
					assembly.MainModule.ImportReference(typeof(DataBind.ObservableRecursiveAttribute));

				var StdHostInterface = assembly.MainModule.ImportReference(typeof(DataBind.IStdHost));
				var IntRef = assembly.MainModule.ImportReference(typeof(int));
				var StdHostAttr = assembly.MainModule.ImportReference(typeof(DataBind.StdHostAttribute));

				// 模板
				var ObservableAttrTemp = new CustomAttribute(MarkAttrCtor);
				ObservableAttrTemp.ConstructorArguments.Add(new CustomAttributeArgument(IntRef, 1));

				var AsPropertyAttr =
					assembly.MainModule.ImportReference(typeof(DataBind.AutoFieldPropertyAttribute));

				foreach (var type in types)
				{
					// field auto -> property
#if true
					if (type.CustomAttributes.Any(attr => CILUtils.IsSameAttr(attr, AsPropertyAttr)))
					{
						isAnyChanged = true;
						DataBindTool.HandleAutoConvFieldToProperty(type, AsPropertyAttr, postTask);
					}
					else
					{
						DataBindTool.HandleAutoConvFieldToPropertySeperately(type, AsPropertyAttr, postTask, ref isAnyChanged);
					}
#endif

					if (
						type.CustomAttributes.Any(attr =>
							CILUtils.IsSameAttr(attr, StdHostAttr)
							)) 
					{
						if (type.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, StdHostInterface)) ==
						    false)
						{
							isAnyChanged = true;
							CILUtils.InjectInteface(assembly, type, StdHostInterface);
						}
					}

					if(CILUtils.FindInterface(type, StdHostInterface)!=null)
					// if (type.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, StdHostInterface)))
					{
						DataBindTool.HandleHost(type, ref isAnyChanged);

						DataBindTool.HandleObservable(type, ObservableAttrTemp, ref isAnyChanged);
					}
					else
					{
						var attr = type.CustomAttributes.FirstOrDefault(c => CILUtils.IsSameAttr(c, MarkAttr));
						if (attr != null)
						{
							isAnyChanged = true;

							DataBindTool.HandleObservable(type, ObservableAttrTemp, ref isAnyChanged);
						}
					}

					var attrRecursive =
						type.CustomAttributes.FirstOrDefault(c => CILUtils.IsSameAttr(c, MarkRecursiveAttr));
					if (attrRecursive != null)
					{
						DataBindTool.HandleObservableRecursive(type, ObservableAttrTemp, ref isAnyChanged);
					}
				}

				postTask0.Merge(postTask);
			}
		}

		public static void SafeWriteAssembly(AssemblyDefinition assembly, string fileName,
			WriterParameters parameters)
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
				catch (Exception ex2)
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
				// var referx=assembly.MainModule.ImportReference(typeof(TupleElementNamesAttribute));
				// var referxCtor = assembly.MainModule.ImportReference(
				// 	typeof(TupleElementNamesAttribute).GetConstructor(new Type[] { typeof(string[]) }));
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
				catch (Exception ex2)
				{
					Console.Exception(ex);
					Console.Exception(ex2);
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
			using var assembly = new AssemblyDataBindModifier();
			assembly.LoadAssembly(assemblyPath, options);
			assembly.SupportDataBindInMemory(options, postTask);
			assembly.HandleDataBindPostTask(options, postTask);
			assembly.SaveAssembly(options);
		}

		public static void SupportDataBind(string[] assemblyPaths, BindOptions buildOptions, PostTask postTask)
		{
			var modifiers = new List<AssemblyDataBindModifier>();
			foreach (var dllPath in assemblyPaths)
			{
				try
				{
					var assembly = new AssemblyDataBindModifier();
					assembly.LoadAssembly(dllPath, buildOptions);
					modifiers.Add(assembly);
				}
				catch (Exception ex)
				{
					Console.Error($"LoadAssembly-Exception: {dllPath}");
					Console.Exception(ex);
				}
			}

			foreach (var assembly in modifiers)
			{
				try
				{
					assembly.SupportDataBindInMemory(buildOptions, postTask);
				}
				catch (Exception ex)
				{
					Console.Error($"SupportDataBindInMemory-Exception: {assembly.FullName}");
					Console.Exception(ex);
				}
			}

			foreach (var assembly in modifiers)
			{
				try
				{
					assembly.HandleDataBindPostTask(buildOptions, postTask);
				}
				catch (Exception ex)
				{
					Console.Error($"SupportDataBindPostTask-Exception: {assembly.FullName}");
					Console.Exception(ex);
				}
			}

			foreach (var assembly in modifiers)
			{
				try
				{
					assembly.SaveAssembly(buildOptions);
				}
				catch (Exception ex)
				{
					Console.Error($"SaveAssembly-Exception: {assembly.FullName}");
					Console.Exception(ex);
				}
			}

			foreach (var assembly in modifiers)
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
	}
}