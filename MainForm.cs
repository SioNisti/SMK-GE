using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using SnesConnectorLibrary;
using SnesConnectorLibrary.Requests;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SMK_GE
{
    public partial class MainForm : Form
    {

        public List<Byte> fromGhost = new List<Byte>();
        public List<Byte> toGhost = new List<Byte>();
        public string toPath = "";
        public static string sd2snesCOM = "";

        public MainForm()
        {
            InitializeComponent();
            Task.Run(FindSD2SNES).Wait();
        }

        static void FindSD2SNES()
        {
            Dotnet2Snes.Dotnet2Snes connection = new Dotnet2Snes.Dotnet2Snes(8080);
            foreach (string thing in connection.GetDeviceList().Result)
            {
                string[] split = thing.Split(' ');
                sd2snesCOM = split[1];
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (sd2snesCOM != "")
            {
                try
                {
                    label1.Text = $"SD2SNES found on: {sd2snesCOM}";
                    Dotnet2Snes.Dotnet2Snes connection = new Dotnet2Snes.Dotnet2Snes(8080);
                    _ = connection.Attach(sd2snesCOM);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label1.Text = $"SD2SNES connection error";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new()
            {
                Filter = "SRM files (*.srm)|*.srm",
                FilterIndex = 0,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            fromGhost = new List<Byte>();
            string fromPath = openFileDialog1.FileName;

            using (var stream = new FileStream(fromPath, FileMode.Open, FileAccess.Read))
            {
                //goes through the first 1536 bytes (to hex 600) from the file
                for (int i = 0; i <= 1535; i++)
                {
                    fromGhost.Add((Byte)stream.ReadByte()); //saves the bytes into a list of bytes
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new()
            {
                Filter = "SRM files (*.srm)|*.srm",
                FilterIndex = 0,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            toGhost = new List<Byte>();
            toPath = openFileDialog1.FileName;

            using (var stream = new FileStream(toPath, FileMode.Open, FileAccess.Read))
            {
                //all bytes from the file
                for (int i = 0; i < stream.Length; i++)
                {
                    toGhost.Add((Byte)stream.ReadByte()); //saves the bytes into a list of bytes
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (fromGhost.Count <= 0)
            {
                MessageBox.Show("Please select a save file to get a ghost from!", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (toGhost.Count <= 0)
            {
                MessageBox.Show("Please select a save file to send a ghost to!", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "SRM files (*.srm)|*.srm";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = Path.GetFileName(toPath);

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string savePath = saveFileDialog1.FileName;

            for (var i = 0; i < 1536; i++)
            {
                toGhost[i] = fromGhost[i];
            }
            var stream = new FileStream(savePath, FileMode.CreateNew, FileAccess.ReadWrite);
            foreach (var item in toGhost)
            {
                stream.WriteByte((byte)item);
            }
            stream.Close();
            MessageBox.Show("Ghost transferred!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillListBox(listBox1);
        }

        public static async void FillListBox(ListBox listBox1)
        {
            listBox1.Items.Clear();
            Dotnet2Snes.Dotnet2Snes connection = new Dotnet2Snes.Dotnet2Snes(8080);
            _ = connection.Attach(sd2snesCOM);
            var FileList = await connection.List("/sd2snes/smkge/ghosts");
            for (var i = 2; i < FileList.Item2.Count; i++)
            {
                listBox1.Items.Add(FileList.Item2[i]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            SaveGhostDialog SaveGhostDialog = new SaveGhostDialog(radioButton1.Checked, sd2snesCOM, listBox1);
            SaveGhostDialog.ShowDialog();
        }
    }
}
