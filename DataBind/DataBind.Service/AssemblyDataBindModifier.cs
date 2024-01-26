using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace DataBind.Service
{
	public class AssemblyDataBindModifier : IDisposable
	{
		public AssemblyDefinition Assembly;
		protected bool IsAnyChanged;
		public string FullName => Assembly.FullName;

		public void LoadAssembly(string inputPath, BindOptions options)
		{
			Assembly = DataBindModifierHelper.LoadAssembly(inputPath, options);
		}

		public void SupportDataBindInMemory(BindOptions options,
			PostTask postTask0)
		{
			DataBindModifierHelper.SupportDataBindInMemory(Assembly, options, postTask0, ref IsAnyChanged);
		}

		public void HandleDataBindPostTask(BindOptions options,
			PostTask postTask0)
		{
			DataBindModifierHelper.HandleDataBindPostTask(Assembly, options, postTask0, ref IsAnyChanged);
		}

		public void SaveAssembly(BindOptions options)
		{
			if (IsAnyChanged)
			{
				DataBindModifierHelper.SaveAssembly(Assembly, options);
			}
		}

		public void Dispose()
		{
			Assembly.Dispose();
			Assembly = null;
		}
	}
}