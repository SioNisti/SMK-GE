using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace SMK_GE
{
    public partial class SaveGhostDialog : Form
    {
        public string GameRegion = "USA";
        public string COM = "";
        public ListBox listbox;
        public SaveGhostDialog(bool US, string COM, ListBox listbox1)
        {
            listbox = listbox1;
            InitializeComponent();
            this.COM = COM;
            if (!US)
            {
                GameRegion = "Japan";
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory("temp");

            string GhostName = textBox1.Text;
            if (!textBox1.Text.Contains(".srm"))
            {
                GhostName = $"{textBox1.Text}.srm";
            }
            int epoch = (int)Math.Floor((DateTime.Now - DateTime.UnixEpoch).TotalSeconds);

            Debug.WriteLine(GhostName);
            Debug.WriteLine($"/sd2snes/saves/Super Mario Kart ({GameRegion}).srm");
            Debug.WriteLine($"temp/{epoch}{GameRegion}.srm");
            Debug.WriteLine($"/sd2snes/smkge/ghosts/{GhostName}");

            Dotnet2Snes.Dotnet2Snes connection = new Dotnet2Snes.Dotnet2Snes(8080);
            _ = connection.Attach(COM);
            _ = connection.MoveFromSnes(@$"/sd2snes/saves/Super Mario Kart ({GameRegion}).srm", @$"temp/{epoch}{GameRegion}.srm");
            
            //DOESNT WORK FOR SOMEREASON
            _ = connection.MoveToSnes(@$"temp/{epoch}{GameRegion}.srm", @$"/sd2snes/smkge/ghosts/{GhostName}.srm");
            
            MainForm.FillListBox(listbox);
            this.Close();
        }
    }
}
