using System;
using System.Linq;
using Mono.Cecil;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using CiLin;

namespace DataBindService
{
    public class Field2PropInfo
    {
        public ModuleDefinition moduleDef;
        public TypeDefinition typeDef;
        public FieldDefinition fieldDef;
        public PropertyDefinition propertyDef;
    }
    public class PostTask
    {
        public List<Field2PropInfo> field2PropInfos = new List<Field2PropInfo>();

        public void AddField2PropInfo(ModuleDefinition moduleDef, TypeDefinition typeDef, FieldDefinition fieldDef,PropertyDefinition propertyDef)
        {
            this.field2PropInfos.Add(new Field2PropInfo()
            {
                moduleDef = moduleDef,
                typeDef = typeDef,
                fieldDef = fieldDef,
                propertyDef = propertyDef,
            });
        }

        public void Clear()
        {
            this.field2PropInfos.Clear();
        }

        public void Merge(PostTask postTask)
        {
            this.field2PropInfos.AddRange(postTask.field2PropInfos);
        }
    }
}
