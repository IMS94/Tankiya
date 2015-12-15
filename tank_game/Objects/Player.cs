using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    //class for the player
    public class Player : UnmovableMapItem
    {
       
        public int direction { get; set; }
        public int whetherShot { get; set; }
        public int health { get; set; }
        public int coins { get;set;}
        public int points { get; set; }
        public int cordinateX { get; set; }
        public int cordinateY { get; set; }
        public Player(String name)
        { 
            this.name = name[1]+"";
            points = 0;
            coins = 0;
            health = 100;
            whetherShot =0;
        }

    }
}
