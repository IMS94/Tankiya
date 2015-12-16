using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tank_game
{
    /// <summary>
    /// Class for handle basic 6 send commands
    /// </summary>
    public class BasicCommandSender
    {
        private Communicator com;

        public BasicCommandSender()
        {
            com = Communicator.getInstance();
        }
        #region BasicSendCommands
        public void Join() { com.SendData(Constant.C2S_INITIALREQUEST); }
        public void Up() { com.SendData(Constant.UP); }
        public void Down() { com.SendData(Constant.DOWN); }
        public void Left() { com.SendData(Constant.LEFT); }
        public void Right() { com.SendData(Constant.RIGHT); }
        public void Shoot() { com.SendData(Constant.SHOOT); }

        #endregion


    }
}
