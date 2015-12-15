using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GUI;

namespace tank_game
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
               
                Console.Title = "Mustank Console";
                Console.WriteLine("Client started...");
                Map map = new Map();
                Gui gui = new Gui(map);
                Application.Run(gui);


               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
