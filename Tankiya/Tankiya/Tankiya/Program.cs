using System;
using tank_game;
namespace Tankiya
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameUI game = new GameUI())
            {
                game.Run();
              
            }
            try
            {

                Console.Title = "Mustank Console";
                Console.WriteLine("Client started...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Ocured");
            }
           
        }
    }
#endif
}

