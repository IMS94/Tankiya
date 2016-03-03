using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace tank_game
{
    public class Map
    {
        #region MapVariables
        public MapItem[,] grid = null;
        public int myid { get; set; } //my client id in the game
        public String map_string { get; set; } //map character grid string for cmd

        public List<Bullet> bullet_list; // bullet reference use to update bullet in GUI
        public Battle battle ;
        public CollectResources collect_resources;
        public SearchMethods search_methods;
        public int read_count = 0;
        public int op_id;//temparay var for keyboard check;
        //The map instance to be used all over the game
        private static Map map;
        public int playingMethod { get; set; }

        private Player[] players; //players
        private int player_count;//Number of players in the game (from countable numbers)
        private BasicCommandReader basicCommandReader=new BasicCommandReader();
        private BasicCommandSender basicCommandSender=new BasicCommandSender();
        private Communicator com;

        public String current_mode_discription { get; set; }
        
        #endregion

        private Map()
        {
            grid = new MapItem[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    grid[i, j] = new EmptyCell();
                }
            }
            players = new Player[5];
            bullet_list = new List<Bullet>();
            com = Communicator.getInstance();
            com.StartListening();
            map_string = "";
            playingMethod = 0;
            op_id = 1;
            current_mode_discription = "";
            search_methods = new SearchMethods(grid, players, myid);
            search_methods.clearMapForBFS();
            collect_resources = new CollectResources(grid, players, myid, player_count, search_methods);
            battle = new Battle(grid, players, player_count, myid, search_methods, bullet_list);
            
            } //Constructor to initialize map with all EmptyCells


        public void update_player_count_value() 
        {
            battle.player_count = this.player_count;
            collect_resources.player_count = this.player_count;
            Console.WriteLine("player count values are updated in battle and resources");
        }


        /// <summary>
        /// Get the map instance. This is to make this class a singleton.
        /// </summary>
        /// <returns></returns>
        public static Map GetInstance() { 
            if(Map.map==null){
                Map.map = new Map();
            }

            Map.map.com.setMap(Map.map);
            return Map.map;
        }

        public MapItem[,] GetGrid() {
            return this.grid;
        }


        #region Map Printing functions

        public void updateMapString() 
        {
            string map = "";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                     
                     map=map+grid[i, j].name+"  "; 
                            
                }
                map = map + "\n";

            }
            Console.WriteLine("");
            Console.WriteLine(map);
            this.map_string = map;

            //initial playing method set to coin collect
            playingMethod = 0;

        }
  
        public String getMapString()
        {
            return this.map_string;
        }


        #endregion

        #region Map update functions

        private void updateWorld() {
            collect_resources.updateCoinAqquire();
            collect_resources.updateHealthPackAqquire();
            collect_resources.timerUpdateCoin();
            collect_resources.timerUpdateHealthPack();
            update_player_count_value();
        }
        #endregion

        #region evaluating recieved msgs from communicator functions
        public void read(String read)
        {
            try
            {
                String readMsg = read.Substring(0, read.Length - 2);
                Char c = readMsg[0];
                Console.WriteLine("Type " + c + " found " + readMsg);
                if (!basicCommandReader.Read(readMsg))
                {
                    if (c.Equals('S'))
                    {
                        readAcceptanceS(readMsg);
                    }
                    else if (c.Equals('I'))
                    {
                        readInitiationI(readMsg);
                    }
                    else if (c.Equals('G'))
                    {
                        readMovingG(readMsg);
                    }
                    else if (c.Equals('C'))
                    {
                        readCoinC(readMsg);
                    }
                    else if (c.Equals('L'))
                    {
                        readHealthPackL(readMsg);
                    }
                }
            }
            catch(Exception ex)
            {
                //Console.WriteLine("fault message received from the server : " + ex.ToString());
            }

        }
        private void readAcceptanceS(String readMsg)
        {//S:P1: 1,1:0
            
            String[] mainSplit = readMsg.Split(':');
            String[] subSplit = mainSplit[1].Split(';');
            myid=Int32.Parse(subSplit[0][1]+"");
            player_count += 1;

            //set the name in constructor
            players[myid] = new Player(subSplit[0]);

            //set initial cordinates of the player
            players[myid].cordinateX = Int32.Parse(subSplit[1][0] + "");
            players[myid].cordinateY = Int32.Parse(subSplit[1][2] + "");
            
            
            //set initial dirctions
            players[myid].direction = Int32.Parse(subSplit[2]+"");

            Console.WriteLine("Mustank player no : " + players[myid]);
            Console.WriteLine("Start Cordinate : " + players[myid].cordinateX + "," + players[myid].cordinateY);
            Console.WriteLine("Connected to the server \n");
            
        }
        private void readInitiationI(String readMsg)
        {   //I:P<num>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y>: < x>,<y>;< x>,<y>;< x>,<y>…..< x>,<y>
            try
            {
                String[] mainSplit = readMsg.Split(':');
                

                for (int i = 2; i < mainSplit.Length; i++)
                {
                    String[] cordinates = mainSplit[i].Split(';');
                    if (i == 2)
                    {
                        //initial positions of bricks
                        foreach (String cordinate in cordinates)
                        {
                            
                            this.grid[Int32.Parse(cordinate[0]+""), Int32.Parse(cordinate[2]+"")] = new Brick();
                        }
                    }
                    else if (i == 3)
                    {
                        foreach (String cordinate in cordinates)
                        {
                            this.grid[Int32.Parse(cordinate[0] + ""), Int32.Parse(cordinate[2] + "")] = new Stone();
                        }
                    }

                    else if (i == 4)
                    {
                        foreach (String cordinate in cordinates)
                        {
                            this.grid[Int32.Parse(cordinate[0] + ""), Int32.Parse(cordinate[2] + "")] = new Water();
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine("readMsg is "+readMsg+  "\n Exception occured "+exception.Message);
            }
            //update map string and print
            this.updateMapString();
        }
        private void readMovingG(String read)
        {/*G:P1;< player location  x>,< player location  y>;<Direction>;< whether shot>;<health>;< coins>;< points>:
            …. P5;< player location  x>,< player location  y>;<Direction>;< whether shot>;<health>;
                < coins>;< points>: < x>,<y>,<damage-level>;< x>,<y>,<damage-level>;< x>,<y>,<damage-level>;
                < x>,<y>,<damage-level>…..< x>,<y>,<damage-level> */




            Boolean b = basicCommandReader.Read(read);
            if (!b)
            {


                String[] mainSplit = read.Split(':');
                int playerC = mainSplit.Count() - 2;
                this.player_count = playerC;
                Console.WriteLine("playerC value :" + playerC);
                Console.WriteLine("player_value :" + this.player_count);
                

                for (int i = 1; i < playerC + 1; i++)
                {


                    String[] playerSplit = mainSplit[i].Split(';');
                    int playerNum = Int32.Parse(playerSplit[0][1] + "");
                    if (players[playerNum] == null)
                    {
                        players[playerNum] = new Player(playerNum.ToString());
                    }
                    
                    players[playerNum].cordinateX = Int32.Parse(playerSplit[1][0] + "");
                    players[playerNum].cordinateY = Int32.Parse(playerSplit[1][2] + "");
                    players[playerNum].direction = Int32.Parse(playerSplit[2] + "");
                    players[playerNum].whetherShot = Int32.Parse(playerSplit[3] + "");
                    players[playerNum].health = Int32.Parse(playerSplit[4] + "");
                    players[playerNum].coins = Int32.Parse(playerSplit[5] + "");
                    players[playerNum].points = Int32.Parse(playerSplit[6] + "");
                }
                String[] brickSplit = mainSplit[mainSplit.Count() - 1].Split(';');
                int brickCount = brickSplit.Count();
                
                for (int j = 0; j < brickCount; j++)
                {
                    String[] brick = brickSplit[j].Split(',');
                    int damage_val = Int32.Parse(brick[2] + "");
                    Console.Write(brick[0] + " " + brick[1] + ",");
                    if (grid[Int32.Parse(brick[0] + ""), Int32.Parse(brick[1] + "")] != null && 
                        grid[Int32.Parse(brick[0] + ""), Int32.Parse(brick[1] + "")].GetType().Equals(typeof(Brick)))
                    {
                        ((Brick)(grid[Int32.Parse(brick[0] + ""), Int32.Parse(brick[1] + "")])).health = (4 - damage_val) * 25;
                        if (damage_val == 4)
                        {
                            grid[Int32.Parse(brick[0] + ""), Int32.Parse(brick[1] + "")] = new EmptyCell();
                            Console.WriteLine("The brick at "+brick[0]+" "+brick[1]+" replaced with an empty cell");
                        }

                    }
                }
            }
            updateWorld();
            gamePlay();
            foreach (Coin coin in collect_resources.coin_queue)
            {
                
           //     Console.WriteLine("@readmovingG : Coin Piles in queue " + coin.x_cordinate + " " + coin.y_cordinate +"     "+coin.left_time);
            }
            foreach (HealthPack hp in collect_resources.health_pack_queue)
            {

           //     Console.WriteLine("@readmovingG : Health Pack in queue " + hp.x_cordinate + " " + hp.y_cordinate+"     "+hp.left_time);
            }
            
        }
        private void readCoinC(String read)
        {   //C:<x>,<y>:<LT>:<Val>#


            String[] mainSplit = read.Split(':');
            int x=Int32.Parse(mainSplit[1][0] + "");
            int y=Int32.Parse(mainSplit[1][2] + "");
            Coin coin_pile = new Coin(x, y, Int32.Parse(mainSplit[2] + "")-1000, Int32.Parse(mainSplit[3] + ""));
            Console.WriteLine("@readcoinC coin pile added " + x + " " + y);
            collect_resources.coin_queue.Add(coin_pile);
            grid[x, y] = coin_pile; 
            
        }
        private void readHealthPackL(String read)
        {   //L:<x>,<y>:<LT>#

            String[] mainSplit = read.Split(':');
            int x = Int32.Parse(mainSplit[1][0] + "");
            int y = Int32.Parse(mainSplit[1][2] + "");
            HealthPack health_pack = new HealthPack(x, y, Int32.Parse(mainSplit[2] + "")-1000);
            Console.WriteLine("@readHealthPackL health pack added " + x + " " + y);
            collect_resources.health_pack_queue.Add(health_pack);
            grid[x, y] = health_pack;
        }

        #endregion

        #region Main Methods

        /* playing method will be decided on this value
           
         *          0  ----------------safely collect coin piles (attack if needed)
         *          1  ----------------safely collect health packs(attack if needed)
         *          2------------------ follow and attack (chase and attack opponent)
         
        */

        public void safe_coin_collect()
        {
            if (this.op_id != myid && this.op_id < player_count)
            {
                int attack_value = battle.attack(this.op_id);
                if (attack_value >= 0 && attack_value < 4)
                {
                    sendCommandToServer(attack_value);
                }
                else if (attack_value < 0)
                {
                    if (collect_resources.collectCoin().Count != 0)
                    {
                        sendCommandToServer(collect_resources.collectCoin());
                    }
                    else if (collect_resources.collectHealthPack().Count != 0)
                    {
                        sendCommandToServer(collect_resources.collectHealthPack());
                    }
                }
            }
            else 
            {
                sendCommandToServer(collect_resources.collectCoin());
            }

        }
        public void safe_health_pack_collect()
        {
            if (this.op_id != myid && this.op_id < player_count)
            {
                int attack_value = battle.attack(this.op_id);
                if (attack_value >= 0 && attack_value < 4)
                {
                    sendCommandToServer(attack_value);
                }
                else if (attack_value < 0)
                {
                    if (collect_resources.collectHealthPack().Count != 0)
                    {
                        sendCommandToServer(collect_resources.collectHealthPack());
                    }
                    else if (collect_resources.collectCoin().Count != 0)
                    {
                        sendCommandToServer(collect_resources.collectCoin());
                    }
                   
                }
            }
    
        }
        public void follow_and_attack()
        {
            if (this.op_id != myid && this.op_id < player_count)
            {
                List<int> list =battle.follow_and_attack(this.op_id);
                if (list != null)
                {
                    sendCommandToServer(list);
                }
            }
        }

        public void select_opponent()
        {
            List<int> distances = new List<int>();
            Console.WriteLine("Distances :");
            bool opponent_exist = false;
            for (int i = 0; i < player_count; i++)
            {
                int distance = battle.on_line(i);
                if (distance > 0) { distances.Add(distance); }
                else { distances.Add(1000); }
              //  Console.WriteLine(distances[i] + " , ");
                if (i != myid && players[i].health > 0) { op_id = i; opponent_exist = true; }
                
            }
            if (distances.Min() != 1000)
            {
                int player_with_min_distance = distances.IndexOf(distances.Min());
                op_id = player_with_min_distance;
            }
            if (!opponent_exist)
            {
                op_id = myid;
            }
            Console.WriteLine("Opponent :"+op_id);
            
        }
        //main method which play the game : triggerd by msg reading with the period of 1 second (type G)
        public void gamePlay()
        {
            Player mustank = players[myid];
            List<int> points = new List<int>();
            
            for (int i = 0; i < player_count; i++)
            {
                points.Add(players[i].points);
            }

            select_opponent();


            int player_with_max_points = points.IndexOf(points.Max());
            points[player_with_max_points]=0;
            int player_with_second_max_point = points.IndexOf(points.Max());

            if (op_id == myid)
            {
                playingMethod = 0;
                current_mode_discription = "Safe Coin collecting only mode activated";
                Console.WriteLine(current_mode_discription);
            }

            else if (!(playingMethod == 3 && players[op_id].health > 0) && read_count>1 )
            {
               
                if (mustank.health < 50)
                {
                    playingMethod = 1;
                    current_mode_discription = "Safe Health Pack collecting mode activated";
                    Console.WriteLine(current_mode_discription);

                }
                else if ((player_count < 3 && player_with_max_points == myid) || player_with_max_points != myid)
                {
                    playingMethod = 2;
                    current_mode_discription = "follow and attack mode activated";
                    Console.WriteLine(current_mode_discription);
                }

                else if (player_with_max_points == myid)
                {
                    playingMethod = 0;
                    current_mode_discription = "Safe Coin collecting only mode activated";
                    Console.WriteLine(current_mode_discription);
                }

                
            }

            if (playingMethod == 0)
            {
                safe_coin_collect();
            }
            else if (playingMethod == 1)
            {
                safe_health_pack_collect();
            }
            else if (playingMethod == 2)
            {
                follow_and_attack();
            }
            read_count += 1;
        }

        public void sendCommandToServer(List<int> commandList)
        {

            if (commandList != null)
            {
                String temp = "sending command";
                foreach (int i in commandList)
                {
                    temp += i.ToString();
                }
                Console.WriteLine("selected command List " + temp);

                if (commandList[0] == 0) { basicCommandSender.Up(); }
                else if (commandList[0] == 1) { basicCommandSender.Right(); }
                else if (commandList[0] == 2) { basicCommandSender.Down(); }
                else if (commandList[0] == 3) { basicCommandSender.Left(); }

            }
        }
        public void sendCommandToServer(int command)
        {

                if (command == 0) { basicCommandSender.Up(); }
                else if (command == 1) { basicCommandSender.Right(); }
                else if (command == 2) { basicCommandSender.Down(); }
                else if (command == 3) { basicCommandSender.Left(); }

        }
       
        #endregion

        /// <summary>
        /// Get the players array for external use
        /// </summary>
        /// <returns></returns>
        public Player[] GetPlayers() {
            return this.players;
        }

      
    }
}
