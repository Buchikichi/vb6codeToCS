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
        private readonly string[] Excludes = { "^VERSION", "^Object", "^Attribute", "^Option", "^On Error Resume" };

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

        private StatementLine Load(string filename)
        {
            var begin = 0;
            var lines = File.ReadAllLines(filename, Encoding.GetEncoding("Shift_JIS"));
            var statement = new ClassStatement();
            var replacer = new Replacer("replace.a.txt");

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
                var replaced = replacer.Replace(line.TrimEnd());

                statement.Add(replaced);
            }
            statement.EndLoad();
            return statement;
        }
        #endregion

        #region Replace
        private void Prepare(StatementLine statement)
        {
            var replacer = new Replacer("replace.b.txt");

            foreach (var stmt in statement.ListStatements())
            {
                foreach (var line in stmt.Lines)
                {
                    line.Statement = replacer.Replace(line.Statement);
                }
            }
        }

        private void Finish(StatementLine statement)
        {
            var replacer = new Replacer("replace.c.txt");

            foreach (var stmt in statement.ListStatements())
            {
                foreach (var line in stmt.Lines)
                {
                    line.Statement = replacer.Replace(line.Statement);
                }
            }
        }
        #endregion

        #region Convert
        private void Save(string name, StatementLine statement)
        {
            var contents = new List<string>();

            foreach (var stmt in statement.ListStatements())
            {
                var line = stmt.ToString();

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
