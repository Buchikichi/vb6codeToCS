namespace VB6codeToCS.Converter.Statements
{
    class ControlStatement : StatementLine
    {
        public override string ToString()
        {
            return string.Join("\r\n", Lines);
        }
    }
}
