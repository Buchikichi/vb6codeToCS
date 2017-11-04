using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace VB6codeToCS.Converter
{
    class Replacer
    {
        private List<ReplacePattern> list = new List<ReplacePattern>();

        public string Replace(string text)
        {
            var replaced = text;

            foreach (var pat in list)
            {
                replaced = Regex.Replace(replaced, pat.Pattern, pat.Replacement);
            }
            return replaced;
        }

        #region Initialize
        private void Load(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("VB6codeToCS.Resources." + name);

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var elements = line.Split('\t');

                    if (elements.Length == 1)
                    {
                        list.Add(new ReplacePattern()
                        {
                            Pattern = elements[0],
                            Replacement = string.Empty,
                        });
                    }
                    else if (2 <= elements.Length)
                    {
                        list.Add(new ReplacePattern()
                        {
                            Pattern = elements[0],
                            Replacement = elements[1],
                        });
                    }
                }
            }
            list.Sort((a, b) => b.Pattern.Length - a.Pattern.Length);
        }

        public Replacer(string name)
        {
            Load(name);
        }
        #endregion

        class ReplacePattern
        {
            public string Pattern { get; set; }
            public string Replacement { get; set; }
        }
    }
}
