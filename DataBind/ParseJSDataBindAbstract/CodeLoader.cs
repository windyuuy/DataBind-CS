using System;
using System.Collections.Generic;
using System.Linq.Ext;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ParseJSDataBindAbstract
{
    public class AnnotationCollector
    {
        public List<string> Annotations = new();
        public static Regex AnnotationRegex = new(@"^\s*//");

        public bool FilterAnnotation(string line)
        {
            if (AnnotationRegex.IsMatch(line))
            {
                Annotations.Add(line);
                return true;
            }

            return false;
        }

        public string[] PopAnnotations()
        {
            var annos = Annotations.ToArray();
            Annotations.Clear();
            return annos;
        }
    }
    public class CodeLoader
    {
        public AnnotationCollector AnnotationCollector = new();
        public static readonly Regex TabsMatcher = new("(\t*)");
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
                            pos++;
                            ParseClassMembers(subClass,lines,ref pos,indentCount+1, handler);
                            handler.HandleClassEnd(cls, subClass);
                            isClass = false;
                            subClass = null;
                        }
                    }
                    else
                    {
                        if (!AnnotationCollector.FilterAnnotation(line))
                        {
                            var memberProperty = new Regex(@"(\w+) {get;set;}");
                            var memberFunc = new Regex(@"public(?: static)? (\w+) (\w+)\(");
                            var memberClass = new Regex(@"class (\w+)");
                            var matchProp = memberProperty.Match(line);
                            if (matchProp.Success)
                            {
                                var propName = matchProp.Groups[1].Value;
                                handler.HandleProp(line, cls, propName, AnnotationCollector.PopAnnotations());
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
                                    handler.HandleFunc(line, cls, funcName, beginPos, endPos, AnnotationCollector.PopAnnotations());
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
                                        handler.HandleClassBegin(line, cls, subClass, AnnotationCollector.PopAnnotations());
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // check line indent only
                    var m = TabsMatcher.Match(line);
                    if (m.Success)
                    {
                        if (m.Groups[1].Length != indentCount)
                        {
                            throw new Exception("unmatched indente count");
                        }
                    }
                }

                pos++;
            }
        }
        public void ParseContent(Type root, string[] lines, LineHandler handler)
        {
            var indentTabs = new string('\t', 2);
            var isParsingHead = true;
            var pos = 0;
            for (pos = 0; pos < lines.Length; pos++)
            {
                if (lines[pos].StartsWith(indentTabs))
                {
                    break;
                }

                var line = lines[pos];
                if (isParsingHead)
                {
                    if (line.StartsWith("namespace "))
                    {
                        isParsingHead = false;
                    }
                    else
                    {
                        handler.HandleFileHeader(line);
                    }
                }
            }
            ParseClassMembers(root,lines,ref pos,2, handler);
        }

        public class LineHandler
        {
            public Action<string> HandleFileHeader;
            public Action<string, Type, string, string[]> HandleProp;
            public Action<string, Type, string, int, int, string[]> HandleFunc;
            public Action<string, Type, Type, string[]> HandleClassBegin;
            public Action<Type, Type> HandleClassEnd;
        }
        public void ModifyCode(EnvInfo envInfo, Type root, string content)
        {
            ClassInfo curCls = envInfo;
            var invalidClsStack = 0;
            var lines = content.Replace("\r","").Split('\n');
            ParseContent(root, lines, new LineHandler
            {
                HandleFileHeader = (line)=>
                {
                    envInfo.FileHeaders.Add(line);
                },
                HandleProp = (line, cls, propName, annos) =>
                {
                    if (curCls.MemberMap.TryGetValue(propName, out var member))
                    {
                        member.MemberManualCodeLine = line;
                        member.AnnotationLines = annos;

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
                HandleFunc = (line, cls, funcName,  beginPos, endPos, annos) =>
                {
                    if (curCls.MemberMap.TryGetValue(funcName, out var member))
                    {
                        member.MemberManualCodeLine = line;
                        if (member.Type is not FuncInfo funcInfo)
                        {
                            if (member.Type != null)
                            {
                                funcInfo = member.CastToFunc();
                            }
                            else
                            {
                                throw new Exception($"func info cannot be null: {funcName}");
                            }
                        }
                        funcInfo.FuncBodyManualCodeLines = lines.Slice(beginPos, endPos);
                        member.AnnotationLines = funcInfo.AnnotationLines = annos;
                    }
                },
                HandleClassBegin = (line, cls, subClass, annos) =>
                {
                    if(curCls.InsideTypeMap.TryGetValue(subClass.Name,out var subClsInfo))
                    {
                        curCls = subClsInfo;
                        curCls.AnnotationLines = annos;
                        curCls.TypeDefManualCodeLine = line;
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