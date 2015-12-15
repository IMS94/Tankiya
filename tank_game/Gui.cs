using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace tank_game
{
    partial class Gui : Form
    {
        private BasicCommandSender sc;
        private Map map;
        
        public Gui(Map map)
        {
            InitializeComponent();
            sc = new BasicCommandSender();
            this.map = map;
           
        }
        private void AI_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) { sc.Shoot(); }
            else if (e.KeyCode == Keys.Up) { sc.Up(); }
            else if (e.KeyCode == Keys.Down) { sc.Down(); }
            else if (e.KeyCode == Keys.Left) { sc.Left(); }
            else if (e.KeyCode == Keys.Right) { sc.Right(); }
            else if (e.KeyCode == Keys.J) { sc.Join(); }                            //Join to a game
            else if (e.KeyCode == Keys.C) { map.playingMethod = 0; }                //set playing method to collect coin
            else if (e.KeyCode == Keys.H) { map.playingMethod = 1; }                //set playing method to collect health pack
        }
       
        

        
       
    }
}
