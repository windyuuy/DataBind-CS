using System.Linq;
using System.Text;
using DataBinding.CollectionExt;

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
            public void ExpressMember(CodeBuffer cb, MemberInfo member)
            {
                if (member.UsedCases.Count > 0)
                {
                    cb.AppendCodeLine("/// <usecase>");
                    member.UsedCases.ForEach(useCase => cb.AppendCodeLine($"/// {useCase}<br/>"));
                    cb.AppendCodeLine("/// </usecase>");
                }
                
                if (member.Type is BasicTypeInfo basicTypeInfo)
                {
                    if (string.IsNullOrEmpty(member.MemberManualCodeLine))
                    {
                        cb.AppendLine(member.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public {basicTypeInfo.TypeLiteral} {member.Name} {{get;set;}}");
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
                            cb.Append($"{para1.Type.Name} {para1.Name}");
                            funcInfo.Paras.Skip(1).ForEach(para =>
                            {
                                cb.Append(",");
                                if (member.Type is BasicTypeInfo basicTypeInfo2)
                                {
                                    cb.Append($"{basicTypeInfo2.TypeLiteral} {member.Name}");
                                }
                                else if (member.Type.Members.Length == 0)
                                {
                                    cb.Append($"{UnknownTypeMark} {member.Name}");
                                }
                                else
                                {
                                    cb.Append($"{para.Type.Name} {para.Name}");
                                }
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
                else if (member.Type.Members.Length == 0)
                {
                    if (!string.IsNullOrEmpty(member.MemberManualCodeLine))
                    {
                        cb.AppendLine(member.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public {UnknownTypeMark} {member.Name} {{get;set;}}");
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

                    if (!string.IsNullOrEmpty(member.Type.MemberManualCodeLine))
                    {
                        cb.Append(member.Type.MemberManualCodeLine);
                    }
                    else
                    {
                        cb.AppendCodeLine($"public class {member.Type.Name}");
                    }
                    cb.AppendCodeSegBegin("{");

                    foreach (var member2 in member.Type.Members)
                    {
                        ExpressMember(cb, member2);
                    }

                    cb.AppendCodeSegEnd("}");
                }
            }

            public string WriteCode(EnvInfo envInfo, string envClassName)
            {
                var cb = new CodeBuffer();
                cb.AppendCodeLine("namespace TestingCode");
                cb.AppendCodeSegBegin("{");
                cb.AppendCodeSegBegin($"public class {envClassName}: vm.Host");
                cb.AppendCodeLine("{");

                foreach (var member1 in envInfo.Members)
                {
                    cb.AppendCodeLine("/// <note>");
                    cb.AppendCodeLine($"/// env::{member1.Name}");
                    cb.AppendCodeLine("/// </note>");
                    ExpressMember(cb, member1);
                }

                cb.AppendCodeSegEnd("}");
                cb.AppendCodeSegEnd("}");

                return cb.ToText();
            }
        }

}