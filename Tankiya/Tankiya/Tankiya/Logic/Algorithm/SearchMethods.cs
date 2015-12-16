using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    //class to implement all battle commands and logic
    public class SearchMethods
    {
        private MapItem[,] grid;
        private Player[] players;
        private int myid;
        public SearchMethods(MapItem[,] gridE ,Player[] playersE,int my_idE,int player_count)
        {
            myid = my_idE;
            players = playersE;
            grid=gridE;
        }
        private List<int> checkXYroute(int start_x, int start_y, int end_x, int end_y)
        {

            bool have_direct_path = true;
            int x_relative = end_x - start_x;
            int y_relative = end_y - start_y;
            List<int> direct_path = new List<int>();

            for (int i = 1; i < Math.Abs(x_relative) + 1; i++)
            {
                if (!(grid[start_x + i * (x_relative / Math.Abs(x_relative)), start_y].GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")))
                {
                    have_direct_path = false;
                }
                else
                {
                    if (x_relative > 0) { direct_path.Add(1); }
                    else if (x_relative < 0) { direct_path.Add(3); }

                }
            }
            for (int j = 1; j < Math.Abs(y_relative) + 1; j++)
            {
                if (!(grid[start_x + x_relative, start_y + j * (y_relative / Math.Abs(y_relative))].GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")))
                {
                    have_direct_path = false;
                }
                else
                {
                    if (y_relative > 0) { direct_path.Add(2); }
                    else if (y_relative < 0) { direct_path.Add(0); }
                }
            }

            if (have_direct_path)
            {
                return commandList(direct_path, players[myid].direction);
            }
            else { return null; }
        }//check direct route first X axis then Y axis
        private List<int> checkYXroute(int start_x, int start_y, int end_x, int end_y)
        {

            bool have_direct_path = true;
            int x_relative = end_x - start_x;
            int y_relative = end_y - start_y;
            List<int> direct_path = new List<int>();

            for (int j = 1; j < Math.Abs(y_relative) + 1; j++)
            {
                if (!(grid[start_x, start_y + j * (y_relative / Math.Abs(y_relative))].GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")))
                {
                    have_direct_path = false;
                }
                else
                {
                    if (y_relative > 0) { direct_path.Add(2); }
                    else if (y_relative < 0) { direct_path.Add(0); }
                }
            }
            for (int i = 1; i < Math.Abs(x_relative) + 1; i++)
            {
                if (!(grid[start_x + i * (x_relative / Math.Abs(x_relative)), start_y + y_relative].GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")))
                {
                    have_direct_path = false;
                }
                else
                {
                    if (x_relative > 0) { direct_path.Add(1); }
                    else if (x_relative < 0) { direct_path.Add(3); }

                }
            }

            if (have_direct_path)
            {
                return commandList(direct_path, players[myid].direction);
            }
            else { return null; }
        }//check direct route first Y axis then X axis
        public List<int> checkLrout(int start_x, int start_y, int end_x, int end_y, int player_id) //returns a command list if L exists
        {
            //code to identify whether it have L shape direction
            List<int> path;
            if (players[player_id].direction % 2 == 1)
            {
                path = checkXYroute(start_x, start_y, end_x, end_y);
                if (path != null)
                {
                    return commandList(path, players[player_id].direction);
                }
                path = checkYXroute(start_x, start_y, end_x, end_y);
                if (path != null)
                {
                    return commandList(path, players[player_id].direction);
                }
                return null;
            }
            else
            {
                path = checkYXroute(start_x, start_y, end_x, end_y);
                if (path != null)
                {
                    return commandList(path, players[player_id].direction);
                }
                path = checkXYroute(start_x, start_y, end_x, end_y);
                if (path != null)
                {
                    return commandList(path, players[player_id].direction);
                }
                return null;
            }

        }
        public List<int> getCommandList(int start_x, int start_y, int end_x, int end_y, int player_id)////any command list from start point to end for a given player
        {

            List<int> routeL = checkLrout(start_x, start_y, end_x, end_y, player_id);
            if (routeL == null)
            {
                pathEvaluationBFS(players[myid].cordinateX, players[myid].cordinateY);
                return commandList(((MovableMapItem)(grid[end_x, end_y])).path, players[myid].direction);
            }
            else
            {
                return routeL;
            }
        }
        public List<int> commandList(List<int> pathE, int initial_dir)
        {
            if (pathE.Count > 0)
            {
                List<int> path = pathE;
                List<int> commandList = new List<int>();

                if (path[0] != initial_dir) { commandList.Add(path[0]); }
                for (int i = 0; i < path.Count; i++)
                {
                    commandList.Add(path[i]);
                    if (i + 1 < path.Count)
                    {
                        if (path[i] != path[i + 1]) { commandList.Add(path[i + 1]); }
                    }
                }


                return commandList;
            }
            return null;
        }
        public void pathEvaluationBFS(int x_s, int y_s)
        {
            clearMapForBFS();
            Queue<Cordinate> item_queue = new Queue<Cordinate>();
            item_queue.Enqueue(new Cordinate(x_s, y_s));
            ((MovableMapItem)(grid[x_s, y_s])).color = 1;

            while (item_queue.Count > 0)
            {
                Cordinate cordinate = item_queue.Dequeue();
                MovableMapItem current = ((MovableMapItem)(grid[cordinate.x, cordinate.y]));
                List<int> current_path = ((MovableMapItem)(grid[cordinate.x, cordinate.y])).path;

                #region prioratise by recent value in the path list
                if (current.path.Count > 1 && current.path[current.path.Count - 1] == 2)
                {
                    if (cordinate.y < 9 && (grid[cordinate.x, cordinate.y + 1]).GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")
                   && ((MovableMapItem)(grid[cordinate.x, cordinate.y + 1])).color == 0)
                    {
                        item_queue.Enqueue(new Cordinate(cordinate.x, cordinate.y + 1));
                        ((MovableMapItem)(grid[cordinate.x, cordinate.y + 1])).color = 1;
                        ((MovableMapItem)(grid[cordinate.x, cordinate.y + 1])).path = new List<int>(current_path);
                        ((MovableMapItem)(grid[cordinate.x, cordinate.y + 1])).path.Add(2);

                    }
                }
                else if (current.path.Count > 1 && current.path[current.path.Count - 1] == 1)
                {
                    if (cordinate.x < 9 && (grid[cordinate.x + 1, cordinate.y]).GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")
                                       && ((MovableMapItem)(grid[cordinate.x + 1, cordinate.y])).color == 0)
                    {
                        item_queue.Enqueue(new Cordinate(cordinate.x + 1, cordinate.y));
                        ((MovableMapItem)(grid[cordinate.x + 1, cordinate.y])).color = 1;
                        ((MovableMapItem)(grid[cordinate.x + 1, cordinate.y])).path = new List<int>(current_path);
                        ((MovableMapItem)(grid[cordinate.x + 1, cordinate.y])).path.Add(1);

                    }
                }
                else if (current.path.Count > 1 && current.path[current.path.Count - 1] == 0)
                {
                    if (cordinate.y > 0 && (grid[cordinate.x, cordinate.y - 1]).GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")
                    && ((MovableMapItem)(grid[cordinate.x, cordinate.y - 1])).color == 0)
                    {
                        item_queue.Enqueue(new Cordinate(cordinate.x, cordinate.y - 1));
                        ((MovableMapItem)(grid[cordinate.x, cordinate.y - 1])).color = 1;
                        ((MovableMapItem)(grid[cordinate.x, cordinate.y - 1])).path = new List<int>(current_path);
                        ((MovableMapItem)(grid[cordinate.x, cordinate.y - 1])).path.Add(0);
                    }
                }

                else if (current.path.Count > 1 && current.path[current.path.Count - 1] == 3)
                {

                    if (cordinate.x > 0 && (grid[cordinate.x - 1, cordinate.y]).GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")
                        && ((MovableMapItem)(grid[cordinate.x - 1, cordinate.y])).color == 0)
                    {
                        item_queue.Enqueue(new Cordinate(cordinate.x - 1, cordinate.y));
                        ((MovableMapItem)(grid[cordinate.x - 1, cordinate.y])).color = 1;
                        ((MovableMapItem)(grid[cordinate.x - 1, cordinate.y])).path = new List<int>(current_path);
                        ((MovableMapItem)(grid[cordinate.x - 1, cordinate.y])).path.Add(3);
                    }
                }

                #endregion

                if (cordinate.y < 9 && (grid[cordinate.x, cordinate.y + 1]).GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")
                  && ((MovableMapItem)(grid[cordinate.x, cordinate.y + 1])).color == 0)
                {
                    item_queue.Enqueue(new Cordinate(cordinate.x, cordinate.y + 1));
                    ((MovableMapItem)(grid[cordinate.x, cordinate.y + 1])).color = 1;
                    ((MovableMapItem)(grid[cordinate.x, cordinate.y + 1])).path = new List<int>(current_path);
                    ((MovableMapItem)(grid[cordinate.x, cordinate.y + 1])).path.Add(2);

                }


                if (cordinate.x < 9 && (grid[cordinate.x + 1, cordinate.y]).GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")
                                      && ((MovableMapItem)(grid[cordinate.x + 1, cordinate.y])).color == 0)
                {
                    item_queue.Enqueue(new Cordinate(cordinate.x + 1, cordinate.y));
                    ((MovableMapItem)(grid[cordinate.x + 1, cordinate.y])).color = 1;
                    ((MovableMapItem)(grid[cordinate.x + 1, cordinate.y])).path = new List<int>(current_path);
                    ((MovableMapItem)(grid[cordinate.x + 1, cordinate.y])).path.Add(1);

                }

                if (cordinate.y > 0 && (grid[cordinate.x, cordinate.y - 1]).GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")
                    && ((MovableMapItem)(grid[cordinate.x, cordinate.y - 1])).color == 0)
                {
                    item_queue.Enqueue(new Cordinate(cordinate.x, cordinate.y - 1));
                    ((MovableMapItem)(grid[cordinate.x, cordinate.y - 1])).color = 1;
                    ((MovableMapItem)(grid[cordinate.x, cordinate.y - 1])).path = new List<int>(current_path);
                    ((MovableMapItem)(grid[cordinate.x, cordinate.y - 1])).path.Add(0);
                }

                if (cordinate.x > 0 && (grid[cordinate.x - 1, cordinate.y]).GetType().BaseType.ToString().Equals("tank_game.MovableMapItem")
                        && ((MovableMapItem)(grid[cordinate.x - 1, cordinate.y])).color == 0)
                {
                    item_queue.Enqueue(new Cordinate(cordinate.x - 1, cordinate.y));
                    ((MovableMapItem)(grid[cordinate.x - 1, cordinate.y])).color = 1;
                    ((MovableMapItem)(grid[cordinate.x - 1, cordinate.y])).path = new List<int>(current_path);
                    ((MovableMapItem)(grid[cordinate.x - 1, cordinate.y])).path.Add(3);
                }

                ((MovableMapItem)(grid[cordinate.x, cordinate.y])).color = 2;

            }
        }
        public List<int> getPath(int x,int y)
        {
            try
            {
                return ((MovableMapItem)grid[x, y]).path;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SearchMethods-getPath() function" + ex);
                return null;
            }
        }
        public void clearMapForBFS()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (grid[i, j].GetType().BaseType.ToString().Equals("tank_game.MovableMapItem"))
                    {

                        ((MovableMapItem)(grid[i, j])).color = 0;
                        if (((MovableMapItem)(grid[i, j])).path != null)
                        {
                            ((MovableMapItem)(grid[i, j])).clearPathList();
                        }
                        else { ((MovableMapItem)(grid[i, j])).path = new List<int>(); }

                    }

                }
            }
        }
        
       
    }
}
