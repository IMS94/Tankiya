using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    //parent class for map items- map item which a tank can enter to that cell
    public class MovableMapItem : MapItem
    {
        #region bfs search parameters
        public List<int> path { get; set; }  //list which includes the path
        public int color { get; set; }       //0=white ;1=gray ;2=black
        public void clearPathList() { this.path.Clear(); }
        public void clearColor() { color = 0; }

        #endregion
    }
}
