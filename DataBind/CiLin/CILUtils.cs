using Mono.Cecil;
using System;
using System.Linq;
using Mono.Cecil.Cil;

namespace CiLin
{
	public static class LinqExt
	{
		public static void ForEach<T>(this Mono.Collections.Generic.Collection<T> coll, Action<T> call)
		{
			foreach (var item in coll)
			{
				call(item);
			}
		}
		public static void ForEach<T>(this System.Collections.Generic.IEnumerable<T> coll, Action<T> call)
		{
			for (int i = 0; i < coll.Count(); i++)
			{
				var item = coll.ElementAt(i);
				call(item);
			}
		}
	}

	public class CILUtils
	{
		public static AssemblyDefinition SysAssembly;

		public static Instruction InsertBefore(ILProcessor worker, Instruction target, Instruction instruction)
		{
			worker.InsertBefore(target, instruction);
			return instruction;
		}
		public static Instruction InsertAfter(ILProcessor worker, Instruction target, Instruction instruction)
		{
			worker.InsertAfter(target, instruction);
			return instruction;
		}

		public static void UpdateMethodOffsets(MethodBody body)
		{
			var offset = 0;
			foreach (var instruction in body.Instructions)
			{
				instruction.Offset = offset;
				offset += instruction.GetSize();
			}
		}

		public static bool IsSameTypeReference(TypeReference type1, TypeReference type2)
		{
			if (type1.Namespace == type2.Namespace && type1.Name == type2.Name && type1.Module == type2.Module)
			{
				return true;
			}
			return false;
		}
		public static bool IsSameInterface(InterfaceImplementation inter, TypeReference typeReference)
		{
			if (inter == null)
			{
				return typeReference == null;
			}

			return IsSameTypeReference(inter.InterfaceType, typeReference);
		}

		public static bool IsSameAttr(CustomAttribute attr, TypeReference typeReference)
		{
			if (attr == null)
			{
				return typeReference == null;
			}

			return IsSameTypeReference(attr.AttributeType, typeReference);
		}

