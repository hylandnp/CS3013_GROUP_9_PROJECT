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
    * SKELETON PLAYER-TRACKING CLASS
    *////////////////////////////////////////
    public class SkeletonPlayer
    {
        public int tracking_id { get; set; }
        public string last_gesture { get; set; }
        public Skeleton skeleton { get; set; }

        public SkeletonPlayer()
        {
            this.tracking_id = 0;
            this.last_gesture = "";
            this.skeleton = null;
        }
    }
}
