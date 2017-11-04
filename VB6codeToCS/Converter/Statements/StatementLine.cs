using System.Collections.Generic;
using System.Text;

namespace VB6codeToCS.Converter.Statements
{
    class StatementLine
    {
        public readonly List<SingleLine> Lines = new List<SingleLine>();

        public void Add(string line)
        {
            Lines.Add(new SingleLine(line));
        }

        public override string ToString()
        {
            SingleLine last = null;

            foreach (var line in Lines)
            {
                line.Terminate = false;
                last = line;
            }
            last.Terminate = true;
            return string.Join("\r\n", Lines);
        }
    }

    class SingleLine
    {
        public override string ToString()
        {
            var buff = new StringBuilder();

            buff.Append(Statement);
            if (Terminate && !TerminatedStatement)
            {
                buff.Append(';');
            }
            if (comment != null)
            {
                if (!string.IsNullOrWhiteSpace(Statement))
                {
                    buff.Append(' ');
                }
                if (string.IsNullOrEmpty(Statement))
                {
                    buff.Append("/// ");
                }
                else
                {
                    buff.Append("// ");
                }
                buff.Append(comment);
            }
            return buff.ToString();
        }

        public SingleLine(string line)
        {
            var ix = line.IndexOf("'");

            if (ix != -1)
            {
                Statement = line.Substring(0, ix);
                if (!TerminatedStatement)
                {
                    Statement = Statement.TrimEnd();
                }
                comment = line.Substring(ix + 1).Trim();
            }
            else
            {
                Statement = line;
            }
        }

        #region Members
        private string comment;
        public virtual bool Terminate { get; set; }
        public string Statement { get; set; }
        public bool TerminatedStatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Statement))
                {
                    return true;
                }
                return Statement.EndsWith("{") || Statement.EndsWith("}");
            }
        }
        #endregion
    }
}
