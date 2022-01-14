using System;
using System.Linq;
using Mono.Cecil;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using CiLin;

namespace DataBindService
{
	public class DataBindTool
	{
		public static AssemblyDefinition MainAssembly;
		public static AssemblyDefinition SysAssembly;
		public static void HandleHost(TypeDefinition typeDefinition)
        {
            var DataBindAssembly = AssemblyDefinition.ReadAssembly(typeof(DataBindService.DBRuntimeDemo).Assembly.Location);

			var IFullHostRef = MainAssembly.MainModule.ImportReference(typeof(vm.IFullHost));
			var IHostRef = MainAssembly.MainModule.ImportReference(typeof(vm.IHost));
			if(typeDefinition.Interfaces.Any(inter=>CILUtils.IsSameInterface(inter, IHostRef)))
            {
				return;
            }
			if (typeDefinition.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, IFullHostRef)))
			{
				return;
			}

			#region implement IFullHost
			var VoidRef = MainAssembly.MainModule.ImportReference(typeof(void));
			var BoolRef =MainAssembly.MainModule.ImportReference(typeof(bool));
			var TWatcherCollection = typeof(System.Collections.Generic.ICollection<vm.Watcher>);
			var IWatcherCollectionRef = MainAssembly.MainModule.ImportReference(TWatcherCollection);
			var TDefaultValueType = typeof(System.Collections.Generic.List<vm.Watcher>);
			var WatcherRef=MainAssembly.MainModule.ImportReference(typeof (vm.Watcher));
			//var CombineTypeRef = MainAssembly.MainModule.ImportReference(typeof(vm.CombineType<object, string, Func<object, object, object>>));
			//var CallbackRef = MainAssembly.MainModule.ImportReference(typeof(Action<object, object, object>));
			//         var CombineTypeRef2 = MainAssembly.MainModule.ImportReference(typeof(vm.CombineType<object, string, number, boolean>));

			CILUtils.InjectProperty(MainAssembly, typeDefinition, "_SIsDestroyed", BoolRef);
            CILUtils.InjectField(MainAssembly, typeDefinition, "_Swatchers", IWatcherCollectionRef, FieldAttributes.Family);
			CILUtils.InjectGetOrCreateObjectMethod(MainAssembly, typeDefinition, "GetWatchers", "_Swatchers", IWatcherCollectionRef, TDefaultValueType);

