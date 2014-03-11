﻿using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*CHANGELOG
 * NEIL - Created the class & added texture loading.
 * PATRICK - Added cursor tracking, simple rendering & gesture recognition.
 * NEIL - Minor formatting & bug fixes.
 */

namespace KinectGame_WindowsXNA.Source.Interface
{
    // Cursor icon for the user's Kinect "hand" position or the mouse (for debugging)...
    public class Cursor
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          */
        ///////////////////////////////////////
        public byte player_id { get; set; }

        private Vector3 current_pos,
                        previous_pos;

        public JointType hand_joint { get; set; }

        // Rendering info:
        private Texture2D hand_texture;
        private Rectangle hand_dest_rect;
        private Vector2 hand_origin;



        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          */
        ///////////////////////////////////////
        public Cursor(Texture2D p_texture,
                      JointType p_hand,
                      float p_scale,
                      byte p_player)
        {
            // Initialisation...
            this.hand_texture = p_texture;
            this.hand_joint = p_hand;
            this.player_id = p_player;

            // Set drawing origin to the centre of the texture:
            this.hand_origin = new Vector2((float)Math.Ceiling(this.hand_texture.Bounds.Width / 2.0f),
                                           (float)Math.Ceiling(this.hand_texture.Bounds.Height / 2.0f));

            this.hand_dest_rect = new Rectangle(0, 0,
                                                (int)Math.Ceiling(this.hand_texture.Bounds.Width * p_scale),
                                                (int)Math.Ceiling(this.hand_texture.Bounds.Height * p_scale));

            this.current_pos = Vector3.Zero;
            this.previous_pos = Vector3.Zero;
        }



        /*/////////////////////////////////////////
          * UPDATE FUNCTION(S)
          */
        ///////////////////////////////////////
        public void update(Vector3 p_new_position,
                           TimeSpan p_new_time)
        {
            // Update the positions of the cursor:
            this.previous_pos = this.current_pos;
            this.current_pos = p_new_position;

            this.hand_dest_rect.X = (int)this.current_pos.X;
            this.hand_dest_rect.Y = (int)this.current_pos.Y;
        }



        /*/////////////////////////////////////////
          * RENDERING FUNCTION(S)
          */
        ///////////////////////////////////////
        public void draw(SpriteBatch p_sprite_batch)
        {
            // Render the cursor:
            p_sprite_batch.Begin();
            p_sprite_batch.Draw(this.hand_texture, this.hand_dest_rect, null, Color.White, 0.0f, this.hand_origin, SpriteEffects.None, 0.0f);
            p_sprite_batch.End();
        }
    }
}

