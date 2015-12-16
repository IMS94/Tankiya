using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    //class for the Brick wall
    public class Brick : UnmovableMapItem
    {

        public int health { get; set; }

        public Brick()
        {
            this.name = "B";
        }

    }
}
