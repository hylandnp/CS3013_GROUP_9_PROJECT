using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Fizbin.Kinect.Gestures;
using Fizbin.Kinect.Gestures.Segments;
using System.Timers;
using System.ComponentModel;

/*CHANGELOG
 * NEIL - Created the class.
 */

namespace KinectGame_WindowsXNA.Source.KinectUtils
{
    // Utility class to manage the skeleton stream (modified from Microsoft examples)...
    public class SkeletonStreamManager : INotifyPropertyChanged
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private Skeleton[] skeleton_data = null;
        private Vector2 joint_origin,
                        bone_origin; // co-ordinate centre of the skeleton textures
        private Texture2D joint_texture = null,
                          bone_texture = null; // graphical representations of the skeleton data
        private bool was_drawn = false; // whether or not the skeleton data can be rendered
        private Rectangle rect;

        private RenderTarget2D render_texture = null; // rendering target for scaling the skeleton

        // Fizbin Gesture controller:
        public GestureController fizbin_controller { get; private set; }

        private Timer clear_timer;

        public Dictionary<int, SkeletonPlayer> skeleton_players { get; private set; } // tracking info for skeletons
        public List<int> player_refs { get; private set; } // external/visible tracking info for skeletons


        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          *////////////////////////////////////////
        public SkeletonStreamManager(Rectangle p_dest_rect,
                                     KinectManager p_kinect,
                                     GraphicsDevice p_gfx_device,
                                     Texture2D p_joint,
                                     Texture2D p_bone)
        {
            // Initialisation...
            this.rect = p_dest_rect;

            // Load graphical resources:
            this.joint_texture = p_joint;
            this.bone_texture = p_bone;
            this.fizbin_controller = null;

            this.joint_origin = new Vector2((float)Math.Ceiling(this.joint_texture.Width / 2.0),
                                            (float)Math.Ceiling(this.joint_texture.Height / 2.0));
            this.bone_origin = new Vector2(0.05f, 0.0f);

            // Create the render target & associated data:
            PresentationParameters pres_params = p_gfx_device.PresentationParameters;
            this.render_texture = new RenderTarget2D(p_gfx_device,
                                                     pres_params.BackBufferWidth,
                                                     pres_params.BackBufferHeight,
                                                     false,
                                                     p_gfx_device.DisplayMode.Format,
                                                     DepthFormat.Depth24);

            this.skeleton_players = new Dictionary<int, SkeletonPlayer>();
            this.player_refs = new List<int>();

            if (p_kinect.kinect_sensor != null)
            {
                // Start the Fizbin controller:
                p_kinect.kinect_sensor.SkeletonFrameReady += this.updateSkeleton;

                this.fizbin_controller = new GestureController();
                this.fizbin_controller.GestureRecognized += this.gestureRecognition;
                this.registerGestures();

                this.clear_timer = new Timer(2000); // used to clear last gesture
                this.clear_timer.Elapsed += this.clearGestures;
            }
        }



        /*/////////////////////////////////////////
          * KINECT SKELETON STREAM HANDLER
          *////////////////////////////////////////
        public void updateSkeleton(object p_sender, SkeletonFrameReadyEventArgs p_args)
        {
            // Update the colour stream video...
            using (var current_frame = p_args.OpenSkeletonFrame())
            {
                if (current_frame != null)
                {
                    // Resize/recreated the skeleton data array if necessary:
                    if (this.skeleton_data == null ||
                        this.skeleton_data.Length != current_frame.SkeletonArrayLength)
                    {
                        this.skeleton_data = new Skeleton[current_frame.SkeletonArrayLength];
                    }

                    current_frame.CopySkeletonDataTo(this.skeleton_data);
                    List<int> skeleton_list = new List<int>();

                    // Check for gestures:
                    foreach(var skeleton in this.skeleton_data)
                    {
                        if(skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            skeleton_list.Add(skeleton.TrackingId);

                            // Check if new skeleton:
                            if (!this.skeleton_players.ContainsKey(skeleton.TrackingId))
                            {
                                SkeletonPlayer new_player = new SkeletonPlayer();

                                new_player.tracking_id = skeleton.TrackingId;
                                new_player.last_gesture_str = "";
                                new_player.skeleton = skeleton;

                                this.skeleton_players.Add(new_player.tracking_id, new_player);
                            }

                            // Remove/clean-up any extra/unwanted skeletons:
                            if(skeleton_list.Count < this.skeleton_players.Count)
                            {
                                List<int> active_list = new List<int>(this.skeleton_players.Keys);

                                for (int i = 0; i < skeleton_list.Count; i++ )
                                {
                                    if(active_list.Contains(skeleton_list[i]))
                                    {
                                        active_list.Remove(skeleton_list[i]);
                                    }
                                }

                                for (int i = 0; i < active_list.Count; i++)
                                {
                                    this.skeleton_players.Remove(active_list[i]);
                                }
                            }
                        }
                    }

                    this.player_refs = this.skeleton_players.Keys.ToList();

                    // Process current skeletons:
                    foreach (var id in this.player_refs)
                    {
                        this.fizbin_controller.UpdateAllGestures(skeleton_players[id].skeleton);
                    }
                }
            }
        }



