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
    // Utility class to manage the skeleton stream (modified from Microsoft examples)...
    public class SkeletonStreamManager
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

            if (p_kinect.kinect_sensor != null) p_kinect.kinect_sensor.SkeletonFrameReady += this.updateSkeleton;
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
                }
            }
        }



        /*/////////////////////////////////////////
          * SKELETON STREAM HANDLER SHUTDOWN
          *////////////////////////////////////////
        public void close(KinectManager p_kinect)
        {
            // Remove colour frame listener...
            if (p_kinect != null && p_kinect.kinect_sensor != null) p_kinect.kinect_sensor.SkeletonFrameReady -= this.updateSkeleton;
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
          * JOINT POSITION FUNCTION(S)
          *////////////////////////////////////////
        //public Vector2 getJointPos(JointType p_joint, byte p_skeleton_id, KinectManager p_kinect)
        //{
        //    // Return the screen position of the specified skeleton joint (if applicable)...
        //    if (this.skeleton_data != null &&
        //        p_skeleton_id < this.skeleton_data.Length &&
        //        p_skeleton_id >= 0 &&
        //        this.skeleton_data[p_skeleton_id] != null)
        //    {
        //        return new Vector2(this.skeleton_data[p_skeleton_id].Joints[p_joint].Position.X,
        //                           this.skeleton_data[p_skeleton_id].Joints[p_joint].Position.Y);
        //    }
        //    else
        //    {
        //        return Vector2.Zero; // return (0, 0) co-ordinate if not able to find position
        //    }
        //}


        //public Vector2 getDepthJointPos(JointType p_joint, byte p_skeleton_id, KinectManager p_kinect)
        //{
        //    // Return the depth stream position of the specified skeleton joint (if applicable)...
        //    if (this.skeleton_data != null && 
        //        p_skeleton_id < this.skeleton_data.Length &&
        //        p_skeleton_id >= 0 &&
        //        this.skeleton_data[p_skeleton_id] != null)
        //    {
        //        var temp_point = p_kinect.kinect_sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(this.skeleton_data[p_skeleton_id].Joints[p_joint].Position,
        //                                                                                              p_kinect.kinect_sensor.DepthStream.Format);
        //        return new Vector2(temp_point.X,
        //                           temp_point.Y);
        //    }
        //    else
        //    {
        //        return Vector2.Zero; // return (0, 0) co-ordinate if not able to find position
        //    }
        //}


        //public bool currentlyTracking(JointType p_joint, byte p_skeleton_id)
        //{
        //    // Check if the current skeleton's joint is being successfully tracked:
        //    if (p_skeleton_id < this.skeleton_data.Length &&
        //        p_skeleton_id >= 0 &&
        //        this.skeleton_data[p_skeleton_id] != null &&
        //        this.skeleton_data[p_skeleton_id].TrackingState == SkeletonTrackingState.Tracked)
        //    {
        //        return (this.skeleton_data[p_skeleton_id].Joints[p_joint].TrackingState == JointTrackingState.Tracked);
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}



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
