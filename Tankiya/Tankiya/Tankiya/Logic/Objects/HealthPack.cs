using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    //class for the HealthPack
    public class HealthPack : MovableMapItem
    {
        public int x_cordinate { get; set; }
        public int y_cordinate { get; set; }
        public int left_time { get; set; }
        public HealthPack(int x,int y,int lt)
        {
            this.name = "H";
            this.x_cordinate = x;
            this.y_cordinate = y;
            this.left_time = lt;
        }
        public bool timer_update()
        {
            left_time -= 1000;
            if (left_time <= 0)
            { return true;}
            return false;
        }

    }
}