            #endregion
        }

        public static void HandleObservable(TypeDefinition typeDefinition, CustomAttribute attr)
		{
			var needInject = false;

			{
				var item = attr.ConstructorArguments[0];
				if (item.Value.Equals(0))
				{
					needInject = true;
				}
			}

			if (!needInject)
			{
				return;
			}

            #region implement IObservable
            var DataBindAssembly = AssemblyDefinition.ReadAssembly(typeof(DataBindService.DBRuntimeDemo).Assembly.Location);

			var ObserverRef = MainAssembly.MainModule.ImportReference(typeof(vm.Observer));

			CILUtils.InjectField(MainAssembly, typeDefinition, "___Sob__", ObserverRef, FieldAttributes.Family);
			CILUtils.InjectGetFieldMethod(MainAssembly, typeDefinition, "_SgetOb", "___Sob__", ObserverRef);
			CILUtils.InjectSetFieldMethod(MainAssembly, typeDefinition, "_SsetOb", "___Sob__", ObserverRef);

			CILUtils.InjectEvent(MainAssembly, typeDefinition, "PropertyGot", typeof(vm.PropertyGetEventHandler));
			CILUtils.InjectEvent(MainAssembly, typeDefinition, "PropertyChanged", typeof(vm.PropertyChangedEventHandler));

			var RuntimeDemoRef = MainAssembly.MainModule.ImportReference(typeof(DataBindService.DBRuntimeDemo));
			var RuntimeDemoDef = DataBindAssembly.MainModule.Types.First(t => t != null && t.Namespace == RuntimeDemoRef.Namespace && t.FullName == RuntimeDemoRef.FullName);
			var NotifyPropertyGotMethod = RuntimeDemoDef.Methods.First(m => m.Name == "NotifyPropertyGot");
			var NotifyPropertyChangedMethod = RuntimeDemoDef.Methods.First(m => m.Name == "NotifyPropertyChanged");
			var GetMethodNotify = CILUtils.CopyMethod(MainAssembly, typeDefinition, "NotifyPropertyGot", RuntimeDemoDef, NotifyPropertyGotMethod);
			var SetMethodNotify = CILUtils.CopyMethod(MainAssembly, typeDefinition, "NotifyPropertyChanged", RuntimeDemoDef, NotifyPropertyChangedMethod);

			{
				var eventField = typeDefinition.Fields.First(f => f.Name == "PropertyGot");
				var PropertyGetEventArgsCtor = MainAssembly.MainModule.ImportReference(typeof(vm.PropertyGetEventArgs).GetConstructor(new Type[] { typeof(string), typeof(object) }));
				var PropertyGetEventHandler = MainAssembly.MainModule.ImportReference(typeof(vm.PropertyGetEventHandler).GetMethod("Invoke"));

				GetMethodNotify.Body.Instructions.Clear();
				var worker = GetMethodNotify.Body.GetILProcessor();
				worker.Append(worker.Create(OpCodes.Nop));
				worker.Append(worker.Create(OpCodes.Ldarg_0));
				worker.Append(worker.Create(OpCodes.Ldfld, eventField));
				worker.Append(worker.Create(OpCodes.Dup));
				var inst1 = worker.Create(OpCodes.Ldarg_0);
				worker.Append(worker.Create(OpCodes.Brtrue, inst1));
				worker.Append(worker.Create(OpCodes.Pop));
				var inst2 = worker.Create(OpCodes.Ret);
				worker.Append(worker.Create(OpCodes.Br_S, inst2));
				worker.Append(inst1);
				worker.Append(worker.Create(OpCodes.Ldarg_2));
				worker.Append(worker.Create(OpCodes.Ldarg_1));
				worker.Append(worker.Create(OpCodes.Newobj, PropertyGetEventArgsCtor));
				worker.Append(worker.Create(OpCodes.Callvirt, PropertyGetEventHandler));
				worker.Append(worker.Create(OpCodes.Nop));
				worker.Append(inst2);

			}
			{
				var eventField = typeDefinition.Fields.First(f => f.Name == "PropertyChanged");
				var PropertyChangeEventArgsCtor = MainAssembly.MainModule.ImportReference(typeof(vm.PropertyChangedEventArgs).GetConstructor(new Type[] { typeof(string), typeof(object), typeof(object) }));
				var PropertyChangeEventHandler = MainAssembly.MainModule.ImportReference(typeof(vm.PropertyChangedEventHandler).GetMethod("Invoke"));

				SetMethodNotify.Body.Instructions.Clear();
				var worker = SetMethodNotify.Body.GetILProcessor();
				worker.Append(worker.Create(OpCodes.Nop));
				worker.Append(worker.Create(OpCodes.Ldarg_0));
				worker.Append(worker.Create(OpCodes.Ldfld, eventField));
				worker.Append(worker.Create(OpCodes.Dup));
				var inst1 = worker.Create(OpCodes.Ldarg_0);
				worker.Append(worker.Create(OpCodes.Brtrue, inst1));
				worker.Append(worker.Create(OpCodes.Pop));
				var inst2 = worker.Create(OpCodes.Ret);
				worker.Append(worker.Create(OpCodes.Br_S, inst2));
				worker.Append(inst1);
				worker.Append(worker.Create(OpCodes.Ldarg_3));
				worker.Append(worker.Create(OpCodes.Ldarg_1));
				worker.Append(worker.Create(OpCodes.Ldarg_2));
				worker.Append(worker.Create(OpCodes.Newobj, PropertyChangeEventArgsCtor));
				worker.Append(worker.Create(OpCodes.Callvirt, PropertyChangeEventHandler));
				worker.Append(worker.Create(OpCodes.Nop));
				worker.Append(inst2);

			}

			var NotifyPropertyGot = typeDefinition.Methods.First(m => m.Name == "NotifyPropertyGot");
			var NotifyPropertyChanged = typeDefinition.Methods.First(m => m.Name == "NotifyPropertyChanged");

			var IObservableRef = MainAssembly.MainModule.ImportReference(typeof(vm.IObservable));
			var IObservableDef = new InterfaceImplementation(IObservableRef);
			typeDefinition.Interfaces.Add(IObservableDef);
			var IObservableEventDelegateRef = MainAssembly.MainModule.ImportReference(typeof(vm.IObservableEventDelegate));
			var IObservableEventDelegateDef = new InterfaceImplementation(IObservableEventDelegateRef);
			typeDefinition.Interfaces.Add(IObservableEventDelegateDef);

			typeDefinition.Properties.ForEach(p =>
			{
				Instruction[] getMethodInstCopy;

				// get
				{
					var getMethod = p.GetMethod;
                    if (getMethod != null)
                    {
						getMethodInstCopy = new Instruction[p.GetMethod.Body.Instructions.Count];
						p.GetMethod.Body.Instructions.CopyTo(getMethodInstCopy, 0);

						var getWorker = getMethod.Body.GetILProcessor();

						var localVar = new VariableDefinition(getMethod.ReturnType);
						List<Instruction> getFinalInst = new List<Instruction>();
						getFinalInst.Add(getWorker.Create(OpCodes.Stloc_S, localVar));
						getFinalInst.Add(getWorker.Create(OpCodes.Ldloc_S, localVar));
						getFinalInst.Add(getWorker.Create(OpCodes.Ldarg_0));
						getFinalInst.Add(getWorker.Create(OpCodes.Ldloc_S, localVar));
						if (getMethod.ReturnType.IsValueType)
						{
							getFinalInst.Add(getWorker.Create(OpCodes.Box, getMethod.ReturnType));
						}
						getFinalInst.Add(getWorker.Create(OpCodes.Ldstr, p.Name));
						getFinalInst.Add(getWorker.Create(OpCodes.Callvirt, NotifyPropertyGot));
						getFinalInst.Add(getWorker.Create(OpCodes.Nop));

						CILUtils.InjectBeforeReturn(getMethod, getFinalInst.ToArray());

						getMethod.Body.Variables.Add(localVar);
					}
				}

				// set
				{
					// TODO: 优化value判定，优化效率
					var setMethod = p.SetMethod;
					if(setMethod != null)
                    {
						var setWorker = setMethod.Body.GetILProcessor();

						List<Instruction> setFinalInst = new List<Instruction>();
						setFinalInst.Add(setWorker.Create(OpCodes.Ldarg_0));
						setFinalInst.Add(setWorker.Create(OpCodes.Ldarg_1));
						if (setMethod.ReturnType.IsValueType)
						{
							setFinalInst.Add(setWorker.Create(OpCodes.Box, setMethod.ReturnType));
						}
						setFinalInst.Add(setWorker.Create(OpCodes.Ldarg_1));
						if (setMethod.ReturnType.IsValueType)
						{
							setFinalInst.Add(setWorker.Create(OpCodes.Box, setMethod.ReturnType));
						}
						setFinalInst.Add(setWorker.Create(OpCodes.Ldstr, p.Name));
						setFinalInst.Add(setWorker.Create(OpCodes.Callvirt, NotifyPropertyChanged));
						setFinalInst.Add(setWorker.Create(OpCodes.Nop));

						CILUtils.InjectBeforeReturn(setMethod, setFinalInst.ToArray());
					}
				}
			});
			#endregion

			#region implement IWithPrototype
			var ObjectRef = MainAssembly.MainModule.ImportReference(typeof(object));
			var IWithPrototypeRef = MainAssembly.MainModule.ImportReference(typeof(System.ListExt.IWithPrototype));
			CILUtils.InjectField(MainAssembly,typeDefinition, "___Sproto__", ObjectRef, FieldAttributes.Family);
			CILUtils.InjectField(MainAssembly, typeDefinition, "_", ObjectRef, FieldAttributes.Family);
			CILUtils.InjectGetFieldMethod(MainAssembly, typeDefinition, "GetProto", "___Sproto__", ObjectRef);
			CILUtils.InjectSetFieldMethod(MainAssembly, typeDefinition, "SetProto", "___Sproto__", ObjectRef);
			CILUtils.InjectInteface(MainAssembly, typeDefinition, IWithPrototypeRef);
			#endregion
		}
	}
}
