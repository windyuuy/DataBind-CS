﻿using System.Linq;
using System.Text;
using DataBind.CollectionExt;

namespace ParseJSDataBindAbstract.CodeWriter
{
        public class CodeBuffer
        {
            StringBuilder strb = new StringBuilder();

            int tabN = 0;
            string tabS = "";

            public void UpdateIntent()
            {
                tabS = new string('\t', tabN);
            }

            public void AddIntent()
            {
                tabN++;
                UpdateIntent();
            }

            public void SubIntent()
            {
                tabN--;
                UpdateIntent();
            }

            public CodeBuffer AppendCodeLine(string str)
            {
                this.strb.Append(tabS).AppendLine(str);
                return this;
            }

            public CodeBuffer AppendCode(string str)
            {
                this.strb.Append(tabS).Append(str);
                return this;
            }

            public CodeBuffer AppendCodeSegBegin(string str)
            {
                this.strb.Append(tabS).AppendLine(str);
                this.AddIntent();
                return this;
            }

            public CodeBuffer AppendCodeSegEnd(string str)
            {
                this.SubIntent();
                this.strb.Append(tabS).AppendLine(str);
                return this;
            }

            public CodeBuffer AppendLineIntent()
            {
                this.strb.Append(tabS);
                return this;
            }

            public CodeBuffer Append(string str)
            {
                this.strb.Append(str);
                return this;
            }

            public CodeBuffer AppendLine(string str)
            {
                this.strb.AppendLine(str);
                return this;
            }

            public string ToText()
            {
                return this.strb.ToString();
            }
        }