        /*/////////////////////////////////////////
          * SKELETON STREAM HANDLER SHUTDOWN
          *////////////////////////////////////////
        public void close(KinectManager p_kinect)
        {
            // Remove frame listener...
            if (p_kinect != null && p_kinect.kinect_sensor != null)
            {
                p_kinect.kinect_sensor.SkeletonFrameReady -= this.updateSkeleton;
                this.fizbin_controller.GestureRecognized -= this.gestureRecognition;
            }
        }



        /*/////////////////////////////////////////
          * GETTER FUNCTION(S)
          *////////////////////////////////////////
        public Skeleton getSkeleton(byte p_skeleton_id)
        {
            // Return the specified player skeleton...
            if (this.skeleton_data != null &&
                p_skeleton_id < this.skeleton_data.Length &&
                p_skeleton_id >= 0)
            {
                return this.skeleton_data[p_skeleton_id];
            }
            else
            {
                return null; // not applicable
            }
        }


        public Skeleton[] getSkeletonArray()
        {
            if(this.skeleton_data != null)
            {
                return this.skeleton_data;
            }
            else
            {
                return null; // not applicable
            }
        }



        /*/////////////////////////////////////////
          * GESTURE RECOGNITION FUNCTION(S)
          *////////////////////////////////////////
        public event PropertyChangedEventHandler PropertyChanged;

        private void gestureRecognition(object p_sender, GestureEventArgs p_args)
        {
            // Handle recognised gesture arguments:
            string temp_gesture_str = "";
            GestureType temp_gesture = GestureType.NONE;

            switch (p_args.GestureName)
            {
                case "Menu":
                    {
                        temp_gesture_str = "Menu Gesture";
                        temp_gesture = GestureType.MENU;
                        break;
                    }
                case "WaveRight":
                    {
                        temp_gesture_str = "Wave Right Hand";
                        temp_gesture = GestureType.WAVE_RIGHT_HAND;
                        break;
                    }
                case "WaveLeft":
                    {
                        temp_gesture_str = "Wave Left Hand";
                        temp_gesture = GestureType.WAVE_LEFT_HAND;
                        break;
                    }
                case "JoinedHands":
                    {
                        temp_gesture_str = "Joined Hands";
                        temp_gesture = GestureType.JOINED_HANDS;
                        break;
                    }
                case "SwipeLeft":
                    {
                        temp_gesture_str = "Swipe Right Hand (To The Left)";
                        temp_gesture = GestureType.SWIPE_RIGHT_TO_LEFT;
                        break;
                    }
                case "SwipeRight":
                    {
                        temp_gesture_str = "Swipe Left Hand (To The Right)";
                        temp_gesture = GestureType.SWIPE_LEFT_TO_RIGHT;
                        break;
                    }
                case "SwipeUp":
                    {
                        temp_gesture_str = "Swipe Up";
                        temp_gesture = GestureType.SWIPE_UP;
                        break;
                    }
                case "SwipeDown":
                    {
                        temp_gesture_str = "Swipe Down";
                        temp_gesture = GestureType.SWIPE_DOWN;
                        break;
                    }
                case "ZoomIn":
                    {
                        temp_gesture_str = "Zoom In (Move Hands Apart)";
                        temp_gesture = GestureType.ZOOM_IN;
                        break;
                    }
                case "ZoomOut":
                    {
                        temp_gesture_str = "Zoom Out (Move Hands Together)";
                        temp_gesture = GestureType.ZOOM_OUT;
                        break;
                    }
                default:
                    {
                        temp_gesture_str = "";
                        break;
                    }
            }

            if (this.skeleton_players.ContainsKey(p_args.TrackingId) &&
                this.skeleton_players[p_args.TrackingId] != null)
            {
                //var current_skeleton = this.skeleton_players[p_args.TrackingId];

                if(this.skeleton_players[p_args.TrackingId].last_gesture_str != temp_gesture_str &&
                   this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
                }

                this.skeleton_players[p_args.TrackingId].last_gesture_str = temp_gesture_str;
                this.skeleton_players[p_args.TrackingId].last_gesture = temp_gesture;
            }

            this.clear_timer.Start();
        }


