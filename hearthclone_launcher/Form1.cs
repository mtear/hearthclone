using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hearthclone_launcher
{
    public partial class Form1 : Form
    {
        public static string VERSION_CODE = "0.0.2a";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object se, EventArgs ee)
        {
            button1.Text = "Updating...";
            button1.Enabled = false;
            CheckUpdateUpdater();
        }

        void CheckUpdateUpdater()
        {
            label1.Text = "Updating updater...";
            if (File.Exists("update.exe") && File.Exists("u.ver"))
            {
                string uver = File.ReadAllText("u.ver").Trim();
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += (sender, e) => {
                    if (VERSION_CODE != e.Result)
                    {
                        //Update updater
                        File.Delete("update.exe");
                        wc.DownloadFile("http://www.polybellum.com/hearthclone/latest/update.exe", "update.exe");
                        CheckUpdateGame();
                    }
                    else
                    {
                        CheckUpdateGame();
                    }
                };
                wc.DownloadStringAsync(new Uri("http://www.polybellum.com/hearthclone/latest/u.txt"));
            }
            else
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("http://www.polybellum.com/hearthclone/latest/update.exe", "update.exe");
                File.WriteAllText("u.ver", wc.DownloadString("http://www.polybellum.com/hearthclone/latest/u.txt"));
                CheckUpdateGame();
            }
        }

        void CheckUpdateGame()
        {
            label1.Text = "Updating game...";
            if (File.Exists("hc.exe") && File.Exists("v.ver"))
            {
                string uver = File.ReadAllText("v.ver").Trim();
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += (sender, e) => {
                    if (VERSION_CODE != e.Result)
                    {
                        //Update updater
                        File.Delete("hc.exe");
                        wc.DownloadFile("http://www.polybellum.com/hearthclone/latest/hc.exe", "hc.exe");
                        CheckUpdateLauncher();
                    }
                    else
                    {
                        CheckUpdateLauncher();
                    }
                };
                wc.DownloadStringAsync(new Uri("http://www.polybellum.com/hearthclone/latest/v.txt"));
            }
            else
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("http://www.polybellum.com/hearthclone/latest/hc.exe", "hc.exe");
                File.WriteAllText("v.ver", wc.DownloadString("http://www.polybellum.com/hearthclone/latest/v.txt"));
                CheckUpdateLauncher();
            }
        }

        void CheckUpdateLauncher()
        {
            label1.Text = "Updating launcher...";
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (sender, e) => {
                if (VERSION_CODE != e.Result)
                {
                    //Need to update launcher
                    Process.Start("update.exe");
                    Environment.Exit(0);
                }
                label1.Text = "Update complete";
                button1.Text = "Play";
                button1.Enabled = true;
            };
            wc.DownloadStringAsync(new Uri("http://www.polybellum.com/hearthclone/latest/l.txt"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("hc.exe");
            Environment.Exit(0);
        }
    }
}
