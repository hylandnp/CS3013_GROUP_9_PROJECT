using System;

/*CHANGELOG
 * NEIL - Created class & modified window setup.
 */

namespace KinectGame_WindowsXNA
{
#if WINDOWS || XBOX
    static class ProgramMain
    {
        /*/////////////////////////////////////////
         * PROGRAM ENTRY POINT
         */////////////////////////////////////////
        static void Main(string[] args)
        {
            using (KinectGame_WindowsXNA game = new KinectGame_WindowsXNA())
            {
                // Configure window:
                game.Window.Title = "Kinect Puzzle Game";
                game.Window.AllowUserResizing = false;

                // Start the game:
                game.Run();
            }
        }
    }
#endif
}

