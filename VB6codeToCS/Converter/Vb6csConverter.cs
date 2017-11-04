using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace VB6codeToCS.Converter
{
    class Vb6csConverter
    {
        private const string Indent = "        ";
        private readonly string[] Excludes = { "^VERSION", "^Object", "^Attribute", "^Option" };

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

        private List<StatementLine> Load(string filename)
        {
            var resultList = new List<StatementLine>();
            var begin = 0;
            var lines = File.ReadAllLines(filename, Encoding.GetEncoding("Shift_JIS"));
            var replacer = new Replacer("replace.b.txt");
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
                    currentLine = new StatementLine();
                    resultList.Add(currentLine);
                }
                if (currentLine != null)
                {
                    var replaced = replacer.Replace(line.TrimEnd());

                    currentLine.Add(replaced);
                }
            }
            return resultList;
        }
        #endregion

        #region Finish
        private void Finish(List<StatementLine> lines)
        {
            var replacer = new Replacer("replace.c.txt");

            foreach (var statementLine in lines)
            {
                var oldLines = new List<string>();

                oldLines.AddRange(statementLine.Lines);
                statementLine.Lines.Clear();
                foreach (var line in oldLines)
                {
                    var replaced = replacer.Replace(line);

                    if (!string.IsNullOrWhiteSpace(replaced))
                    {
                        replaced = Indent + replaced;
                    }
                    statementLine.Lines.Add(replaced);
                }
            }
        }
        #endregion

        private void Save(string name, List<StatementLine> lines)
        {
            var contents = new List<string>();

            foreach (var statementLine in lines)
            {
                contents.AddRange(statementLine.Lines);
            }
            File.WriteAllLines(name, contents);
        }

        public void Convert(string filename)
        {
            var lines = Load(filename);
            var newName = filename + ".cs";

            Finish(lines);
            Save(newName, lines);
        }
    }

    class StatementLine
    {
        public void Add(string line)
        {
            Lines.Add(line);
        }

        #region Properties
        public List<string> Lines { get; set; } = new List<string>();
        #endregion
    }
}
