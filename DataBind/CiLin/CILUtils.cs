using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace CiLin
{
	public static class LinqExt
	{
		public static void ForEach<T>(this Mono.Collections.Generic.Collection<T> coll, Action<T,int> call)
		{
			for(var i=0;i<coll.Count;i++)
			{
				var item=coll[i];
				call(item,i);
			}
		}
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

	public static class CILUtils
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
			{
				var method = FindMethod(targetType, methodName);
				if (method!=null)
				{
					return method;
				}
			}
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
			set.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, propertyType));
			set.SemanticsAttributes = MethodSemanticsAttributes.None;
			set.Body.InitLocals = true;
			targetType.Methods.Add(set);

			return set;
		}
		public static void InjectGetOrCreateObjectMethod(AssemblyDefinition assembly, TypeDefinition targetType, string methodName, string fieldName, TypeReference propertyType, Type defaultValueType)
		{
			if (FindMethod(targetType, methodName) != null)
			{
				return;
			}
			
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
			{
				var method = FindMethod(targetType, methodName);
				if (method != null)
				{
					return;
				}
			}
			
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
			{
				if (FindField(targetType, fieldName)!=null)
				{
					return;
				}
			}
			//define the field we store the value in
			FieldDefinition field = new FieldDefinition(fieldName, fieldAttributes, fieldType);
			targetType.Fields.Add(field);
		}

		public static void CopyCollection<T>(Mono.Collections.Generic.Collection<T> target, IEnumerable<T> source)
		{
			foreach (var t in source)
			{
				target.Add(t);
			}
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
			// if (targetType.FullName == "TestDataBind.Tests.TestEnhancedScroller.TRawData" && 
			//     ( targetMethodName=="NotifyPropertyChanged"))
			// {
			// 	Console.Write("");
			// 	foreach (var targetMethodParameter in targetMethod.Parameters)
			// 	{
			// 		var result = targetMethodParameter.CustomAttributes;
			// 	}
			// }
			CopyCollection(targetMethod.GenericParameters, sourceMethod.GenericParameters);

			targetType.Methods.Add(targetMethod);

			return targetMethod;
		}

		public static CustomAttribute CopyCustomAttribute(AssemblyDefinition assembly, CustomAttribute customAttribute)
		{
			var copyAttr = new CustomAttribute(customAttribute.Constructor, customAttribute.GetBlob());
			customAttribute.ConstructorArguments.ForEach(argv =>
			{
				var argvCopy = new CustomAttributeArgument(argv.Type, argv.Value);
				copyAttr.ConstructorArguments.Add(argvCopy);
			});
			return copyAttr;
		}

		public static bool IsFieldExist(this TypeDefinition targetType, string fieldName)
		{
			return FindField(targetType, fieldName) != null;
		}
		public static FieldDefinition FindField(this TypeDefinition targetType, string fieldName)
		{
			var baseType = targetType;
			while (true)
			{
				var field=baseType.Fields.FirstOrDefault(f => f.Name == fieldName);
				if (field!=null)
				{
					return field;
				}

				if (baseType.BaseType is TypeDefinition baseDef)
				{
					baseType = baseDef;
				}
				else
				{
					break;
				}
			}

			return null;
		}
		public static MethodDefinition FindMethod(this TypeDefinition targetType, string methodName)
		{
			var baseType = targetType;
			while (true)
			{
				var method=baseType.Methods.FirstOrDefault(f => f.Name == methodName);
				if (method!=null)
				{
					return method;
				}

				if (baseType.BaseType is TypeDefinition baseDef)
				{
					baseType = baseDef;
				}
				else
				{
					break;
				}
			}

			return null;
		}
		public static PropertyDefinition FindProperty(this TypeDefinition targetType, string propertyName)
		{
			var baseType = targetType;
			while (true)
			{
				var prop=baseType.Properties.FirstOrDefault(f => f.Name == propertyName);
				if (prop!=null)
				{
					return prop;
				}

				if (baseType.BaseType is TypeDefinition baseDef)
				{
					baseType = baseDef;
				}
				else
				{
					break;
				}
			}

			return null;
		}
		public static EventDefinition FindEvent(this TypeDefinition targetType, string eventName)
		{
			var baseType = targetType;
			while (true)
			{
				var eventP=baseType.Events.FirstOrDefault(f => f.Name == eventName);
				if (eventP!=null)
				{
					return eventP;
				}

				if (baseType.BaseType is TypeDefinition baseDef)
				{
					baseType = baseDef;
				}
				else
				{
					break;
				}
			}

			return null;
		}
		public static InterfaceImplementation FindInterface(this TypeDefinition targetType, TypeReference interfaceRefer)
		{
			var baseType = targetType;
			while (true)
			{
				var interfaceP=baseType.Interfaces.FirstOrDefault(i=>IsSameInterface(i, interfaceRefer));
				if (interfaceP!=null)
				{
					return interfaceP;
				}

				if (baseType.BaseType is TypeDefinition baseDef)
				{
					baseType = baseDef;
				}
				else
				{
					break;
				}
			}

			return null;
		}

		public static PropertyDefinition InjectProperty(AssemblyDefinition assembly, TypeDefinition targetType, string propertyName, Type returnType)
		{
			TypeReference propertyType = assembly.MainModule.ImportReference(returnType);
			return InjectProperty(assembly, targetType, propertyName, propertyType);
		}
		public static PropertyDefinition InjectProperty(AssemblyDefinition assembly, TypeDefinition targetType, string propertyName, TypeReference propertyType, FieldDefinition fieldDefinition=null)
		{
			//Import the void type
			TypeReference voidRef = assembly.MainModule.ImportReference(typeof(void));
			
			var TCompilerGenerated = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);
			var rCompilerGenerated = assembly.MainModule.ImportReference(TCompilerGenerated.GetConstructor(new Type[] { }));

			var fieldName = ConvertToFieldName(propertyName);
			var field = fieldDefinition;
            if(field == null)
            {
				field = FindField(targetType, fieldName);
				var prop = FindProperty(targetType, propertyName);
				if (prop != null)
				{
					return prop;
				}
            }

            if (field == null)
			{
				//define the field we store the value in
				field = new FieldDefinition(fieldName, FieldAttributes.Private, propertyType);
				targetType.Fields.Add(field);
			}

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
#if false //应该是冗余的
			get.Body.GetILProcessor().Append(get.Body.GetILProcessor().Create(OpCodes.Stloc_0));
			Instruction inst = get.Body.GetILProcessor().Create(OpCodes.Ldloc_0);
			get.Body.GetILProcessor().Append(get.Body.GetILProcessor().Create(OpCodes.Br_S, inst));
			get.Body.GetILProcessor().Append(inst);
