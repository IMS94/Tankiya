using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    /// <summary>
    /// Class for handle basic 8 receiving commands
    /// </summary>

    class BasicCommandReader     {
        private Communicator com;

        public BasicCommandReader()
        {
            com = Communicator.getInstance();
        }
        
        
        //Basic receive commands

        public Boolean Read(String msg)
        {
            if (msg.Equals(Constant.S2C_HITONOBSTACLE))
            {
                Console.WriteLine("cannot move,obstacles found");
                return true;
            }
            else if (msg.Equals(Constant.S2C_CELLOCCUPIED))
            {
                Console.WriteLine("Cell is already occupied");
                return true;
            }
            else if (msg.Equals(Constant.S2C_NOTALIVE))
            {
                Console.WriteLine("Dead");
                return true;
            }
            else if (msg.Equals(Constant.S2C_TOOEARLY))
            {
                Console.WriteLine("Too Quick response");
                return true;
            }
            else if (msg.Equals(Constant.S2C_INVALIDCELL))
            {
                Console.WriteLine("Invalid Cell");
                return true;
            }

            else if (msg.Equals(Constant.S2C_GAMEJUSTFINISHED))
            {
                Console.WriteLine("Game has finished");
                return true;
            }
            else if (msg.Equals(Constant.S2C_NOTSTARTED))
            {
                Console.WriteLine("game not started yet");
                return true;
            }
            else if (msg.Equals(Constant.S2C_NOTACONTESTANT))
            {
                Console.WriteLine("Not a valid contestant");
                return true;
            }
            return false;
        }

    }
}
