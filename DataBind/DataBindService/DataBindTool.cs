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

		public static void HandleAutoConvFieldToProperty(TypeDefinition typeDefinition, TypeReference AsPropertyAttr, PostTask postTask)
        {
			var attr = typeDefinition.CustomAttributes.FirstOrDefault(attr => CILUtils.IsSameAttr(attr, AsPropertyAttr));
			typeDefinition.CustomAttributes.Remove(attr);
			var fields=typeDefinition.Fields.ToArray();
			foreach(var field in fields)
            {
				if(IsValidFieldConvToProperty(field))
                {
					var prop = CILUtils.ConvertFieldToProperty(MainAssembly, typeDefinition, field);
					postTask.AddField2PropInfo(MainAssembly.MainModule, typeDefinition, field, prop);
				}
            }
        }

		public static void HandleAutoConvFieldToPropertySeperately(TypeDefinition typeDefinition,TypeReference AsPropertyAttr, PostTask postTask)
		{
			foreach (var field in typeDefinition.Fields.ToArray())
			{
				var attr = field.CustomAttributes.FirstOrDefault(attr => CILUtils.IsSameAttr(attr, AsPropertyAttr));
				if (attr!=null)
                {
                    if (IsValidFieldConvToProperty(field))
                    {
						field.CustomAttributes.Remove(attr);
						var prop=CILUtils.ConvertFieldToProperty(MainAssembly, typeDefinition, field);
						postTask.AddField2PropInfo(MainAssembly.MainModule, typeDefinition, field, prop);
					}
				}
			}
		}

		public static bool IsValidFieldConvToProperty(FieldDefinition field)
        {
			return field.IsStatic==false && ( field.IsPublic || field.IsFamily);
		}

		public static void HandleHost(TypeDefinition typeDefinition)
		{
			using var DataBindServiceAssembly = AssemblyDefinition.ReadAssembly(typeof(DataBindService.DBRuntimeDemo).Assembly.Location);
			using var DataBindAssembly = AssemblyDefinition.ReadAssembly(typeof(DataBinding.HostExt2).Assembly.Location);

			var IFullHostRef = MainAssembly.MainModule.ImportReference(typeof(VM.IFullHost));
			var IHostRef = MainAssembly.MainModule.ImportReference(typeof(VM.IHost));
			if (typeDefinition.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, IHostRef)))
			{
				return;
			}
			if (typeDefinition.Interfaces.Any(inter => CILUtils.IsSameInterface(inter, IFullHostRef)))
			{
				return;
			}

			#region implement IFullHost
			var VoidRef = MainAssembly.MainModule.ImportReference(typeof(void));
			var BoolRef = MainAssembly.MainModule.ImportReference(typeof(bool));
			var TWatcherCollection = typeof(System.Collections.Generic.ICollection<VM.Watcher>);
			var IWatcherCollectionRef = MainAssembly.MainModule.ImportReference(TWatcherCollection);
			var TDefaultValueType = typeof(System.Collections.Generic.List<VM.Watcher>);
			var WatcherRef = MainAssembly.MainModule.ImportReference(typeof(VM.Watcher));
			//var CombineTypeRef = MainAssembly.MainModule.ImportReference(typeof(vm.CombineType<object, string, Func<object, object, object>>));
			//var CallbackRef = MainAssembly.MainModule.ImportReference(typeof(Action<object, object, object>));
			//         var CombineTypeRef2 = MainAssembly.MainModule.ImportReference(typeof(vm.CombineType<object, string, number, boolean>));

			CILUtils.InjectProperty(MainAssembly, typeDefinition, "_SIsDestroyed", BoolRef);
			CILUtils.InjectField(MainAssembly, typeDefinition, "_Swatchers", IWatcherCollectionRef, FieldAttributes.Family);
			CILUtils.InjectGetOrCreateObjectMethod(MainAssembly, typeDefinition, "GetWatchers", "_Swatchers", IWatcherCollectionRef, TDefaultValueType);

			CILUtils.InjectInteface(MainAssembly, typeDefinition, IFullHostRef);

			#endregion

			#region implement IHost
			var implementIHost = false;
			if (implementIHost)
            {
				// implement IHost
				CILUtils.InjectInteface(MainAssembly, typeDefinition, IHostRef);

				// define _SaddWatcher
				{
					var HostExt2Type = typeof(DataBinding.HostExt2);
					var WatchMethodRef = MainAssembly.MainModule.ImportReference(HostExt2Type.GetMethod("AddWatcher"));
					var methodAttribute = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
					var _SaddWatcherMethod = new MethodDefinition("_SaddWatcher", methodAttribute, VoidRef);
					_SaddWatcherMethod.Parameters.Add(new ParameterDefinition("watcher", ParameterAttributes.None, WatcherRef));
					var ILWorker = _SaddWatcherMethod.Body.GetILProcessor();
					ILWorker.Append(ILWorker.Create(OpCodes.Nop));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_0));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_1));
					ILWorker.Append(ILWorker.Create(OpCodes.Call, WatchMethodRef));
					ILWorker.Append(ILWorker.Create(OpCodes.Nop));
					ILWorker.Append(ILWorker.Create(OpCodes.Ret));
					typeDefinition.Methods.Add(_SaddWatcherMethod);
				}

				// define _Sdestroy
				{
					var HostExt2Type = typeof(DataBinding.HostExt2);
					var DestroyMethodRef = MainAssembly.MainModule.ImportReference(HostExt2Type.GetMethod("Destroy"));
					var methodAttribute = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
					var _SdestroyWatcherMethod = new MethodDefinition("_Sdestroy", methodAttribute, VoidRef);
					var ILWorker = _SdestroyWatcherMethod.Body.GetILProcessor();
					ILWorker.Append(ILWorker.Create(OpCodes.Nop));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_0));
					ILWorker.Append(ILWorker.Create(OpCodes.Call, DestroyMethodRef));
					ILWorker.Append(ILWorker.Create(OpCodes.Nop));
					ILWorker.Append(ILWorker.Create(OpCodes.Ret));
					typeDefinition.Methods.Add(_SdestroyWatcherMethod);
				}

				// define _SremoveWatcher
				{
					var HostExt2Type = typeof(DataBinding.HostExt2);
					var DestroyMethodRef = MainAssembly.MainModule.ImportReference(HostExt2Type.GetMethod("RemoveWatcher"));
					var methodAttribute = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
					var _SremoveWatcherMethod = new MethodDefinition("_SremoveWatcher", methodAttribute, VoidRef);
					_SremoveWatcherMethod.Parameters.Add(new ParameterDefinition("watcher", ParameterAttributes.None, WatcherRef));
					var ILWorker = _SremoveWatcherMethod.Body.GetILProcessor();
					ILWorker.Append(ILWorker.Create(OpCodes.Nop));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_0));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_1));
					ILWorker.Append(ILWorker.Create(OpCodes.Call, DestroyMethodRef));
					ILWorker.Append(ILWorker.Create(OpCodes.Nop));
					ILWorker.Append(ILWorker.Create(OpCodes.Ret));
					typeDefinition.Methods.Add(_SremoveWatcherMethod);
				}

				// define _Swatch
				{
					var HostExt2Type = typeof(DataBinding.HostExt2);
					var WatchMethodRef0 = MainAssembly.MainModule.ImportReference(HostExt2Type.GetMethod("_Watch0"));
					var WatchMethodRef = MainAssembly.MainModule.ImportReference(HostExt2Type.GetMethods()
						.First(m => {
							return m.Name == "Watch" && m.GetParameters()[1].ParameterType.Name == WatchMethodRef0.Parameters[1].ParameterType.Name;
						}
						));
					var ExpOrFnType = MainAssembly.MainModule.ImportReference(WatchMethodRef.Parameters[0].ParameterType);
					var methodAttribute = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
					var _SwatchMethod = new MethodDefinition("_Swatch", methodAttribute, WatcherRef);
					var paramNames = new List<string>()
				{
					"expOrFn",
					"cb",
					"loseValue",
					"sync",
				};
					for (var i = 1; i < WatchMethodRef.Parameters.Count; i++)
					{
						var p = WatchMethodRef.Parameters[i];
						var pName = paramNames[i - 1];
						_SwatchMethod.Parameters.Add(new ParameterDefinition(pName, p.Attributes, p.ParameterType));
					}
					_SwatchMethod.Body.InitLocals = true;
					_SwatchMethod.Body.Variables.Add(new VariableDefinition(WatcherRef));
					_SwatchMethod.Body.MaxStackSize = 5;
					var p4 = _SwatchMethod.Parameters[3];
					var ILWorker = _SwatchMethod.Body.GetILProcessor();
					ILWorker.Append(ILWorker.Create(OpCodes.Nop));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_0));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_1));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_2));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_3));
					ILWorker.Append(ILWorker.Create(OpCodes.Ldarg_S, p4));
					ILWorker.Append(ILWorker.Create(OpCodes.Call, WatchMethodRef));
					ILWorker.Append(ILWorker.Create(OpCodes.Stloc_0));
					var inst0 = ILWorker.Create(OpCodes.Ldloc_0);
					ILWorker.Append(ILWorker.Create(OpCodes.Br_S, inst0));
					ILWorker.Append(inst0);
					ILWorker.Append(ILWorker.Create(OpCodes.Ret));
					typeDefinition.Methods.Add(_SwatchMethod);
				}
			}
            #endregion

        }

        public static void HandleObservable(TypeDefinition typeDefinition, CustomAttribute attr0)
		{
			var needInject = false;

			var attr = typeDefinition.CustomAttributes.FirstOrDefault(a => a != null && CILUtils.IsSameTypeReference(a.AttributeType, attr0.AttributeType));
			if (attr == null)
			{
				needInject = true;
				attr = CILUtils.CopyCustomAttribute(MainAssembly,attr0);
				typeDefinition.CustomAttributes.Add(attr);
            }
            else
            {
				var item = attr.ConstructorArguments[0];
				if (item.Value.Equals(0))
				{
					var newItem = new CustomAttributeArgument(item.Type, attr0.ConstructorArguments[0].Value);
					attr.ConstructorArguments.Remove(item);
					attr.ConstructorArguments.Add(newItem);
					needInject = true;
				}
			}

			if (!needInject)
			{
				return;
			}

			#region implement IObservable
			using var DataBindAssembly = AssemblyDefinition.ReadAssembly(typeof(DataBindService.DBRuntimeDemo).Assembly.Location);

			var DebuggerStepThroughAttrRef = MainAssembly.MainModule.ImportReference(typeof(System.Diagnostics.DebuggerStepThroughAttribute).GetConstructor(new Type[0]));
			var ObserverRef = MainAssembly.MainModule.ImportReference(typeof(VM.Observer));

			CILUtils.InjectField(MainAssembly, typeDefinition, "___Sob__", ObserverRef, FieldAttributes.Family);
			CILUtils.InjectGetFieldMethod(MainAssembly, typeDefinition, "_SgetOb", "___Sob__", ObserverRef);
			CILUtils.InjectSetFieldMethod(MainAssembly, typeDefinition, "_SsetOb", "___Sob__", ObserverRef);

			var GetEvent = CILUtils.InjectEvent(MainAssembly, typeDefinition, "PropertyGot", typeof(VM.PropertyGetEventHandler));
			//GetEvent.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
			GetEvent.AddMethod.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
			GetEvent.RemoveMethod.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
			var SetEvent = CILUtils.InjectEvent(MainAssembly, typeDefinition, "PropertyChanged", typeof(VM.PropertyChangedEventHandler));
			//SetEvent.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
			SetEvent.AddMethod.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
			SetEvent.RemoveMethod.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));

			var IObservableRef = MainAssembly.MainModule.ImportReference(typeof(VM.IObservable));
			var IObservableDef = new InterfaceImplementation(IObservableRef);
			typeDefinition.Interfaces.Add(IObservableDef);
			
			var RuntimeDemoRef = MainAssembly.MainModule.ImportReference(typeof(DataBindService.DBRuntimeDemo));
			var RuntimeDemoDef = DataBindAssembly.MainModule.Types.First(t => t != null && t.Namespace == RuntimeDemoRef.Namespace && t.FullName == RuntimeDemoRef.FullName);
			var NotifyPropertyGotMethod = RuntimeDemoDef.Methods.First(m => m.Name == "NotifyPropertyGot");
			var NotifyPropertyChangedMethod = RuntimeDemoDef.Methods.First(m => m.Name == "NotifyPropertyChanged");
			var GetMethodNotify = CILUtils.CopyMethod(MainAssembly, typeDefinition, "NotifyPropertyGot", RuntimeDemoDef, NotifyPropertyGotMethod);
			var SetMethodNotify = CILUtils.CopyMethod(MainAssembly, typeDefinition, "NotifyPropertyChanged", RuntimeDemoDef, NotifyPropertyChangedMethod);
			
			{
				var eventField = typeDefinition.Fields.First(f => f.Name == "PropertyGot");
				var PropertyGetEventArgsCtor = MainAssembly.MainModule.ImportReference(typeof(VM.PropertyGetEventArgs).GetConstructor(new Type[] { typeof(string), typeof(object) }));
				var PropertyGetEventHandler = MainAssembly.MainModule.ImportReference(typeof(VM.PropertyGetEventHandler).GetMethod("Invoke"));
			
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
			
				GetMethodNotify.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
			}
			{
				var eventField = typeDefinition.Fields.First(f => f.Name == "PropertyChanged");
				var PropertyChangeEventArgsCtor = MainAssembly.MainModule.ImportReference(typeof(VM.PropertyChangedEventArgs).GetConstructor(new Type[] { typeof(string), typeof(object), typeof(object) }));
				var PropertyChangeEventHandler = MainAssembly.MainModule.ImportReference(typeof(VM.PropertyChangedEventHandler).GetMethod("Invoke"));
			
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
			
				SetMethodNotify.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
			}
			
			var NotifyPropertyGot = typeDefinition.Methods.First(m => m.Name == "NotifyPropertyGot");
			var NotifyPropertyChanged = typeDefinition.Methods.First(m => m.Name == "NotifyPropertyChanged");
			
			var IObservableEventDelegateRef = MainAssembly.MainModule.ImportReference(typeof(VM.IObservableEventDelegate));
			var IObservableEventDelegateDef = new InterfaceImplementation(IObservableEventDelegateRef);
			typeDefinition.Interfaces.Add(IObservableEventDelegateDef);
			var BoolRef = MainAssembly.MainModule.ImportReference(typeof(bool));
			
			typeDefinition.Properties.ForEach(p =>
			{
				Instruction[] getMethodInstCopy = null;
			
				var getMethod = p.GetMethod;
				// get
				{
					if (getMethod != null)
					{
						getMethodInstCopy = new Instruction[p.GetMethod.Body.Instructions.Count];
						p.GetMethod.Body.Instructions.CopyTo(getMethodInstCopy, 0);
						p.GetMethod.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
			
						var getWorker = getMethod.Body.GetILProcessor();
			
						VariableDefinition localVar;
						localVar = getMethod.Body.Variables.FirstOrDefault(v => v.VariableType == getMethod.ReturnType);
						if (localVar == null)
						{
							localVar = new VariableDefinition(getMethod.ReturnType);
							getMethod.Body.Variables.Add(localVar);
						}
						List<Instruction> getFinalInst = new List<Instruction>();
						getFinalInst.Add(getWorker.Create(OpCodes.Dup));
						getFinalInst.Add(getWorker.Create(OpCodes.Stloc_S, localVar));
						// getFinalInst.Add(getWorker.Create(OpCodes.Ldloc_S, localVar));
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
					}
				}
			
				// set
				{
					// TODO: 优化value判定，优化效率
					var setMethod = p.SetMethod;
					if (setMethod != null)
					{
						setMethod.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
						var setWorker = setMethod.Body.GetILProcessor();
			
						if (getMethodInstCopy != null)
						{
							var tempLocal = new VariableDefinition(p.PropertyType);
							setMethod.Body.Variables.Add(tempLocal);
			
							List<Instruction> setFinalInst = new List<Instruction>();
							{
								setFinalInst.Add(setWorker.Create(OpCodes.Ldarg_0));
								setFinalInst.Add(setWorker.Create(OpCodes.Ldarg_1));
								if (p.PropertyType.IsValueType)
								{
									setFinalInst.Add(setWorker.Create(OpCodes.Box, p.PropertyType));
								}
								setFinalInst.Add(setWorker.Create(OpCodes.Ldloc_S, tempLocal));
								if (p.PropertyType.IsValueType)
								{
									setFinalInst.Add(setWorker.Create(OpCodes.Box, p.PropertyType));
								}
								setFinalInst.Add(setWorker.Create(OpCodes.Ldstr, p.Name));
								setFinalInst.Add(setWorker.Create(OpCodes.Callvirt, NotifyPropertyChanged));
								setFinalInst.Add(setWorker.Create(OpCodes.Nop));
							}
			
							var privateGetMethodName = $"<{setMethod.Name}>b__pri_get0";
							var privateGetMethod = CILUtils.CopyMethod(MainAssembly, typeDefinition, privateGetMethodName, typeDefinition, p.GetMethod);
							privateGetMethod.Attributes = MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.NewSlot;
							privateGetMethod.SemanticsAttributes = MethodSemanticsAttributes.None;
							privateGetMethod.CustomAttributes.Add(new CustomAttribute(DebuggerStepThroughAttrRef));
							var privateGetMethodWorker = privateGetMethod.Body.GetILProcessor();
							getMethodInstCopy.ForEach(inst =>
							{
								var instCopy= Instruction.Create(OpCodes.Nop);
								instCopy.OpCode = inst.OpCode;
								instCopy.Operand = inst.Operand;
								privateGetMethodWorker.Append(instCopy);
							});
			
			
							{
								var getMethodInstList = new System.Collections.Generic.List<Instruction>();
								Instruction retInst;
								retInst = setMethod.Body.Instructions.FirstOrDefault(inst => inst != null && inst.OpCode == OpCodes.Ret);
								if (retInst == null)
								{
									retInst = setWorker.Create(OpCodes.Ret);
									setFinalInst.Add(retInst);
								}
								getMethodInstList.Add(setWorker.Create(OpCodes.Ldarg_0));
								getMethodInstList.Add(setWorker.Create(OpCodes.Call, privateGetMethod));
								getMethodInstList.Add(setWorker.Create(OpCodes.Stloc, tempLocal));
								getMethodInstList.Add(setWorker.Create(OpCodes.Ldloc, tempLocal));
								getMethodInstList.Add(setWorker.Create(OpCodes.Ldarg_1));
								getMethodInstList.Add(setWorker.Create(OpCodes.Ceq));
								getMethodInstList.Add(setWorker.Create(OpCodes.Brtrue_S, retInst));
								setMethod.Body.InitLocals = true;
			
								CILUtils.InjectBeforeReturn(setMethod, setFinalInst.ToArray());
			
								CILUtils.InjectAtMethodBegin(setMethod, getMethodInstList.ToArray());
			
							}
						}
						else
						{
			
							List<Instruction> setFinalInst = new List<Instruction>();
							{
								setFinalInst.Add(setWorker.Create(OpCodes.Ldarg_0));
								setFinalInst.Add(setWorker.Create(OpCodes.Ldarg_1));
								if (p.PropertyType.IsValueType)
								{
									setFinalInst.Add(setWorker.Create(OpCodes.Box, p.PropertyType));
								}
								setFinalInst.Add(setWorker.Create(OpCodes.Ldarg_1));
								if (p.PropertyType.IsValueType)
								{
									setFinalInst.Add(setWorker.Create(OpCodes.Box, p.PropertyType));
								}
								setFinalInst.Add(setWorker.Create(OpCodes.Ldstr, p.Name));
								setFinalInst.Add(setWorker.Create(OpCodes.Callvirt, NotifyPropertyChanged));
								setFinalInst.Add(setWorker.Create(OpCodes.Nop));
							}
			
							CILUtils.InjectBeforeReturn(setMethod, setFinalInst.ToArray());
						}
			
			
					}
				}
			});
			#endregion

			#region implement IWithPrototype
			var ObjectRef = MainAssembly.MainModule.ImportReference(typeof(object));
			var IWithPrototypeRef = MainAssembly.MainModule.ImportReference(typeof(DataBinding.CollectionExt.IWithPrototype));

			CILUtils.InjectInteface(MainAssembly, typeDefinition, IWithPrototypeRef);

			CILUtils.InjectField(MainAssembly, typeDefinition, "___Sproto__", ObjectRef, FieldAttributes.Family);
			var P_ = CILUtils.InjectProperty(MainAssembly, typeDefinition, "_", ObjectRef);

			CILUtils.InjectGetFieldMethod(MainAssembly, typeDefinition, "GetProto", "___Sproto__", ObjectRef);
			var SetProtoDef = CILUtils.InjectSetFieldMethod(MainAssembly, typeDefinition, "SetProto", "___Sproto__", ObjectRef);
			{
				var setMethod = SetProtoDef;
				var retInst = setMethod.Body.Instructions.Last();

				var setWorker = setMethod.Body.GetILProcessor();
				var appendSetInsts = new System.Collections.Generic.List<Instruction>();
				appendSetInsts.Add(setWorker.Create(OpCodes.Ldarg_0));
				appendSetInsts.Add(setWorker.Create(OpCodes.Ldarg_1));
				appendSetInsts.Add(setWorker.Create(OpCodes.Call, P_.SetMethod));

				appendSetInsts.ForEach(inst =>
				{
					setWorker.InsertBefore(retInst, inst);
				});
			}
			#endregion
		}

		public static void HandleObservableRecursive(TypeDefinition typeDefinition,CustomAttribute attr0)
        {
			HandleObservable(typeDefinition, attr0);
			typeDefinition.Properties.ToArray().ForEach(prop =>
			{
				var propTypeDef = MainAssembly.MainModule.Types.FirstOrDefault(t => CILUtils.IsSameTypeReference(t, prop.PropertyType));
				if(propTypeDef != null)
                {
					var attr = typeDefinition.CustomAttributes.FirstOrDefault(a => a != null && CILUtils.IsSameTypeReference(a.AttributeType, attr0.AttributeType));
					HandleObservableRecursive(propTypeDef, attr);
				}
			});
        }
	}
}
