using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace hearthclone_updater
{
    class Program
    {
        static void Main(string[] args)
        {
            //Delete file
            bool goodDelete = false;
            int attempts = 0;
            while (!goodDelete)
            {
                try
                {
                    attempts++;
                    if (File.Exists("launcher.exe"))
                    {
                        File.Delete("launcher.exe");
                    }
                    goodDelete = true;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
                if(attempts == 5 && !goodDelete)
                {
                    MessageBox.Show("The launcher could not be updated.");
                    Environment.Exit(0);
                }
            }
            //Download new file
            WebClient wc = new WebClient();
            bool goodDownload = false;
            attempts = 0;
            while (!goodDownload) {
                try
                {
                    attempts++;
                    wc.DownloadFile("http://polybellum.com/hearthclone/latest/launcher.exe", "launcher.exe");
                    goodDownload = true;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
                if (attempts == 5 && !goodDownload)
                {
                    MessageBox.Show("The launcher could not be updated.");
                    Environment.Exit(0);
                }
            }

            //Run game
            try
            {
                System.Diagnostics.Process.Start("hc.exe");
            }
            catch { MessageBox.Show("The game could not be started."); }
        }
    }
}
