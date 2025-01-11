using System.Diagnostics;
using System.IO;

namespace SMK_GE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public List<Byte> fromGhost = new List<Byte>();
        public List<Byte> toGhost = new List<Byte>();
        public string toPath = "";

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "SRM files (*.srm)|*.srm";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

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
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "SRM files (*.srm)|*.srm";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

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
            } else if (toGhost.Count <= 0) {
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

            for (var i = 0;i < 1536; i++)
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
    }
}
