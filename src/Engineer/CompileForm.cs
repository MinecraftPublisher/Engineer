using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Microsoft.CSharp;

namespace Engineer
{
    public partial class CompileForm : Form
    {
        string givenCode;
        string iconPath = "NULL";
        public CompileForm(string csharpCode)
        {
            givenCode = csharpCode;
            InitializeComponent();
        }

        [Obsolete]
        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtGame.Text;
            string version = txtVersion.Text;
            string owner = txtOwner.Text;
            bool isDebugMode = checkBox1.Checked;

            if(name == "" || version == "" || owner == "")
            {
                MessageBox.Show("You left one of the text boxes blank... Why tho?");
                txtStatus.Text = "Compilation failed due to dumb";
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Choose a folder to save your game";
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtStatus.Text = "Preparing files...";
                    string selectedPath = fbd.SelectedPath;
                    string folderPath = Path.Combine(selectedPath, name);
                    txtStatus.Text = "Writing DLLs...";
                    Directory.CreateDirectory(folderPath);
                    File.WriteAllBytes(Path.Combine(folderPath, "Newtonsoft.Json.dll"), Properties.Resources.Newtonsoft_Json);
                    txtStatus.Text = "Compiling game...";
                    string code = replace(Properties.Resources.code, "DATA_GAME_JSON", givenCode);
                    code = replace(code, "DATA_GAME_NAME", name);
                    code = replace(code, "DATA_GAME_VERSION", version);
                    code = replace(code, "DATA_GAME_OWNER", owner);
                    code = replace(code, "DATA_GAME_DEBUG", isDebugMode.ToString().ToLower());

                    CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                    ICodeCompiler icc = codeProvider.CreateCompiler();
                    string Output = Path.Combine(folderPath, name + ".exe");

                    CompilerParameters options = new CompilerParameters();
                    options.GenerateExecutable = true;
                    options.OutputAssembly = Output;
                    if (iconPath != "NULL")
                    {
                        options.CompilerOptions = @"/win32icon:" + iconPath;
                    }
                    options.ReferencedAssemblies.Add("System.Data.dll");
                    options.ReferencedAssemblies.Add("System.dll");
                    options.ReferencedAssemblies.Add("System.Xml.dll");
                    options.ReferencedAssemblies.Add("System.Linq.dll");
                    options.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                    options.ReferencedAssemblies.Add("System.Core.dll");
                    options.ReferencedAssemblies.Add("Newtonsoft.Json.dll");
                    options.ReferencedAssemblies.Add("System.Threading.dll");
                    options.ReferencedAssemblies.Add("System.Threading.Tasks.dll");
                    options.ReferencedAssemblies.Add("System.IO.dll");
                    options.ReferencedAssemblies.Add("System.Text.RegularExpressions.dll");
                    CompilerResults results = icc.CompileAssemblyFromSource(options, code);

                    if (results.Errors.Count > 0)
                    {
                        foreach (CompilerError CompErr in results.Errors)
                        {
                            txtStatus.Text = "Compilation failure :(";
                            MessageBox.Show("Line number " + CompErr.Line + ", Error Number: " + CompErr.ErrorNumber + ", '" + CompErr.ErrorText + ";" + Environment.NewLine + Environment.NewLine);
                        }
                    }
                    else
                    {
                        txtStatus.Text = "Success!";
                        DialogResult result = MessageBox.Show("Do you want to run your game?", "Success!", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            Process.Start(Path.Combine(folderPath, name + ".exe"));
                        }
                        else
                        {
                            txtStatus.Text = "Game compiled to " + Path.Combine(folderPath, name + ".exe");
                        }
                    }
                }
                else
                {
                    txtStatus.Text = "Compilation aborted";
                }
            }
        }

        private string replace(string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        private void browseIcon_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose an icon";
            ofd.Filter = "Icon file|*.ico";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(ofd.FileName);
                iconPath = ofd.FileName;
                txtStatus.Text = "Icon applied!";
            }
            else
            {
                txtStatus.Text = "Icon selection aborted";
            }
        }

        private void CompileForm_Load(object sender, EventArgs e)
        {

        }
    }
}
