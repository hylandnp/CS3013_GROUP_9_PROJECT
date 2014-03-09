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

using KinectGame_WindowsXNA.Source;

/*CHANGELOG:
 * NEIL - Created class & basic functionality (modified from Microsoft examples).
 * NEIL - Removed the old rendering resources & functions.
 * NEIL - Added reference object to the base game class.
 * NEIL - Moved the colour stream manager to this class so that disconnecting/reconnecting resets the stream managers.
 * NEIL - Added remaining data stream managers.
 */

namespace KinectGame_WindowsXNA.Source.KinectUtils
{
    // Utility class to manage a valid Kinect Sensor.
    public class KinectManager
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private readonly Dictionary<KinectStatus, string> status_map = null; // Kinect status feedback
        private readonly DepthImageFormat depth_image_format;
        private readonly ColorImageFormat colour_image_format;
        private readonly KinectGame_WindowsXNA root_game = null; // root game class

        public KinectSensor kinect_sensor { get; private set; }
        public KinectStatus last_status { get; private set; }

        // Internal stream managers:
        public ColourStreamManager colour_stream { get; private set;}
        public DepthStreamManager depth_stream { get; private set; }
        public SkeletonStreamManager skeleton_stream { get; private set; }

        // Stream manager debug video dimensions:
        private readonly Vector2 debug_video_stream_dimensions; 

        // Debug status message label:
        private SpriteFont msg_font;
        private Vector2 msg_label_pos;



        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          *////////////////////////////////////////
        public KinectManager(ColorImageFormat p_colour_format,
                             DepthImageFormat p_depth_format,
                             KinectGame_WindowsXNA p_game)
        {
            // Initialise the Kinect selector...
            this.colour_image_format = p_colour_format;
            this.depth_image_format = p_depth_format;
            this.root_game = p_game;

            this.colour_stream = null;
            this.depth_stream = null;
            this.skeleton_stream = null;

            this.debug_video_stream_dimensions = new Vector2(200, 150);

            this.status_map = new Dictionary<KinectStatus, string>();
            KinectSensor.KinectSensors.StatusChanged += this.kinectSensorsStatusChanged; // handler function for changes in the Kinect system
            this.discoverSensor();

            this.status_map.Add(KinectStatus.Undefined, "UNKNOWN STATUS MESSAGE");
            this.status_map.Add(KinectStatus.Connected, "Connected.");//string.Empty);
            this.status_map.Add(KinectStatus.DeviceNotGenuine, "Detected device is not genuine!");
            this.status_map.Add(KinectStatus.DeviceNotSupported, "Detected device is not supported!");
            this.status_map.Add(KinectStatus.Disconnected, "Disconnected/Device required!");
            this.status_map.Add(KinectStatus.Error, "Error in Kinect sensor!");
            this.status_map.Add(KinectStatus.Initializing, "Initialising Kinect sensor...");
            this.status_map.Add(KinectStatus.InsufficientBandwidth, "Insufficient bandwidth for Kinect sensor!");
            this.status_map.Add(KinectStatus.NotPowered, "Detected device is not powered!");
            this.status_map.Add(KinectStatus.NotReady, "Detected device is not ready!");

            // Load the status message font:
            this.msg_font = this.root_game.Content.Load<SpriteFont>("Fonts/Segoe16");
            this.msg_label_pos = new Vector2(4.0f, 2.0f);
        }


        ~KinectManager()
        {
            // Shutdown & clean-up (if not already done)...
            this.close();
        }



        /*/////////////////////////////////////////
          * STARTUP/LOAD FUNCTION(S)
          *////////////////////////////////////////
        public void loadStreamManagers()
        {
            // Create/load the Kinect stream managers...
            if (this.skeleton_stream != null) this.skeleton_stream.close(this); // close skeleton stream manager if already open
            if (this.colour_stream != null) this.colour_stream.close(this); // close colour stream manager if already open
            if (this.depth_stream != null) this.depth_stream.close(this); // close depth stream manager if already open

            /*
             * NOTE: Stream manager debug video rects arranged in the order = SKELETON - COLOUR - DEPTH (positioned from the top-right corner of the window)
             * Stream managers use the KinectColorVisualizer & KinectDepthVisualizer XNA effect files (from Microsoft samples) to correctly format the texture data.
             */

            this.colour_stream = new ColourStreamManager(new Rectangle(this.root_game.GraphicsDevice.Viewport.Width - (int)this.debug_video_stream_dimensions.X * 2,
                                                                       0,
                                                                       (int)this.debug_video_stream_dimensions.X,
                                                                       (int)this.debug_video_stream_dimensions.Y),
                                                         this.root_game.Content.Load<Effect>("Effects_Shaders/KinectColorVisualizer"),
                                                         this,
                                                         this.root_game.GraphicsDevice);

            this.depth_stream = new DepthStreamManager(new Rectangle(this.root_game.GraphicsDevice.Viewport.Width - (int)this.debug_video_stream_dimensions.X,
                                                                     0,
                                                                     (int)this.debug_video_stream_dimensions.X,
                                                                     (int)this.debug_video_stream_dimensions.Y),
                                                       this.root_game.Content.Load<Effect>("Effects_Shaders/KinectDepthVisualizer"),
                                                       this,
                                                       this.root_game.GraphicsDevice);

            this.skeleton_stream = new SkeletonStreamManager(new Rectangle(this.root_game.GraphicsDevice.Viewport.Width - (int)this.debug_video_stream_dimensions.X * 3,
                                                                           0,
                                                                           (int)this.debug_video_stream_dimensions.X,
                                                                           (int)this.debug_video_stream_dimensions.Y),
                                                             this,
                                                             this.root_game.GraphicsDevice,
                                                             this.root_game.Content.Load<Texture2D>("Textures/Kinect/SkeletonJoint"),
                                                             this.root_game.Content.Load<Texture2D>("Textures/Kinect/SkeletonBone"));
        }



