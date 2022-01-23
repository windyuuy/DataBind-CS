
using DataBindService;
using UnityEditor.Callbacks;
using UnityEditor;

using System.IO;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Diagnostics;

namespace DataBinding.Editor.DataBindEntry
{
	public class MyCustomBuildProcessor0
	{
		public virtual void HandleDLLs(BuildReport report)
		{
		}

	}

	public class DataBindEntry : MyCustomBuildProcessor0, IPostBuildPlayerScriptDLLs
	{
		#region IPostBuildPlayerScriptDLLs
		public int callbackOrder { get { return 0; } }

		public override void HandleDLLs(BuildReport report)
		{
			var targets = report.files
				.Select(f => f.path)
				.Where(p => p.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase));
			BindEntry.SupportU3DDataBind(targets);
		}

		public virtual void OnPostBuildPlayerScriptDLLs(BuildReport report)
		{
			HandleDLLs(report);
		}

        #endregion

        public static bool HasSupport = false;
		[PostProcessBuild(1000)]
		private static void OnPostprocessBuildPlayer(BuildTarget buildTarget, string buildPath)
		{
			HasSupport = false;
		}

		[PostProcessScene]
		public static void SupportU3DDataBindPost()
		{
			if (HasSupport == true)
			{
				return;
			}
			HasSupport = true;

			SupportU3DDataBind();
		}
		[InitializeOnLoadMethod]
		public static void SupportU3DDataBind()
		{

			BindEntry.SupportU3DDataBind();
		}

	}

}
