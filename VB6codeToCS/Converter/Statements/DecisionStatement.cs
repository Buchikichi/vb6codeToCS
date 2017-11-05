using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VB6codeToCS.Converter.Statements
{
    class DecisionStatement : ControlStatement
    {
        private Dictionary<string, string> dict = new Dictionary<string, string>()
        {
            { "=", "==" }, { "And", "&&" }, { "Or", "||" }
        };
        public bool Scanned { get; private set; }

        protected override bool IsEnd(string line)
        {
            var trim = line.Trim();

            return trim == "End If";
        }

        private void Convert(SingleLine singleLine)
        {
            var statement = singleLine.Statement;
            var regex = new Regex("(\\s+|[!-z]+)");
            var buff = new StringBuilder();

            foreach (Match m in regex.Matches(statement))
            {
                var value = m.Value;

                if (!Scanned && dict.ContainsKey(value))
                {
                    value = dict[value];
                }
                else if (value == "Then")
                {
                    Scanned = true;
                }
                buff.Append(value);
            }
            singleLine.Statement = buff.ToString();
            //Debug.WriteLine($">>[{singleLine.Statement}]");
        }

        public override void AddLine(SingleLine singleLine)
        {
            //Debug.WriteLine($"IF[{singleLine.Statement}]");
            if (!Scanned)
            {
                Convert(singleLine);
            }
            base.AddLine(singleLine);
        }
    }
}
