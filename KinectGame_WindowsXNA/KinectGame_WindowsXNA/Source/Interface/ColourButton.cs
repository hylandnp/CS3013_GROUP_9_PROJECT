using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using KinectGame_WindowsXNA.Source.KinectUtils;


/*CHANGELOG
 * NEIL - Created the empty class.
 * RICHARD - Basic timing setup.
 * GAVAN - Button functionality added.
 * GAVAN - Button timing & better colour-change code added.
 */

namespace KinectGame_WindowsXNA.Source.Interface
{
    // Colour selection button for the Kinect game...
    public class ColourButton
    {
        private Texture2D image;
        private Rectangle location;
        private bool clicked,
                     hover;
        private float target_time;
        private Stopwatch timer;
        private GestureType target_gesture;

        public Color button_colour { get; set; }


        public ColourButton(Texture2D p_texture,
                            float p_target_time,
                            Vector2 p_pos,
                            GestureType p_gesture,
                            Color p_colour)
        {
            // Initialisation...
            this.image = p_texture;
            this.target_gesture = p_gesture;
            this.button_colour = p_colour;

            this.location = new Rectangle((int)Math.Ceiling(p_pos.X), 
                                          (int)Math.Ceiling(p_pos.Y),
                                          this.image.Width,
                                          this.image.Height);
            
            this.target_time = p_target_time  * 1000; // in microseconds
            this.hover = false;

            this.timer = new Stopwatch();
        }



        /*/////////////////////////////////////////
          * UPDATE FUNCTION
          *////////////////////////////////////////
        public void Update(Cursor p_player_cursor, GameTime p_time)
        {
            // Update the simple game button...
            this.clicked = false;
            Vector2 current_pos = p_player_cursor.get2DPosition();

            if (this.location.Contains(new Point((int)Math.Ceiling(current_pos.X),
                                                 (int)Math.Ceiling(current_pos.Y))))
            {
                // If the player's cursor is over the button's rect bounds:
                if (!this.hover)
                {
                    this.hover = true;
                    if (!this.timer.IsRunning) this.timer.Restart();
                }
            }
            else
            {
                this.hover = false;
                this.clicked = false;
                this.timer.Reset();
            }

            // Check if target hover time has elapsed:
            if (this.hover && this.timer.IsRunning)
            {
                long current_time = this.timer.ElapsedMilliseconds;

                if (current_time >= this.target_time)
                {
                    this.timer.Reset();

                    if (this.target_gesture == GestureType.NONE || p_player_cursor.gesture == this.target_gesture)
                    {
                        this.clicked = true;
                    }
                }
            }
        }



        /*/////////////////////////////////////////
          * RENDER FUNCTION
          *////////////////////////////////////////
        public void draw(SpriteBatch p_sprite_batch)
        {
            // Draw the button's texture to the screen...
            p_sprite_batch.Begin();

            if(this.image != null)
            {
                p_sprite_batch.Draw(this.image, this.location, this.button_colour);
            }

            p_sprite_batch.End();
        }



        /*/////////////////////////////////////////
          * OTHER FUNCTION(S)
          *////////////////////////////////////////
        public bool isClicked()
        {
            // Check for clicked callback, resets the clicked callback boolean...
            bool is_clicked = this.clicked;
            this.clicked = false;
            return is_clicked;
        }


        public void setPosition(int p_xpos, int p_ypos)
        {
            // Change the position of the button...
            this.location.X = p_xpos;
            this.location.Y = p_ypos;
        }


        public void setPosition(Vector2 p_pos)
        {
            // Change the position of the button...
            this.location.X = (int)Math.Ceiling(p_pos.X);
            this.location.Y = (int)Math.Ceiling(p_pos.Y);
        }
    }
}