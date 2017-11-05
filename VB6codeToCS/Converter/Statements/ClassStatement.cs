using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VB6codeToCS.Converter.Statements
{
    class ClassStatement : ControlStatement
    {
        private List<SubStatement> MethodList
        {
            get
            {
                var list = new List<SubStatement>();

                foreach (var child in Children)
                {
                    if (child is SubStatement)
                    {
                        list.Add((SubStatement)child);
                    }
                }
                return list;
            }
        }

        private List<string> MethodNames
        {
            get
            {
                var list = new List<string>();

                foreach (var method in MethodList)
                {
                    var name = method.Name;

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        list.Add(name);
                    }
                }
                return list;
            }
        }

        protected override void End()
        {
            var methodNames = MethodNames;

            foreach (var method in MethodList)
            {
                foreach (var child in method.Children)
                {
                    foreach (var line in child.Lines)
                    {
                        var replaced = line.Statement;

                        if (replaced.Contains("="))
                        {
                            continue;
                        }
                        foreach (var name in methodNames)
                        {
                            replaced = Regex.Replace(replaced, $"\\b{name}\\b((?![(]).*)", $"{name}($1)");
                        }
                        line.Statement = replaced;
                    }
                }
            }
        }

        public void EndLoad()
        {
            End();
        }
    }
}
