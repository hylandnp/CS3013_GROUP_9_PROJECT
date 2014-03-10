using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*CHANGELOG
 * NEIL - Created the class & added texture loading.
 */

namespace KinectGame_WindowsXNA.Source.Interface
{
    // Cursor icon for the user's Kinect "hand" position or the mouse (for debugging)...
    public class Cursor
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        public float SwipeMinimalLength { get; set; }
        public float SwipeMaximalLength { get; set; }
        public int SwipeMininalDuration { get; set; }
        public int SwipeMaximalDuration { get; set; }
  
        private Vector3 current_pos,
                        previous_pos;

        private Texture2D hand_texture;

         public void SwipeGestureDetector()
        {
            SwipeMinimalLength = 0.4f;
            SwipeMaximalLength = 0.2f;
            SwipeMininalDuration = 250;
            SwipeMaximalDuration = 1500; 
        }


        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          *////////////////////////////////////////
        public Cursor(Texture2D p_texture)
        {
            // Initialisation...
            this.hand_texture = p_texture;
        }
        public bool swiped(float startposX, float startposY, int time){
            float endposX = current_pos.X;
            float endposY = current_pos.Y;
            var currentTime = DateTime.Now;
            if (time > SwipeMininalDuration && time < SwipeMaximalDuration)
            {
                if ((startposX - endposX > SwipeMinimalLength && startposX - endposX < SwipeMaximalLength)
                    || (endposX - startposX > SwipeMinimalLength &&  endposX - startposX < SwipeMaximalLength)
                    || (startposY - endposY > SwipeMinimalLength && startposY - endposY < SwipeMaximalLength)
                    || (endposY - startposY > SwipeMinimalLength && startposY - endposY < SwipeMaximalLength))
                {
                    return true;
                }
            }     
            return false;
        }
        public void swipeDirection(float startPosX, float startPosY)
        {
            float endPosX = current_pos.X;
            float endPosY = current_pos.Y;
            if (startPosX - endPosX > 0)
            {
                Console.WriteLine("Left Swipe");
            }
            else if (startPosX - endPosX < 0)
            {
                Console.WriteLine("Right Swipe");
            }
            if (startPosY - endPosY > 0)
            {
                Console.WriteLine("Downward Swipe");
            }
            else if (startPosY - endPosY < 0)
            {
                Console.WriteLine("Upward Swipe");
            }
        }
       

    }

}

