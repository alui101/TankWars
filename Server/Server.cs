using ServerControllerSpace;
using System.Diagnostics;

namespace Server
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class starts the server and contains a busy loop that updates the world.
    /// </summary>
    class Server
    {
        static void Main(string[] args)
        {
            ServerController controller = new ServerController();

            controller.StartServer();

            // Server needs to update the world every N miliseconds. This is done in order to control the frame rate. 
            int msPerFrame = controller.GetMSPerFrame();

            Stopwatch watch = new Stopwatch();

            // Busy loop that updates the world.
            while (true)
            {
                watch.Start();

                while (watch.ElapsedMilliseconds < msPerFrame) { }

                watch.Reset();
                controller.IncrementFrames();
                controller.UpdateWorld();
            }
       }
    }
}
