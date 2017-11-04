using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using VB6codeToCS.Converter.Statements;

namespace VB6codeToCS.Converter
{
    class Vb6csConverter
    {
        private const string Indent = "        ";
        private readonly string[] Excludes = { "^VERSION", "^Object", "^Attribute", "^Option", "^[\\s]*On Error" };
        private readonly string[] Controls = { "[\\s]+If.+", "[\\s]+Case.+" };

        #region Load
        private bool Exclude(string line)
        {
            bool exclude = false;

            foreach (var pattern in Excludes)
            {
                if (Regex.IsMatch(line, pattern))
                {
                    exclude = true;
                    break;
                }
            }
            return exclude;
        }

        private bool IsControl(string line)
        {
            bool isControl = false;

            foreach (var pattern in Controls)
            {
                if (Regex.IsMatch(line, pattern))
                {
                    isControl = true;
                    break;
                }
            }
            return isControl;
        }

        private List<StatementLine> Load(string filename)
        {
            var resultList = new List<StatementLine>();
            var begin = 0;
            var lines = File.ReadAllLines(filename, Encoding.GetEncoding("Shift_JIS"));
            StatementLine currentLine = null;

            foreach (var line in lines)
            {
                var lower = line.Trim().ToLower();

                if (Exclude(line))
                {
                    continue;
                }
                if (lower == "begin" || lower.StartsWith("begin "))
                {
                    begin++;
                }
                if (lower == "end")
                {
                    begin--;
                    continue;
                }
                if (0 < begin)
                {
                    continue;
                }
                if (!line.EndsWith("_"))
                {
                    if (IsControl(line))
                    {
                        currentLine = new ControlStatement();
                    }
                    else
                    {
                        currentLine = new StatementLine();
                    }
                    resultList.Add(currentLine);
                }
                if (currentLine != null)
                {
                    currentLine.Add(line.TrimEnd());
                }
            }
            return resultList;
        }
        #endregion

        #region Replace
        private void Prepare(List<StatementLine> lines)
        {
            var replacer = new Replacer("replace.b.txt");

            foreach (var statementLine in lines)
            {
                foreach (var line in statementLine.Lines)
                {
                    line.Statement = replacer.Replace(line.Statement);
                }
            }
        }

        private void Finish(List<StatementLine> lines)
        {
            var replacer = new Replacer("replace.c.txt");

            foreach (var statementLine in lines)
            {
                foreach (var line in statementLine.Lines)
                {
                    line.Statement = replacer.Replace(line.Statement);
                }
            }
        }
        #endregion

        #region Convert
        private void Save(string name, List<StatementLine> lines)
        {
            var contents = new List<string>();

            foreach (var statementLine in lines)
            {
                var line = statementLine.ToString();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    line = Indent + line;
                }
                contents.Add(line);
            }
            File.WriteAllLines(name, contents);
        }

        public void Convert(string filename)
        {
            var lines = Load(filename);
            var newName = filename + ".cs";

            Prepare(lines);
            Finish(lines);
            Save(newName, lines);
        }
        #endregion
    }
}
