using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VB6codeToCS.Converter;

namespace VB6codeToCS
{
    public partial class MainForm : Form
    {
        #region Convert
        private bool IsTarget(string name)
        {
            var lower = name.ToLower();

            return lower.EndsWith(".bas") || lower.EndsWith(".cls") || lower.EndsWith(".frm");
        }

        private List<string> ListFiles(string[] files)
        {
            var fileSet = new HashSet<string>();

            foreach (var name in files)
            {
                if (Directory.Exists(name))
                {
                    var subs = Directory.GetFiles(name, "*", SearchOption.AllDirectories);

                    foreach (var sub in subs)
                    {
                        if (IsTarget(sub))
                        {
                            fileSet.Add(sub);
                        }
                    }
                }
                else if (IsTarget(name))
                {
                    fileSet.Add(name);
                }
            }
            return fileSet.ToList();
        }

        private void Convert(string[] files)
        {
            var converter = new Vb6csConverter();
            var list = ListFiles(files);

            foreach (var name in list)
            {
                Debug.WriteLine($"[{name}]");
                converter.Convert(name);
            }
        }
        #endregion

        #region Initialize
        private void Initialize()
        {
            DragEnter += (sender, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            };
            DragDrop += (sender, e) =>
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                Convert(files);
            };
            ExitButton.Click += (sender, e) => Close();
        }

        public MainForm()
        {
            InitializeComponent();
            Initialize();
        }
        #endregion
    }
}
