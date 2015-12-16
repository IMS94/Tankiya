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
        public MapItem[,] grid { get; set; } //Main map which contains data
        public int myid { get; set; } //my client id in the game
        public String map_string { get; set; } //map character grid string for cmd

        public Battle battle ;
        public CollectResources collect_resources;
        
        /* playing method will be decided on this value
           
         *          0  ---------------- collect coin piles
         *          1  ---------------- collect health packs
         
        */
        public int playingMethod { get; set; }

        private Player[] players; //players
        private int player_count;//Number of players in the game (from countable numbers)
        private BasicCommandReader basicCommandReader=new BasicCommandReader();
        private BasicCommandSender basicCommandSender=new BasicCommandSender();
        private Communicator com;
        
        #endregion

        public Map()
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
            SearchMethods search_methods = new SearchMethods(grid, players, myid, player_count);
            collect_resources = new CollectResources(grid,players,myid,player_count,search_methods);
            search_methods.clearMapForBFS();
            com = Communicator.getInstance();
            com.setMap(this);
            com.StartListening();
            map_string = "";
            playingMethod = 0;
        } //Constructor to initialize map with all EmptyCells

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
        }
        #endregion

        #region evaluating recieved msgs from communicator functions
        public void read(String read)
        {
            String readMsg = read.Substring(0, read.Length - 2);
            Char c = readMsg[0];
            Console.WriteLine("Type "+c+" found " + readMsg);
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
                    if (grid[Int32.Parse(brick[0] + ""), Int32.Parse(brick[1] + "")] != null)
                    {
                        ((Brick)(grid[Int32.Parse(brick[0] + ""), Int32.Parse(brick[1] + "")])).health = (4 - damage_val) * 25;
                        if (damage_val == 4)
                        {
                            grid[Int32.Parse(brick[0] + ""), Int32.Parse(brick[1] + "")] = new EmptyCell();
                        }

                    }


                }
            }
            updateWorld();
            gamePlay();
            foreach (HealthPack hp in collect_resources.health_pack_queue)
            {
                
                Console.WriteLine("coin piles in queue " + hp.x_cordinate + " " + hp.y_cordinate);
            }
            
        }
        private void readCoinC(String read)
        {   //C:<x>,<y>:<LT>:<Val>#


            String[] mainSplit = read.Split(':');
            int x=Int32.Parse(mainSplit[1][0] + "");
            int y=Int32.Parse(mainSplit[1][2] + "");
            Coin coin_pile = new Coin(x, y, Int32.Parse(mainSplit[2] + ""), Int32.Parse(mainSplit[3] + ""));
            collect_resources.coin_queue.Add(coin_pile);
            grid[x, y] = coin_pile; 
            
        }
        private void readHealthPackL(String read)
        {   //L:<x>,<y>:<LT>#

            String[] mainSplit = read.Split(':');
            int x = Int32.Parse(mainSplit[1][0] + "");
            int y = Int32.Parse(mainSplit[1][2] + "");
            HealthPack health_pack = new HealthPack(x, y, Int32.Parse(mainSplit[2] + ""));
            collect_resources.health_pack_queue.Add(health_pack);
            grid[x, y] = health_pack;
        }

        #endregion

        #region Main Methods

        public void gamePlay()
        {
            if (playingMethod == 0) 
            {
                sendCommandToServer(collect_resources.collectCoin());    
            }
            else if (playingMethod == 1) 
            {
                sendCommandToServer(collect_resources.collectHealthPack());
            }
                
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
