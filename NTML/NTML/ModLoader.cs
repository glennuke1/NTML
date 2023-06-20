using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace NTML
{
    public partial class ModLoader : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        private string _modsPath = "";

        public ModLoader()
        {
            InitializeComponent();
        }

        public static void PopulateListBox(ListBox lsb, string Folder, string FileType)
        {
            DirectoryInfo dinfo = new DirectoryInfo(Folder);
            FileInfo[] Files = dinfo.GetFiles(FileType);
            foreach (FileInfo file in Files)
            {
                lsb.Items.Add(file.Name);
            }
        }

        private void ModLoader_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 5, 5));

            foreach (Button button in GetAll(this, typeof(Button)))
            {
                button.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 2, 2));
            }

            string configFilePath = Path.Combine(Application.UserAppDataPath, "config.txt");
            if (File.Exists(configFilePath))
            {
                _modsPath = File.ReadAllText(configFilePath);
            }
            else
            {
                _modsPath = Path.Combine(Application.StartupPath, "Mods");
            }
            label1.Text = _modsPath;

            if (!Directory.Exists(_modsPath))
            {
                Directory.CreateDirectory(_modsPath);
            }

            listBox1.Items.Clear();
            PopulateListBox(listBox1, _modsPath, "*.dll");

            comboBox1.SelectedIndex = 0;
        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        private void buttonSelectModsFolder_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = _modsPath;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _modsPath = folderBrowserDialog.SelectedPath;

                listBox1.Items.Clear();
                label1.Text = _modsPath;

                string configFilePath = Path.Combine(Application.UserAppDataPath, "config.txt");
                File.WriteAllText(configFilePath, _modsPath);
            }
        }

        private void buttonInstallMod_Click(object sender, EventArgs e)
        {
            string filePath = Directory.GetParent(_modsPath) + "/Managed/Assembly-CSharp.dll";
            string filePath2 = Directory.GetParent(_modsPath) + "/Managed/NTLoader.dll";

            string fileUrl = "https://drive.google.com/u/0/uc?id=1Ly9LmyBxYUl0uAWj_jgQM1EH-5VKOyw4&export=download"; //Assembly-CSharp.dll https://drive.google.com/u/0/uc?id=1Ly9LmyBxYUl0uAWj_jgQM1EH-5VKOyw4&export=download
            string fileUrl2 = "https://drive.google.com/u/0/uc?id=1jYCbEYdUgeNUyfe725gYiePefYV5YwJh&export=download"; //NTLoader.dll https://drive.google.com/u/0/uc?id=1jYCbEYdUgeNUyfe725gYiePefYV5YwJh&export=download

            if (!File.Exists(filePath2))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(fileUrl2, filePath2);
                }
            }

            try
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(fileUrl, filePath);
                    }
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    string legacyFileUrl = "https://drive.google.com/u/0/uc?id=1RPCwmmnR6BQ1oe9Dx9cdXvv_ITUfD1tZ&export=download";
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(legacyFileUrl, filePath);
                    }
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    string legacyFileUrl = "https://drive.google.com/u/0/uc?id=18tROArgWoEQSZq2hgOJjUa5ge0BR7h3V&export=download";
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(legacyFileUrl, filePath);
                    }
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
            MessageBox.Show("Install successful");
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            PopulateListBox(listBox1, _modsPath, "*.dll");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        Point lastPoint;

        private void ModLoader_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void ModLoader_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left += e.X - lastPoint.X;
                Top += e.Y - lastPoint.Y;
            }
        }

        private void TopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left += e.X - lastPoint.X;
                Top += e.Y - lastPoint.Y;
            }
        }
    }
}
