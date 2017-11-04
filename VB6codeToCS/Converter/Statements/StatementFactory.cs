using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VB6codeToCS.Converter.Statements
{
    class StatementFactory
    {
        private static Dictionary<string, Type> dict = new Dictionary<string, Type>()
        {
            {"[\\s]*Sub ", typeof(SubStatement)},
            {"[\\s]*Function ", typeof(SubStatement)},
            {"[\\s]*With ", typeof(WithStatement)}
        };

        public static StatementLine Create(string line)
        {
            StatementLine result = null;

            foreach (var pair in dict)
            {
                if (Regex.IsMatch(line, pair.Key))
                {
                    //Debug.WriteLine($"IsMatch[{pair.Key}]");
                    result = (StatementLine)Activator.CreateInstance(pair.Value);
                    break;
                }
            }
            if (result == null)
            {
                result = new StatementLine();
            }
            result.AddLine(new SingleLine(line));
            return result;
        }
    }
}
