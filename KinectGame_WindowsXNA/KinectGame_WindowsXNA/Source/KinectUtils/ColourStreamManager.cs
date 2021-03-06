﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*CHANGELOG
 * NEIL - Created the class.
 * NEIL - Slight modifications.
 */

namespace KinectGame_WindowsXNA.Source.KinectUtils
{
    // Utility class to render the colour stream output to a texture (modified from Microsoft examples)...
    public class ColourStreamManager
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private byte[] colour_data = null;
        private Texture2D colour_texture = null;
        private Effect colour_visualiser = null; // the XNA effect used to correctly format the RGBA pixel data
        private Rectangle rect;



        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          *////////////////////////////////////////
        public ColourStreamManager(Rectangle p_dest_rect,
                                   Effect p_colour_vis,
                                   KinectManager p_kinect,
                                   GraphicsDevice p_gfx_device)
        {
            // Initialisation...
            this.rect = p_dest_rect;
            this.colour_visualiser = p_colour_vis;

            this.colour_texture = new Texture2D(p_gfx_device,
                                                p_kinect.kinect_sensor.ColorStream.FrameWidth,
                                                p_kinect.kinect_sensor.ColorStream.FrameHeight);
            this.colour_data = new byte[p_kinect.kinect_sensor.ColorStream.FramePixelDataLength];

            if(p_kinect.kinect_sensor != null) p_kinect.kinect_sensor.ColorFrameReady += this.updateColourVideo;
        }



        /*/////////////////////////////////////////
          * KINECT COLOUR STREAM HANDLER
          *////////////////////////////////////////
        public void updateColourVideo(object p_sender, ColorImageFrameReadyEventArgs p_args)
        {
            // Update the colour stream video...
            //if (was_drawn)
            //{
                using (var current_frame = p_args.OpenColorImageFrame())
                {
                    if (current_frame != null &&
                       this.colour_data != null &&
                       this.colour_texture != null)
                    {
                        // Load array of pixels:
                        current_frame.CopyPixelDataTo(this.colour_data);
                    }
                }

            //    was_drawn = true;
            //}
        }



        /*/////////////////////////////////////////
          * COLOUR STREAM HANDLER SHUTDOWN
          *////////////////////////////////////////
        public void close(KinectManager p_kinect)
        {
            // Remove colour frame listener...
            if(p_kinect != null && p_kinect.kinect_sensor != null) p_kinect.kinect_sensor.ColorFrameReady -= this.updateColourVideo;
        }



        /*/////////////////////////////////////////
          * RENDERING FUNCTION(S)
          *////////////////////////////////////////
        public void draw(SpriteBatch p_sprite_batch, KinectManager p_kinect)
        {
            // Render the colour stream video...
            if(p_kinect != null &&
               p_sprite_batch != null &&
               this.colour_texture != null)// &&
               //this.was_drawn)
            {
                this.colour_texture.SetData(this.colour_data);

                p_sprite_batch.Begin(SpriteSortMode.Immediate, null, null, null, null, this.colour_visualiser);
                p_sprite_batch.Draw(colour_texture,
                                    this.rect,
                                    Color.White);
                p_sprite_batch.End();

                //was_drawn = false;
            }
        }
    }
}
