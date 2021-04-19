using EngineerLib;
using System;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Diagnostics;

namespace Engineer
{
    public partial class Main : Form
    {
        string[] arguments = { };
        public Main(string[] arguments)
        {
            this.arguments = arguments;
            InitializeComponent();
        }

        public static string logs = "";
        public string savePath = "NULL";

        private void Main_Load(object sender, EventArgs e)
        {
            FileAssociations.SetAssociation(".eng", "ENGINEER_EDITOR_FILE", "Engineer project", 
                Assembly.GetExecutingAssembly().Location);

            rtbInput.CanPaste(DataFormats.GetFormat(DataFormats.UnicodeText));
            rtbInput.BringToFront();
            rtbInput.Focus();
            if (arguments.Length == 2)
            {
                string mode = arguments[0];

                if (mode == "-file")
                {
                    rtbInput.Text = File.ReadAllText(arguments[1]);
                }
                else if (mode == "-string")
                {
                    rtbInput.Text = arguments[1];
                }
                else
                {
                    MessageBox.Show("Invalid startup argument \"" + arguments[0] + "\"");
                }
            }
            else if (arguments.Length == 1)
            {
                savePath = arguments[0];
                this.Text = "Engineer - " + Path.GetFileName(arguments[0]);
                isEdited = true;
                rtbInput.Text = File.ReadAllText(arguments[0]);
                isEdited = false;
            }

            WebClient newsDownloader = new WebClient();
            newsDownloader.DownloadStringCompleted += NewsDownloader_DownloadStringCompleted;
            newsDownloader.DownloadStringAsync(new Uri("https://github.com/MinecraftPublisher/Engineer/raw/main/news.txt"));
        }

        private void NewsDownloader_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            lblNews.Text = e.Result;
        }

        private void NewsDownloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            logs = "";
            string result = EngineerLibParser.parseData(rtbInput.Text, logString);

            if (result == "ERROR")
            {
                MessageBox.Show("Failed to parse code!\nCheck the logs for more info.");
            }
            else
            {
                CompileForm compiler = new CompileForm(Regex.Replace(result, "\"", "\\\""));
                compiler.ShowDialog();
            }
        }

        private string logString(string input, int index)
        {
            logs += "[" + DateTime.Now.ToString() + " ] Line " + index + " -> " + input + "\n";
            richTextBox1.Text = "LogBox - Logs will appear here.\n" + logs;

            return input;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Engineer Files|*.eng";
            ofd.Title = "Choose a file to open";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                savePath = ofd.FileName;
                isEdited = true;
                this.Text = "Engineer - " + Path.GetFileName(savePath);
                isEdited = false;
                rtbInput.Text = File.ReadAllText(savePath);
            }
        }

        bool isEdited = false;

        private void rtbInput_TextChanged(object sender, EventArgs e)
        {
            if (isEdited)
            {
                return;
            }
            else
            {
                isEdited = true;
                this.Text = this.Text + "*";
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (savePath == "NULL")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Engineer Files|*.eng";
                sfd.Title = "Choose a path to save";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    savePath = sfd.FileName;
                    File.WriteAllText(savePath, rtbInput.Text);
                    if (isEdited)
                    {
                        this.Text = "Engineer - " + Path.GetFileName(savePath);
                        isEdited = false;
                    }
                }
            }
            else
            {
                File.WriteAllText(savePath, rtbInput.Text);
                if (isEdited)
                {
                    this.Text = "Engineer - " + Path.GetFileName(savePath);
                    isEdited = false;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Engineer Files|*.eng";
            sfd.Title = "Choose a path to save";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                savePath = sfd.FileName;
                File.WriteAllText(savePath, rtbInput.Text);
                if (isEdited)
                {
                    this.Text = "Engineer - " + Path.GetFileName(savePath);
                    isEdited = false;
                }
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(isEdited)
            {
                if(MessageBox.Show("You haven't saved your file, do you want to save it now?", "Unsaved project", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                }
            }
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
    public class FileAssociation
    {
        public string Extension { get; set; }
        public string ProgId { get; set; }
        public string FileTypeDescription { get; set; }
        public string ExecutableFilePath { get; set; }
    }

    public class FileAssociations
    {
        // needed so that Explorer windows get refreshed after the registry is updated
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        public static void EnsureAssociationsSet()
        {
            var filePath = Process.GetCurrentProcess().MainModule.FileName;
            EnsureAssociationsSet(
                new FileAssociation
                {
                    Extension = ".eng",
                    ProgId = "ENGINEER_EDITOR_FILE",
                    FileTypeDescription = "Engineer project",
                    ExecutableFilePath = filePath
                });
        }

        public static void EnsureAssociationsSet(params FileAssociation[] associations)
        {
            bool madeChanges = false;
            foreach (var association in associations)
            {
                madeChanges |= SetAssociation(
                    association.Extension,
                    association.ProgId,
                    association.FileTypeDescription,
                    association.ExecutableFilePath);
            }

            if (madeChanges)
            {
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public static bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
            return madeChanges;
        }

        private static bool SetKeyDefaultValue(string keyPath, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key.GetValue(null) as string != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }
    }
}