        private void registerGestures()
        {
            // Define all recognisable gestures:
            IRelativeGestureSegment[] joined_hands_segments = new IRelativeGestureSegment[20];
            JoinedHandsSegment1 joined_hands_segment = new JoinedHandsSegment1();

            for (int i = 0; i < 20; i++)
            {
                // Gesture consists of the same thing 10 times 
                joined_hands_segments[i] = joined_hands_segment;
            }
            this.fizbin_controller.AddGesture("JoinedHands", joined_hands_segments);


            IRelativeGestureSegment[] menu_segments = new IRelativeGestureSegment[20];
            MenuSegment1 menu_segment = new MenuSegment1();

            for (int i = 0; i < 20; i++)
            {
                // Festure consists of the same thing 20 times 
                menu_segments[i] = menu_segment;
            }
            this.fizbin_controller.AddGesture("Menu", menu_segments);


            IRelativeGestureSegment[] swipe_left_segments = new IRelativeGestureSegment[3];
            swipe_left_segments[0] = new SwipeLeftSegment1();
            swipe_left_segments[1] = new SwipeLeftSegment2();
            swipe_left_segments[2] = new SwipeLeftSegment3();
            this.fizbin_controller.AddGesture("SwipeLeft", swipe_left_segments);


            IRelativeGestureSegment[] swipe_right_segments = new IRelativeGestureSegment[3];
            swipe_right_segments[0] = new SwipeRightSegment1();
            swipe_right_segments[1] = new SwipeRightSegment2();
            swipe_right_segments[2] = new SwipeRightSegment3();
            this.fizbin_controller.AddGesture("SwipeRight", swipe_right_segments);


            IRelativeGestureSegment[] wave_right_segments = new IRelativeGestureSegment[6];
            WaveRightSegment1 wave_right_segment_1 = new WaveRightSegment1();
            WaveRightSegment2 wave_right_segment_2 = new WaveRightSegment2();
            wave_right_segments[0] = wave_right_segment_1;
            wave_right_segments[1] = wave_right_segment_2;
            wave_right_segments[2] = wave_right_segment_1;
            wave_right_segments[3] = wave_right_segment_2;
            wave_right_segments[4] = wave_right_segment_1;
            wave_right_segments[5] = wave_right_segment_2;
            this.fizbin_controller.AddGesture("WaveRight", wave_right_segments);


            IRelativeGestureSegment[] wave_left_segments = new IRelativeGestureSegment[6];
            WaveLeftSegment1 wave_left_segment_1 = new WaveLeftSegment1();
            WaveLeftSegment2 wave_left_segment_2 = new WaveLeftSegment2();
            wave_left_segments[0] = wave_left_segment_1;
            wave_left_segments[1] = wave_left_segment_2;
            wave_left_segments[2] = wave_left_segment_1;
            wave_left_segments[3] = wave_left_segment_2;
            wave_left_segments[4] = wave_left_segment_1;
            wave_left_segments[5] = wave_left_segment_2;
            this.fizbin_controller.AddGesture("WaveLeft", wave_left_segments);


            IRelativeGestureSegment[] zoom_in_segments = new IRelativeGestureSegment[3];
            zoom_in_segments[0] = new ZoomSegment1();
            zoom_in_segments[1] = new ZoomSegment2();
            zoom_in_segments[2] = new ZoomSegment3();
            this.fizbin_controller.AddGesture("ZoomIn", zoom_in_segments);


            IRelativeGestureSegment[] zoom_out_segments = new IRelativeGestureSegment[3];
            zoom_out_segments[0] = new ZoomSegment3();
            zoom_out_segments[1] = new ZoomSegment2();
            zoom_out_segments[2] = new ZoomSegment1();
            this.fizbin_controller.AddGesture("ZoomOut", zoom_out_segments);


            IRelativeGestureSegment[] swipe_up_segments = new IRelativeGestureSegment[3];
            swipe_up_segments[0] = new SwipeUpSegment1();
            swipe_up_segments[1] = new SwipeUpSegment2();
            swipe_up_segments[2] = new SwipeUpSegment3();
            this.fizbin_controller.AddGesture("SwipeUp", swipe_up_segments);


            IRelativeGestureSegment[] swipe_down_segments = new IRelativeGestureSegment[3];
            swipe_down_segments[0] = new SwipeDownSegment1();
            swipe_down_segments[1] = new SwipeDownSegment2();
            swipe_down_segments[2] = new SwipeDownSegment3();
            this.fizbin_controller.AddGesture("SwipeDown", swipe_down_segments);
        }


        private void clearGestures(object p_sender, ElapsedEventArgs p_args)
        {
            foreach (var id in this.player_refs)
            {
                this.skeleton_players[id].last_gesture_str = "";
                this.skeleton_players[id].last_gesture = GestureType.NONE;
            }

            this.clear_timer.Stop();
        }