        /*/////////////////////////////////////////
          * SKELETON POSITION FUNCTION(S)
          *////////////////////////////////////////
        public Vector2 getBasicSkeletonJointPosition(JointType p_joint, byte p_skeleton_id)
        {
            // Return the basic (Kinect-percieved) co-ordinate position of a specified player's skeleton joint...
            return this.skeleton_stream.getJointPos(p_joint, p_skeleton_id, this);
        }





        /*/////////////////////////////////////////
          * SHUTDOWN/UNLOAD FUNCTION(S)
          *////////////////////////////////////////
        public void close()
        {
            // Ensure the Kinect sensor is shut down...
            if(this.kinect_sensor != null &&
               this.kinect_sensor.IsRunning)
            {
                // Close all open stream managers:
                if (this.skeleton_stream != null) this.skeleton_stream.close(this);
                if (this.colour_stream != null) this.colour_stream.close(this);
                if (this.depth_stream != null) this.depth_stream.close(this);

                // Close all open streams and stop the Kinect sensor:
                if (this.kinect_sensor.SkeletonStream.IsEnabled) this.kinect_sensor.SkeletonStream.Disable();
                if (this.kinect_sensor.ColorStream.IsEnabled) this.kinect_sensor.ColorStream.Disable();
                if (this.kinect_sensor.DepthStream.IsEnabled) this.kinect_sensor.DepthStream.Disable();
                this.kinect_sensor.Stop();
            }
        }



        /*/////////////////////////////////////////
          * DEBUG RENDERING FUNCTION(S)
          *////////////////////////////////////////
        public void drawStreamManagers(SpriteBatch p_sprite_batch)
        {
            // Render the video stream output from the Kinect (for debugging)...
            if(this.kinect_sensor != null &&
               this.kinect_sensor.Status == KinectStatus.Connected)
            {
                if(this.colour_stream != null)
                {
                    this.colour_stream.draw(p_sprite_batch, this);
                }

                if(this.depth_stream != null)
                {
                    this.depth_stream.draw(p_sprite_batch, this);
                }

                if(this.skeleton_stream != null)
                {
                    this.skeleton_stream.draw(p_sprite_batch, this, this.root_game.GraphicsDevice);
                }
            }
        }


        public void skeletonStreamPreRender(SpriteBatch p_sprite_batch)
        {
            // Pre-Render the skeleton to a scalable texture (for debugging):
            if(this.skeleton_stream != null)
            {
                this.skeleton_stream.drawToTexture(this, this.root_game.GraphicsDevice, p_sprite_batch);
            }
        }


        public void drawStatusMessage(SpriteBatch p_sprite_batch)
        {
            // Render the current status message to the top-left corner of the window (for debugging)...
            p_sprite_batch.Begin();
            p_sprite_batch.DrawString(this.msg_font,
                                      this.status_map[this.last_status],
                                      this.msg_label_pos,
                                      Color.White);
            p_sprite_batch.End();
        }



        /*/////////////////////////////////////////
          * INTERNAL SUPPORT FUNCTION(S)
          *////////////////////////////////////////
        private void kinectSensorsStatusChanged(object p_sender, StatusChangedEventArgs p_args)
        {
            // Handle Kinect status events (stop the sensor if device is disconnected)...
            if(p_args.Status != KinectStatus.Connected)
            {
                // Close all stream managers, then stop the current Kinect sensor:
                if (this.skeleton_stream != null) this.skeleton_stream.close(this);
                if (this.colour_stream != null) this.colour_stream.close(this);
                if (this.depth_stream != null) this.depth_stream.close(this);

                p_args.Sensor.Stop();
            }

            this.last_status = p_args.Status;
            this.discoverSensor();
        }



        private void discoverSensor()
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
                        this.kinect_sensor.SkeletonStream.Enable(new TransformSmoothParameters()
                                                                 {
                                                                    Smoothing = 0.5f,
                                                                    Correction = 0.25f,
                                                                    Prediction = 0.25f,
                                                                    JitterRadius = 0.05f,
                                                                    MaxDeviationRadius = 0.04f
                                                                  });
                        this.kinect_sensor.ColorStream.Enable(this.colour_image_format);
                        this.kinect_sensor.DepthStream.Enable(this.depth_image_format);

                        this.kinect_sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;

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

                        if (this.kinect_sensor.IsRunning)
                        {
                            // If the Kinect sensor has been acquired, load the stream managers:
                            this.loadStreamManagers();
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