		public static MethodDefinition InjectSetFieldMethod(AssemblyDefinition assembly, TypeDefinition targetType, string methodName, string fieldName, TypeReference propertyType)
		{
			//Import the void type
			TypeReference voidRef = assembly.MainModule.ImportReference(typeof(void));

			//define the field we store the value in
			var field = targetType.Fields.First(f => f.Name == fieldName);

			//Create the set method
			MethodDefinition set = new MethodDefinition(methodName,
				Mono.Cecil.MethodAttributes.Public |
				//Mono.Cecil.MethodAttributes.SpecialName |
				// Mono.Cecil.MethodAttributes.Final |
				Mono.Cecil.MethodAttributes.NewSlot |
				Mono.Cecil.MethodAttributes.Virtual |
				Mono.Cecil.MethodAttributes.HideBySig, voidRef);
			var setWorker = set.Body.GetILProcessor();
			setWorker.Append(setWorker.Create(OpCodes.Nop));
			setWorker.Append(setWorker.Create(OpCodes.Ldarg_0));
			setWorker.Append(setWorker.Create(OpCodes.Ldarg_1));
			setWorker.Append(setWorker.Create(OpCodes.Stfld, field));
			//setWorker.Append(setWorker.Create(OpCodes.Ldarg_1));
			//setWorker.Append(setWorker.Create(OpCodes.Stloc_0));
			//var inst = setWorker.Create(OpCodes.Ldloc_0);
			//setWorker.Append(setWorker.Create(OpCodes.Br_S, inst));
			//setWorker.Append(inst);
			setWorker.Append(setWorker.Create(OpCodes.Ret));
			set.Parameters.Add(new ParameterDefinition("value",ParameterAttributes.None,propertyType));
			set.SemanticsAttributes = MethodSemanticsAttributes.None;
			set.Body.InitLocals = true;
			targetType.Methods.Add(set);

			return set;
		}
		public static void InjectGetOrCreateObjectMethod(AssemblyDefinition assembly, TypeDefinition targetType, string methodName, string fieldName, TypeReference propertyType, Type defaultValueType)
		{
			var TDefaultValueCtor = defaultValueType.GetConstructor(new Type[0]);
			var FieldTypeCtroRef = assembly.MainModule.ImportReference(TDefaultValueCtor);
			//define the field we store the value in
			var field = targetType.Fields.First(f => f.Name == fieldName);
			var BoolRef = assembly.MainModule.ImportReference(typeof(bool));

			//Create the get method
			MethodDefinition get = new MethodDefinition(methodName,
				Mono.Cecil.MethodAttributes.Public |
				//Mono.Cecil.MethodAttributes.SpecialName |
				//Mono.Cecil.MethodAttributes.Final |
				Mono.Cecil.MethodAttributes.NewSlot |
				Mono.Cecil.MethodAttributes.Virtual |
				Mono.Cecil.MethodAttributes.HideBySig, propertyType);
			var getWorker = get.Body.GetILProcessor();
			getWorker.Append(getWorker.Create(OpCodes.Nop));
			getWorker.Append(getWorker.Create(OpCodes.Ldarg_0));
			getWorker.Append(getWorker.Create(OpCodes.Ldfld, field));
			getWorker.Append(getWorker.Create(OpCodes.Ldnull));
			getWorker.Append(getWorker.Create(OpCodes.Cgt_Un));
			getWorker.Append(getWorker.Create(OpCodes.Stloc_0));
			getWorker.Append(getWorker.Create(OpCodes.Ldloc_0));
			Instruction inst1 = getWorker.Create(OpCodes.Nop);
			Instruction inst2 = getWorker.Create(OpCodes.Ldloc_1);
			getWorker.Append(getWorker.Create(OpCodes.Brfalse_S, inst1));
			getWorker.Append(getWorker.Create(OpCodes.Nop));
			getWorker.Append(getWorker.Create(OpCodes.Ldarg_0));
			getWorker.Append(getWorker.Create(OpCodes.Ldfld, field));
			getWorker.Append(getWorker.Create(OpCodes.Stloc_1));
			getWorker.Append(getWorker.Create(OpCodes.Br_S, inst2));
			getWorker.Append(inst1);
			getWorker.Append(getWorker.Create(OpCodes.Ldarg_0));
			getWorker.Append(getWorker.Create(OpCodes.Newobj, FieldTypeCtroRef));
			getWorker.Append(getWorker.Create(OpCodes.Stfld, field));
			getWorker.Append(getWorker.Create(OpCodes.Ldarg_0));
			getWorker.Append(getWorker.Create(OpCodes.Ldfld, field));
			getWorker.Append(getWorker.Create(OpCodes.Stloc_1));
			getWorker.Append(getWorker.Create(OpCodes.Br_S, inst2));
			getWorker.Append(inst2);
			getWorker.Append(getWorker.Create(OpCodes.Ret));
			get.Body.Variables.Add(new VariableDefinition(BoolRef));
			get.Body.Variables.Add(new VariableDefinition(propertyType));
			get.Body.InitLocals = true;
			get.Body.MaxStackSize = 2;
			get.SemanticsAttributes = MethodSemanticsAttributes.None;
			targetType.Methods.Add(get);
		}
		public static void InjectGetFieldMethod(AssemblyDefinition assembly, TypeDefinition targetType, string methodName, string fieldName, TypeReference propertyType)
		{
			//define the field we store the value in
			var field = targetType.Fields.First(f => f.Name == fieldName);

			//Create the get method
			MethodDefinition get = new MethodDefinition(methodName,
				Mono.Cecil.MethodAttributes.Public |
				//Mono.Cecil.MethodAttributes.SpecialName |
				//Mono.Cecil.MethodAttributes.Final |
				Mono.Cecil.MethodAttributes.NewSlot |
				Mono.Cecil.MethodAttributes.Virtual |
				Mono.Cecil.MethodAttributes.HideBySig, propertyType);
			var getWorker = get.Body.GetILProcessor();
			getWorker.Append(getWorker.Create(OpCodes.Ldarg_0));
			getWorker.Append(getWorker.Create(OpCodes.Ldfld, field));
			getWorker.Append(getWorker.Create(OpCodes.Stloc_0));
			Instruction inst = getWorker.Create(OpCodes.Ldloc_0);
			getWorker.Append(getWorker.Create(OpCodes.Br_S, inst));
			getWorker.Append(inst);
			getWorker.Append(getWorker.Create(OpCodes.Ret));
			get.Body.Variables.Add(new VariableDefinition(propertyType));
			get.Body.InitLocals = true;
			get.SemanticsAttributes = MethodSemanticsAttributes.None;
			targetType.Methods.Add(get);

		}
		public static void InjectField(AssemblyDefinition assembly, TypeDefinition targetType, string fieldName, TypeReference fieldType, FieldAttributes fieldAttributes)
		{
			//define the field we store the value in
			FieldDefinition field = new FieldDefinition(fieldName, fieldAttributes, fieldType);
			targetType.Fields.Add(field);
		}

