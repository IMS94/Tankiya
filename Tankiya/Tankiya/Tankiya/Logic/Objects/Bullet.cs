using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace tank_game
{
    //class for a Bullet
    //REMEMBER BEFORE CODE
    //X axis is normal Y axis is inverse
    //lower y values are in upper and higher y s are in down
    public class Bullet {

        private MapItem[,] grid;
        public Cordinate current_cordinate { get; set; }
        public int direction{get;set;}

        public Player[] players;
        public bool isAlive;
        public int player_count;
        public int my_id;
        public Bullet(int my_idE ,MapItem[,] gridE,Player[] playersE,int player_countE)
        {
            this.my_id = my_idE;
            grid = gridE;
            players = playersE;
            player_count = player_countE;
            isAlive = false;
            direction = players[my_id].direction;
            current_cordinate = new Cordinate(players[my_id].cordinateX, players[my_id].cordinateY);
            
            Cordinate next_cordinate = getNext(current_cordinate);
            if (next_cordinate != null)
            {
                current_cordinate = next_cordinate;
                isAlive = true;
                Thread t = new Thread(update_bullet_location);
                t.Start();
            }
        }


        public Cordinate getNext(Cordinate now)
        {
            Cordinate cordinate = new Cordinate(now.x, now.y);
            if (direction == 0)
            {
                cordinate.y -= 1;
            }
            else if (direction == 1)
            {
                cordinate.x += 1;
            }
            else if (direction == 2)
            {
                cordinate.y += 1;
            }
            else if (direction == 3)
            {
                cordinate.x -= 1;
            }
            if (cordinate.y < 10 && cordinate.x < 10 &&  cordinate.y > -1 && cordinate.x > -1)
            {
                Console.WriteLine(check_if_any_player_at_cordinate(cordinate.x,cordinate.y));
                if ((grid[cordinate.x, cordinate.y].GetType().BaseType.ToString().Equals("tank_game.MovableMapItem") && 
                    !check_if_any_player_at_cordinate(cordinate.x,cordinate.y)) ||
                    grid[cordinate.x, cordinate.y].GetType().ToString().Equals("tank_game.Water"))  
                   {

                    return cordinate;
                }
                else
                { return null; }
            }
            else
            {
                return null;
            }
            
        }

        
        public void update_bullet_location()
        {
            
            while (current_cordinate!=null)
            {
                Thread.Sleep(333);
                current_cordinate = getNext(current_cordinate);   
            }
            isAlive = false;
        }
        
        public bool check_if_any_player_at_cordinate(int x, int y)
        {
            for (int i = 0; i < player_count; i++)
            {
                if (my_id!= i && players[i].cordinateX == x && players[i].cordinateY == y && players[i].health>0)
                {
                    return true;
                }
            }
            return false;
               
        }
        
    }
}
