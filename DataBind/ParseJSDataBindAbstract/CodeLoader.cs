using System;
using System.Linq.Ext;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ParseJSDataBindAbstract
{
    public class CodeLoader
    {
        void ParseClassMembers(Type cls,string[] lines,ref int pos, int indentCount, LineHandler handler)
        {
            var indentTabs = new string('\t', indentCount);
            var isClass = false;
            Type subClass = null;
            var parentIndentCount = new string('\t', indentCount - 1);
            var endTag = parentIndentCount + "}";
            while (pos<lines.Length)
            {
                var line = lines[pos];
                if (line.StartsWith(endTag))
                {
                    break;
                }

                if (line.StartsWith(indentTabs))
                {
                    if (line.StartsWith(indentTabs+"{"))
                    {
                        if (isClass)
                        {
                            ParseClassMembers(subClass,lines,ref pos,indentCount+1, handler);
                            handler.HandleClassEnd(cls, subClass);
                            isClass = false;
                            subClass = null;
                        }
                    }
                    else
                    {
                        var memberProperty = new Regex(@"(\w+) {get;set;}");
                        var memberFunc = new Regex(@"public(?: static)? (\w+) (\w+)\(");
                        var memberClass = new Regex(@"class (\w+)");
                        var matchProp = memberProperty.Match(line);
                        if (matchProp.Success)
                        {
                            var propName = matchProp.Groups[1].Value;
                            handler.HandleProp(cls, propName, line);
                        }
                        else
                        {
                            var matchFunc = memberFunc.Match(line);
                            if (matchFunc.Success)
                            {
                                var funcName = matchFunc.Groups[2].Value;
                                // var funcBodyIndent = new string('\t', indentCount + 1);
                                while (!lines[pos].StartsWith(indentTabs+"{"))
                                {
                                    pos++;
                                }
                                pos++;
                                var beginPos = pos;
                                while (!lines[pos].StartsWith(indentTabs+"}"))
                                {
                                    pos++;
                                }
                                var endPos = pos;
                                pos++;
                                handler.HandleFunc(cls, funcName, line, beginPos, endPos);
                            }
                            else
                            {
                                var matchClass = memberClass.Match(line);
                                if (matchClass.Success)
                                {
                                    var className = matchClass.Groups[1].Value;
                                    isClass = true;
                                    subClass = cls.GetNestedType(className);
                                    if (subClass == null)
                                    {
                                        throw new Exception("class cannot be null");
                                    }
                                    handler.HandleClassBegin(cls, subClass, line);
                                }
                            }
                        }
                    }
                }

                pos++;
            }
        }
        public void ParseContent(Type root, string[] lines, LineHandler handler)
        {
            var indentTabs = new string('\t', 2);
            var pos = 0;
            for (pos = 0; pos < lines.Length; pos++)
            {
                if (lines[pos].StartsWith(indentTabs))
                {
                    break;
                }
            }
            ParseClassMembers(root,lines,ref pos,2, handler);
        }

        public class LineHandler
        {
            public Action<Type, string, string> HandleProp;
            public Action<Type, string, string, int, int> HandleFunc;
            public Action<Type, Type, string> HandleClassBegin;
            public Action<Type, Type> HandleClassEnd;
        }
        public void ModifyCode(EnvInfo envInfo, Type root, string content)
        {
            ClassInfo curCls = envInfo;
            var invalidClsStack = 0;
            var lines = content.Replace("\r","").Split('\n');
            ParseContent(root, lines, new LineHandler
            {
                HandleProp = (cls, propName, line) =>
                {
                    if (curCls.MemberMap.TryGetValue(propName, out var member))
                    {
                        member.MemberManualCodeLine = line;

                        var prop = cls.GetProperty(propName)?.PropertyType;
                        if (prop == null)
                        {
                            throw new Exception($"prop cannot be null: {propName}");
                        }
                        if (member.Type.Name != prop.Name)
                        {
                            member.Type.ChangeName(prop.Name);
                        }
                    }
                },
                HandleFunc = (cls, funcName, line, beginPos, endPos) =>
                {
                    if (curCls.MemberMap.TryGetValue(funcName, out var member))
                    {
                        member.MemberManualCodeLine = line;
                        var funcInfo = member.Type as FuncInfo;
                        if (funcInfo == null)
                        {
                            throw new Exception($"func info cannot be null: {funcName}");
                        }
                        funcInfo.FuncBodyManualCodeLines = lines.Slice(beginPos, endPos);
                    }
                },
                HandleClassBegin = (cls, subClass, line) =>
                {
                    if(curCls.InsideTypeMap.TryGetValue(subClass.Name,out var subClsInfo))
                    {
                        curCls = subClsInfo;
                    }
                    else
                    {
                        invalidClsStack++;
                    }
                },
                HandleClassEnd = (cls, subClass) =>
                {
                    if (invalidClsStack > 0)
                    {
                        invalidClsStack--;
                    }
                    else
                    {
                        curCls = curCls.Parent;
                    }
                },
            });
        }
    }
}