        /*/////////////////////////////////////////
          * RENDERING FUNCTION(S)
          *////////////////////////////////////////
        public void drawToTexture(KinectManager p_kinect,
                                  GraphicsDevice p_gfx_device,
                                  SpriteBatch p_sprite_batch)
        {
            // Render the skeleton stream video to the render target...
            if (p_kinect != null &&
                p_kinect.kinect_sensor != null &&
                !this.was_drawn)
            {
                p_gfx_device.SetRenderTarget(this.render_texture); // draw to the render texture
                p_gfx_device.Clear(Color.DarkRed);

                if (this.skeleton_data != null)
                {
                    p_sprite_batch.Begin();

                    foreach (var skeleton in this.skeleton_data)
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // Draw upper body:
                            this.drawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight, p_kinect, p_sprite_batch);
                            
                            // Draw lower body
                            this.drawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight, p_kinect, p_sprite_batch);

                            // Draw left arm:
                            this.drawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft, p_kinect, p_sprite_batch);

                            // Draw right arm:
                            this.drawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight, p_kinect, p_sprite_batch);

                            // Draw left leg:
                            this.drawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft, p_kinect, p_sprite_batch);

                            // Draw right leg:
                            this.drawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight, p_kinect, p_sprite_batch);
                            this.drawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight, p_kinect, p_sprite_batch);

                            // Draw all the joints:
                            foreach (Joint j in skeleton.Joints)
                            {
                                Color joint_colour = Color.Green;

                                if (j.TrackingState != JointTrackingState.Tracked)
                                {
                                    joint_colour = Color.Yellow;
                                }

                                p_sprite_batch.Draw(this.joint_texture,
                                                    this.skeletonToPoint(j.Position, p_kinect),
                                                    null,
                                                    joint_colour,
                                                    0.0f,
                                                    this.joint_origin,
                                                    1.0f,
                                                    SpriteEffects.None,
                                                    0.0f);
                            }
                        }
                        else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            // If only tracking position, draw a blue dot:
                            p_sprite_batch.Draw(this.joint_texture,
                                                          this.skeletonToPoint(skeleton.Position, p_kinect),
                                                          null,
                                                          Color.Blue,
                                                          0.0f,
                                                          this.joint_origin,
                                                          1.0f,
                                                          SpriteEffects.None,
                                                          0.0f);
                        }
                    }

                    p_sprite_batch.End();
                }

                p_gfx_device.SetRenderTarget(null); // back to regular drawing
                this.was_drawn = true;
            }
        }


        public void draw(SpriteBatch p_sprite_batch,
                         KinectManager p_kinect,
                         GraphicsDevice p_gfx_device)
        {
            // Render the skeleton stream video...
            if (this.was_drawn)
            {
                p_sprite_batch.Begin();

                p_sprite_batch.Draw(this.render_texture,
                                    this.rect,
                                    Color.White);

                p_sprite_batch.End();
                this.was_drawn = false;
            }
        }


        private void drawBone(JointCollection p_joints,
                              JointType p_start,
                              JointType p_end,
                              KinectManager p_kinect,
                              SpriteBatch p_sprite_batch)
        {
            // Draw a single joint-bone-joint...
            Vector2 start = this.skeletonToPoint(p_joints[p_start].Position, p_kinect);
            Vector2 end = this.skeletonToPoint(p_joints[p_end].Position, p_kinect);
            Vector2 difference = end - start;
            Vector2 scale = new Vector2(1.0f, difference.Length() / this.bone_texture.Height);

            float angle = (float)Math.Atan2(difference.Y, difference.X) - MathHelper.PiOver2;

            // Set colour(s):
            Color colour = Color.LightGreen;

            if (p_joints[p_start].TrackingState != JointTrackingState.Tracked ||
                p_joints[p_end].TrackingState != JointTrackingState.Tracked)
            {
                colour = Color.Gray;
            }

            p_sprite_batch.Draw(this.bone_texture,
                                start,
                                null,
                                colour,
                                angle,
                                this.bone_origin,
                                scale,
                                SpriteEffects.None,
                                1.0f);
        }



        /*/////////////////////////////////////////
          * CO-ORDINATE FUNCTION(S)
          *////////////////////////////////////////
        private Vector2 skeletonToPoint(SkeletonPoint p_point,
                                        KinectManager p_kinect)
        {
            // Function mapping a SkeletonPoint from one space to another...
            if(p_kinect != null &&
               p_kinect.kinect_sensor != null &&
               p_kinect.kinect_sensor.DepthStream != null)
            {
                var depth_pt = p_kinect.kinect_sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(p_point,
                                                                                                    p_kinect.kinect_sensor.DepthStream.Format);
                return new Vector2(depth_pt.X,
                                   depth_pt.Y);
            }
            else
            {
                return Vector2.Zero;
            }
        }
    }
}
