using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    //class for the Coin Pile
    public class Coin : MovableMapItem
    {
        public int x_cordinate { get; set; }
        public int y_cordinate { get; set; }
        public int value { get; set; }
        public int left_time { get; set; }
        public Coin(int x,int y,int lt,int value)
        {
            this.name = "C";
            this.x_cordinate = x;
            this.y_cordinate = y;
            this.value = value;
            this.left_time = lt;
        }
        public bool timer_update()
        {
            left_time -= 1000;
            if (left_time <= 0)
            { return true; }
            return false;
        }

    }
}