		public static void CopyCollection<T>(Mono.Collections.Generic.Collection<T> target, Mono.Collections.Generic.Collection<T> source)
		{
			source.ForEach(t =>
			{
				target.Add(t);
			});
		}

		public static MethodDefinition CopyMethod(AssemblyDefinition assembly, TypeDefinition targetType, string targetMethodName, TypeDefinition sourceType, MethodDefinition sourceMethod)
		{
			var targetMethod = new MethodDefinition(targetMethodName, sourceMethod.Attributes, sourceMethod.ReturnType);
			targetMethod.Body.InitLocals = sourceMethod.Body.InitLocals;
			targetMethod.Body.MaxStackSize = sourceMethod.Body.MaxStackSize;
			CopyCollection(targetMethod.CustomAttributes, sourceMethod.CustomAttributes);
			CopyCollection(targetMethod.Body.Variables, sourceMethod.Body.Variables);
			//CopyCollection(targetMethod.Body.Instructions, sourceMethod.Body.Instructions);
			CopyCollection(targetMethod.Parameters, sourceMethod.Parameters);
			CopyCollection(targetMethod.GenericParameters, sourceMethod.GenericParameters);

			targetType.Methods.Add(targetMethod);

			return targetMethod;
		}
		public static PropertyDefinition InjectProperty(AssemblyDefinition assembly, TypeDefinition targetType, string propertyName, Type returnType)
		{
			TypeReference propertyType = assembly.MainModule.ImportReference(returnType);
			return InjectProperty(assembly, targetType, propertyName, propertyType);
		}
		public static PropertyDefinition InjectProperty(AssemblyDefinition assembly, TypeDefinition targetType, string propertyName, TypeReference propertyType)
		{
			//Import the void type
			TypeReference voidRef = assembly.MainModule.ImportReference(typeof(void));

			//define the field we store the value in
			FieldDefinition field = new FieldDefinition(ConvertToFieldName(propertyName), FieldAttributes.Private, propertyType);
			targetType.Fields.Add(field);

			//Create the get method
			MethodDefinition get = new MethodDefinition("get_" +
			propertyName, Mono.Cecil.MethodAttributes.Public |
			Mono.Cecil.MethodAttributes.SpecialName |
			//Mono.Cecil.MethodAttributes.Final |
			Mono.Cecil.MethodAttributes.NewSlot |
			Mono.Cecil.MethodAttributes.Virtual |
			Mono.Cecil.MethodAttributes.HideBySig, propertyType);
			get.Body.GetILProcessor().Append(get.Body.GetILProcessor().Create(OpCodes.Ldarg_0));
			get.Body.GetILProcessor().Append(get.Body.GetILProcessor().Create(OpCodes.Ldfld, field));
			get.Body.GetILProcessor().Append(get.Body.GetILProcessor().Create(OpCodes.Stloc_0));
			Instruction inst = get.Body.GetILProcessor().Create(OpCodes.Ldloc_0);
			get.Body.GetILProcessor().Append(get.Body.GetILProcessor().Create(OpCodes.Br_S, inst));
			get.Body.GetILProcessor().Append(inst);
			get.Body.GetILProcessor().Append(get.Body.GetILProcessor().Create(OpCodes.Ret));
			get.Body.Variables.Add(new VariableDefinition(propertyType));
			get.Body.InitLocals = true;
			get.SemanticsAttributes = MethodSemanticsAttributes.Getter;
			targetType.Methods.Add(get);

			//Create the set method
			MethodDefinition set = new MethodDefinition("set_" +
			propertyName, Mono.Cecil.MethodAttributes.Public |
			Mono.Cecil.MethodAttributes.SpecialName |
			// Mono.Cecil.MethodAttributes.Final |
			Mono.Cecil.MethodAttributes.NewSlot |
			Mono.Cecil.MethodAttributes.Virtual |
			Mono.Cecil.MethodAttributes.HideBySig, voidRef);
			var setWorker = set.Body.GetILProcessor();
			setWorker.Append(setWorker.Create(OpCodes.Ldarg_0));
			setWorker.Append(setWorker.Create(OpCodes.Ldarg_1));
			setWorker.Append(setWorker.Create(OpCodes.Stfld, field));
			setWorker.Append(setWorker.Create(OpCodes.Ret));
			set.Parameters.Add(new ParameterDefinition(propertyType));
			set.SemanticsAttributes =
			MethodSemanticsAttributes.Setter;
			targetType.Methods.Add(set);

			//create the property
			PropertyDefinition propertyDefinition = new PropertyDefinition(propertyName, PropertyAttributes.None, propertyType)
			{
				GetMethod = get,
				SetMethod = set
			};

			//add the property to the type.
			targetType.Properties.Add(propertyDefinition);

			return propertyDefinition;
		}
		private static string ConvertToFieldName(string propertyName)
		{
			var fieldName = new System.Text.StringBuilder();
			//<CCC>k__BackingField
			fieldName.Append("<");
			fieldName.Append(propertyName);
			//fieldName.Append(">k__BackingField");
			fieldName.Append(">k__$BF");

			return fieldName.ToString();
		}


