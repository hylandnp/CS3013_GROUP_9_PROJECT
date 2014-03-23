using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * NEIL - Created the class.
 */

namespace KinectGame_WindowsXNA.Source.KinectUtils
{
    /*/////////////////////////////////////////
    * SKELETON GESTURE REFERENCE
    *////////////////////////////////////////
    public enum GestureType : byte
    {
        NONE,
        MENU,
        WAVE_RIGHT_HAND,
        WAVE_LEFT_HAND,
        JOINED_HANDS,
        SWIPE_LEFT_TO_RIGHT,
        SWIPE_RIGHT_TO_LEFT,
        SWIPE_UP,
        SWIPE_DOWN,
        ZOOM_IN,
        ZOOM_OUT
    }


    /*/////////////////////////////////////////
    * SKELETON PLAYER-TRACKING CLASS
    *////////////////////////////////////////
    public class SkeletonPlayer
    {
        public int tracking_id { get; set; }
        public string last_gesture_str { get; set; }
        public GestureType last_gesture { get; set; }
        public Skeleton skeleton { get; set; }


        public SkeletonPlayer()
        {
            this.tracking_id = 0;
            this.last_gesture_str = "";
            this.last_gesture = GestureType.NONE;
            this.skeleton = null;
        }
    }
}