        public class CodeWriter
        {
            public string UnknownTypeMark = "object?";
            public void ExpressMember(CodeBuffer cb, MemberInfo member, Dictionary<ClassInfo,bool> typeSpace)
            {
                if (member.AnnotationLines?.Length > 0)
                {
                    foreach (var memberAnnotationLine in member.AnnotationLines)
                    {
                        cb.AppendLine(memberAnnotationLine);
                    }
                }
                else if (member.UsedCases.Count > 0)
                {
                    cb.AppendCodeLine("/// <usecase>");
                    member.UsedCases.ForEach(useCase => cb.AppendCodeLine($"/// {useCase}<br/>"));
                    cb.AppendCodeLine("/// </usecase>");
                }
                
                if (member.Type is BasicTypeInfo basicTypeInfo)
                {
                    if (!string.IsNullOrEmpty(member.MemberManualCodeLine))
                    {
                        cb.AppendLine(member.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public {basicTypeInfo.TypeLiteral} {member.Name} {{get;set;}}");
                    }
                }
                else if (member.Type is ArrayTypeInfo arrayTypeInfo)
                {
                    if (!string.IsNullOrEmpty(member.MemberManualCodeLine))
                    {
                        cb.AppendLine(member.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public {arrayTypeInfo.InferTypeLiteral(UnknownTypeMark)} {member.Name} {{get;set;}}");
                    }
                }
                else if (member.Type is DictionaryTypeInfo dictionaryTypeInfo)
                {
                    if (!string.IsNullOrEmpty(member.MemberManualCodeLine))
                    {
                        cb.AppendLine(member.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public {dictionaryTypeInfo.InferTypeLiteral(UnknownTypeMark)} {member.Name} {{get;set;}}");
                    }
                }
                else if (member.Type is FuncInfo funcInfo)
                {
                    if (!string.IsNullOrEmpty(member.MemberManualCodeLine))
                    {
                        cb.AppendLine(member.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCode($"public {funcInfo.RetType?.Type.Name ?? "void"} {member.Name}(");
                        if (funcInfo.Paras.Count > 0)
                        {
                            var para1 = funcInfo.Paras[0];

                            void AppendPara(MemberInfo para, int index)
                            {
                                if (para.Type is BasicTypeInfo basicTypeInfo2)
                                {
                                    if (para.Type is StringTypeInfo)
                                    {
                                        cb.Append($"{basicTypeInfo2.TypeLiteral} p_{index}");
                                    }
                                    else
                                    {
                                        cb.Append($"{basicTypeInfo2.TypeLiteral} p_{para.Name}");
                                    }
                                }
                                else if (para.Type.MemberCount == 0)
                                {
                                    cb.Append($"{para.InferType(UnknownTypeMark)} {para.Name}");
                                }
                                else
                                {
                                    cb.Append($"{para.Type.FullName} {para.Name}");
                                }
                            }
                            // cb.Append($"{para1.Type.Name} {para1.Name}");
                            AppendPara(para1, 0);
                            funcInfo.Paras.Skip(1).ForEach((para,index) =>
                            {
                                cb.Append(",");
                                // if (para.Type is BasicTypeInfo basicTypeInfo2)
                                // {
                                //     cb.Append($"{basicTypeInfo2.TypeLiteral} {para.Name}");
                                // }
                                // else if (para.Type.MembersCount == 0)
                                // {
                                //     cb.Append($"{UnknownTypeMark} {para.Name}");
                                // }
                                // else
                                // {
                                //     cb.Append($"{para.Type.Name} {para.Name}");
                                // }
                                AppendPara(para,index+1);
                            });
                        }

                        cb.AppendLine(")");
                    }

                    cb.AppendCodeLine("{");
                    if (funcInfo.FuncBodyManualCodeLines != null)
                    {
                        funcInfo.FuncBodyManualCodeLines.ForEach(line => cb.AppendLine(line));
                    }
                    cb.AppendCodeLine("}");
                }
                else if (member.Type.MemberCount == 0)
                {
                    if (!string.IsNullOrEmpty(member.MemberManualCodeLine))
                    {
                        cb.AppendLine(member.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public {member.InferType(UnknownTypeMark)} {member.Name} {{get;set;}}");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(member.MemberManualCodeLine))
                    {
                        cb.AppendLine(member.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public {member.Type.Name} {member.Name} {{get;set;}} = new {member.Type.Name}();");
                    }

                    // add class annotations
                    if (member.Type.AnnotationLines?.Length > 0)
                    {
                        foreach (var memberAnnotationLine in member.Type.AnnotationLines)
                        {
                            cb.AppendLine(memberAnnotationLine);
                        }
                    }
                    
                    // add class def
                    AddClassDef(cb, member.Type, typeSpace);
                }
            }

            public void AddClassDef(CodeBuffer cb, ClassInfo classInfo, Dictionary<ClassInfo,bool> typeSpace)
            {
                if (!(
                        classInfo.IsComposedType
                        || classInfo.InsideTypeCount == 0
                        || typeSpace.ContainsKey(classInfo)
                        ))
                {
                    typeSpace.Add(classInfo, true);
                    if (!string.IsNullOrEmpty(classInfo.TypeDefManualCodeLine))
                    {
                        cb.AppendLine(classInfo.TypeDefManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public class {classInfo.Name}");
                    }
                    cb.AppendCodeSegBegin("{");

                    foreach (var member2 in classInfo.Members)
                    {
                        ExpressMember(cb, member2, typeSpace);
                    }

                    foreach (var item in classInfo.InsideTypeMap)
                    {
                        if (!typeSpace.ContainsKey(item.Value))
                        {
                            AddClassDef(cb, item.Value, typeSpace);
                        }
                    }

                    cb.AppendCodeSegEnd("}");
                }
                else
                {
                    // added already
                }
            }

            public string WriteCode(EnvInfo envInfo)
            {
                var cb = new CodeBuffer();

                foreach (var envInfoUsingNamespace in envInfo.UsingNamespaces)
                {
                    cb.AppendCodeLine($"using {envInfoUsingNamespace};");
                }
                foreach (var item in envInfo.UsingAlias)
                {
                    cb.AppendCodeLine($"using {item.Key} = {item.Value};");
                }

                var fileHeaders = envInfo.FileHeaders;
                if (fileHeaders.Count > 0)
                {
                    for (var i = 0; i < fileHeaders.Count; i++)
                    {
                        cb.AppendLine(fileHeaders[i]);
                    }
                }
                else
                {
                    cb.AppendLine("");
                }
                
                cb.AppendCodeLine("namespace TestingCode");
                cb.AppendCodeSegBegin("{");
                cb.AppendCodeLine($"public class {envInfo.Name}: DataBind.IStdHost");
                cb.AppendCodeSegBegin("{");

                var typeSpace = new Dictionary<ClassInfo, bool>();
                foreach (var member1 in envInfo.Members)
                {
                    if (!(member1.AnnotationLines?.Length > 0))
                    {
                        cb.AppendCodeLine("/// <note>");
                        cb.AppendCodeLine($"/// env::{member1.Name}");
                        cb.AppendCodeLine("/// </note>");
                    }

                    ExpressMember(cb, member1, typeSpace);
                }

                foreach (var item in envInfo.InsideTypeMap)
                {
                    if (!typeSpace.ContainsKey(item.Value))
                    {
                        AddClassDef(cb, item.Value, typeSpace);
                    }
                }

                cb.AppendCodeSegEnd("}");
                cb.AppendCodeSegEnd("}");

                return cb.ToText();
            }
        }

}