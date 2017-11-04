namespace VB6codeToCS.Converter.Statements
{
    class SubStatement : ControlStatement
    {
        protected override bool IsEnd(string line)
        {
            var trim = line.Trim();

            return !line.Contains("Lib") && (trim == "End Sub" || trim == "End Function");
        }
    }
}
