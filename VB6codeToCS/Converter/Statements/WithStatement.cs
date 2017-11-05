using System.Text.RegularExpressions;

namespace VB6codeToCS.Converter.Statements
{
    class WithStatement : ControlStatement
    {
        public string Variable { get; private set; }

        protected override bool IsEnd(string line)
        {
            var trim = line.Trim();

            return trim == "End With";
        }

        protected override void End()
        {
            foreach (var child in Children)
            {
                foreach (var line in child.Lines)
                {
                    var replaced = line.Statement;

                    replaced = Regex.Replace(replaced, "(^|\\s+|[(])[.]", $"$1{Variable}.");
                    line.Statement = replaced;
                }
            }
        }

        public override void AddLine(SingleLine singleLine)
        {
            var statement = singleLine.Statement.Trim();
            var elements = statement.Split(' ');

            Variable = elements[1];
        }
    }
}
