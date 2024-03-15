using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

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

	public class U3DBindHelper
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
			var filePaths = System.IO.Directory.GetFiles(@".\Library\ScriptAssemblies\", @"*.dll",
				System.IO.SearchOption.TopDirectoryOnly);
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
				.Where(p => IsValidDllToSupportForU3D(p))
				.ToArray();
			var buildOptions = new BindOptions();
			// if (resolvePaths != null)
			{
				var resolver = new DefaultAssemblyResolver();
				//resolver.AddSearchDirectory(@".");
				//resolver.AddSearchDirectory(@"bin");
				// resolver.AddSearchDirectory(@".\Assets\Framework\Third\Demigiant\DOTween\");
				// resolver.AddSearchDirectory(@".\TestProjects\CommobLibs\Managed\UnityEngine\");
				// foreach (var resolvePath in resolvePaths)
				// {
				// 	resolver.AddSearchDirectory(resolvePath);
				// }
				resolver.AddSearchDirectory(@"E:\PROCS\IDESS\Unity\2022.3.16f1\Editor\Data\MonoBleedingEdge\lib\mono\unityjit-win32\");

				buildOptions.Resolver = resolver;
			}

			DataBindModifierHelper.SupportDataBind(validAssemblyPaths, buildOptions);
		}

		public static bool IsValidDllToSupportForU3D(string filePath)
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
	}
}