using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Microsoft.Kinect;
using System.IO;
using System.Diagnostics;

/*CHANGELOG:
 * NEIL - Created class & basic functionality (from Microsoft examples).
 */

namespace KinectGame_WindowsXNA.Source.KinectUtils
{
    // Utility class to select a valid Kinect Sensor.
    public class KinectSelector
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private readonly Dictionary<KinectStatus, string> status_map = null; // Kinect status feedback
        private readonly DepthImageFormat depth_image_format;
        private readonly ColorImageFormat colour_image_format;
        //private Texture2D ui_background; // background of the Kinect selector UI
        //private SpriteFont ui_font; // font of the Kinect selector UI

        public KinectSensor kinect_sensor { get; private set; }
        public KinectStatus last_status { get; private set; }



        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          *////////////////////////////////////////
        public KinectSelector(ColorImageFormat p_colour_format,
                              DepthImageFormat p_depth_format)
                              //Texture2D p_background,
                              //SpriteFont p_font)
        {
            // Initialise the Kinect selector...
            this.colour_image_format = p_colour_format;
            this.depth_image_format = p_depth_format;
            //this.ui_font = p_font;
            //this.ui_background = p_background;

            status_map = new Dictionary<KinectStatus, string>();
            KinectSensor.KinectSensors.StatusChanged += this.KinectSensorsStatusChanged;
            this.DiscoverSensor();

            this.status_map.Add(KinectStatus.Undefined, string.Empty);
            this.status_map.Add(KinectStatus.Connected, string.Empty);
            this.status_map.Add(KinectStatus.DeviceNotGenuine, "Detected device is not genuine!");
            this.status_map.Add(KinectStatus.DeviceNotSupported, "Detected device is not supported!");
            this.status_map.Add(KinectStatus.Disconnected, "Disconnected/Device required!");
            this.status_map.Add(KinectStatus.Error, "Error in Kinect sensor!");
            this.status_map.Add(KinectStatus.Initializing, "Initialising Kinect sensor...");
            this.status_map.Add(KinectStatus.InsufficientBandwidth, "Insufficient bandwidth for Kinect sensor!");
            this.status_map.Add(KinectStatus.NotPowered, "Detected device is not powered!");
            this.status_map.Add(KinectStatus.NotReady, "Detected device is not ready!");
        }

        ~KinectSelector()
        {
            // Shutdown & clean-up...
            this.close();
        }



        /*/////////////////////////////////////////
          * SHUTDOWN FUNCTION(S)
          *////////////////////////////////////////
        public void close()
        {
            // Ensure the Kinect sensor is shut down:
            if(this.kinect_sensor != null)
            {
                this.kinect_sensor.Stop();
            }
        }



        /*/////////////////////////////////////////
          * RENDERING FUNCTION(S)
          *////////////////////////////////////////
        //public void draw(GameTime p_time,
        //                 Game p_game,
        //                 ref SpriteBatch p_sprite_batch,
        //                 ref GraphicsDeviceManager p_graphics)
        //{
        //    // Render the Kinect selector UI...
        //    if(this.ui_background != null && this.ui_font != null)
        //    {
        //        // If there's no Kinect sensor connected, display status messages:
        //        if (this.kinect_sensor == null || this.last_status != KinectStatus.Connected)
        //        {
        //            p_sprite_batch.Begin();

        //            Vector2 bg_pos = new Vector2((float)Math.Ceiling((p_game.GraphicsDevice.Viewport.Width - this.ui_background.Width) / 2.0),
        //                                         (float)Math.Ceiling((p_game.GraphicsDevice.Viewport.Height - this.ui_background.Height) / 2.0));

        //            // Draw background:
        //            p_sprite_batch.Draw(this.ui_background,
        //                                bg_pos,
        //                                Color.White);

        //            // Select message:
        //            string msg = "Please wait...";
        //            if(this.kinect_sensor != null)
        //            {
        //                msg = this.status_map[this.last_status];
        //            }

        //            // Draw the message string:
        //            Vector2 size = this.ui_font.MeasureString(msg);
        //            p_sprite_batch.DrawString(this.ui_font,
        //                                      msg,
        //                                      new Vector2((float)Math.Ceiling((p_game.GraphicsDevice.Viewport.Width - size.X) / 2.0),
        //                                                  bg_pos.Y + ui_background.Height + size.Y),
        //                                      Color.White);

        //            p_sprite_batch.End();
        //        }
        //    }
        //}



        /*/////////////////////////////////////////
          * INTERNAL SUPPORT FUNCTION(S)
          *////////////////////////////////////////
        private void KinectSensorsStatusChanged(object p_sender, StatusChangedEventArgs p_args)
        {
            // Handle Kinect status events (stop the sensor if device is disconnected)...
            if(p_args.Status != KinectStatus.Connected)
            {
                p_args.Sensor.Stop();
            }

            this.last_status = p_args.Status;
            this.DiscoverSensor();
        }



        private void DiscoverSensor()
        {
            // Attempt to connect to a valid Kinect sensor...
            this.kinect_sensor = KinectSensor.KinectSensors.FirstOrDefault();

            if(this.kinect_sensor != null)
            {
                this.last_status = this.kinect_sensor.Status;

                // Enable the connected sensor:
                if(this.last_status == KinectStatus.Connected)
                {
                    try
                    {
                        this.kinect_sensor.SkeletonStream.Enable();
                        this.kinect_sensor.ColorStream.Enable(this.colour_image_format);
                        this.kinect_sensor.DepthStream.Enable(this.depth_image_format);

                        try
                        {
                            // Start the sensor with the current stream settings:
                            this.kinect_sensor.Start();
                        }
                        catch(IOException ex)
                        {
                            // Sensor in use or otherwise unavailable:
                            Debug.WriteLine(ex.ToString());
                            this.kinect_sensor = null;
                        }
                    }
                    catch(InvalidOperationException ex)
                    {
                        // Sensor may be unplugged or in some invalid state:
                        Debug.WriteLine(ex.ToString());
                        this.kinect_sensor = null;
                    }
                }
            }
            else
            {
                this.last_status = KinectStatus.Disconnected;
            }
        }
    }
}
