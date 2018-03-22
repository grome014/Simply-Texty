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
using System.Diagnostics;
using System.Collections;

namespace Group8_Homework8
{
    public partial class SearchDialog : Form
    {
        public event EventHandler Accept;
        public event EventHandler Double_Click;

        private string fileExtension;
        private string fileText;
        private ArrayList filePaths;
        private bool m_bClosing;
        private bool threadPaused;
        private int numOfFiles;
        protected delegate void ShowProgressDelegate(FileInfo file);
        protected delegate void FileExtensionDelegate(object sender, EventArgs e);

        public string FileText
        {
            get { return fileText; }
            set
            {
                fileText = value;
            }
        }

        public SearchDialog()
        {
            InitializeComponent();
            fileExtension = "*.txt";
            m_bClosing = false;
            threadPaused = false;
            numOfFiles = 0;
            filePaths = new ArrayList();
            this.extensionsComboBox.Items.Add("*.txt");
            this.extensionsComboBox.Items.Add("*.html");
            this.extensionsComboBox.Items.Add("*.htm");
            this.extensionsComboBox.SelectedItem = "*.txt";
            this.filesListBox.HorizontalScrollbar = true;
            InitializeHelp();
        }

        private void InitializeHelp()
        {
            this.helpProvider.SetHelpString(filesListBox, "List of files found.");
            this.helpProvider.SetHelpString(startButton, "Start search.");
            this.helpProvider.SetHelpString(stopButton, "Stop search.");
            this.helpProvider.SetHelpString(pauseButton, "Pause search.");
            this.helpProvider.SetHelpString(extensionsComboBox, "Search for files using this extension.");
        }

        //you will need System.IO and System.Diagnostics
        //Change the configuration to Debug to see a list of folders that are being read.
        //Change to release to see just the folders that cannot be read.
        //The search will be faster in Release configuration

        private void Search()
        {
            foreach (String drive in Directory.GetLogicalDrives())
            {
                if (this.backgroundWorker.CancellationPending) { return; }
                pauseThread();
                Debug.WriteLine(drive);
                foreach (DirectoryInfo child in getDirectories(drive))
                {
                    if (this.backgroundWorker.CancellationPending) { return; }
                    pauseThread();
                    Debug.WriteLine(child.FullName);
                    FindFiles(child);
                }
            }
        }

        private void FindFiles(DirectoryInfo dir)
        {
            try
            {
                DirectoryInfo[] children = getDirectories(dir);
                if (children.Length > 0)
                {
                    foreach (DirectoryInfo child in children)
                    {
                        if (this.backgroundWorker.CancellationPending) { return; }
                        pauseThread();
                        Debug.WriteLine(child.FullName);
                        FindFiles(child);
                    }
                }
                else
                {
                    if (this.backgroundWorker.CancellationPending) { return; }
                    pauseThread();
                    FileInfo[] Files = dir.GetFiles(fileExtension);
                    if (Files.Length > 0)
                    {
                        if (this.backgroundWorker.CancellationPending) { return; }
                        pauseThread();
                        //Found some text files.
                        //Do something
                        foreach (FileInfo files in Files)
                        {
                            this.backgroundWorker.ReportProgress(0, files);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private void AddItem(FileInfo file)
        {
            Debug.Assert(this.InvokeRequired == false);
            if (file != null)
            {
                this.filesListBox.Items.Add(file.FullName);
                numOfFiles++;
            }
        }

        private bool AttrOn(FileAttributes attr, FileAttributes field)
        {
            return (attr & field) == field;
        }

        public DirectoryInfo[] getDirectories(DirectoryInfo dir)
        {
            if (AttrOn(dir.Attributes, FileAttributes.Offline))
            {
                Console.Out.WriteLine(dir.Name + " is not mapped ");
                return new DirectoryInfo[] { };
            }
            if (!dir.Exists)
            {
                Console.Out.WriteLine(dir.Name + " does not exist ");
                return new DirectoryInfo[] { };
            }
            try
            {
                return dir.GetDirectories();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(ex.StackTrace);
                return new DirectoryInfo[] { };
            }
        }

        public DirectoryInfo[] getDirectories(String strDrive)
        {
            DirectoryInfo dir = new DirectoryInfo(strDrive);
            return getDirectories(dir);
        }

        //File extensions
        private void extensionsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.Assert(this.InvokeRequired == false);
            fileExtension = extensionsComboBox.SelectedItem as string;
        }

        private void ChangeExtension(string extension)
        {
            Debug.Assert(this.InvokeRequired == false);
            fileExtension = extension;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            this.startButton.Enabled = false;
            this.stopButton.Enabled = true;
            this.pauseButton.Enabled = true;
            threadPaused = false;
            numOfFiles = 0;

            this.filesListBox.Items.Clear();
            this.filesListBox.Items.Add("***Beginning search! Please wait while your files are being found.***");
            // Begin
            this.backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Search();

            // Indicate cancelation
            if (this.backgroundWorker != null && this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Reset UI
            this.startButton.Enabled = true;
            this.stopButton.Enabled = false;
            this.pauseButton.Enabled = false;
            this.pauseButton.Text = "Pause";

            // Was the worker thread cancelled?
            if (e.Cancelled)
            {
                this.filesListBox.Items.Add("***Search stopped!***");
            }
            else
            {
                this.filesListBox.Items.Add("***Search Complete!***");
                MessageBox.Show(numOfFiles + " files were found.");
            }
            if (m_bClosing)
            {
                MessageBox.Show("Closing after thread was cancelled.");
                this.Close();
                m_bClosing = false;
            }
        }

        private void SearchDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
                m_bClosing = true;
                e.Cancel = true;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker.IsBusy)
            {
                this.stopButton.Enabled = false;
                this.pauseButton.Enabled = false;
                this.pauseButton.Text = "Pause";
                this.filesListBox.Items.Add("***Stopping search.***");
                this.backgroundWorker.CancelAsync();
                return;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //AddItem(e.UserState as FileInfo);
            ShowProgressDelegate progressDel = new ShowProgressDelegate(AddItem);
            FileExtensionDelegate extensionDel = new FileExtensionDelegate(extensionsComboBox_SelectedIndexChanged);
            this.Invoke(progressDel, e.UserState as FileInfo);
            this.Invoke(extensionDel, sender, e.UserState as EventArgs);
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (threadPaused)
            {
                this.pauseButton.Text = "Pause";
                threadPaused = false;
                this.filesListBox.Items.Add("***Continuing search.***");
            }
            else
            {
                this.pauseButton.Text = "Continue";
                threadPaused = true;
                this.filesListBox.Items.Add("***Paused.***");
            }
        }

        private void pauseThread()
        {
            while (threadPaused)
            {
                if (this.backgroundWorker.CancellationPending) { return; }
                System.Threading.Thread.Sleep(100);
            }
        }

        private void SearchDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Accept != null) Accept(this, EventArgs.Empty);
        }

        private void filesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!this.filesListBox.SelectedItem.ToString().Contains("*"))
            {
                fileText = System.IO.File.ReadAllText(this.filesListBox.SelectedItem.ToString());
                if (Double_Click != null)
                    Double_Click(this, EventArgs.Empty);
            }
        }
    }
}
