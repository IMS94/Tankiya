using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace tank_game
{
    //class to implement all battle commands and logic
    public class Battle
    {
        //0 north      1 east     2 south    3 west
        
        private MapItem[,] grid;
        private Player[] players;
        private int my_id;
        private int op_id{get;set;}
        private int my_x;
        private int my_y;
        private SearchMethods search_methods;
        private Thread shooting_thread;
        Communicator com = Communicator.getInstance();
        private List<Bullet> bullet_list;
        public int player_count { get; set; }
           

        public Battle(MapItem[,] gridE, Player[] playersE, int player_countE,int my_idE, SearchMethods searchMethods,List<Bullet> bullet_listE)
        {
            my_id = my_idE;
            players = playersE;
            grid=gridE;
            search_methods = searchMethods;
            bullet_list = bullet_listE;
            player_count = player_countE;
           
            }
        
        //if any oponent is on sight //use for shoot
        public int on_sight(int op_id)
        {
            if (my_id == op_id) { return -1; }
            int op_x = players[op_id].cordinateX;
            int op_y = players[op_id].cordinateY;

            this.my_x = players[my_id].cordinateX;
            this.my_y = players[my_id].cordinateY;
            if (my_x == op_x && ((my_y > op_y && players[my_id].direction == 0) ||(my_y < op_y && players[my_id].direction == 2) ) &&
                is_movable_only_verticle(op_x, my_y, op_y))
            {
                Console.WriteLine("on_sight caught : my_x=" + my_x + " my_y=" + my_y + "    op_x=" + op_x + " op_y="+  op_y);
                return Math.Abs(my_y-op_y);
                
            }
            else if (my_y == op_y && ((my_x > op_x && players[my_id].direction == 3) || (my_x < op_x && players[my_id].direction == 1))
                && is_movable_only_horizontal(my_x, op_x, op_y))
            {
                Console.WriteLine("on_sight caught : my_x=" + my_x + " my_y=" + my_y + "    op_x=" + op_x + " op_y=" + op_y);
                return Math.Abs(my_x - op_x);
                
            }
            return -1;
        }

        //if any opponent in same line //use for select the oponent
        public int on_line(int op_id)
        {
            
            if (my_id == op_id || players[op_id].health==0) { return -1; }
            int op_x = players[op_id].cordinateX;
            int op_y = players[op_id].cordinateY;

            this.my_x = players[my_id].cordinateX;
            this.my_y = players[my_id].cordinateY;
            if (my_x == op_x && (my_y != op_y) && is_movable_only_verticle(op_x, my_y, op_y))
            {
                return Math.Abs(my_y - op_y);

            }
            else if (my_y == op_y && my_x != op_x && is_movable_only_horizontal(my_x, op_x, op_y))
            {
                return Math.Abs(my_x - op_x);

            }
            return -1;
        }

        
        //return rotation direction if on same line ,return next move if is_attackable ,return 5 if shooting enabled ;
        public int attack(int op_id)
        {
            this.op_id = op_id;
            my_x = players[my_id].cordinateX;
            my_y = players[my_id].cordinateY;
            int op_x = players[op_id].cordinateX;
            int op_y = players[op_id].cordinateY;

            //if on sight
            if (on_sight(op_id) > 0 && players[op_id].health>0 ) {
                start_shooting_thread();
                return 5;
            }
            

            //if on same line
            else if (my_x == op_x ) 
            {
                if (my_y - op_y > 0) { return 0; }
                else { return 2; } 
            }
            else if (my_y == op_y)
            {
                if (my_x - op_x > 0) { return 3; }
                else { return 1; }
            }

            // if attakable
            else if (is_attakable(op_id) > 0)
            {
                return is_attakable(op_id);
            }
            
            //none attacking ability
            return -1;
        
         }

        //shooting thread manager
        public void start_shooting_thread()
        {
            if (shooting_thread == null)
            {
                shooting_thread = new Thread(shoot);
                shooting_thread.Start();
            }
            else
            {
                if (shooting_thread.ThreadState.ToString().Equals("Stopped") )
                {
                    try
                    {
                        shooting_thread = null;
                        shooting_thread = new Thread(shoot);
                        shooting_thread.Start();
                        Console.WriteLine("Thread started successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Thread started failed although status is stopped: "+ex.ToString());
                        
                    }
                }
                else
                {
                    Console.WriteLine("Thread started failed Current Thread Status : " + shooting_thread.ThreadState.ToString());

                }
            }
        
        }

        //shoot if on sight
        public void shoot() 
        {

            int op_x = players[op_id].cordinateX;
            int op_y = players[op_id].cordinateY;
            int distance=1;
            Console.WriteLine("Before while loop in shoot Distance :" + distance + "     Health :" + players[op_id].health  +"Op _id :"+op_id);
            
            while (distance > 0 && players[op_id].health > 0)
            {
                Console.WriteLine("Before if in shoot Distance :" + distance + "     Health :" + players[op_id].health);
            
                distance = on_sight(op_id);
                if (distance > 0 && players[op_id].health>0)
                {
                    Console.WriteLine("After if in shoot Distance :" + distance +"     Health :"+players[op_id].health);
                    com.SendData(Constant.SHOOT);
                    distance = 1;
                    update_bullet_list();
                    Console.WriteLine("player_count =" + player_count);
                    bullet_list.Add(new Bullet(my_id, grid, players, player_count));
                    int waiting_time = (int)((float)distance * 1000 / 3 ) ;
                    Console.WriteLine("Shoot on P:" + op_id);
                    Thread.Sleep(waiting_time);
                 }
            }
            
        }
        //class to update bullet list
        public void update_bullet_list()
        {
            
                int n = bullet_list.Count();
                for (int i = 0; i < n; i++)
                {
                    if (!bullet_list[i].isAlive)
                    {
                        bullet_list.RemoveAt(i);
                        Console.WriteLine("bullet removed");
                        i = i - 1;
                        n = bullet_list.Count();
                    }
                    
                }
          
        }

        //follow and attack any given target
        public List<int> follow_and_attack(int op_id)
        {
            my_x = players[my_id].cordinateX;
            my_y = players[my_id].cordinateY;

            this.op_id = op_id;
            if (on_sight(op_id) > 0)
            {

                start_shooting_thread();
                return null;
            }
            else
            {
                return search_methods.getCommandList(my_x,my_y,players[op_id].cordinateX,players[op_id].cordinateY,my_id);
            }
        }

        //is_attackable = check if the mustank can be attacked by any opponent after one position change in the map
        //return the next direction to move if attackable for a player else -1
        private int is_attakable(int player_id)
        {
            int op_x = players[player_id].cordinateX;
            int op_y = players[player_id].cordinateY;

            if (Math.Abs(my_x - op_x) == 1)
            {
                if (is_movable_only_verticle(op_x, my_y, op_y) && my_y!= op_y)
                {
                    if (my_x - op_x > 0)
                    {
                        return 3;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else if (Math.Abs(my_y - op_y) == 1)
            {
                if (is_movable_only_horizontal(my_x, op_x, op_y) && my_x!=op_x)
                {
                    if (my_y - op_y > 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return 2;
                    }
                }


            }

            return -1;
        }

        //check any verticle cell range is attackable(a bullet can penetrate)
        private bool is_movable_only_verticle(int x ,int y_start,int y_end)
        {
            if (y_start > y_end) { return check_verticle_free_value_depend(x, y_end, y_start); }
            else { return check_verticle_free_value_depend(x, y_start, y_end); }
        
        }
        private bool check_verticle_free_value_depend(int x, int y_small, int y_large)
        {
            for (int i = y_small; i <= y_large; i++)
            {
                if (!((grid[x, i].GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")) || (grid[x, i].GetType().ToString().Equals("tank_game.Water"))
                    || (grid[x, i].GetType().ToString().Equals("tank_game.Brick"))))
                {
                    return false;
                }
            }
            return true;

        }
  
        //check if any horizontal range is attackable(a bullet can penetrate)
        private bool is_movable_only_horizontal(int x_start, int x_end, int y)
        {
            if (x_start > x_end) { return check_horizontal_free_value_depend(x_end, x_start, y);} 
            else{return check_horizontal_free_value_depend(x_start,x_end,y);}
            
        }
        private bool check_horizontal_free_value_depend(int x_small, int x_large, int y) 
        {
            for (int i = x_small; i <= x_large; i++)
            {
                if (!((grid[i, y].GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")) ||
                    (grid[i,y].GetType().ToString().Equals("tank_game.Water")) ||
                    (grid[i, y].GetType().ToString().Equals("tank_game.Brick")) ))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
