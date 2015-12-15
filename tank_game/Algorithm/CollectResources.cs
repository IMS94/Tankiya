using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    //class to implement all resource finding logic and command
    public class CollectResources
    {
        private MapItem[,] grid;
        private Player[] players;
        private int myid;
        private SearchMethods SearchMethods;
        private int player_count;
        public List<Coin> coin_queue { get; set; } //current coins
        public List<HealthPack> health_pack_queue { get; set; } //current health packs
        

        /*
         * create 2d cost grid for all the players for all the coins
         * fill the grid with costs if coin is reachable
         * create two lists which have my costs and my commands to available coins
         * search in the grid for other players whether they have law cost than me for a perticular coin
         * if no return it
         * else replace current my_count maximum with 10000 and redo
        */

        public List<int> collectCoin()  //get command for collect coin
        {
            int[,] coin_cost_grid = new int[player_count, coin_queue.Count];
            List<int> my_costs = new List<int>();
            List<List<int>> my_coin_command_list =new List<List<int>>();

            try
            {
                for (int i = 0; i < player_count; i++)
                {
                    SearchMethods.pathEvaluationBFS(players[i].cordinateX, players[i].cordinateY);
                    for (int j = 0; j < coin_queue.Count; j++)
                    {
                        List<int> coin_command_list = SearchMethods.commandList(SearchMethods.getPath
                            (coin_queue[j].x_cordinate, coin_queue[j].y_cordinate), players[i].direction);
                        if (coin_command_list != null && ((coin_queue[j].left_time / 1000) > coin_command_list.Count()))
                        {
                            coin_cost_grid[i, j] = coin_command_list.Count;
                            if (i == myid)
                            {
                                my_coin_command_list.Add(coin_command_list);
                                my_costs.Add(coin_command_list.Count);
                            }
                        }

                    }
                }

                int index_min_my_costs = my_costs.IndexOf(my_costs.Min());
                int index_min_grid=0;
                int total_coins = my_costs.Count();
                while (total_coins > 0)
                {
                    for (int j = 0; j < coin_queue.Count; j++)
                    {
                        if (coin_cost_grid[myid, j] == my_costs.Min())
                        {
                            index_min_grid = j;
                        }
                    }
                    index_min_my_costs = my_costs.IndexOf(my_costs.Min());
                    bool use_less = false;
                    for (int i = 0; i < player_count; i++)
                    {
                        if (i != myid)
                        {
                            if (coin_cost_grid[i, index_min_grid] < my_costs.Min())
                            {
                                use_less = true;
                            }

                        }
                    }
                    if (use_less)
                    {
                        my_costs[index_min_my_costs] = 1000;
                    }
                    else
                    {
                        return my_coin_command_list[index_min_my_costs];
                    }
                    total_coins -= 1;
                }
                return null;
            }
            catch(Exception ex)
            { 
                Console.WriteLine(ex.ToString());
                return null;
            }
            

        }
        public List<int> collectHealthPack()  //make command for collect Health Pack
        {
            List<int> costs = new List<int>();
            List<List<int>> list_of_commandLists = new List<List<int>>();
            foreach (HealthPack health_pack in health_pack_queue)
            {
                List<int> health_command_list = SearchMethods.getCommandList(players[myid].cordinateX, players[myid].cordinateY, health_pack.x_cordinate, health_pack.y_cordinate, myid);
                if (health_command_list != null && ((health_pack.left_time / 1000) > health_command_list.Count()))
                {
                    //add commandLists for the lists
                    costs.Add(health_command_list.Count);
                    list_of_commandLists.Add(health_command_list);
                }

            }
            //select minimum cost commandlist
            if (costs.Count > 0)
            {
                int index_min_cost = costs.IndexOf(costs.Min());
                return list_of_commandLists[index_min_cost];
            }
            return null;
        }
        public CollectResources(MapItem[,] gridE ,Player[] playersE,int my_idE,int player_countE,SearchMethods searchMethods)
        {
            SearchMethods = searchMethods;
            this.grid =gridE ;
            coin_queue=new List<Coin>();
            health_pack_queue=new List<HealthPack>();
            players = playersE;
            myid = my_idE;
            player_count = player_countE;
        }


        #region usual resource update
        public void updateCoinAqquire()
        {
            foreach (Player player in players)
            {
                if (player != null)
                {
                    for (int i = 0; i < coin_queue.Count; i++)
                    {
                        if (coin_queue[i].x_cordinate == player.cordinateX && coin_queue[i].y_cordinate == player.cordinateY)
                        {
                            grid[coin_queue[i].x_cordinate, coin_queue[i].y_cordinate] = new EmptyCell();
                            coin_queue.RemoveAt(i);
                        }
                    }
                }
            }
        }
        public void updateHealthPackAqquire()
        {
            foreach (Player player in players)
            {
                if (player != null)
                {
                    for (int i = 0; i < health_pack_queue.Count; i++)
                    {
                        if (health_pack_queue[i].x_cordinate == player.cordinateX && health_pack_queue[i].y_cordinate == player.cordinateY)
                        {
                            grid[health_pack_queue[i].x_cordinate, health_pack_queue[i].y_cordinate] = new EmptyCell();
                            health_pack_queue.RemoveAt(i);
                        }
                    }
                }
            }
        }
        public void timerUpdateCoin()
        {
            for (int i = 0; i < coin_queue.Count; i++)
            {
                bool vanished = coin_queue[i].timer_update();
                if (vanished)
                {
                    grid[coin_queue[i].x_cordinate, coin_queue[i].y_cordinate] = new EmptyCell();
                    coin_queue.RemoveAt(i);
                }
            }
        }
        public void timerUpdateHealthPack()
        {
            for (int i = 0; i < health_pack_queue.Count; i++)
            {
                bool vanished = health_pack_queue[i].timer_update();
                if (vanished)
                {
                    grid[health_pack_queue[i].x_cordinate, health_pack_queue[i].y_cordinate] = new EmptyCell();
                    health_pack_queue.RemoveAt(i);
                }
            }
        }
        #endregion

    }
}
