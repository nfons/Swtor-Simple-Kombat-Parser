using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KombatParser
{
    public partial class overlay : Form
    {
        String Combatfile_Name;
        

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        Timer timer = new Timer();

        public overlay()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = (250) * (1);              // Timer will tick evert second
            timer.Enabled = true;                       // Enable the timer
            timer.Start();                              // Start the timer
            this.TopMost = true;


            IntPtr hWnd = FindWindow(null, "Star Wars: The Old Republic");
            RECT rect;
            GetWindowRect(hWnd, out rect);
            if (rect.X == -32000)
            {
                // the game is minimized
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Location = new Point(rect.X + 10, rect.Y + 10);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void timer_Tick(object sender, EventArgs e)
        {
            IntPtr hWnd = FindWindow(null, "Star Wars: The Old Republic");
            RECT rect;
            GetWindowRect(hWnd, out rect);
            Process[] pname = Process.GetProcessesByName("swtor");
            if (pname.Length == 0)
                this.label1.Text = "not running";
            else
                this.label1.Text = "running";

            if (rect.X == -32000)
            {
                // the game is minimized
                this.WindowState = FormWindowState.Minimized;
                this.Enabled = true;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Enabled = false;

                //this.Location = new Point(rect.X + 10, rect.Y + 10);
            }
            this.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
               this.Combatfile_Name = this.openFileDialog1.FileName;
               this.label1.Text = this.Combatfile_Name;
            }

            //Test Data
            LogLine l = new LogLine("[03/20/2012 15:03:19] [@Peskend] [Separatist Scout {505105333878784}] [Hammer Shot {801299163512832}] [ApplyEffect {836045448945477}: Damage {836045448945501}] (0 -miss {836045448945502}) <1>");
        }
    }

    /*
     * Special Thanks
     * */
    // Danial Afzal
    // iotasquared@gmail.com
     class LogLine
    {
        public string source;
        public string target;
        public string ability;
        public string event_type, event_detail;
        public bool crit_value;
        public int value;
        public string value_type;
        public int threat;

        static Regex regex =
            new Regex(@"\[(.*)\] \[(.*)\] \[(.*)\] \[(.*)\] \[(.*)\] \((.*)\)[.<]*([!>]*)[\s<]*(\d*)?[>]*",
                RegexOptions.Compiled);
        static Regex id_regex = new Regex(@"\s*\{\d*}\s*", RegexOptions.Compiled);

        public LogLine(string line)
        {
            line = id_regex.Replace(line, "");
            MatchCollection matches = regex.Matches(line);
            source = matches[0].Groups[2].Value;
            target = matches[0].Groups[3].Value;
            ability = matches[0].Groups[4].Value;
            if (matches[0].Groups[5].Value.Contains(":"))
            {
                event_type = matches[0].Groups[5].Value.Split(':')[0];
                event_detail = matches[0].Groups[5].Value.Split(':')[1].Trim();
            }
            else
            {
                event_type = matches[0].Groups[5].Value;
                event_detail = "";
            }

            crit_value = matches[0].Groups[6].Value.Contains("*");
            string[] raw_value = matches[0].Groups[6].Value.Replace("*", "").Split(' ');
            value = raw_value[0].Length > 0 ? int.Parse(raw_value[0]) : 0;
            if (raw_value.Length > 1)
            {
                value_type = raw_value[1];
            }
            else
            {
                value_type = "";
            }
            threat = matches[0].Groups[8].Value.Length > 0 ? int.Parse(matches[0].Groups[8].Value) : 0;
        }
    }
}
