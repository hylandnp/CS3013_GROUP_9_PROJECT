using Microsoft.Kinect;
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
 * NEIL - Added selected colour icon.
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

        private JointType hand_joint;

        // Rendering info:
        private Texture2D hand_texture,
                          colour_texture;
        private Rectangle hand_dest_rect,
                          hand_source_rect,
                          colour_rect;
        private Vector2 hand_origin,
                        debug_msg_pos;
        private SpriteFont debug_font;

        public Color selected_colour { get; set; }
        public string debug_message { get; set; }


        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          */
        ///////////////////////////////////////
        public Cursor(Texture2D p_texture,
                      Texture2D p_col_texture,
                      SpriteFont p_debug_font,
                      JointType p_hand,
                      float p_scale,
                      byte p_player)
        {
            // Initialisation...
            this.hand_texture = p_texture;
            this.colour_texture = p_col_texture;
            this.hand_joint = p_hand;
            this.player_id = p_player;
            this.selected_colour = Color.White;
            this.debug_font = p_debug_font;
            this.debug_message = "";

            // Set drawing origin to the centre of the texture:
            this.hand_origin = new Vector2((float)Math.Ceiling((this.hand_texture.Bounds.Width / 2.0f) - (256 / 2.0)),
                                           (float)Math.Ceiling(this.hand_texture.Bounds.Height / 2.0f));

            this.hand_dest_rect = new Rectangle(0, 0,
                                                (int)Math.Ceiling((this.hand_texture.Bounds.Width - 256) * p_scale),
                                                (int)Math.Ceiling(this.hand_texture.Bounds.Height * p_scale));

            // Position the colour icon over the hand texture (with offset):
            this.colour_rect = new Rectangle(this.hand_dest_rect.X - 20,
                                             this.hand_dest_rect.Y + 25,
                                             (int)Math.Ceiling(this.colour_texture.Bounds.Width * p_scale),
                                             (int)Math.Ceiling(this.colour_texture.Bounds.Height * p_scale));

            this.debug_msg_pos = new Vector2(0, 0);

            // Reset hands & texture source rectangle:
            this.swapHands();
            this.swapHands();

            this.current_pos = Vector3.Zero;
            this.previous_pos = Vector3.Zero;
        }



        /*/////////////////////////////////////////
          * UPDATE FUNCTION(S)
          *////////////////////////////////////////
        public void update(Vector3 p_new_position,
                           TimeSpan p_new_time)
        {
            // Update the positions of the cursor:
            this.previous_pos = this.current_pos;
            this.current_pos = p_new_position;

            this.hand_dest_rect.X = (int)this.current_pos.X;
            this.hand_dest_rect.Y = (int)this.current_pos.Y;

            // Position the colour icon over the hand texture (with offset):
            this.colour_rect.X = this.hand_dest_rect.X - 20;
            this.colour_rect.Y = this.hand_dest_rect.Y + 25;

            // Position debug message:
            this.debug_msg_pos.X = this.hand_dest_rect.X + 25;
            this.debug_msg_pos.Y = this.hand_dest_rect.Y + 25;
        }



        /*/////////////////////////////////////////
          * GETTER/SETTER FUNCTION(S)
          *////////////////////////////////////////
        public JointType getHandJoint()
        {
            return this.hand_joint;
        }


        public void swapHands()
        {
            if(this.hand_joint == JointType.HandLeft)
            {
                this.hand_joint = JointType.HandRight;

                // Set right-hand of texture drawable:
                this.hand_source_rect = new Rectangle(265, 0,
                                                      this.hand_texture.Bounds.Width - 265,
                                                      this.hand_texture.Bounds.Height);
            }
            else
            {
                this.hand_joint = JointType.HandLeft;

                // Set right-hand of texture drawable:
                this.hand_source_rect = new Rectangle(0, 0,
                                                      this.hand_texture.Bounds.Width - 265,
                                                      this.hand_texture.Bounds.Height);
            }
        }



        /*/////////////////////////////////////////
          * RENDERING FUNCTION(S)
          *////////////////////////////////////////
        public void draw(SpriteBatch p_sprite_batch)
        {
            // Render the cursor:
            if(this.current_pos != Vector3.Zero)
            {
                p_sprite_batch.Begin();
                p_sprite_batch.Draw(this.hand_texture, this.hand_dest_rect, this.hand_source_rect, Color.White, 0.0f, this.hand_origin, SpriteEffects.None, 0.0f);
                p_sprite_batch.Draw(this.colour_texture, this.colour_rect, this.selected_colour);
                if(this.debug_msg_pos != null) p_sprite_batch.DrawString(this.debug_font, this.debug_message, this.debug_msg_pos, Color.Wheat);
                p_sprite_batch.End();
            }
        }
    }
}

