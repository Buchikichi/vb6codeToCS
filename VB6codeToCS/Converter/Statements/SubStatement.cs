using System.Text.RegularExpressions;

namespace VB6codeToCS.Converter.Statements
{
    class SubStatement : ControlStatement
    {
        public string Name { get; private set; }

        protected override bool IsEnd(string line)
        {
            var trim = line.Trim();

            return !line.Contains("Lib") && (trim == "End Sub" || trim == "End Function");
        }

        public override void AddLine(SingleLine singleLine)
        {
            var statement = singleLine.Statement.Trim();

            Name = Regex.Replace(statement, ".*[\\s]+([A-Za-z0-9_]+)[(].*", "$1");
            base.AddLine(singleLine);
        }
    }
}