		private static string ConvertToEventName(string propertyName)
		{
			var fieldName = new System.Text.StringBuilder();
			fieldName.Append(propertyName);

			return fieldName.ToString();
		}
		public static void InjectEvent(AssemblyDefinition assembly, TypeDefinition assemblyTypes, string propertyName, Type returnType)
		{
			//var sys2 = AssemblyDefinition.ReadAssembly(typeof(System.IO.FileAttributes).Assembly.Location);
			//var sys= AssemblyDefinition.ReadAssembly(typeof(System.Delegate).Assembly.Location);
			var sys = SysAssembly;

			//Import the void type
			TypeReference voidRef = SysAssembly.MainModule.ImportReference(typeof(void));

			//Import the return type
			TypeReference propertyType = assembly.MainModule.ImportReference(returnType);

			var TCompilerGenerated = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);
			var TDebuggerBrowsableAttribute = typeof(System.Diagnostics.DebuggerBrowsableAttribute);
			var TDebuggerBrowsableState = typeof(System.Diagnostics.DebuggerBrowsableState);

			var rCompilerGenerated = assembly.MainModule.ImportReference(TCompilerGenerated.GetConstructor(new Type[] { }));
			var rDebuggerBrowsable = assembly.MainModule.ImportReference(TDebuggerBrowsableAttribute.GetConstructor(new Type[] { TDebuggerBrowsableState }));

			//define the field we store the value in
			FieldDefinition field = new FieldDefinition(ConvertToEventName(propertyName), FieldAttributes.Private, propertyType);
			field.CustomAttributes.Add(new CustomAttribute(rCompilerGenerated));
			field.CustomAttributes.Add(new CustomAttribute(rDebuggerBrowsable));

			assemblyTypes.Fields.Add(field);

			var rDelegate = assembly.MainModule.ImportReference(typeof(System.Delegate));
			var tDelegate = sys.MainModule.GetType(rDelegate.Namespace, rDelegate.Name);
			var mCombine = tDelegate.Methods.First(m => m.Name == "Combine");
			mCombine.Resolve();
			var mRemove = tDelegate.Methods.First(m => m.Name == "Remove");

