using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*CHANGELOG
 * NEIL - Created the class.
 */

namespace KinectGame_WindowsXNA.Source.KinectUtils
{
    // Utility class to render the colour stream output to a texture (modified from Microsoft examples)...
    public class DepthStreamManager
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private short[] depth_data = null;
        private Texture2D depth_texture = null;
        private Effect depth_visualiser = null; // the XNA effect used to correctly format the RGBA pixel data
        private Rectangle rect;
        private bool was_drawn = false;


        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          *////////////////////////////////////////
        public DepthStreamManager(Rectangle p_dest_rect,
                                  Effect p_depth_vis,
                                  KinectManager p_kinect,
                                  GraphicsDevice p_gfx_device)
        {
            // Initialisation...
            this.rect = p_dest_rect;
            this.depth_visualiser = p_depth_vis;

            this.depth_texture = new Texture2D(p_gfx_device,
                                               p_kinect.kinect_sensor.DepthStream.FrameWidth,
                                               p_kinect.kinect_sensor.DepthStream.FrameHeight,
                                               false,
                                               SurfaceFormat.Bgra4444);
            this.depth_data = new short[p_kinect.kinect_sensor.DepthStream.FramePixelDataLength];

            if(p_kinect.kinect_sensor != null) p_kinect.kinect_sensor.DepthFrameReady += this.updateDepthVideo;
        }



        /*/////////////////////////////////////////
          * KINECT DEPTH STREAM HANDLER
          *////////////////////////////////////////
        public void updateDepthVideo(object p_sender, DepthImageFrameReadyEventArgs p_args)
        {
            // Update the depth stream video...
            //if (was_drawn)
            //{
                using (var current_frame = p_args.OpenDepthImageFrame())
                {
                    if (current_frame != null &&
                       this.depth_data != null &&
                       this.depth_texture != null)
                    {
                        // Load array of pixels:
                        current_frame.CopyPixelDataTo(this.depth_data);
                    }
                }

                was_drawn = true;
            //}
        }



        /*/////////////////////////////////////////
          * DEPTH STREAM HANDLER SHUTDOWN
          *////////////////////////////////////////
        public void close(KinectManager p_kinect)
        {
            // Remove colour frame listener...
            if (p_kinect != null && p_kinect.kinect_sensor != null) p_kinect.kinect_sensor.DepthFrameReady -= this.updateDepthVideo;
        }



        /*/////////////////////////////////////////
          * RENDERING FUNCTION(S)
          *////////////////////////////////////////
        public void draw(SpriteBatch p_sprite_batch, KinectManager p_kinect)
        {
            // Render the depth stream video...
            if (p_kinect != null &&
                p_sprite_batch != null &&
                this.depth_texture != null &&
                this.was_drawn)
            {
                this.depth_texture.SetData<short>(this.depth_data);

                p_sprite_batch.Begin(SpriteSortMode.Immediate, null, null, null, null, this.depth_visualiser);
                p_sprite_batch.Draw(depth_texture,
                                    this.rect,
                                    Color.White);
                p_sprite_batch.End();
            }
        }
    }
}
