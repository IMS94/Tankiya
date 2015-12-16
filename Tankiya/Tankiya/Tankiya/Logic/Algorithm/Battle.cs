using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    //class to implement all battle commands and logic
    public class Battle
    {
        private MapItem[,] grid;
        private Player[] players;
        private int my_id;
       
        public Battle(MapItem[,] gridE ,Player[] playersE,int my_idE)
        {
            my_id = my_idE;
            players = playersE;
            grid=gridE;
        }
       
    }
}
