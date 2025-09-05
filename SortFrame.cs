using SortingViz.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortingViz
{
    public sealed partial class SortFrame : Form
    {
        private readonly SortThread _thread;

        public SortFrame(string title, Control mainComp, SortThread thread)
        {
            Text = title; _thread = thread; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false;
            Controls.Add(mainComp); mainComp.Dock = DockStyle.Fill;
            Load += (s, e) => { var screen = Screen.FromControl(this).Bounds; Location = new Point(screen.Width / 8, screen.Height / 8); };
            FormClosing += (s, e) => { _thread?.RequestStop(); };
        }
    }
}
