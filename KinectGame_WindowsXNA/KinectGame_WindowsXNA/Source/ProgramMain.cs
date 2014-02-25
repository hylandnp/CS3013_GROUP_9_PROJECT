using System;

/*CHANGELOG
 * NEIL - Created class.
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
                // Start the game:
                game.Run();
            }
        }
    }
#endif
}