#endif
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
			get.CustomAttributes.TryAddCustomAttribute(rCompilerGenerated);
			set.CustomAttributes.TryAddCustomAttribute(new CustomAttribute(rCompilerGenerated));
			PropertyDefinition propertyDefinition = new PropertyDefinition(propertyName, PropertyAttributes.None, propertyType)
			{
				GetMethod = get,
				SetMethod = set
			};
			propertyDefinition.CustomAttributes.TryAddCustomAttribute(new CustomAttribute(rCompilerGenerated));

			//add the property to the type.
			targetType.Properties.Add(propertyDefinition);

			return propertyDefinition;
		}
		public static string ConvertToFieldName(string propertyName)
		{
			return $"<{propertyName}>k__$BF";
		}
		public static string ConvertToPropName(string fieldName)
		{
			// return fieldName;
			if (fieldName[0] != '_')
			{
				string targetName;
				if (char.IsUpper(fieldName[0]))
				{
					targetName = $"{char.ToLower(fieldName[0])}{fieldName.Substring(1)}";
				}
				else
				{
					targetName = $"{char.ToUpper(fieldName[0])}{fieldName.Substring(1)}";
				}

				return targetName;
			}

			return fieldName;
		}

		public static PropertyDefinition ConvertFieldToProperty(AssemblyDefinition assembly, TypeDefinition typeDefinition, FieldDefinition field)
		{
			var fieldName0 = field.Name;
			var propName = ConvertToPropName(fieldName0);
			if (typeDefinition.FindProperty(propName) != null)
			{
				throw new Exception($"Field<{fieldName0}> should be null");
			}
			field.Name = propName;

			var TCompilerGenerated = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);
			var rCompilerGenerated = assembly.MainModule.ImportReference(TCompilerGenerated.GetConstructor(new Type[] { }));

			var prop = CILUtils.InjectProperty(assembly, typeDefinition, fieldName0, field.FieldType, field);
			PropertyAttributes propertyAttributes = PropertyAttributes.None;
			if ((field.Attributes & FieldAttributes.HasDefault) != 0)
			{
				propertyAttributes |= PropertyAttributes.HasDefault;
			}
			prop.Attributes = propertyAttributes;
			MethodAttributes methodAttributes;
			if (field.IsStatic)
			{
				methodAttributes = MethodAttributes.Static;
			}
			else
			{
				methodAttributes = MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot;
			}
			if (field.IsAssembly)
			{
				methodAttributes |= MethodAttributes.Assembly;
			}
			if (field.IsRuntimeSpecialName)
			{
				methodAttributes |= MethodAttributes.RTSpecialName;
			}
			var getMethodAttrs = methodAttributes;
			var setMethodAttrs = methodAttributes;
			if (field.IsPublic)
			{
				getMethodAttrs |= MethodAttributes.Public;

				if (field.IsInitOnly)
				{
					setMethodAttrs |= MethodAttributes.Private;
                }
                else
                {
					setMethodAttrs |= MethodAttributes.Public;
				}
			}
			if (field.IsFamily)
			{
				getMethodAttrs |= MethodAttributes.Family;

				if (field.IsInitOnly)
				{
					setMethodAttrs |= MethodAttributes.Private;
				}
				else
				{
					setMethodAttrs |= MethodAttributes.Family;
				}
			}
			methodAttributes |= MethodAttributes.SpecialName;
			prop.GetMethod.Attributes = getMethodAttrs;
			// prop.GetMethod.CustomAttributes.TryAddCustomAttribute(new CustomAttribute(rCompilerGenerated));
			prop.SetMethod.Attributes = setMethodAttrs;
			// prop.SetMethod.CustomAttributes.TryAddCustomAttribute(new CustomAttribute(rCompilerGenerated));

			var fieldAttributes = FieldAttributes.Private;
            if (field.IsStatic)
            {
				fieldAttributes|=FieldAttributes.Static;
			}
			field.Attributes = fieldAttributes;

			//ReplaceFieldReferWithPropertyDef(assembly,field,prop);

			return prop;
		}

		public static void TryAddCustomAttribute(this Collection<CustomAttribute> customAttributes, MethodReference attr)
		{
			if (!customAttributes.Any(a => IsSameAttr(a, attr.DeclaringType)))
			{
				customAttributes.Add(new CustomAttribute(attr));
			}
		}

		public static void TryAddCustomAttribute(this Collection<CustomAttribute> customAttributes, CustomAttribute attr)
		{
			if (!customAttributes.Any(a => IsSameAttr(a, attr.AttributeType)))
			{
				customAttributes.Add(attr);
			}
		}

		public static void TryAddInterface(this TypeDefinition typeDefinition, TypeReference ii)
		{
			var idef = FindInterface(typeDefinition, ii);
			if (idef == null)
			{
				typeDefinition.Interfaces.Add(new InterfaceImplementation(ii));
			}
		}

		public static void TryAddInterface(this TypeDefinition typeDefinition, InterfaceImplementation ii)
		{
			var idef = FindInterface(typeDefinition, ii.InterfaceType);
			if (idef == null)
			{
				typeDefinition.Interfaces.Add(ii);
			}
		}

		private static string ConvertToEventName(string propertyName)
		{
			var fieldName = new System.Text.StringBuilder();
			fieldName.Append(propertyName);

			return fieldName.ToString();
		}
		public static EventDefinition InjectEvent(AssemblyDefinition assembly, TypeDefinition assemblyTypes, string propertyName, Type returnType)
		{
			{
				var eventDef = FindEvent(assemblyTypes, propertyName);
				if (eventDef != null)
				{
					return eventDef;
				}
			}
			
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
			field.CustomAttributes.TryAddCustomAttribute(new CustomAttribute(rCompilerGenerated));
			field.CustomAttributes.TryAddCustomAttribute(new CustomAttribute(rDebuggerBrowsable));

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
			// add.CustomAttributes.TryAddCustomAttribute(new CustomAttribute(rCompilerGenerated));
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
			remove.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, propertyType));
			remove.SemanticsAttributes = MethodSemanticsAttributes.RemoveOn;
			// remove.CustomAttributes.TryAddCustomAttribute(new CustomAttribute(rCompilerGenerated));
			assemblyTypes.Methods.Add(remove);

			//create the event
			var evt1 = new EventDefinition(propertyName, EventAttributes.None, propertyType)
			{
				AddMethod = add,
				RemoveMethod = remove,
			};

			//add the property to the type.
			assemblyTypes.Events.Add(evt1);

			return evt1;
		}

		public static void InjectAtMethodBegin(MethodDefinition methodDefinition, Instruction[] instructions)
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
				if (inst.Operand is Instruction secInst)
				{
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
			var idef = FindInterface(typeDefinition, interfaceRefer);
			if (idef != null)
			{
				return idef;
			}
			var interfaceDef = new InterfaceImplementation(interfaceRefer);
			typeDefinition.Interfaces.Add(interfaceDef);
			return interfaceDef;
		}

		public static bool ReplaceFieldReferWithPropertyDef(AssemblyDefinition assembly,
			(FieldDefinition, PropertyDefinition)[] field2PropInfos)
        {
			var anyReferExist = false;

			foreach (var module in assembly.Modules)
			{
				foreach (var type in module.Types)
				{
					foreach (var method in type.Methods)
					{
						foreach (var field2PropInfo in field2PropInfos)
						{
							var (fieldDefinition, property) = field2PropInfo;
							anyReferExist |=
								ReplaceFieldReferWithPropertyDef(method, property, type, fieldDefinition);
						}
					}
				}
			}

			return anyReferExist;
        }

		private static bool ReplaceFieldReferWithPropertyDef(MethodDefinition method, PropertyDefinition property,
			TypeDefinition type, FieldDefinition fieldDefinition)
		{
			bool anyReferExist = false;
			if (method.Body != null && property.GetMethod!=method && property.SetMethod!=method && false==method.Name.EndsWith(">b__pri_get0"))
			{
				var insts = method.Body.Instructions;
				var isContructorInited = false;
				var isConstructor = method.IsConstructor && type == fieldDefinition.DeclaringType;
				insts.ForEach((inst,index) =>
				{
					if (isConstructor)
					{
						if (!isContructorInited)
						{
							// 跳过构造函数初始化阶段代码
							if (inst.OpCode == OpCodes.Call && inst.Operand is MethodReference ctorB && ctorB.Name.Equals(".ctor")
							    && (index >= 1 && insts[index - 1].OpCode == OpCodes.Ldarg_0)
							   )
							{
								isContructorInited = true;
								return;
							}
							return;
						}
					}
					var exist = true;
					// TODO: 考虑其他涉及fld的指令
					if (inst.OpCode == OpCodes.Ldfld && inst.Operand is FieldDefinition && inst.Operand == fieldDefinition)
					{
						inst.OpCode = OpCodes.Call;
						inst.Operand = property.GetMethod;
					}
					else if (inst.OpCode == OpCodes.Ldsfld && inst.Operand is FieldDefinition && inst.Operand == fieldDefinition)
					{
						inst.OpCode = OpCodes.Call;
						inst.Operand = property.GetMethod;
					}
					else if (inst.OpCode == OpCodes.Stfld && inst.Operand is FieldDefinition && inst.Operand == fieldDefinition)
					{
						inst.OpCode = OpCodes.Call;
						inst.Operand = property.SetMethod;
					}
					else if (inst.OpCode == OpCodes.Stsfld && inst.Operand is FieldDefinition && inst.Operand == fieldDefinition)
					{
						inst.OpCode = OpCodes.Call;
						inst.Operand = property.SetMethod;
					}
					else
					{
						exist= false;
					}

					if (exist)
					{
						anyReferExist = true;
					}
				});
			}

			return anyReferExist;
		}
	}
}