			var rInterlocked = assembly.MainModule.ImportReference(typeof(System.Threading.Interlocked));
			var tInterlocked = sys.MainModule.GetType(rInterlocked.Namespace, rInterlocked.Name);
			var tMethod = tInterlocked.Methods.First(m => m.Name == "CompareExchange" && m.HasGenericParameters);
			var rMethod = assembly.MainModule.ImportReference(tMethod);
			var rMethod2 = new GenericInstanceMethod(rMethod);
			rMethod2.GenericArguments.Add(propertyType);
			rMethod2.Resolve();

			//Create the get method
			MethodDefinition add = new MethodDefinition("add_" + propertyName,
				Mono.Cecil.MethodAttributes.Public |
				Mono.Cecil.MethodAttributes.SpecialName |
				//Mono.Cecil.MethodAttributes.Final |
				Mono.Cecil.MethodAttributes.NewSlot |
				Mono.Cecil.MethodAttributes.Virtual |
				Mono.Cecil.MethodAttributes.HideBySig, voidRef);
			var addWorker = add.Body.GetILProcessor();
			addWorker.Append(addWorker.Create(OpCodes.Ldarg_0));
			addWorker.Append(addWorker.Create(OpCodes.Ldfld, field));
			addWorker.Append(addWorker.Create(OpCodes.Stloc_0));
			var inst1 = addWorker.Create(OpCodes.Ldloc_0);
			addWorker.Append(inst1);
			addWorker.Append(addWorker.Create(OpCodes.Stloc_1));
			addWorker.Append(addWorker.Create(OpCodes.Ldloc_1));
			addWorker.Append(addWorker.Create(OpCodes.Ldarg_1));
			var m1 = typeof(System.Delegate).GetMethod("Combine", new[] { typeof(System.Delegate), typeof(System.Delegate) });
			var opd1 = assembly.MainModule.ImportReference(m1);
			addWorker.Append(addWorker.Create(OpCodes.Call, opd1));
			addWorker.Append(addWorker.Create(OpCodes.Castclass, propertyType));
			addWorker.Append(addWorker.Create(OpCodes.Stloc_2));
			addWorker.Append(addWorker.Create(OpCodes.Ldarg_0));
			addWorker.Append(addWorker.Create(OpCodes.Ldflda, field));
			addWorker.Append(addWorker.Create(OpCodes.Ldloc_2));
			addWorker.Append(addWorker.Create(OpCodes.Ldloc_1));
			addWorker.Append(addWorker.Create(OpCodes.Call, rMethod2));
			addWorker.Append(addWorker.Create(OpCodes.Stloc_0));
			addWorker.Append(addWorker.Create(OpCodes.Ldloc_0));
			addWorker.Append(addWorker.Create(OpCodes.Ldloc_1));
			addWorker.Append(addWorker.Create(OpCodes.Bne_Un_S, inst1));
			addWorker.Append(addWorker.Create(OpCodes.Ret));
			add.Body.Variables.Add(new VariableDefinition(propertyType));
			add.Body.Variables.Add(new VariableDefinition(propertyType));
			add.Body.Variables.Add(new VariableDefinition(propertyType));
			add.Body.InitLocals = true;
			add.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, propertyType));
			add.SemanticsAttributes = MethodSemanticsAttributes.AddOn;

			assembly.MainModule.ImportReference(rCompilerGenerated);
			assemblyTypes.Module.ImportReference(rCompilerGenerated);
			add.CustomAttributes.Add(new CustomAttribute(rCompilerGenerated));
			assemblyTypes.Methods.Add(add);

			//Create the set method
			MethodDefinition remove = new MethodDefinition("remove_" + propertyName,
				Mono.Cecil.MethodAttributes.Public |
				Mono.Cecil.MethodAttributes.SpecialName |
				// Mono.Cecil.MethodAttributes.Final |
				Mono.Cecil.MethodAttributes.NewSlot |
				Mono.Cecil.MethodAttributes.Virtual |
				Mono.Cecil.MethodAttributes.HideBySig, voidRef);
			var removeWorker = remove.Body.GetILProcessor();
			removeWorker.Append(removeWorker.Create(OpCodes.Ldarg_0));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldfld, field));
			removeWorker.Append(removeWorker.Create(OpCodes.Stloc_0));
			var inst2 = removeWorker.Create(OpCodes.Ldloc_0);
			removeWorker.Append(inst2);
			removeWorker.Append(removeWorker.Create(OpCodes.Stloc_1));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldloc_1));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldarg_1));
			removeWorker.Append(removeWorker.Create(OpCodes.Call, opd1));
			removeWorker.Append(removeWorker.Create(OpCodes.Castclass, propertyType));
			removeWorker.Append(removeWorker.Create(OpCodes.Stloc_2));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldarg_0));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldflda, field));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldloc_2));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldloc_1));
			removeWorker.Append(removeWorker.Create(OpCodes.Call, rMethod2));
			removeWorker.Append(removeWorker.Create(OpCodes.Stloc_0));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldloc_0));
			removeWorker.Append(removeWorker.Create(OpCodes.Ldloc_1));
			removeWorker.Append(removeWorker.Create(OpCodes.Bne_Un_S, inst2));
			removeWorker.Append(removeWorker.Create(OpCodes.Ret));
			remove.Body.Variables.Add(new VariableDefinition(propertyType));
			remove.Body.Variables.Add(new VariableDefinition(propertyType));
			remove.Body.Variables.Add(new VariableDefinition(propertyType));
			remove.Body.InitLocals = true;
			remove.Parameters.Add(new ParameterDefinition(propertyType));
			remove.SemanticsAttributes = MethodSemanticsAttributes.RemoveOn;
			remove.CustomAttributes.Add(new CustomAttribute(rCompilerGenerated));
			assemblyTypes.Methods.Add(remove);

			//create the event
			var event1 = new EventDefinition(propertyName, EventAttributes.None, propertyType)
			{
				AddMethod = add,
				RemoveMethod = remove,
			};

			//add the property to the type.
			assemblyTypes.Events.Add(event1);

		}

		public static void InjectAtMethodBegin(MethodDefinition methodDefinition,Instruction[] instructions)
        {
			var ILWorker = methodDefinition.Body.GetILProcessor();
            var headInst = ILWorker.Body.Instructions[0];
			if (headInst == null)
			{
				foreach (var instruction in instructions)
				{
					ILWorker.Append(instruction);
				}
			}
			else
			{
				foreach (var instruction in instructions)
				{
					ILWorker.InsertBefore(headInst, instruction);
				}
			}

			UpdateMethodOffsets(methodDefinition.Body);
        }
        public static void InjectBeforeReturn(MethodDefinition methodDefinition, Instruction[] instructions)
		{
			var ILWorker = methodDefinition.Body.GetILProcessor();
			var replaceRetInst = instructions[0];
			// 替换直接跳转到ret的情况，避免衔接的指令失效
			methodDefinition.Body.Instructions.ForEach(inst =>
			{
				if (inst.Operand is Instruction)
				{
					var secInst = (Instruction)inst.Operand;
					if (secInst.OpCode == OpCodes.Ret)
					{
						inst.Operand = replaceRetInst;
					}
				}

			});
			// 所有ret指令前插指令
			// TODO: 优化跳转实现，优化包体
			var retCodes = methodDefinition.Body.Instructions.Where(inst => inst.OpCode == OpCodes.Ret);
			retCodes.ForEach(inst =>
			{
				foreach (var insertInst in instructions)
				{
					ILWorker.InsertBefore(inst, insertInst);
				}
			});

			UpdateMethodOffsets(methodDefinition.Body);
		}

		public static InterfaceImplementation InjectInteface(AssemblyDefinition assembly, TypeDefinition typeDefinition, TypeReference interfaceRefer)
		{
			var InterfaceDef = new InterfaceImplementation(interfaceRefer);
			typeDefinition.Interfaces.Add(InterfaceDef);
			return InterfaceDef;
		}
	}
}
