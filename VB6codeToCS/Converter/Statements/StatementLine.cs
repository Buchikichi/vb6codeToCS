using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VB6codeToCS.Converter.Statements
{
    class StatementLine
    {
        private int depth;
        private StatementLine parent;
        private StatementLine currentChild;
        private bool continues;
        protected List<StatementLine> Children = new List<StatementLine>();
        public readonly List<SingleLine> Lines = new List<SingleLine>();

        protected virtual bool IsEnd(string line)
        {
            return false;
        }

        protected virtual void End()
        {
        }

        public virtual void AddLine(SingleLine singleLine)
        {
            Lines.Add(singleLine);
        }

        public void Add(string line)
        {
            if (currentChild == null)
            {
                var child = StatementFactory.Create(line);

                Debug.WriteLine($"Create:{depth}[{line}]");
                Children.Add(child);
                if (child is ControlStatement)
                {
                    currentChild = child;
                    currentChild.depth = depth + 1;
                    currentChild.parent = this;
                    Debug.WriteLine($"ControlStatement:{currentChild.depth}");
                }
            }
            else if (continues)
            {
                Lines.Add(new SingleLine(line));
            }
            else
            {
                currentChild.Add(line);
                if (currentChild.IsEnd(line))
                {
                    currentChild.End();
                    currentChild = null;
                }
            }
            continues = line.EndsWith("_");
        }

        public override string ToString()
        {
            var buff = new StringBuilder();
            SingleLine last = null;

            foreach (var line in Lines)
            {
                line.Terminate = false;
                last = line;
            }
            last.Terminate = true;
            buff.Append(string.Join("\r\n", Lines));
            return buff.ToString();
        }

        public List<StatementLine> ListStatements()
        {
            var list = new List<StatementLine>
            {
                this
            };
            foreach (var child in Children)
            {
                list.AddRange(child.ListStatements());
            }
            return list;
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
