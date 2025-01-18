using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMK_GE
{
    internal class ge2snes
    {
        public static List<string> GetGhosts(string COM)
        {

            Dotnet2Snes.Dotnet2Snes connection = new Dotnet2Snes.Dotnet2Snes(8080);
            _ = connection.Attach(COM);
            var FileList = connection.List("/sd2snes/saves");
            Debug.WriteLine(FileList.Result.Item2[2]);
            List<string> Files = new List<string>();
            for (var i = 2; i < FileList.Result.Item2.Count; i++)
            {
                Files.Add(FileList.Result.Item2[i]);
            }
            return Files;
        }
    }
}
