using System;
using DataBind.Service;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DataBind.Editor.DataBindEntry
{
	public class U3DDataBindEntry : IPostBuildPlayerScriptDLLs
	{
		#region IPostBuildPlayerScriptDLLs

		public virtual int callbackOrder
		{
			get { return 0; }
		}

		public virtual void HandleDLLs(BuildReport report)
		{
			// 使用反射绕开 unity 无法自动升级API报错
			// var files = report.files
			BuildFile[] files;
			var getFilesMethod = report.GetType().GetMethod("GetFiles");
			if (getFilesMethod != null)
			{
				files = (BuildFile[])getFilesMethod.Invoke(report, Array.Empty<object>());
			}
			else
			{
				var filesProperty = report.GetType().GetProperty("files");
				files = (BuildFile[])filesProperty!.GetValue(report);
			}

			var targets = files
				.Select(f => f.path)
				.Where(p => p.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase));
			U3DBindHelper.SupportU3DDataBind(targets);
		}

		public virtual void OnPostBuildPlayerScriptDLLs(BuildReport report)
		{
			if (!IsEnable)
			{
				return;
			}

			HandleDLLs(report);
		}

		#endregion

		public static bool IsEnable
		{
			get
			{
				return PlayerPrefs.GetInt("DataBind.Editor::DataBindEntry.IsEnable", 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("DataBind.Editor::DataBindEntry.IsEnable", value ? 1 : 0);
			}
		}
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
			if (!IsEnable)
			{
				return;
			}

			U3DBindHelper.SupportU3DDataBindInLibrary();
		}
	}
}